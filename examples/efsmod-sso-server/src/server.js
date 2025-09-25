import express from "express";
import cookieParser from "cookie-parser";
import "dotenv/config";
import { createRemoteJWKSet, jwtVerify, decodeJwt } from "jose";

const app = express();
app.use(express.urlencoded({ extended: true }));
app.use(express.json());
app.use(cookieParser(process.env.SESSION_SECRET || "dev-secret"));

// Minimal in-memory user store (replace with DB)
const users = new Map(); // key: sub/oid, value: user object

function getDiscoveryUrl(authority) {
  return `${authority}/.well-known/openid-configuration`;
}

function parseAuthority(authority) {
  // e.g. https://domain/tenantIdOrName/v2.0
  try {
    const u = new URL(authority);
    const parts = u.pathname.replace(/^\//, "").split("/");
    const tenant = parts[0];
    const origin = `${u.protocol}//${u.host}`;
    return { origin, tenant };
  } catch {
    return { origin: "", tenant: "" };
  }
}

async function getOidcConfig(authority) {
  const discoveryUrl = getDiscoveryUrl(authority);
  const res = await fetch(discoveryUrl);
  if (!res.ok) throw new Error(`Failed discovery: ${res.status}`);
  return await res.json();
}

async function verifyIdToken(idToken) {
  const authority = process.env.B2C_AUTHORITY;
  const audience = process.env.SPA_CLIENT_ID; // Quick-start: accept SPA's id_token audience

  const oidc = await getOidcConfig(authority);
  const expectedIssuer = oidc.issuer; // derive from discovery to avoid subtle mismatches
  const JWKS = createRemoteJWKSet(new URL(oidc.jwks_uri));

  // Build allowed issuers (handle B2C tfp/acr variants)
  const baseIssuers = [expectedIssuer, expectedIssuer.replace(/\/$/, "")];
  const decoded = (() => {
    try {
      return decodeJwt(idToken);
    } catch {
      return {};
    }
  })();
  const tfp = decoded?.tfp || decoded?.acr;
  const { origin, tenant } = parseAuthority(authority);
  const tfpIssuers =
    tfp && origin && tenant
      ? [
          `${origin}/tfp/${tenant}/${tfp}/v2.0/`,
          `${origin}/tfp/${tenant}/${tfp}/v2.0`,
        ]
      : [];

  const allowedIssuers = [...baseIssuers, ...tfpIssuers];

  const { payload } = await jwtVerify(idToken, JWKS, {
    issuer: allowedIssuers,
    audience,
    clockTolerance: 60, // allow small clock skew
  });
  return payload; // contains claims (sub/oid, emails, name, given_name, family_name, etc.)
}

function upsertUserFromClaims(claims) {
  const id = claims.oid || claims.sub;
  const email = claims.email || claims.preferred_username || "";
  const user = {
    id,
    email,
    name:
      claims.name ||
      `${claims.given_name || ""} ${claims.family_name || ""}`.trim(),
    given_name: claims.given_name || "",
    family_name: claims.family_name || "",
    tenantId: claims.tid || "",
    createdAt: new Date().toISOString(),
  };
  users.set(id, { ...users.get(id), ...user });
  return users.get(id);
}

app.get("/", (req, res) => {
  const who = req.signedCookies.user
    ? JSON.parse(req.signedCookies.user)
    : null;
  res.send(`<h1>EFSMod SSO Example</h1>
    <p>${who ? `Signed in as ${who.email}` : "Not signed in"}</p>
    <p>POST an id_token to /sso/entra to sign in.</p>`);
});

// Secure session creator (demo: signed cookie)
function setSession(res, user) {
  res.cookie("user", JSON.stringify(user), { httpOnly: true, signed: true });
}

// SSO endpoint: validate id_token, JIT-provision, create session, redirect
app.post("/sso/entra", async (req, res) => {
  try {
    const { id_token, return: returnPath = "/profile" } = req.body;
    if (!id_token) return res.status(400).send("Missing id_token");

    const claims = await verifyIdToken(id_token);
    const user = upsertUserFromClaims(claims);
    setSession(res, user);

    // Redirect to requested path; in a real app, ensure it's a safe/allowed path
    res.redirect(returnPath);
  } catch (err) {
    // Enhanced diagnostics to help align issuer/audience/policy
    try {
      const debugClaims = decodeJwt(req.body?.id_token || "");
      console.error("SSO error:", err?.message || err);
      console.error("Token debug claims:", {
        iss: debugClaims?.iss,
        aud: debugClaims?.aud,
        tid: debugClaims?.tid,
        tfp: debugClaims?.tfp || debugClaims?.acr,
        exp: debugClaims?.exp,
        nbf: debugClaims?.nbf,
      });
      // Also log expected values
      const authority = process.env.B2C_AUTHORITY;
      const oidc = await getOidcConfig(authority).catch(() => ({}));
      console.error("Expected (from discovery):", {
        issuer: oidc?.issuer,
        jwks_uri: oidc?.jwks_uri,
        audience: process.env.SPA_CLIENT_ID,
      });
    } catch {}
    res.status(401).send("Invalid token");
  }
});

// Fallback login (demo)
app.get("/login", (req, res) => {
  const { email = "", return: returnPath = "/profile" } = req.query;
  res.send(`<h1>Login (demo)</h1>
    <p>Email hint: ${email}</p>
    <p>After implementing login, redirect to ${returnPath}</p>`);
});

app.get("/profile", (req, res) => {
  const who = req.signedCookies.user
    ? JSON.parse(req.signedCookies.user)
    : null;
  if (!who) return res.redirect("/login?return=/profile");
  res.send(`<h1>EFSMod Profile</h1><pre>${JSON.stringify(who, null, 2)}</pre>`);
});

const port = process.env.PORT || 4000;
app.listen(port, () =>
  console.log(`EFSMod SSO server listening on http://localhost:${port}`)
);
