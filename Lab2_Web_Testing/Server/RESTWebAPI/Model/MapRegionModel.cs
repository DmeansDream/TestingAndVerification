using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RESTWebAPI.Model
{
    public class MapRegionModel
    {
        [Key]
        [Required]
        public int RegionId { get; set; }

        [Required]
        [StringLength(50)]
        public string RegionName { get; set; }

        [Required]
        [EnumDataType(typeof(Biomes))]
        public string RegionBiome { get; set; }
    }

    public enum Biomes
    {
        AutumnForest,
        WinterLake,
        SunnyPlain
    }
}
