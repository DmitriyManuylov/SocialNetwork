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

using SocialNetwork.Hubs;
using SocialNetwork.Models.UserInfoModels;
using SocialNetwork.Controllers;

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
            string connectionString = Configuration.GetConnectionString("PostgreSqlConnection");

            services.AddDbContext<SocialNetworkDbContext>(options => options.UseNpgsql(connectionString)).AddLogging();

            services.AddScoped<ILiteChatRoomsRepository, EFLiteChatRoomsRepository>();
            services.AddScoped<ISocialNetworkRepository, EFSocialNetworkRepository>();
            services.AddScoped<IUsersRepository, EFUsersRepository>();

            services.AddIdentity<NetworkUser, IdentityRole>(options =>
            {

            })
            .AddEntityFrameworkStores<SocialNetworkDbContext>()
            .AddDefaultTokenProviders();

            services.AddSignalR().AddJsonProtocol().AddMessagePackProtocol();

            services.AddControllersWithViews().AddNewtonsoftJson();

            
            _services = services;
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = new PathString("/Account/Login");
                options.LogoutPath = new PathString("/Chat/Chat");
                options.AccessDeniedPath = new PathString("/Chat/Chat");
            });
            services.AddAuthorization(options =>
            {

            });

            services.AddSingleton(_services);
            services.AddSession();
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
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "",
                    pattern: "User{Id:guid}",
                    defaults: new { controller = nameof(SocialNetworkController).Replace("Controller", ""), action = nameof(SocialNetworkController.UserPage) });
                endpoints.MapControllerRoute(
                    name: "",
                    pattern: "Chat{chatId:int}",
                    defaults: new {controller = nameof(SocialNetworkController).Replace("Controller", ""), action = nameof(SocialNetworkController.ChatPage) }
                    );
                endpoints.MapControllerRoute(
                    name: "",
                    pattern: "Messenger",
                    defaults: new { controller = nameof(SocialNetworkController).Replace("Controller", ""), action = nameof(SocialNetworkController.Index) });

                endpoints.MapControllerRoute(
                    name: "",
                    pattern: "{controller}/{action}/{chatId:int}");

                endpoints.MapControllerRoute(
                    name: "",
                    pattern: "{controller}/{action}/{calledUserId:guid}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Chat}/{action=Chat}/{id?}");


                endpoints.MapHub<ChatHub>("/chat");
                endpoints.MapHub<SocialNetworkHub>("/SocialNetwork");
            });

        }
    }
}
