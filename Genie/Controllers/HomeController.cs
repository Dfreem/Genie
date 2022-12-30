
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
        _openAi = new(_config["Settings:openai-key"], Engine.Davinci);
        _genie = _services.GetRequiredService<TheGenie>();
        _genie.Prompt = _config["prompt"];
        _genie.Convo = GetStoredConvo();
    }

    public IActionResult Index()
    {
        
        return View(_genie);
    }

    public IActionResult Privacy()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> AskGenie(string userInput)
    {
        string[] stops = new string[] { "Friend:", "|" };
         _genie.UserInput += userInput + "|";
        CompletionResult completion = await _openAi.Completions.CreateCompletionAsync(new CompletionRequest(
            _genie.Prompt,
            max_tokens: 300,
            temperature: .6,
            top_p: 1.0,
            presencePenalty: 0.5,
            frequencyPenalty: 0.5,
            stopSequences: stops
            ));
        _genie.UserInput = userInput;
        _genie.Response = completion.ToString();
        StoreVolley(_genie.UserInput, _genie.Response);
        return View("Index", _genie);
    }
    /// <summary>
    /// retrieve if exists, conversation stored in convo.json
    /// </summary>
    /// <returns>a <see cref="Conversation"/> with any <see cref="Volley"/>
    /// that is found in the convo.json file.</returns>
    public Conversation GetStoredConvo()
    {
        Conversation convo = new();
        var list = JsonSerializer.Deserialize<List<string>>(_config["convo"]);
        // Chunck apart the list by pairs and create a volley.
        for (int i = 1; i < list?.Count; i += 2)
        {
            // loop starts at one and looks backwards by one so that it won't go out of bounds.
            Volley holder = new() { Question = list[i], Answer = list[i - 1] };

            //add whatever is found to current conversation.
            convo.Add(holder);
        }
        return convo;
    }
    public void StoreVolley(string ask, string response)
    {
        Volley volley = new() { Question = ask, Answer = response };
        _genie.Convo.Add(volley);
        JsonSerializer.Serialize(_genie.Convo);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

