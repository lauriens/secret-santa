using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SecretSanta.Models
{
    public class Friend
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [JsonIgnore]
        public Friend? SecretSanta { get; set; }
    }
}
