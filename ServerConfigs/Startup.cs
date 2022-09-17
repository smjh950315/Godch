using Godch.Controllers;
using Godch.Event;
using Godch.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Godch
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public static void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = false;
                options.ForwardClientCertificate = false;
            });
            services.AddControllersWithViews();            
            services.AddRazorPages(op =>
            {
                op.Conventions.AddPageApplicationModelConvention("/StreamedSingleFileUploadPhysical", model => 
                {
                    model.Filters.Add(new GenerateAntiforgeryTokenCookieAttribute());
                    model.Filters.Add(new DisableFormValueModelBindingAttribute());
                });
            });

            services.AddSignalR();
            services.Configure<FormOptions>(options =>
            {
                options.BufferBodyLengthLimit = long.MaxValue;
                options.KeyLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = long.MaxValue;
                options.MultipartBoundaryLengthLimit = int.MaxValue;
                options.ValueCountLimit = int.MaxValue;
                options.ValueLengthLimit = int.MaxValue;
            });

            services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
                .Configure<Microsoft.AspNetCore.DataProtection.IDataProtectionProvider>((options, dp) =>
                {
                    options.LoginPath = new PathString("/Account/Login");
                    options.LogoutPath = new PathString("/Account/Logout");
                    options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                    options.ReturnUrlParameter = "returnUrl";

                    options.ExpireTimeSpan = TimeSpan.FromDays(14);
                                //options.Cookie.Expiration = TimeSpan.FromMinutes(30);
                                //options.Cookie.MaxAge = TimeSpan.FromDays(14);
                    options.SlidingExpiration = true;

                    options.Cookie.Name = "auth";
                                //options.Cookie.Domain = ".xxx.cn";
                    options.Cookie.Path = "/";
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Cookie.IsEssential = true;
                    options.CookieManager = new ChunkingCookieManager();

                    options.DataProtectionProvider ??= dp;
                    var dataProtector = options.DataProtectionProvider.CreateProtector(CookieAuthenticationDefaults.AuthenticationScheme);
                    options.TicketDataFormat = new Microsoft.AspNetCore.Authentication.TicketDataFormat(dataProtector);

                    options.Events.OnSigningIn = context =>
                    {
                        Console.WriteLine($"{context.Principal.Identity.Name} 正在登入...");
                        return Task.CompletedTask;
                    };

                    options.Events.OnSignedIn = context =>
                    {
                        Console.WriteLine($"{context.Principal.Identity.Name} 已登入");
                        return Task.CompletedTask;
                    };

                    options.Events.OnSigningOut = context =>
                    {
                        Console.WriteLine($"{context.HttpContext.User.Identity.Name} 登出");
                        return Task.CompletedTask;
                    };

                    options.Events.OnValidatePrincipal += context =>
                    {
                        Console.WriteLine($"{context.Principal.Identity.Name} 驗證 Principal");
                        return Task.CompletedTask;
                    };
                });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseExceptionHandler("/Home/Error");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseStaticFiles();
            app.UseRouting();            
            app.UseAuthentication();
            app.UseEndpoints(endpoints => {
                endpoints.MapHub<FunctionHub>("/functionhub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
                endpoints.MapRazorPages();                
            });
        }

    }


}
