using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskAuthenticationAuthorization.Models;

namespace TaskAuthenticationAuthorization
{
    public class Startup
    {
        public const string DiscountPolicy = "Discount";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ShoppingContext>(options => options.UseSqlServer(connection));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.Events.OnValidatePrincipal = RefreshPrincipalAsync;
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(DiscountPolicy, policy => policy
                    .RequireRole(Roles.Buyer)
                    .RequireClaim(BuyerTypes.ClaimType, BuyerTypes.Golden, BuyerTypes.Wholesale));
            });

            services.AddControllersWithViews();
        }

        // Keeps the cookie in sync with the database, so that role / buyerType changes
        // made by an admin take effect immediately, and deleted users are signed out.
        private static async Task RefreshPrincipalAsync(CookieValidatePrincipalContext context)
        {
            var idValue = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idValue, out var userId))
            {
                context.RejectPrincipal();
                return;
            }

            var db = context.HttpContext.RequestServices.GetRequiredService<ShoppingContext>();
            var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                context.RejectPrincipal();
                return;
            }

            var currentRole = context.Principal.FindFirstValue(ClaimTypes.Role);
            var currentType = context.Principal.FindFirstValue(BuyerTypes.ClaimType);
            var expectedType = user.Role == Roles.Buyer ? user.BuyerType : null;
            if (currentRole != user.Role || currentType != expectedType)
            {
                context.ReplacePrincipal(UserPrincipalFactory.Create(user));
                context.ShouldRenew = true;
            }
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
            });
        }
    }
}
