namespace ConceptAPI
{
    public class Concept
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Level { get; set; }
        public int Experiance { get; set; }
        public int Rank { get; set; }
        public int Rarity { get; set; }
    }
}