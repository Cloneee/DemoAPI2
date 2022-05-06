using System.ComponentModel.DataAnnotations.Schema;

namespace DemoAPI2.Models
{
    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public int Price { get; set; } = 0;
        public Product(string name, string description, int price)
        {
            Name = name;
            Description = description;
            Price = price;
        }
    }
}
