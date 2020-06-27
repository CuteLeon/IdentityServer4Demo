using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MVCClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // 清除自带的 Claim 映射配置
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "MyCookies";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("MyCookies")
                .AddOAuth("oidc", options =>
                {
                    options.SignInScheme = "MyCookies";
                    options.AuthorizationEndpoint = "https://localhost:5000";
                    options.TokenEndpoint = "https://localhost:5000/token";
                    options.ClientId = "MVCClient";
                    options.ClientSecret = "6DDB3EB5-BFA3-41C7-B158-DE475548CC00";
                    options.SaveTokens = true;
                    options.CallbackPath = "/";
                    options.Scope.Clear();
                    options.Scope.Add("api1");
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
