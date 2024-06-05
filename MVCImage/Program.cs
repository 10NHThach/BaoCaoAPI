using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVCImage.Services;
using System.Net.Http.Headers;
using MVCImage.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Add HttpClient and ApiService
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7139/api/");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJuYmYiOjE3MTczMzg4MTIsImV4cCI6MTcxNzk0MzYxMiwiaWF0IjoxNzE3MzM4ODEyLCJpc3MiOiJZb3VySXNzdWVyIiwiYXVkIjoiWW91ckF1ZGllbmNlIn0.qMP6-niz7gRABpW8UlDV4EEz8JBcCPrietNAur-BARw");
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

app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Categories}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "images",
    pattern: "{controller=Images}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "history",
    pattern: "{controller=History}/{action=Index}/{id?}");
app.Run();
