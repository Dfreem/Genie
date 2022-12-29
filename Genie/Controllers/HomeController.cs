
namespace Genie.Controllers;

public class HomeController : Controller
{
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
        _openAi = _services.CreateAsyncScope().ServiceProvider.GetRequiredService<OpenAIAPI>();
        _genie = _services.GetRequiredService<TheGenie>();
    }

    public IActionResult Index()
    {
        // load the conversation from the json file loaded into the configuration container.
        var jsonData = JsonConvert.DeserializeObject<Conversation>(_config["convo"]);

        // check for null then set current conversation to retrieved data.
        if (jsonData is not null) _genie.Convo = jsonData;
        return View(_genie);
    }

    public IActionResult Privacy()
    {
        return View();
    }
    [HttpPost]
    public IActionResult AskGenie(string userInput)
    {
        string[] stops = new string[] { "Friend:", "|" };
        _genie.Prompt = System.IO.File.ReadAllTextAsync("./Data/convo.txt") + userInput;
        _genie.Response = _openAi.Completions.CreateAndFormatCompletion(new CompletionRequest(
            _genie.Prompt,
            max_tokens: 300,
            temperature: .7,
            top_p: 1.0,
            presencePenalty: 0.4,
            frequencyPenalty: 0.4,
            stopSequences: stops
            )).Result.ToString();
        return Ok(_genie.Response);
    }
    /// <summary>
    /// retrieve if exists, conversation stored in convo.json
    /// </summary>
    /// <returns></returns>
    public IActionResult GetStoredConvo()
    {
        // if there is no current conversation yet, look for something in convo.json, 
        if (!_genie.Convo.Any())
        {
            List<string>? convo = JsonConvert.DeserializeObject<List<string>>(_config["convo"]);
            // Chunck apart the list by pairs and create a volley.
            for (int i = 1; i < convo?.Count; i += 2)
            {
                // loop starts at one and looks backwards by one so that it won't go out of bounds.
                Volley holder = new() { Question = convo[i], Answer = convo[i - 1] };

                //add whatever is found to current conversation.
                _genie.Convo.Add(holder);
            }
        }
        return View("Index", _genie);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

