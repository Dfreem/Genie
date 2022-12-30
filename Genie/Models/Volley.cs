namespace Genie.Models;
/// <summary>
/// A Volley is a model of one Question/Prompt and one Answer/Response from the AI API.
/// </summary>
public class Volley
{
    public int VolleyID { get; set; }
    public int ConversationID { get; set; }
    public string Question { get; set; } = "Q:";
    public string Answer { get; set; } = "A";

    public override string ToString()
    {
        return "\n" + Question + "\n" + Answer + "\n";
    }
}

