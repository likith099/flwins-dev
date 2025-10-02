
    {
        // Completely public controller for anonymous access
        
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.Message = "Skill up for a better future.";
            ViewBag.IsAuthenticated = false; // Always false for public access
            ViewBag.DisplayName = "Guest";
            ViewBag.UserEmail = "";
            
            return View("~/Views/Flwins/Index.cshtml");
        }
        
        [AllowAnonymous]
        public ActionResult Login()
        {
            // Redirect to Azure AD login
            return Redirect("/.auth/login/aad");
        }
    }
}