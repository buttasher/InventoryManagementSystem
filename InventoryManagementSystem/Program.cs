using InventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Register database context
builder.Services.AddDbContext<InventoryManagementSystemContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbconn")));

// 🔹 Add controllers and views
builder.Services.AddControllersWithViews();

// 🔹 Add session services with Secure Cookies
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Security: Prevents JavaScript access
    options.Cookie.IsEssential = true; // Required for session cookies
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // 🔹 Enforce HTTPS for cookies
    options.Cookie.SameSite = SameSiteMode.None; // 🔹 Allow cookies for cross-origin requests
});

// 🔹 Add cookie policy explicitly
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None; // 🔹 Allow cross-origin cookies
    options.Secure = CookieSecurePolicy.Always; // 🔹 Ensure cookies are only sent over HTTPS
});

var app = builder.Build();

// 🔹 Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Show detailed error messages in development
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 🔹 Apply cookie policy middleware
app.UseCookiePolicy();  // <-- This line is important!

// 🔹 Use session middleware
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}/{id?}");

app.Run();
