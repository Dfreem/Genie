using System;
namespace Genie.Models;

public class TheGenie
{

    public string? Response { get; set; }
    public string? Prompt { get; set; }
    public string? UserInput { get; set; }
    public Conversation Convo { get; set; } = default!;
}

