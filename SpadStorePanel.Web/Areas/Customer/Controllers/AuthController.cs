using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SpadStorePanel.Core.Models;
using SpadStorePanel.Core.Utility;
using SpadStorePanel.Infrastructure.Repositories;
using SpadStorePanel.Web.Controllers;
using SpadStorePanel.Web.ViewModels;

namespace SpadStorePanel.Web.Areas.Customer.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private UsersRepository _userRepo;
        private readonly StaticContentDetailsRepository _staticContentDetailsRepo;

        public AuthController()
        {
        }

        public AuthController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, UsersRepository userRepo, StaticContentDetailsRepository staticContentDetailsRepo) 
        {
            UserManager = userManager;
            SignInManager = signInManager;
            UserRepo = userRepo;
            this._staticContentDetailsRepo = staticContentDetailsRepo;
        }
        public UsersRepository UserRepo
        {
            get
            {
                return _userRepo ?? new UsersRepository();
            }
            private set
            {
                _userRepo = value;
            }
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Auth/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [AllowAnonymous]
        public ActionResult LoginPartial()
        {
            return PartialView();
        }
        [AllowAnonymous]
        public ActionResult RegisterPartial()
        {
            return PartialView();
        }
        [AllowAnonymous]
        public ActionResult AccessDenied(string returnUrl = null)
        {
            ViewBag.ReturnUrl = "/Admin/Dashboard/Index";
            if (!string.IsNullOrEmpty(returnUrl))
                ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        //
        // POST: /Auth/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(ViewModels.LoginViewModel model, string returnUrl)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards Auth lockout
            // To enable password failures to trigger Auth lockout, change to shouldLockout: true
            var user = UserManager.FindByName(model.UserName);
            if (user == null)
            {
                ViewBag.LoginError = "نام کاربری وارد شده صحیح نیست.";
                //ModelState.AddModelError(string.Empty, "نام کاربری وارد شده صحیح نیست.");
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal("/Customer/Dashboard"); // Or use returnUrl
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ViewBag.LoginError = "نام کاربری یا رمز عبور وارد شده صحیح نیست.";
                    //ModelState.AddModelError("", "نام کاربری وارد شده صحیح نیست.");
                    return View(model);
            }
        }

        //
        // GET: /Auth/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //[AllowAnonymous]
        //public ActionResult AccountLoginSection()
        //{
        //    var model = new LoginViewModel();

        //    return PartialView(model);
        //}

        ////
        //// POST: /Auth/Login
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> AccountLoginSection(ViewModels.LoginViewModel model, string returnUrl)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // This doesn't count login failures towards Auth lockout
        //    // To enable password failures to trigger Auth lockout, change to shouldLockout: true
        //    var user = UserManager.FindByName(model.UserName);
        //    if (user == null)
        //    {
        //        ViewBag.LoginError = "نام کاربری وارد شده صحیح نیست.";
        //        //ModelState.AddModelError(string.Empty, "نام کاربری وارد شده صحیح نیست.");
        //        //return RedirectToAction("Login", "Auth", new { returnUrl = "نام کاربری یا رمز عبور وارد شده صحیح نیست." });
        //        return View(model);
        //    }

        //    var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal("/Customer/Dashboard"); // Or use returnUrl
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
        //        case SignInStatus.Failure:
        //        default:
        //            ViewBag.LoginError = "نام کاربری یا رمز عبور وارد شده صحیح نیست.";
        //            //ModelState.AddModelError("", "نام کاربری وارد شده صحیح نیست.");
        //            return View(model);
        //    }
        //}

        //[AllowAnonymous]
        //public ActionResult AccountRegisterSection()
        //{
        //    var model = new RegisterCustomerViewModel();

        //    return PartialView(model);
        //}

        ////
        //// POST: /Auth/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> AccountRegisterSection(RegisterCustomerViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        #region Check for duplicate username or email
        //        if (UserRepo.UserNameExists(model.UserName))
        //        {
        //            ViewBag.RegisterError = "نام کاربری قبلا ثبت شده.";
        //            //ModelState.AddModelError("", "نام کاربری قبلا ثبت شده");
        //            return View(model);
        //        }
        //        if (UserRepo.EmailExists(model.Email))
        //        {
        //            ViewBag.RegisterError = "ایمیل قبلا ثبت شده.";
        //            //ModelState.AddModelError("", "ایمیل قبلا ثبت شده");
        //            return View(model);
        //        }
        //        #endregion

        //        var user = new User { UserName = model.UserName, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
        //        UserRepo.CreateUser(user, model.Password);
        //        if (user.Id != null)
        //        {
        //            // Add Customer Role
        //            UserRepo.AddUserRole(user.Id, StaticVariables.CustomerRoleId);

        //            // Add Customer
        //            var customer = new Core.Models.Customer()
        //            {
        //                UserId = user.Id,
        //                IsDeleted = false,
        //                InsertDate = DateTime.Now
        //            };
        //            UserRepo.AddCustomer(customer);
        //            //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

        //            // For more information on how to enable Auth confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //            // Send an email with this link
        //            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            // var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            // await UserManager.SendEmailAsync(user.Id, "Confirm your Auth", "Please confirm your Auth by clicking <a href=\"" + callbackUrl + "\">here</a>");

        //            return RedirectToAction("Login", "Auth");
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}


        //
        // POST: /Auth/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterCustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                #region Check for duplicate username or email
                if (UserRepo.UserNameExists(model.UserName))
                {
                    ViewBag.RegisterError = "نام کاربری قبلا ثبت شده.";
                    //ModelState.AddModelError("", "نام کاربری قبلا ثبت شده");
                    return View(model);
                }
                if (UserRepo.EmailExists(model.Email))
                {
                    ViewBag.RegisterError = "ایمیل قبلا ثبت شده.";
                    //ModelState.AddModelError("", "ایمیل قبلا ثبت شده");
                    return View(model);
                }
                #endregion

                var user = new User { UserName = model.UserName, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
                UserRepo.CreateUser(user, model.Password);
                if (user.Id != null)
                {
                    // Add Customer Role
                    UserRepo.AddUserRole(user.Id, StaticVariables.CustomerRoleId);

                    // Add Customer
                    var customer = new Core.Models.Customer()
                    {
                        UserId = user.Id,
                        IsDeleted = false,
                        InsertDate = DateTime.Now
                    };
                    UserRepo.AddCustomer(customer);
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable Auth confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your Auth", "Please confirm your Auth by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Login", "Auth");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        //
        // GET: /Auth/ForgotPasswordConfirmation
        [HttpPost]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        // GET: /Account/ExternalLoginFailure
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";


        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Dashboard");
        }
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}