namespace Genie;
public class Startup
{
    readonly IConfiguration _configuration;
    readonly string _key;

    public Startup(IConfiguration config)
    {
        _configuration = config;
        // OpenAi Api key, located in appsettings.json, see appsettingsExample.json
        _key = config["Settings:openai-key"];
    }
    public void ConfigureServices(IServiceCollection services)
    {
        // add database context, genie model and ai service to the service collection.
        // connection string located in appsettings.json, see example.

        services
            .AddDbContext<GenieDBContext>(options => options.UseMySql(_configuration.GetConnectionString("MySqlConnection"), MySqlServerVersion.Parse("mysql-8.0.30")))
            .AddSingleton<TheGenie>(new TheGenie())
            .AddTransient<OpenAIAPI>(_ => new OpenAIAPI(_key, Engine.Davinci));
    }
}