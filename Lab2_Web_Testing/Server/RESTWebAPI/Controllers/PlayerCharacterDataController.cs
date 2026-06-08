using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTWebAPI.Model;
using RESTWebAPI.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using RESTWebAPI.Data.DTO;
using RESTWebAPI.Mappers;

namespace RESTWebAPI.Controllers
{
    [Route("api/pcdata")]
    [ApiController]
    public class PlayerCharacterDataController : ControllerBase
    {
        private readonly APIContext _context;

        public PlayerCharacterDataController(APIContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreatePC([FromBody] PlayerCharacterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.ID != 0)
            {
                return BadRequest();
            }

            _context.PcData.Add(model);
            _context.SaveChanges();


            return CreatedAtAction(nameof(GetPCByID), new { id = model.ID }, model);
        }

        [HttpGet("{id}")]
        public IActionResult GetPCByID([FromRoute] int id, [FromQuery] bool fullInfo = false)
        {
            if (id <= 0) return BadRequest();

            PlayerCharacterModel model = _context.PcData.Find(id);

            if (model == null) return NotFound(new { message = $"PC with {id} not found" });

            if (fullInfo) return Ok(model);

            PlayerUsernameDTO usernameDTO = model.ToPlayerUsernameDTO();

            return Ok(usernameDTO);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePlayerStats([FromRoute] int id, [FromBody] UpdatePlayerStatsDTO dto)
        {
            var model = _context.PcData.FirstOrDefault(x => x.ID == id);

            if (model == null) return NotFound();

            model.MaxHealth = dto.MaxHealth;
            model.Health = dto.Health;
            model.Damage = dto.Damage;

            _context.SaveChanges();
            return Ok(model);
        }

        [HttpDelete("{id}")]
        public IActionResult RemovePlayerCharacter([FromRoute] int id)
        {
            var model = _context.PcData.FirstOrDefault(x => x.ID == id);

            if (model == null)
                return NotFound();

            _context.PcData.Remove(model);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
