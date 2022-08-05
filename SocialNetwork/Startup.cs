
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SocialNetwork.Models;
using SocialNetwork.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

namespace SocialNetwork
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        IServiceCollection _services;
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            string identityConnectionString = Configuration.GetConnectionString("IdentityConnection");

            services.AddDbContext<SocialNetworkDbContext>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<NetworkUsersDbContext>(options => options.UseSqlServer(identityConnectionString));
            services.AddScoped<ILiteChatRoomsRepository, EFLiteChatRoomsRepository>();

            services.AddIdentity<NetworkUser, IdentityRole>(options =>
            {
                //options.Password.
            })
            .AddEntityFrameworkStores<SocialNetworkDbContext>()
            .AddDefaultTokenProviders();

            services.AddSignalR().AddJsonProtocol().AddMessagePackProtocol();

            services.AddControllersWithViews().AddNewtonsoftJson();

            
            _services = services;
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = new PathString("/Account/Register");
                options.LogoutPath = new PathString("/Home/Index");
                options.AccessDeniedPath = new PathString("/Home/Index");
            });
            services.AddAuthorization(options =>
            {

            });

            services.AddSingleton(_services);

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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/chat");
            });

            //app.Run(async context =>
            //{

            //    var sb = new StringBuilder();
            //    sb.Append("<h1>Все сервисы</h1>");
            //    sb.Append("<table>");
            //    sb.Append("<tr><th>Тип</th><th>Lifetime</th><th>Реализация</th></tr>");
            //    foreach (var svc in _services)
            //    {
            //        sb.Append("<tr>");
            //        sb.Append($"<td>{svc.ServiceType.FullName}</td>");
            //        sb.Append($"<td>{svc.Lifetime}</td>");
            //        sb.Append($"<td>{svc.ImplementationType?.FullName}</td>");
            //        sb.Append("</tr>");
            //    }
            //    sb.Append("</table>");
            //    context.Response.ContentType = "text/html;charset=utf-8";
            //    await context.Response.WriteAsync(sb.ToString());
            //});
        }
    }
}
