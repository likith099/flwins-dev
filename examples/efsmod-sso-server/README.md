# EFSMod SSO Server (Example)

A minimal example server showing how EFSMod can accept an Azure AD B2C id_token, validate it, JIT-provision a user, create a session, and redirect to a profile.

## Quick start

1. Copy env file:

```bash
cp .env.example .env
```

2. Install deps and run:

```bash
npm install
npm run dev
```

3. Frontend settings:
- Set `NEXT_PUBLIC_EFSMOD_URL=http://localhost:4000` in your main app's `.env.local`
- From your Profile page, click "SR Application Form"

## How it works
- `POST /sso/entra` accepts `id_token` from the SPA (quick-start mode)
- Verifies token using Azure AD B2C discovery + JWKS (issuer/audience/exp)
- Extracts claims and upserts user in memory (replace with DB)
- Sets a signed cookie as session and redirects to `/profile` (or `return` parameter)

## Production hardening
- Prefer access tokens issued to an EFSMod API (not the SPA id_token)
- Replace cookie demo with proper session management
- Persist users in a database
- Check allowed redirect paths (avoid open redirects)
- Use HTTPS and secure cookies
- Consider CSRF protection for POST
