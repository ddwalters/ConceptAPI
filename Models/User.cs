namespace ConceptAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirebaseUid { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public string DisplayName { get; set; } = null!;
        public int Level { get; set; }
        public int Experiance { get; set; }
        public int Gold { get; set; } // Used for concept/ card upgrades
        public int Shards { get; set; } // needed to gacha pull!
        public bool IsAnonymous { get; set; }
        public bool IsAdmin { get; set; }
    }
}