using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SES_Portal.Data;
using SES_Portal.Services;
using SES_Portal.Pages.Chat;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddScoped<AnnouncementService>();
builder.Services.AddScoped<FavoriteProjectService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<SidebarService>();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, CustomClaimsPrincipalFactory>();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/Index");
    options.Conventions.AuthorizeFolder("/Employees");
    options.Conventions.AuthorizeFolder("/Projects");
});

builder.Services.AddSignalR();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ChatHub>("/chatHub");
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}
app.Run();
