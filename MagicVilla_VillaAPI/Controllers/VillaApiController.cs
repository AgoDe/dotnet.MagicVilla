using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers;

[Route("api/[controller]")] // definisce il nome della rotta
[ApiController]
public class VillaApiController : ControllerBase
{
    private readonly ILogger<VillaApiController> _logger;

    public VillaApiController(ILogger<VillaApiController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<VillaDto>> GetVillas()
    {
        _logger.LogInformation("Getting all villas");
        return Ok( VillaStore.villaList);
    }
    
    [HttpGet("{id:int}", Name = "GetVilla")]
    public ActionResult<VillaDto> GetVilla(int id)
    {
        if (id == 0)
        {
            _logger.LogError("Get villa Error with id");
            return BadRequest();
        }

        var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
        if (villa == null) return NotFound();
        
        return Ok(villa);
    }

    [HttpPost]
    public ActionResult<VillaDto> CreateVilla([FromBody]VillaDto villaDto)
    {
        // if (!ModelState.IsValid) return BadRequest(ModelState); // non necessario perchè già incluso nell'attributo [ApiController]  
        // custom validation
        if (VillaStore.villaList.FirstOrDefault(v => v.Name.ToLower() == villaDto.Name.ToLower()) != null )
        {
            ModelState.AddModelError("CustomError", "Villa already exist");
            return BadRequest(ModelState);
        }
        if (villaDto == null) return BadRequest();
        if (villaDto.Id > 0) return StatusCode(StatusCodes.Status500InternalServerError);

        villaDto.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;
        VillaStore.villaList.Add(villaDto);
        return CreatedAtRoute("GetVilla", new { id = villaDto.Id }, villaDto); // ritorna il 201 con la route dell'oggetto negli headers (location)
        return Created("GetVilla", villaDto); // ritorna il classico 201
    }

    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    public IActionResult DeleteVilla(int id)
    {
        if (id == 0) return BadRequest();

        var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
        if (villa == null) return NotFound();

        VillaStore.villaList.Remove(villa);
        return NoContent();
    }

    [HttpPut("{id:int}", Name = "UpdateVilla")]
    public IActionResult UpdateVilla(int id, [FromBody] VillaDto villaDto)
    {
        if (villaDto == null || id != villaDto.Id) return BadRequest();

        var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
        villa.Name = villaDto.Name;
        villa.Sqft = villaDto.Sqft;
        villa.Occupancy = villaDto.Occupancy;

        return NoContent();
    }

    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
    {
        if (patchDto == null || id == 0)
        {
            return BadRequest();
        }

        var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
        if (villa == null)
        {
            return NotFound();
        }
        
        patchDto.ApplyTo(villa, ModelState);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return NoContent();
    }
    
}