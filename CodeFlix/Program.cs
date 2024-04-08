using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CodeFlix.Data;
using Microsoft.AspNetCore.Identity;
using CodeFlix.Models;
namespace CodeFlix
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<CodeFlixContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("SoftITOFlixContext") ?? throw new InvalidOperationException("Connection string 'CodeFlixContext' not found.")));
            builder.Services.AddIdentity<CodeFlixUser, CodeFlixRole>().AddDefaultTokenProviders()
                .AddEntityFrameworkStores<CodeFlixContext>();
            //builder.Services.AddDefaultIdentity<CodeFlixUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //  .AddEntityFrameworkStores<CodeFlixContext>();
            // builder.Services.AddIdentity<CodeFlixUser, IdentityRole>()
            //       .AddEntityFrameworkStores<CodeFlixContext>().AddDefaultTokenProviders();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();
            CodeFlixContext? context = app.Services.CreateScope().ServiceProvider.GetService<CodeFlixContext>();
            RoleManager<CodeFlixRole>? roleManager = app.Services.CreateScope().ServiceProvider.GetService<RoleManager<CodeFlixRole>>();
            UserManager<CodeFlixUser>? userManager = app.Services.CreateScope().ServiceProvider.GetService<UserManager<CodeFlixUser>>();
            DBInitializer dBInitializer = new DBInitializer(context, roleManager, userManager);

            app.Run();
        }
    }
}
