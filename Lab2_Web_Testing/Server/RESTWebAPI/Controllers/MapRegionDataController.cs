using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTWebAPI.Data;
using RESTWebAPI.Model;

namespace RESTWebAPI.Controllers
{
    [Route("api/mapdata")]
    [ApiController]
    public class MapRegionDataController : ControllerBase
    {
        private readonly APIContext _context;

        public MapRegionDataController(APIContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateNewRegion([FromBody] MapRegionModel model, [FromQuery] bool isAdmin = false)
        {
            if (!isAdmin)
                return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.RegionId != 0)
                return BadRequest();

            _context.RegionData.Add(model);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetRegionByID), new { id = model.RegionId }, model);
        }

        [HttpGet("{id}")]
        public IActionResult GetRegionByID([FromRoute] int id)
        {
            if (id <= 0 ) return BadRequest();

            MapRegionModel model = _context.RegionData.FirstOrDefault(x => x.RegionId == id);

            if (model == null)
            {
                return NotFound();
            }

            return Ok(model);
        }
    }
}
