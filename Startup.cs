using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RIAT.DAL.Entity.Models;
using RIAT.DAL.Entity.Data;
using RIAT.UI.Web.Data;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using RIAT.UI.Web.Models;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using RIAT.UI.Web.Services;
using Chat.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Collections;

namespace RIAT.UI.Web
{
    public class Startup
    {
        private static IList<CultureInfo> supportedCultures = new[] { new CultureInfo("pt-BR"), new CultureInfo("es-AR"), new CultureInfo("en-US") };
        private static CultureInfo defaultCulture = new CultureInfo("es-AR");

        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<RIATContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                //options.Password.RequireDigit = false;
                //options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddRoleManager<RoleManager<IdentityRole>>()
            .AddRoles<IdentityRole>()
            .AddDefaultUI()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            //services.AddAuthorization();
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //});

            //services.AddAuthorization(options =>
            //{
            //    IDictionary policyClaims = new Dictionary<string, string> ();
            //    // set up your policies here - what are the claims under "identity_roles"
            //    // (or other claim type) you want?
            //    foreach (var policyClaim in policyClaims)
            //    {
            //        options.AddPolicy(policyClaim.Key, policy => policy.RequireClaim("identity_roles", policyClaim.Value));
            //    }
            //});

            //services.AddIdentityServerAuthentication(options =>
            //{
            //    options.Authority = identityUrl;
            //    options.RequireHttpsMetadata = false;
            //    options.TokenRetriever = new Func<HttpRequest, string>(req => {
            //        var fromHeader = TokenRetrieval.FromAuthorizationHeader();
            //        var fromQuery = TokenRetrieval.FromQueryString();
            //        return fromHeader(req) ?? fromQuery(req);
            //    });
            //});

            services.AddSignalR();

            //services.AddAuthentication().AddFacebook(facebookOptions =>
            //{
            //    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
            //    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            //});

            //services.AddAuthentication().AddGoogle(options =>
            //{
            //    options.ClientId = Configuration["Authentication:Google:ClientId"];
            //    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            //});

            //services.AddAuthentication().AddLinkedIn(options =>
            //{
            //    options.ClientId = Configuration["Authentication:LinkedIn:ClientId"];
            //    options.ClientSecret = Configuration["Authentication:LinkedIn:ClientSecret"];
            //    options.Events = new OAuthEvents()
            //    {
            //        OnRemoteFailure = loginFailureHandler =>
            //            {
            //                var authProperties = options.StateDataFormat.Unprotect(loginFailureHandler.Request.Query["state"]);
            //                loginFailureHandler.Response.Redirect("/Account/login");
            //                loginFailureHandler.HandleResponse();
            //                return Task.FromResult(0);
            //            }
            //    };
            //});

            // using Microsoft.AspNetCore.Identity.UI.Services;
            services.AddTransient<IMyEmailSender, Services.EmailSender>();
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            //services.AddSingleton<Services.IEmailSender, Services.EmailSender>();

            #region Localization
            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
            services.Configure<RequestLocalizationOptions>(opts =>
            {
                opts.DefaultRequestCulture = new RequestCulture(defaultCulture.Name);
                opts.SupportedCultures = supportedCultures;
                opts.SupportedUICultures = supportedCultures;
                opts.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider()
                };
            });

            // Add framework services.
            services.AddControllersWithViews()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix,
                    opts => { opts.ResourcesPath = "Resources"; })
                .AddDataAnnotationsLocalization();
            #endregion

            services.AddControllers();

            services.AddCors(options =>
              options.AddPolicy("CorsPolicy",
                  builder => builder
                      //.WithOrigins("http://localhost/", "http://localhost/RIAT.UI.Web/", "http://rism.com.br/")
                      //.AllowAnyOrigin()
                      .SetIsOriginAllowed(_ => true)
                      .AllowAnyMethod()
                      .AllowAnyHeader()                      
                      .AllowCredentials()));

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

           

            //services.AddSignalR(o =>
            //{
            //    o.EnableDetailedErrors = true;
            //    o.MaximumReceiveMessageSize = 102400000;
            //});

            services.AddRazorPages();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [Obsolete]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            SetUpLocalization(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseCors("CorsPolicy");

            //app.UseSignalR(routes =>
            // {
            //     routes.MapHub<ChatHub>("/chatHub");
            // });

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(name: "default", 
            //                                 pattern: "{controller=Home}/{action=Index}/{id?}");
            //    endpoints.MapRazorPages();
            //});


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHubNovo>("/chat");
                endpoints.MapControllerRoute(name: "default",
                                             pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }

        private static void SetUpLocalization(IApplicationBuilder app)
        {
            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture, defaultCulture),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,

            };
            app.UseRequestLocalization(options);

            // Configure the Localization middleware
            app.UseRequestLocalization(options);
        }
    }
}
