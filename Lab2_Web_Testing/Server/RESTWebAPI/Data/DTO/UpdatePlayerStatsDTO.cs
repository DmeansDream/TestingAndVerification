using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RESTWebAPI.Data.DTO
{
    public class UpdatePlayerStatsDTO
    {
        [Key]
        [Required]
        public int ID { get; set; }
        public int MaxHealth { get; set; }
        public int Health { get; set; }
        public int Damage { get; set; }
    }
}
