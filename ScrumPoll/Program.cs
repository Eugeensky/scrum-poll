using DAL;
using ScrumPoll.Hubs;
using ScrumPoll.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication("Cookies").AddCookie(opt => opt.LoginPath = "/Login/Index");
builder.Services.AddAuthorization();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<Database>();
builder.Services.AddTransient<AuthorizationService>();
builder.Services.AddSignalR();
var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(name: "default", pattern: "{controller=Login}/{action=Index}");
    endpoints.MapHub<PollHub>("/PollHub");
});

app.Run();
