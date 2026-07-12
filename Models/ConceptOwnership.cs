namespace ConceptAPI.Models
{
    public class ConceptOwnership
    {
        public int Id { get; set; }
        public int ConceptId { get; set; }
        public int UserId { get; set; }
        public int Level { get; set; }
        public int Rank { get; set; }
        public int Experiance { get; set; }
    }
}