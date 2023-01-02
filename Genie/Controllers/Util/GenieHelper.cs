using System;
namespace Genie.Controllers.Util
{
    /// <summary>
    /// Helper class storing methods that help with Genie conversation and storage.
    /// </summary>
    public static class GenieHelper
    {
        /// <summary>
        /// formats the conversation retrieved from the convo.txt file into
        /// a <see cref="Conversation"/> object.
        /// </summary>
        /// <param name="convoString">The string the was retirevied from the convo file.</param>
        /// <returns>a <see cref="Conversation"/> object containing the conversation stored in the file.</returns>
        public static Conversation ToConversation(string convoString)
        {
            Conversation convo = new();
            string holder = convoString.Replace("Compooter Genie:", "\n");

            // Chunck apart the list by pairs and create a volley.
            string[] splits = holder.Split('|',
                StringSplitOptions.RemoveEmptyEntries |
                StringSplitOptions.TrimEntries);

            for (int i = 1; i < splits?.Length; i += 2)
            {
                convo.Add(new Volley() { Question = splits[i - 1], Answer = splits[i] });
            }
            return convo;
        }

        public static Volley ToVolley(string ask, string response)
        {
            return new Volley() { Question = ask, Answer = response.Replace("Compooter Genie:", "") };
        }

        /// <summary>
        /// Store the prompt/question and <see cref="TheGenie"/>'s response.
        /// </summary>
        /// <param name="ask">The user entered portion of the AI prompt</param>
        /// <param name="response">the string sent back by the AI service in response to the user input.</param>
        /// <param name="filepath">the location to save the conversation to.</param>
        /// <returns>The conversation that was created from the user input and response, that was stored in the text fiel.</returns>
        public static Volley StoreVolley(Volley v, string filepath)
        {
            System.IO.File.AppendAllText(filepath, v.ToString());
            return v;
        }

        
    }
}

