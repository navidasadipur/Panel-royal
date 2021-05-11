//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Mvc;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin.Security;
//using Newtonsoft.Json;
//using SpadStorePanel.Core.Models;
//using SpadStorePanel.Core.Utility;
//using SpadStorePanel.Infrastructure.Repositories;
//using SpadStorePanel.Web.Providers;
//using SpadStorePanel.Web.ViewModels;

//namespace SpadStorePanel.Web.Areas.Customer.Controllers
//{
//    [Authorize]
//    public class AuthController : Controller
//    {
//        private ApplicationSignInManager _signInManager;
//        private ApplicationUserManager _userManager;
//        private UsersRepository _userRepo;
//        private SMSLogRepository _smsLogRepo;

//        public AuthController()
//        {
//            if (_smsLogRepo == null)
//                _smsLogRepo = new SMSLogRepository(new Infrastructure.MyDbContext(), new LogsRepository(new Infrastructure.MyDbContext()));
//            if (_userRepo == null)
//                _userRepo = new UsersRepository();
//        }

//        public AuthController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, UsersRepository userRepo, SMSLogRepository smsLogRepository)
//        {
//            UserManager = userManager;
//            SignInManager = signInManager;
//            UserRepo = userRepo;
//            _smsLogRepo = smsLogRepository;

//            if (_smsLogRepo == null)
//                _smsLogRepo = new SMSLogRepository(new Infrastructure.MyDbContext(), new LogsRepository(new Infrastructure.MyDbContext()));
//            if(_userRepo == null)
//                _userRepo = new UsersRepository();
//        }
//        public UsersRepository UserRepo
//        {
//            get
//            {
//                return _userRepo ?? new UsersRepository();
//            }
//            private set
//            {
//                _userRepo = value;
//            }
//        }
//        public ApplicationSignInManager SignInManager
//        {
//            get
//            {
//                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
//            }
//            private set
//            {
//                _signInManager = value;
//            }
//        }

//        public ApplicationUserManager UserManager
//        {
//            get
//            {
//                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
//            }
//            private set
//            {
//                _userManager = value;
//            }
//        }

//        [HttpGet]
//        [AllowAnonymous]
//        public string SendCode(string phone="")
//        {
//            if(string.IsNullOrEmpty(phone))
//            {
//                return "invalid";
//            }

//            string code = GenerateConfirmCode();

//            // check if this phone number exists
//            var phoneNumberExists = _userRepo.PhoneNumberExists(phone);
//            var user = new User { UserName = phone, PhoneNumber = phone, FirstName = "", LastName = "", VerificationCode = code };
//            if (!phoneNumberExists)
//            {
//                // add a new user
//                UserRepo.CreateUser(user, code);

//                if (user.Id != null)
//                {
//                    // Add Customer Role
//                    UserRepo.AddUserRole(user.Id, StaticVariables.CustomerRoleId);

//                    // Add Customer
//                    var customer = new Core.Models.Customer()
//                    {
//                        UserId = user.Id,
//                        IsDeleted = false
//                    };
//                    UserRepo.AddCustomer(customer);
//                }
//            }
//            else
//            {
//                _userRepo.UpdateVerificationCode(user, code);
//            }

//            // generate a code and send by sms

//            SMSLog smsLog = new SMSLog();
//            smsLog.ReceiverMobileNo = phone;
//            smsLog.MessageBody = "گالری سه شین\n";
//            smsLog.MessageBody += "کد تایید: " + code;
//            smsLog.SendDateTime = DateTime.Now;
//            smsLog.IsFlash = false;
//            smsLog.PatternCode = "";
//            smsLog.LineNumber = ConfigurationManager.AppSettings.Get("LineNumber");



//            SMSDotIrProvider sMSDotIrProvider = new SMSDotIrProvider();
//            sMSDotIrProvider.SendSMS(ref smsLog);

//            _smsLogRepo.Add(smsLog);

//            return "ok";
//        }

//        [AllowAnonymous]
//        public ActionResult AccountLogin(string returnUrl)
//        {
//            ViewBag.ReturnUrl = returnUrl;
//            return View();
//        }


