
namespace Genie.Controllers;

public class HomeController : Controller
{
    const string CONVO_FILE = "./Data/convo.txt";
    private readonly ILogger<HomeController> _logger;
    readonly IServiceProvider _services;
    readonly TheGenie _genie;
    readonly OpenAIAPI _openAi;
    readonly IConfiguration _config;

    public HomeController(ILogger<HomeController> logger, IServiceProvider services, IConfiguration config)
    {
        _logger = logger;
        _services = services;
        _config = config;
        _openAi = new(System.Environment.GetEnvironmentVariable("API_KEY"), Engine.Davinci);
        _genie = _services.GetRequiredService<TheGenie>();
        _genie.Prompt = System.IO.File.ReadAllText("./Data/genie.txt");
    }

    public IActionResult Index()
    {
        _genie.Convo = GenieHelper.ToConversation(System.IO.File.ReadAllText(CONVO_FILE));
        return View(_genie);
    }

    public IActionResult Privacy()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> AskGenie(string userInput)
    {
        _genie.UserInput = userInput;
        string[] stops = new string[] { "Friend:", "|" };
        _genie.Prompt += _genie.UserInput + "Compooter Genie:" + "|";
        CompletionResult completion = await _openAi.Completions.CreateCompletionAsync(new CompletionRequest(
            _genie.Prompt,
            max_tokens: 300,
            temperature: .7,
            top_p: 1.0,
            presencePenalty: 0.4,
            frequencyPenalty: 0.6,
            stopSequences: stops
            ));
        _genie.Response = completion.ToString();
        Volley v = GenieHelper.ToVolley(_genie.UserInput, _genie.Response);
        _genie.Convo.Add(v);
        GenieHelper.StoreVolley(v, CONVO_FILE);
        _genie.UserInput = "";
        return View("Index", _genie);
    }
    /// <summary>
    /// Called in response to the Erase button being pushed. Erase the current conversation from memory.
    /// If stored in a DB by this point, this will not remove the record.
    /// </summary>
    /// <returns>Index View.</returns>
    public IActionResult BlankConvo()
    {
        _genie.Convo = new();
        _genie.Response = "";
        _genie.UserInput = "";
        System.IO.File.WriteAllText(CONVO_FILE, "");
        return View("Index", _genie);
    }

    public void DownloadConvo()
    {
        Response.ContentType = "txt/plain";
        Response.SendFileAsync(CONVO_FILE);
       
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

