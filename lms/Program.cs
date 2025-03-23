using lms.Data;
using lms.Interfaces;
using lms.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<lms.Models.ApplicationUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<lms.Data.ApplicationDbContext>()
    .AddDefaultTokenProviders();
    
// Register repositories
builder.Services.AddScoped<lms.Interfaces.ICourseRepository, lms.Repositories.EFCourseRepository>();
builder.Services.AddScoped<lms.Interfaces.ICartItemRepository, lms.Repositories.EFCartItemRepository>();
builder.Services.AddScoped<lms.Interfaces.IStudentCourseRepository, lms.Repositories.EFStudentCourseRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
