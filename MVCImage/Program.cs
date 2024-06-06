using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVCImage.Services;
using System.Net.Http.Headers;
using MVCImage.Models;
using MVCImage.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Cấu hình HttpClient cho AccountController
builder.Services.AddHttpClient<AccountController>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7139/"); // URL API cơ bản
    client.Timeout = TimeSpan.FromSeconds(30); // Có thể tùy chỉnh timeout nếu cần
});
// Cấu hình Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add HttpClient and ApiService
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7139/api/");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjIiLCJuYmYiOjE3MTc3NDU4ODQsImV4cCI6MTcxODM1MDY4NCwiaWF0IjoxNzE3NzQ1ODg0LCJpc3MiOiJZb3VySXNzdWVyIiwiYXVkIjoiWW91ckF1ZGllbmNlIn0.xp3BKk72q2SWSawGexSnIlwFxhLIOWkFMQ8pK4w67O8");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Categories}/{action=Index}/{id?}");
app.Run();
