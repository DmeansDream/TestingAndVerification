using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RESTWebAPI.Model
{
    public class PlayerCharacterModel
    {
        [NotNull]
        [Required]
        public string Name { get; set; }

        [Key]
        [Required]
        public int ID { get; set; }
        public int MaxHealth { get; set; }
        public int Health { get; set; }
        public int Damage { get; set; }
    }
}
