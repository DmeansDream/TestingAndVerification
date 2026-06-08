using Microsoft.EntityFrameworkCore;
using RESTWebAPI.Model;

namespace RESTWebAPI.Data
{
    public class APIContext : DbContext
    {
        public DbSet<PlayerCharacterModel> PcData { get; set; }
        public DbSet<MapRegionModel> RegionData { get; set; }

        public APIContext(DbContextOptions options) : base(options)
        {
        }

    }
}
