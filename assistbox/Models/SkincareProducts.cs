namespace assistbox.Models
{
    public class SkincareProduct
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Concerns { get; set; } = new List<string>(); 
    }
}