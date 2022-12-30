
namespace Genie.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    readonly IServiceProvider _services;
    readonly TheGenie _genie;
    readonly OpenAIAPI _openAi;
    readonly IConfiguration _config;
    GenieDBContext _context;

    public HomeController(ILogger<HomeController> logger, IServiceProvider services, IConfiguration config, GenieDBContext context)
    {
        _context = context;
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
        string list = _config["convo"];
        // Chunck apart the list by pairs and create a volley.
        List<string> splits = list.Split(new[] { 'Q', 'A', ':' },
            StringSplitOptions.RemoveEmptyEntries |
            StringSplitOptions.TrimEntries).ToList();
        for (int i = 1; i < splits.Count; i+=2)
        {
            convo.Add(new Volley() { Question = splits[i - 1], Answer = splits[i] });
        }
        return convo;
    }
    public void StoreVolley(string ask, string response)
    {
        Volley volley = new() { Question = ask, Answer = response };
        _genie.Convo.Add(volley);
        System.IO.File.AppendAllText("./Data/Files/convo.txt", volley.ToString());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

