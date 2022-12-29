namespace Genie.Data;

public class GenieDBContext : DbContext
{
    DbSet<Volley> Volleys { get; set; } = default!;
    DbSet<Conversation> Conversations { get; set; }
}