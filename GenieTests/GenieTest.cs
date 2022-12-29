using Newtonsoft.Json;

namespace GenieTests;

public class GenieTest
{
    TheGenie _genie;
    [SetUp]
    public void Setup()
    {
        _genie = new()
        {
            Convo = new()
            {
                new Volley()
                {
                    Question = "Test Question",
                    Answer = "this is a test Answer1"
                },
                new Volley()
                {
                    Question = "Boring Question",
                    Answer = "this is a test Answer2"
                },
                new Volley()
                {
                    Question = "What is your name?",
                    Answer = "this is a test Answer3"
                }
            }
        };
    }

    [Test]
    public void Test()
    {
        var convoFile = System.IO.File.ReadAllText("conovo.json");
        Assert.Pass();
    }
}
