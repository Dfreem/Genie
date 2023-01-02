namespace Genie.Models;
/// <summary>
/// A Volley is a model of one Question/Prompt and one Answer/Response from the AI API.
/// </summary>
public class Volley
{
    public int VolleyID { get; set; }
    public int ConversationID { get; set; }
    public string Question { get; set; } = "";
    public string Answer { get; set; } = "";

    public override string ToString()
    {
        return Question + "\n" + Answer + "\n\n";
    }
}

