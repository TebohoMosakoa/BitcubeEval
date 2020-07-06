using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegistrationPage.Areas.Identity.Data;
using RegistrationPage.Models;

[assembly: HostingStartup(typeof(RegistrationPage.Areas.Identity.IdentityHostingStartup))]
namespace RegistrationPage.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<RegistrationPageDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("RegistrationPageDbContextConnection")));

                services.AddDefaultIdentity<RegistrationPageUser>(options => {
                        options.SignIn.RequireConfirmedAccount = false;
                        options.User.RequireUniqueEmail = true;
                    })
                    .AddEntityFrameworkStores<RegistrationPageDbContext>();
            });
        }
    }
}