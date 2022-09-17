using Godch.Models;
using Godch.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using sMsg = Godch.ServerMessage;
using MyLib;
using MyLib.Mvc;
using System.Security.Policy;

namespace Godch.Controllers
{
    public partial class AccountController : Controller
    {
        private readonly IOptionsMonitor<CookieAuthenticationOptions> _cookieAuthOptionsMonitor;
        private readonly string _folder;
        public AccountController(IOptionsMonitor<CookieAuthenticationOptions> cookieAuthOptions, IHostingEnvironment env)
        {
            _cookieAuthOptionsMonitor = cookieAuthOptions;
            // 把上傳目錄設為：wwwroot\UploadFolder
            _folder = $@"{env.WebRootPath}\_GodchData\UserData";
        }
        public IActionResult Index()
        {
            if (!IsLogin())
            {
                return HomePage();
            }
            return View(CurrentUser());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(NewUser newuser)
        {
            User user = new();
            if(newuser == null) { return View(newuser); }
            var acc = db.Users.Where(u => u.Account == newuser.Account).FirstOrDefault() != null;
            var eml = db.Users.Where(u => u.EmailAddress == newuser.EmailAddress).FirstOrDefault() != null;
            if (acc || eml)
            {
                if (acc) { ViewBag.AccErr = sMsg.InvalidAccount; }
                if (eml) { ViewBag.EmlErr = sMsg.InvalidEmail; }
                return View(newuser);
            }            
            user.Account = newuser.Account;
            user.Password = newuser.Password;
            user.FullName = newuser.FullName;
            user.UserName = newuser.NickName ?? user.FullName;
            user.EmailAddress = newuser.EmailAddress;
            user.Description = newuser.Description;
            user.Register = Time.TimeNowInt();            
            act.Create(user);
            AddDefaultPhoto(user.UserId);
            vLogin vlogin = new vLogin
            {
                Account = newuser.Account,
                Password = newuser.Password
            };
            await CreateThenLogin(vlogin);
            return RedirectToAction("Index", "Account");
        }
        [HttpPost]
        public async Task<IActionResult> CreateThenLogin(vLogin vLogin)
        {
            var uid = AccountHelper.LogDataCheck(vLogin);
            if (uid != null)
            {
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaims(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, uid),
                new Claim(ClaimTypes.Name, uid)
                });
                var principal = new ClaimsPrincipal(identity);
                // 登入
                var properties = new AuthenticationProperties
                {
                    IsPersistent = vLogin.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12),
                    AllowRefresh = true
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
            }
            return RedirectToAction("Index", "Account");
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(vLogin vLogin)
        {
            var uid = AccountHelper.LogDataCheck(vLogin);
            if (uid != null)
            {
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaims(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, uid),
                new Claim(ClaimTypes.Name, uid)
                });
                var principal = new ClaimsPrincipal(identity);
                // 登入
                var properties = new AuthenticationProperties
                {
                    IsPersistent = vLogin.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12),
                    AllowRefresh = true
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
            }
            return RedirectToAction("LoginStateVerify");
        }
        public IActionResult LoginStateVerify()
        {
            int login_state = 0;
            var suser = mCast.Cast(CurrentUser());
            if (suser != null)
            {
                login_state = 1;
                if (suser.Photo == null)
                {
                    suser.Photo = "/_GodchData/Img/member2.png";
                }
                return View(suser);
            }
            ViewBag.LoginState = login_state;
            return View(new sUser());
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [DisableFormValueModelBindingFilter]
        public async Task<IActionResult> PhotoUpload()
        {
            var user = CurrentUser();
            if(user == null) { return Redirect("~/Home"); }
            int imgCount = 0;
            GodchDirectories.UserData.AddFolderIfNotExist($"{user.UserId}");
            var formProvider = await Request.StreamFile(
                (file)=>
                {
                    if(file == null) { return Stream.Null; }
                    imgCount++;
                    return System.IO.File.Create($"{Config.UserData}\\{user.UserId}\\photo.jpg");
                }
                );
            return Ok("Success!");
        }
    }
}