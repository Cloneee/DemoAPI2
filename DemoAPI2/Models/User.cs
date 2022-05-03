using System.ComponentModel.DataAnnotations.Schema;

namespace DemoAPI2.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; } = String.Empty;
        public byte[] HashedPassword { get; set; }
        public byte[] PasswordSalt { get; set; }
        public Boolean IsAdmin { get; set; } = false;
    }
}
