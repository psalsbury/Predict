using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Predict.Helper;
using Predict.Models;
using System;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using NLog;
using Predict.GoogleCaptcha;
using Predict.ViewModels;
using Quartz;
using RegisterViewModel = Predict.ViewModels.RegisterViewModel;

namespace Predict.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly NLog.Logger _logger;

        public AccountController()
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set => _signInManager = value;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        // GET: Fixtures
        public ActionResult UpdateAccount()
        {
            var context = new ApplicationDbContext();
            var userId = User.Identity.GetUserId();
            var player = context.Players.FirstOrDefault(p => p.Id == userId);
            var user = context.Users.FirstOrDefault(p => p.Id == userId);

            var registerViewModel = new UpdateRegisterViewModel()
            {
                Id = player.Id,
                DisplayName = player.DisplayName,
                Email = user.Email,
                SupportTeamId = player.SupportTeamId,
                Teams = context.Teams.ToList()
            };
            return View("UpdateRegister", registerViewModel);
        }

        // GET: /Account/RegisterSendCodeNotification
        [AllowAnonymous]
        public ActionResult RegisterSendCodeNotification()
        {
            return View();
        }


        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {

                if (!ModelState.IsValid) return View(model);
                var captchaResponse = Request["g-recaptcha-response"];
                var response = ValidateCaptcha(captchaResponse);
                if (!response)
                {
                    ModelState.AddModelError("", "Please Complete Google Captcha");
                    return View(model);
                }

                var user = UserManager.FindByEmail(model.Email);

                if (user != null && !UserManager.IsEmailConfirmed(user.Id))
                {
                    ModelState.AddModelError("", "Email has not been confirmed");
                    return View(model);
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result =
                    await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                switch (result)
                {
                    case SignInStatus.Success:
                        _logger.Info("Account - Login (HttpPost) - {0} logged in", model.Email);
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new {ReturnUrl = returnUrl, model.RememberMe});
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            }

            catch (Exception e)
            {
                _logger.Log(LogLevel.Info, model.Email + " has errored logging in. Error is as follows --> " + e.Message);

                throw;
            }
        }

        //
    // GET: /Account/VerifyCode
    [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync()) return View("Error");
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe,
                model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            var context = new ApplicationDbContext();
            var registerViewModel = new RegisterViewModel
            {
                Events = context.Events.Where(a => a.StartDateTime >= DateTime.UtcNow).ToList()
            };
            context.Dispose();
            return View(registerViewModel);
        }


        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateRegister(UpdateRegisterViewModel model)
        {

            var context = new ApplicationDbContext();
            var userId = User.Identity.GetUserId();
            var player = context.Players.FirstOrDefault(p => p.Id == model.Id);

            if (player != null && (player.DisplayName != model.DisplayName | player.SupportTeamId != model.SupportTeamId))
            {

                var nbrWithSameDisplayName = context.Players.Count(a => a.DisplayName == model.DisplayName && a.Id != model.Id);
                if (nbrWithSameDisplayName != 0)
                {
                    ModelState.AddModelError("","This display name is already taken");
                    return View("UpdateRegister", model);
                }

                player.DisplayName = model.DisplayName;
                player.SupportTeamId = model.SupportTeamId;
                context.Players.AddOrUpdate(player);
                context.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {

            try
            {

                var context = new ApplicationDbContext();

                _logger.Log(LogLevel.Info, model.Email + " has registered");

                var captchaResponse = Request["g-recaptcha-response"];
                var response = ValidateCaptcha(captchaResponse);
                if (!response)
                {
                    ModelState.AddModelError("Captcha", "Please Complete Google Captcha");
                    model.Events = context.Events.Where(a => a.StartDateTime >= DateTime.UtcNow).ToList();
                    return View("Register", model);
                }

                var confirmEmailAddress = System.Convert.ToBoolean(ConfigurationManager.AppSettings["ConfirmEmailOnRegister"]);
                
                if(model.Email.Contains("thinkmoney.co.uk"))
                    confirmEmailAddress = false;

                if (ModelState.IsValid)
                {
                    if (model.Password != model.ConfirmPassword)
                    {
                        ModelState.AddModelError("Password", "The password and confirmation password do not match.");
                        model.Events = context.Events.Where(a => a.StartDateTime >= DateTime.UtcNow).ToList();
                        return View("Register", model);
                    }

                    var nbrWithSameDisplayName = context.Players.Count(a => a.DisplayName == model.DisplayName);
                    if (nbrWithSameDisplayName != 0)
                    {
                        ModelState.AddModelError("Password", "This display name is already taken");
                        model.Events = context.Events.Where(a => a.StartDateTime >= DateTime.UtcNow).ToList();
                        return View("Register", model);
                    }

                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email, EmailConfirmed = !confirmEmailAddress };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {

                        _logger.Log(LogLevel.Info, model.Email + " Event id = " + model.EventId);
                        var myEvent = context.Events.FirstOrDefault(a => a.Id == model.EventId);

                        var player = new Player
                        {
                            Id = user.Id
                            , DisplayName = model.DisplayName
                            , PlayerName = model.DisplayName // both the same//
                            , CreatedDateTime = DateTime.UtcNow
                            , ModifiedDateTime = DateTime.UtcNow
                        };

                        if (confirmEmailAddress)
                            player.EmailConfirmedDateTime = DateTime.MinValue;

                        _logger.Log(LogLevel.Info, model.Email + " Adding player, Display name = " + model.DisplayName);

                        context.Players.Add(player);

                        if (myEvent != null)
                        {
                            var eventPlayer = new EventPlayer
                            {
                                EventId = model.EventId,
                                PlayerId = user.Id,
                                Enabled = true,
                                CreatedDateTime = DateTime.UtcNow,
                                ModifiedDateTime = DateTime.UtcNow
                            };
                            context.EventPlayers.Add(eventPlayer);

                            var defaultPoolId = myEvent.DefaultPoolId;
                            if (defaultPoolId > 0)
                            {
                                // Add the player to be associated to the global pool
                                var poolPlayer = new PoolPlayer
                                {
                                    Enabled = true,
                                    CreatedDateTime = DateTime.UtcNow,
                                    ModifiedDateTime = DateTime.UtcNow,
                                    PlayerId = user.Id,
                                    PoolId = defaultPoolId
                                };
                                context.PoolPlayers.Add(poolPlayer);

                                var globalPoolPlayer = new EventPoolPlayer
                                {
                                    PoolId = defaultPoolId,
                                    PlayerId = user.Id,
                                    EventId = model.EventId,
                                    AdminApprovedDateTime = DateTime.UtcNow,
                                    Enabled = true,
                                    CreatedDateTime = DateTime.UtcNow,
                                    ModifiedDateTime = DateTime.UtcNow
                                };
                                context.EventPoolPlayers.Add(globalPoolPlayer);
                            }
                        }

                        _logger.Log(LogLevel.Info, model.Email + " player saved ok");

                        // If this is me, then the pool will not yet have been created
                        if (user.Email == "pete@salsbury.co.uk")
                        {
                            UserManager.AddToRole(user.Id, "Admin");
                        }
                        else
                        {
                            UserManager.AddToRole(user.Id, "Player");
                        }

                        _logger.Log(LogLevel.Info, model.Email + " role saved ok");

                        await Task.Run(() => context.SaveChanges());
                        context.Dispose();

                        _logger.Log(LogLevel.Info, model.Email + " changes saved to database ok");

                            _logger.Log(LogLevel.Info, model.Email + " confirm email address = " + confirmEmailAddress.ToString());

                            if (confirmEmailAddress)
                        {
                            // Send an email with this link
                            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code },
                                Request.Url.Scheme);
                            await UserManager.SendEmailAsync(user.Id, "Confirm your account",
                                "Thank you for registering  with predictioncomp.com.<br><br>Please confirm your account by clicking this link <a href=\"" + callbackUrl + "\">" + callbackUrl + "</a>");
                            return RedirectToAction("RegisterSendCodeNotification", "Account");
                        }
                        else
                        {
                            await SignInManager.SignInAsync(user, false, false);
                        }

                        return RedirectToAction("Index", "Home");
                    }

                    AddErrors(result);
                }

                model.Events = context.Events.Where(a => a.StartDateTime >= DateTime.UtcNow).ToList();
                return View("Register", model);

            }

            catch (Exception e)
            {
                _logger.Log(LogLevel.Info,model.Email + " has errored registering. Error is as follows --> " + e.Message);
                throw;
            }

        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            try
            {

                _logger.Log(LogLevel.Info, "confirming email starting");
                _logger.Log(LogLevel.Info, "userid --> " + userId);
                _logger.Log(LogLevel.Info, "code --> " + code);

                if (userId == null || code == null) return View("Error");

                _logger.Log(LogLevel.Info, "both vars are not null");
                var result = await UserManager.ConfirmEmailAsync(userId, code);
                if (result.Succeeded)
                {
                    _logger.Log(LogLevel.Info, "email confirmed");
                    // If user successfully clicked on the email link to activate account, then set the db
                    var context = new ApplicationDbContext();

                    var player = context.Players.FirstOrDefault(a => a.Id == userId);
                    if (player != null)
                    {
                        player.EmailConfirmedDateTime = DateTime.UtcNow;
                        player.ModifiedDateTime = DateTime.UtcNow;
                        context.Players.AddOrUpdate(player);
                        await Task.Run(() => context.SaveChanges());
                    }

                    context.Dispose();
                    return View("ConfirmEmail");
                }
                else
                {
                    _logger.Log(LogLevel.Info, "email not confirmed");
                    _logger.Log(LogLevel.Info, result.Errors.First);
                    return View("Error");
                }

            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Info,  e.Message);
                throw;
            }
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult ResendEmailToken(string returnUrl)
        {
            var resendEmailTokenModel = new ResendEmailTokenModel();
            ViewBag.ReturnUrl = returnUrl;
            return View(resendEmailTokenModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResendEmailToken(ResendEmailTokenModel resendEmailTokenModel)
        {
           var user = UserManager.FindByName(resendEmailTokenModel.Email);
            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    string code = UserManager.GenerateEmailConfirmationToken(user.Id);

                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code },
                        Request.Url.Scheme);
 
                    UserManager.SendEmail(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>.");
                    return RedirectToAction("RegisterSendCodeNotification", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Email Address already confirmed");
                    return View(resendEmailTokenModel);
                }
            }
            else
            {
                ModelState.AddModelError("", "Email Address Not Found");
                return View(resendEmailTokenModel);
            }
        }

        //private void SendEmailConfirmationToken(string email);
        //{
        //    var user = UserManager.FindByName(email);
        //    //if (user != null)
        //    //{
        //    //    if (!user.EmailConfirmed)
        //    //    {
        //    //        string code = manager.GenerateEmailConfirmationToken(user.Id);
        //    //        string callbackUrl = IdentityHelper.GetUserConfirmationRedirectUrl(code, user.Id, Request);
        //    //        manager.SendEmail(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>.");

        //    //        FailureText.Text = "Confirmation email sent. Please view the email and confirm your account.";
        //    //        ErrorMessage.Visible = true;
        //    //        ResendConfirm.Visible = false;
        //    //    }
        //    //}
        //}

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !await UserManager.IsEmailConfirmedAsync(user.Id)) return View("EmailNotFound");

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code },
                    Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password",
                    "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null) return RedirectToAction("ResetPasswordConfirmation", "Account");
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded) return RedirectToAction("ResetPasswordConfirmation", "Account");
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider,
                Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null) return View("Error");
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose })
                .ToList();
            return View(new SendCodeViewModel
            { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid) return View();

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider)) return View("Error");
            return RedirectToAction("VerifyCode",
                new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null) return RedirectToAction("Login");

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation",
                        new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model,
            string returnUrl)
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Manage");

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null) return View("ExternalLoginFailure");
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, false, false);
                        return RedirectToLocal(returnUrl);
                    }
                }

                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            try
            {
                // Clear session variables
                SessionHelper.ClearSessionVariables(Session);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, "ERROR LOGGING OUT " + e.InnerException);
                throw;
            }
            finally
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            }
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

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

        public bool ValidateCaptcha(string response)
        {
            try
            {

                bool isLocal = HttpContext.Request.IsLocal;
                if (isLocal)
                {
                    return true;
                }
                else
                {
                    string secret = "6LdmK4IaAAAAAKASSz-RCWhCPPPSP9demqX0sGu0";

                    var client = new WebClient();
                    var reply = client.DownloadString(string.Format(
                        "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

                    GoogleCaptchaResponse myDeserializedClass =
                        JsonConvert.DeserializeObject<GoogleCaptchaResponse>(reply);
                    _logger.Log(LogLevel.Info, reply);

                    return myDeserializedClass.success;
                }
            }
            catch (Exception e)
            {
                {
                    _logger.Log(LogLevel.Info, " Google captcha has failed. Error is as follows --> " + e.Message);

                    return true;
                }
            }
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors) ModelState.AddModelError("", error);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
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
                if (UserId != null) properties.Dictionary[XsrfKey] = UserId;
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
    }
}