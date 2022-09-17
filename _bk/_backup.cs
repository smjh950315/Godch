using MyLib;
using System.Xml;

namespace Godch
{
    //public class _backup
    //{
        //public void ConnectGroup(long chid)
        //{
        //    int? uid = Context.User == null || Context.User.Identity == null || Context.User.Identity.Name == null ?
        //        null : Convert.ToInt32(Context.User.Identity.Name);
        //    if (HubAction.IsGroupMember(chid, uid))
        //    {
        //        ChatConnection conn = new ChatConnection();
        //        conn.ConnectionID = Context.ConnectionId;
        //        conn.ChatID = chid;
        //        conn.UserID = uid.Value;
        //        conn.UserName = q.UserName(uid);
        //        Conns.Add(conn);
        //        Groups.AddToGroupAsync(Context.ConnectionId, conn.ChatID.ToString());
        //        string group = conn.ChatID.ToString();
        //        _ = new List<ChatMsg>();
        //        List<ChatMsg> msgs = HubAction.ReadData(group);
        //        foreach (ChatMsg msg in msgs)
        //        {
        //            string? uName = q.UserName(Convert.ToInt32(msg.User));
        //            string?[] UserName = { uName };
        //            string[] Message = { msg.Message };
        //            //Clients.Caller.SendAsync("ReceiveMessage", UserName, Message);
        //            Clients.Caller.SendAsync("ReceiveMessage", msg.User, UserName, Message);
        //        }
        //    }
        //}



        //public async void WorkSave(Work w, IFormFile file)
        //{
        //    if (file.Length > 0)
        //    {
        //        string rename = UserID() + "_" + w.WorkId + "_" + file.FileName;
        //        var fullPath = cfg.Works + rename;

        //        const int FILE_WRITE_SIZE = 84975;//寫出緩沖區大小
        //        int writeCount = 0;
        //        using (var stream = file.OpenReadStream())
        //        {
        //            var trustedFileNameForFileStorage = Path.GetRandomFileName();
        //            await WriteFileAsync(stream, fullPath);
        //        }
        //        w.FileUrl = rename;
        //        act.Update(w);
        //    }
        //}
        //public async Task<IActionResult> UploadingFormFile(IFormFile file)
        //{
        //    using (var stream = file.OpenReadStream())
        //    {
        //        var trustedFileNameForFileStorage = Path.GetRandomFileName();
        //        await WriteFileAsync(stream, Path.Combine(_targetFilePath, trustedFileNameForFileStorage));
        //    }
        //    return Created(nameof(MassUploadController), null);
        //}
        //public static async Task<int> WriteFileAsync(System.IO.Stream stream, string path)
        //{
        //    const int FILE_WRITE_SIZE = 84975;//寫出緩沖區大小
        //    int writeCount = 0;
        //    using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write, FILE_WRITE_SIZE, true))
        //    {
        //        byte[] byteArr = new byte[FILE_WRITE_SIZE];
        //        int readCount = 0;
        //        while ((readCount = await stream.ReadAsync(byteArr, 0, byteArr.Length)) > 0)
        //        {
        //            await fileStream.WriteAsync(byteArr, 0, readCount);
        //            writeCount += readCount;
        //        }
        //    }
        //    return writeCount;
        //}



        //#pragma warning disable ASP5001 // 類型或成員已經過時
        //            builder.Services.AddMvc().AddRazorPagesOptions(options =>
        //            {
        //                options.Conventions
        //                    .AddPageApplicationModelConvention("/StreamedSingleFileUploadDb",
        //                        model =>
        //                        {
        //                            model.Filters.Add(
        //                                new GenerateAntiforgeryTokenCookieAttribute());
        //                            model.Filters.Add(
        //                                new DisableFormValueModelBindingAttribute());
        //                        });
        //                options.Conventions
        //                    .AddPageApplicationModelConvention("/StreamedSingleFileUploadPhysical",
        //                        model =>
        //                        {
        //                            model.Filters.Add(
        //                                new GenerateAntiforgeryTokenCookieAttribute());
        //                            model.Filters.Add(
        //                                new DisableFormValueModelBindingAttribute());
        //                        });
        //            })
        //            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        //#pragma warning restore ASP5001 // 類型或成員已經過時
    //}
}
//using Microsoft.AspNetCore.Authentication;
//namespace Godch
//{
//    public class AuthenticationMiddleware
//    {
//        private readonly RequestDelegate _next;
//        public IAuthenticationSchemeProvider Schemes { get; private set; }
//        public AuthenticationMiddleware(RequestDelegate next, IAuthenticationSchemeProvider schemes)
//        {
//            _next = next;
//            Schemes = schemes ?? throw new ArgumentNullException(nameof(schemes));
//        }
//        public async Task Invoke(HttpContext context)
//        {
//            // 記錄原始路徑和原始基路徑
//            context.Features.Set<IAuthenticationFeature>(new AuthenticationFeature
//            {
//                OriginalPath = context.Request.Path,
//                OriginalPathBase = context.Request.PathBase
//            });

//            // 如果有顯式指定的身份認證方案，優先處理（這裡不用看，直接看下面）
//            var handlers = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
//            foreach (var scheme in await Schemes.GetRequestHandlerSchemesAsync()) 
//            {
//                var handler = await handlers.GetHandlerAsync(context, scheme.Name) as IAuthenticationRequestHandler;
//                if (handler != null && await handler.HandleRequestAsync())
//                {
//                    return;
//                }
//            }
//            // 使用預設的身份認證方案進行認證，並賦值 HttpContext.User
//            var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync();
//            if (defaultAuthenticate != null)
//            {
//                var result = await context.AuthenticateAsync(defaultAuthenticate.Name);
//                if (result?.Principal != null)
//                {
//                    context.User = result.Principal;
//                }
//            }
//            await _next(context);
//        }
//    }

//}
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Godch.Models;
//using Godch.ViewModels;
//using LibNetFramework.CSharp;
//using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

//namespace Godch.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [DisableFormValueModelBindingFilter]
//    public class FileController : ControllerBase
//    {
//        private readonly string _folder;
//        private readonly static Dictionary<string, string> _contentTypes = new Dictionary<string, string>
//        {
//            {".png", "image/png"},
//            {".jpg", "image/jpeg"},
//            {".jpeg", "image/jpeg"},
//            {".gif", "image/gif"}
//        };
//        public FileController(IHostingEnvironment env)
//        {
//            // 把上傳目錄設為：wwwroot\UploadFolder
//            _folder = $@"{env.WebRootPath}\_GodchData\Works";
//        }
//        [Route("large")]
//        [HttpPost]
//        [DisableFormValueModelBindingFilter]
//        public async Task<IActionResult> Upload()
//        {
//            var photoCount = 0;
//            var formValueProvider = await Request.StreamFile((file) =>
//            {
//                photoCount++;
//                return System.IO.File.Create($"{_folder}\\{file.FileName}");
//            });

//            var model = new uWork
//            {
//                WorkName = formValueProvider.GetValue("WorkName").ToString(),
//                UpLoadTime = _MyTime.TimeNowInt()
//            };

//            // ...

//            return Ok(new
//            {
//                WorkName = model.WorkName,
//                UpLoadTime = model.UpLoadTime.ToString()
//            });
//        }
//    }
//}
