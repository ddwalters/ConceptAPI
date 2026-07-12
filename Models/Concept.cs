namespace ConceptAPI.Models
{
    public class Concept
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Rarity { get; set; }
    }
}