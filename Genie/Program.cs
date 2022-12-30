using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>()
    {
                { "prompt", File.ReadAllText("./Data/Files/genie.txt") },
                { "convo", File.ReadAllText("./Data/Files/convo.json") }
    });

builder.Services
    .AddDbContext<GenieDBContext>(options =>
        options
        .UseMySql(builder.Configuration.GetConnectionString("MySqlConnection"), MySqlServerVersion.Parse("mysql-8.0.30"))
        )
        .AddSingleton<TheGenie>(new TheGenie());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

