
using HoardingManagement.Interface;
using HoardingManagement.Repository;

namespace Hoarding_managment
{
    public class Program
    {
        public IConfiguration Configuration { get; }
       // private string _connectionString;
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

           var _connectionString =builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContextPool<db_hoarding_managementContext>(options =>
             options.UseMySql(
                        _connectionString,
                        ServerVersion.AutoDetect(_connectionString)
            ));

            builder.Services.AddSession();

            builder.Services.AddScoped<IAuth,AuthRepository>();
            builder.Services.AddScoped<IDashboard, DashboardRepository>();
            builder.Services.AddScoped<ICustomer, CustomerRepository>();
            builder.Services.AddScoped<IVendor,VendorRepository>();
            builder.Services.AddScoped<IQuotation, QuotationRepository>();
            builder.Services.AddScoped<IOngoingCampain, OngoingCampainRepository>();  
            builder.Services.AddScoped<AutocompleteService>();  
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();   
            app.UseCookiePolicy();  
            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Dashboard}/{action=Index}/{id?}");

            app.Run();
        }
    }
}