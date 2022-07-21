using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDo_List.Models;

var builder = WebApplication.CreateBuilder(args);

//Getting Connection string
string connString = builder.Configuration.GetConnectionString("DefaultConnection");
//Getting Assembly Name
var migrationAssembly = typeof(Program).Assembly.GetName().Name;

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
           options.UseSqlServer(connString, sql => sql.MigrationsAssembly(migrationAssembly)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CreateListPolicy",
        policy => policy.RequireClaim("Create List"));
    options.AddPolicy("EditListPolicy",
        policy => policy.RequireClaim("Edit List"));
    options.AddPolicy("DisplayListPolicy",
        policy => policy.RequireClaim("Display List"));
    options.AddPolicy("DeleteListPolicy",
        policy => policy.RequireClaim("Delete List"));
    
});



// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
