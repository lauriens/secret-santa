using System.Text.Json.Serialization;

namespace SecretSanta.Models
{
    public class Friend
    {
        public string Name { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public Friend? SecretSanta { get; set; }
    }
}
