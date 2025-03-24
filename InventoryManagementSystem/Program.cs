using InventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Register database context
builder.Services.AddDbContext<InventoryManagementSystemContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbconn")));

// 🔹 Add controllers and views
builder.Services.AddControllersWithViews();

// 🔹 Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Security: Prevents JavaScript access
    options.Cookie.IsEssential = true; // Required for session cookies
});

var app = builder.Build();

// 🔹 Configure middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 🔹 Use session middleware
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}/{id?}");

app.Run();
