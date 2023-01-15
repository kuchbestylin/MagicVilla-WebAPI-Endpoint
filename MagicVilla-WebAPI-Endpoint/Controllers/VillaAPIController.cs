using MagicVilla_WebAPI_Endpoint.Data;
using MagicVilla_WebAPI_Endpoint.Models;
using MagicVilla_WebAPI_Endpoint.Models.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_WebAPI_Endpoint.Controllers
{
    [Route("api/MagicVilla")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ILogger<VillaAPIController> _logger;
        private readonly ApplicationDbContext _db;

        public VillaAPIController(ILogger<VillaAPIController> logger, ApplicationDbContext db)
        {
            this._logger = logger;
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)] 
        public ActionResult<IEnumerable<VillaDTO>> GetVillas() 
        {
            _logger.LogInformation("Call to get all villas");
            return Ok(_db.Villas.ToList());
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(200, Type = typeof(VillaDTO))]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(400)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            _logger.LogInformation("Getting Villa of ID=" + id);
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villa)
        {
            if(_db.Villas.FirstOrDefault(v => v.Name == villa.Name) != null)
            {
                ModelState.AddModelError("InputError", "Villa already exits");
                return BadRequest(ModelState);
            }
            if(villa == null)
            {
                return BadRequest(villa);
            }
            Villa model = new()
            {
                Amenity = villa.Amenity,
                Description = villa.Description,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft
            };
            _db.Villas.Add(model);
            _db.SaveChanges();
            return CreatedAtRoute("GetVilla", new {id = villa.Id} , villa);
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            if(villa == null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpadateVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpadateVilla(int id, [FromBody] VillaDTO villa)
        {
            if(villa == null || villa.Id != id)
            {
                return BadRequest();
            }
            //var entry = VillaStore.vallaList.FirstOrDefault(u => u.Id == id);
            //if(entry == null)
            //{
            //    return NotFound();
            //}
            //entry.Name = villa.Name;
            //entry.Sqft = villa.Sqft;
            //entry.Description = villa.Description;
            Villa model = new()
            {
                Amenity = villa.Amenity,
                Description = villa.Description,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft
            };
            _db.Villas.Update(model);
            _db.SaveChanges();
            return NoContent();
            
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult PatchVilla(int id, JsonPatchDocument<VillaDTO> patch)
        {
            if (patch == null || id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.AsNoTracking().FirstOrDefault(U => U.Id == id);
            if(villa == null)
            {
                return NotFound();
            }
            VillaDTO villaDTO = new()
            {
                Amenity = villa.Amenity,
                Description = villa.Description,
                Id= villa.Id,
                ImageUrl = villa.ImageUrl,
                Name= villa.Name,
                Occupancy= villa.Occupancy,
                Rate= villa.Rate,
                Sqft= villa.Sqft
            };
            patch.ApplyTo(villaDTO, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Villa model = new()
            {
                Amenity = villaDTO.Amenity,
                Description = villaDTO.Description,
                Id = villaDTO.Id,
                ImageUrl = villaDTO.ImageUrl,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft
            };
            _db.Villas.Update(model); 
            _db.SaveChanges();

            return NoContent();

        }
    }
}
