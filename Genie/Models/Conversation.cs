using System;

namespace Genie.Models
{
    public class Conversation : List<Volley>
    {
        public int ConversationId { get; set; }

        public IEnumerator<Volley> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}

