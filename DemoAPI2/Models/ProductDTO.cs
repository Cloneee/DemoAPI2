using System.ComponentModel.DataAnnotations.Schema;

namespace DemoAPI2.Models
{
    public class ProductDTO
    {
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public int Price { get; set; } = 0;
    }
}
