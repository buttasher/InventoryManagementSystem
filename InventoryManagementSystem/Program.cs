using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.Hubs; // ✅ Add this for SignalR hub
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Register database context
builder.Services.AddDbContext<InventoryManagementSystemContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbconn")));

// 🔹 Register custom services
builder.Services.AddScoped<AiReportingService>();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// 🔹 Register controllers with views
builder.Services.AddControllersWithViews();

// 🔹 Register SignalR
builder.Services.AddSignalR();
builder.Services.AddScoped<NotificationService>();

// 🔹 Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
});

// 🔹 Add cookie policy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always;
});

var app = builder.Build();

// 🔹 Middleware pipeline
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

// 🔹 Middleware for cookies & session
app.UseCookiePolicy();
app.UseSession();

app.UseAuthorization();

// 🔹 Map default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}/{id?}");

// 🔹 Map SignalR hub
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