//        [HttpPost]
//        [AllowAnonymous]
//        public async Task<string> AccountLogin(LoginRegisterModel model, string returnUrl)
//        {
//            if (!ModelState.IsValid)
//            {
//                return JsonConvert.SerializeObject("invalid");
//            }

//            var user = new User { PhoneNumber = model.Phone, VerificationCode = model.ConfirmCode };
//            var res = _userRepo.CheckVerificationCode(user);
//            if(res)
//            {
//                user = _userRepo.GetUserByPhoneNumber(model.Phone);
//                if (user != null)
//                {
//                    SpadStorePanel.Web.Models.ApplicationUser appUser = new SpadStorePanel.Web.Models.ApplicationUser();
//                    appUser.UserName = user.UserName;
//                    appUser.Id = user.Id;
//                    await SignInManager.SignInAsync(appUser, true, true);
//                    return JsonConvert.SerializeObject("ok"); // Or use returnUrl
//                }
//                else
//                {
//                    ModelState.AddModelError("", "کد تایید وارد شده صحیح نیست.");
//                    return JsonConvert.SerializeObject("invalid code");
//                }
//            }
//            else
//            {
//                ModelState.AddModelError("", "کد تایید وارد شده صحیح نیست.");
//                return JsonConvert.SerializeObject("invalid code");
//            }

            
//        }

//        [AllowAnonymous]
//        public ActionResult Confirm()
//        {
//            return View();
//        }

//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public ActionResult Confirm(ConfirmPhoneModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(model);
//            }



//            return View();
//        }



//        //
//        // GET: /Auth/Login
//        [AllowAnonymous]
//        public ActionResult Login(string returnUrl)
//        {
//            ViewBag.ReturnUrl = returnUrl;
//            return View();
//        }
//        [AllowAnonymous]
//        public ActionResult AccessDenied(string returnUrl = null)
//        {
//            ViewBag.ReturnUrl = "/Admin/Dashboard/Index";
//            if (!string.IsNullOrEmpty(returnUrl))
//                ViewBag.ReturnUrl = returnUrl;
//            return View();
//        }
//        //
//        // POST: /Auth/Login
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Login(ViewModels.LoginViewModel model, string returnUrl)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(model);
//            }

//            // This doesn't count login failures towards Auth lockout
//            // To enable password failures to trigger Auth lockout, change to shouldLockout: true
//            var user = UserManager.FindByName(model.UserName);
//            if (user == null)
//            {
//                ModelState.AddModelError("", "نام کاربری وارد شده صحیح نیست.");
//                return View(model);
//            }

//            var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: false);
//            switch (result)
//            {
//                case SignInStatus.Success:
//                    {
//                        if (!string.IsNullOrEmpty(returnUrl))
//                            return RedirectToLocal(returnUrl); // Or use returnUrl

//                        return RedirectToLocal("/Customer/Dashboard"); // Or use returnUrl
//                    }
//                case SignInStatus.LockedOut:
//                    return View("Lockout");
//                case SignInStatus.RequiresVerification:
//                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
//                case SignInStatus.Failure:
//                default:
//                    ModelState.AddModelError("", "نام کاربری وارد شده صحیح نیست.");
//                    return View(model);
//            }
//        }

//        //
//        // GET: /Auth/Register
//        [AllowAnonymous]
//        public ActionResult Register()
//        {
//            return View();
//        }

