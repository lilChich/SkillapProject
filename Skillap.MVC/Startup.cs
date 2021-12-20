using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Skillap.BLL;
using Skillap.BLL.Infrastructure.Security;
using Skillap.BLL.Interfaces.IServices;
using Skillap.BLL.Interfaces.JWT;
using Skillap.BLL.Service;
using Skillap.BLL.User.Login;
using Skillap.DAL.EF;
using Skillap.DAL.Entities;
using Skillap.DAL.Interfaces;
using Skillap.DAL.Repositories;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.Google;
using AutoMapper;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Skillap.MVC
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
            //services.AddMediatR(typeof(LoginHandler).Assembly);
            services.AddAutoMapper(Assembly.Load("Skillap.MVC"), Assembly.Load("Skillap.DAL"));
            services.AddAutoMapper(Assembly.Load("Skillap.BLL"), Assembly.Load("Skillap.DAL"));

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages();


            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUsers, ApplicationRole>(options =>
            {
                options.User.AllowedUserNameCharacters = null;
                options.SignIn.RequireConfirmedAccount = false;

            })
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.MaxFailedAccessAttempts = 3;

                options.User.AllowedUserNameCharacters = null;
                options.User.RequireUniqueEmail = true;
            });


            services.AddScoped<IAuthService, UserService>();
            services.AddScoped<IMasterService, UserService>();

            services.AddScoped<IUnitOfWork, EFUnitOfWork>();
            services.AddScoped<UserManager<ApplicationUsers>>();
            services.AddScoped<SignInManager<ApplicationUsers>>();
            services.AddScoped<RoleManager<ApplicationRole>>();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"]));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    opt =>
                    {
                            opt.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = key,
                                ValidateAudience = false,
                                ValidateIssuer = false
                            };
                    }
                );

            services.AddScoped<IJwtGenerator, JwtGenerator>();

            services.AddAuthentication()
                .AddGoogle(options =>
                {

                    options.ClientId = "292451837131-ag81bku39pauhgs0ln0oc35ei57l2vhu.apps.googleusercontent.com";
                    options.ClientSecret = "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";
                    options.Scope.Add("profile");
                    options.Events.OnCreatingTicket = (context) =>
                    {
                        var picture = context.User.GetProperty("picture").GetString();

                        context.Identity.AddClaim(new Claim("image", picture));

                        return Task.CompletedTask;
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
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
               /* endpoints.MapControllerRoute(
                   name: "Identity",
                   pattern: "{area:exists}/{controller=MainAccount}/{action=Index}/{id?}");*/

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
