using MagicVilla_WebAPI_Endpoint.Data;
using MagicVilla_WebAPI_Endpoint.Models;
using MagicVilla_WebAPI_Endpoint.Models.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MagicVilla_WebAPI_Endpoint.Controllers
{
    [Route("api/MagicVilla")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ILogger<VillaAPIController> _logger;

        public VillaAPIController(ILogger<VillaAPIController> logger)
        {
            this._logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)] 
        public ActionResult<IEnumerable<VillaDTO>> GetVillas() 
        {
            _logger.LogInformation("Call to get all villas");
            return Ok(VillaStore.vallaList);
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
            var villa = VillaStore.vallaList.FirstOrDefault(x => x.Id == id);
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
            if(VillaStore.vallaList.FirstOrDefault(v => v.Name == villa.Name) != null)
            {
                ModelState.AddModelError("InputError", "Villa already exits");
                return BadRequest(ModelState);
            }
            if(villa == null)
            {
                return BadRequest(villa);
            }
            villa.Id = VillaStore.vallaList.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            if (villa.Id < 1)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            VillaStore.vallaList.Add(villa);

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
            var villa = VillaStore.vallaList.FirstOrDefault(u => u.Id == id);
            if(villa == null)
            {
                return NotFound();
            }
            VillaStore.vallaList.Remove(villa);
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
            var entry = VillaStore.vallaList.FirstOrDefault(u => u.Id == id);
            if(entry == null)
            {
                return NotFound();
            }
            entry.Name = villa.Name;
            entry.Sqft = villa.Sqft;
            entry.Description = villa.Description;

            return NoContent();
            
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult PatchVilla(int id, JsonPatchDocument<VillaDTO> patch)
        {
            if (patch == null || id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.vallaList.FirstOrDefault(U => U.Id == id);
            if(villa == null)
            {
                return BadRequest();
            }
            patch.ApplyTo(villa, ModelState);
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();

        }
    }
}