//        //
//        // POST: /Auth/Register
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Register(RegisterCustomerViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                #region Check for duplicate username or email
//                if (UserRepo.UserNameExists(model.UserName))
//                {
//                    ModelState.AddModelError("", "نام کاربری قبلا ثبت شده");
//                    return View(model);
//                }
//                if (UserRepo.EmailExists(model.Mobile))
//                {
//                    ModelState.AddModelError("", "شماره موبایل قبلا ثبت شده");
//                    return View(model);
//                }
//                #endregion

//                var user = new User { UserName = model.UserName, PhoneNumber = model.Mobile, FirstName = model.FirstName, LastName = model.LastName };
//                var password = GeneratePassword();
//                UserRepo.CreateUser(user, password);
//                if (user.Id != null)
//                {
//                    // Add Customer Role
//                    UserRepo.AddUserRole(user.Id, StaticVariables.CustomerRoleId);

//                    // Add Customer
//                    var customer = new Core.Models.Customer()
//                    {
//                        UserId = user.Id,
//                        IsDeleted = false
//                    };
//                    UserRepo.AddCustomer(customer);
//                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

//                    // For more information on how to enable Auth confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
//                    // Send an email with this link
//                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
//                    // var callbackUrl = Url.Action("https://www.youtube.com/watch?v=5XA4Z-SOif8", "Auth", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
//                    // await UserManager.SendEmailAsync(user.Id, "Confirm your Auth", "Please confirm your Auth by clicking <a href=\"" + callbackUrl + "\">here</a>");

//                    SMSLog smsLog = new SMSLog();
//                    smsLog.ReceiverMobileNo = model.Mobile;
//                    smsLog.MessageBody = "گالری سه شین\n";
//                    smsLog.MessageBody += "پسورد شما: "+ password;
//                    smsLog.MessageBody += "\nدر صورت تمایل می توانید از پنل کاربری پسورد خود را تغییر دهید";
//                    smsLog.SendDateTime = DateTime.Now;
//                    smsLog.IsFlash = false;
//                    smsLog.PatternCode = "";
//                    smsLog.LineNumber = ConfigurationManager.AppSettings.Get("LineNumber");



//                    SMSDotIrProvider sMSDotIrProvider = new SMSDotIrProvider();
//                    sMSDotIrProvider.SendSMS(ref smsLog);

//                    _smsLogRepo.Add(smsLog);

//                    return RedirectToAction("Login", "Auth");
//                }
//            }

//            // If we got this far, something failed, redisplay form
//            return View(model);
//        }
//        //
//        // GET: /Auth/ForgotPasswordConfirmation
//        [HttpPost]
//        public ActionResult LogOff()
//        {
//            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
//            return RedirectToAction("Index", "Home", new { area = "" });
//        }
//        // GET: /Account/ExternalLoginFailure
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                if (_userManager != null)
//                {
//                    _userManager.Dispose();
//                    _userManager = null;
//                }

//                if (_signInManager != null)
//                {
//                    _signInManager.Dispose();
//                    _signInManager = null;
//                }
//            }

//            base.Dispose(disposing);
//        }

//        #region Helpers
//        private string GeneratePassword()
//        {
//            string password = "";

//            var bytes = Guid.NewGuid().ToByteArray();

//            password = BitConverter.ToString(bytes).Replace("-", "").Substring(0,6).ToLower();

//            return password;
//        }

//        private string GenerateConfirmCode(int length=6)
//        {
//            string code = "";

//            var bytes = Guid.NewGuid().ToByteArray();

//            foreach (var number in bytes)
//            {
//                if (code.Length >= length)
//                    break;

//                code += (number % 10).ToString();
//            }

//            return code;
//        }

//        // Used for XSRF protection when adding external logins
//        private const string XsrfKey = "XsrfId";

//        private IAuthenticationManager AuthenticationManager
//        {
//            get
//            {
//                return HttpContext.GetOwinContext().Authentication;
//            }
//        }

//        private void AddErrors(IdentityResult result)
//        {
//            foreach (var error in result.Errors)
//            {
//                ModelState.AddModelError("", error);
//            }
//        }

//        private ActionResult RedirectToLocal(string returnUrl)
//        {
//            if (Url.IsLocalUrl(returnUrl))
//            {
//                return Redirect(returnUrl);
//            }
//            return RedirectToAction("Index", "Dashboard");
//        }
//        internal class ChallengeResult : HttpUnauthorizedResult
//        {
//            public ChallengeResult(string provider, string redirectUri)
//                : this(provider, redirectUri, null)
//            {
//            }

//            public ChallengeResult(string provider, string redirectUri, string userId)
//            {
//                LoginProvider = provider;
//                RedirectUri = redirectUri;
//                UserId = userId;
//            }

//            public string LoginProvider { get; set; }
//            public string RedirectUri { get; set; }
//            public string UserId { get; set; }

//            public override void ExecuteResult(ControllerContext context)
//            {
//                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
//                if (UserId != null)
//                {
//                    properties.Dictionary[XsrfKey] = UserId;
//                }
//                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
//            }
//        }
//        #endregion
//    }
//}