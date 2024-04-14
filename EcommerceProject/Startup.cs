using EcommerceProject.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EcommerceProject.Repository;
using EcommerceProject.Models;
using Microsoft.Extensions.Options;
using Microsoft.CodeAnalysis.Options;
using Stripe;
using EcommerceProject.Services;

namespace EcommerceProject
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddControllersWithViews();
            
            services.AddDbContext<EcommerceContext>(
                Options => Options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")
                ));
            services.AddIdentity<ApplicationUsers, IdentityRole>().
                AddEntityFrameworkStores<EcommerceContext>().AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = _configuration["Application:LoginPath"];
                config.Events.OnRedirectToLogout = context =>
                {
                    context.HttpContext.Session.Remove("products");
                    return Task.CompletedTask;
                };
            });

            services.Configure<StripeSettings>(_configuration.GetSection("StripeSettings"));

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 5;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;

                options.SignIn.RequireConfirmedEmail = true;
            });

            services.Configure<SMTPConfigModel>(_configuration.GetSection("SMTPConfig"));

            services.AddScoped<ProductRepository,ProductRepository > ();
            services.AddScoped<SpecialTagNameRepository,SpecialTagNameRepository> ();
            services.AddScoped<ProductAddRepository,ProductAddRepository> ();
            services.AddScoped<OrderRepository,OrderRepository> ();
            services.AddScoped<AccountRepository,AccountRepository> ();
            services.AddScoped<CartRepository,CartRepository> ();
            services.AddScoped<EmailService,EmailService> ();
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUsers>, ApplicationUserClaimsPrincipleFactory>();
        }

        public void Configure(IApplicationBuilder app,IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            StripeConfiguration.ApiKey = _configuration.GetSection("StripeSettings:SecretKey").Get<string>();
            
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                      name: "areas",
                      pattern: "{area=Customer}/{controller=Home}/{action=ShowAllRecords}/{id?}"
                    );
                });


            });
        }
    }
}
