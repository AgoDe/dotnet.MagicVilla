using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers;

[Route("api/[controller]")] // definisce il nome della rotta
[ApiController]
public class VillaApiController : ControllerBase
{
    private readonly ILogger<VillaApiController> _logger;
    private readonly ApplicationDbContext _db;
    

    public VillaApiController(ApplicationDbContext db, ILogger<VillaApiController> logger)
    {
        _db = db;
        _logger = logger;
    }
    
    
    [HttpGet]
    public ActionResult<IEnumerable<VillaDto>> GetVillas()
    {
        _logger.LogInformation("Getting all villas");
        return Ok( _db.Villas.ToList());
    }
    
    
    [HttpGet("{id:int}", Name = "GetVilla")]
    public ActionResult<VillaDto> GetVilla(int id)
    {
        if (id == 0)
        {
            _logger.LogError("Get villa Error with id");
            return BadRequest();
        }

        var villa = _db.Villas.FirstOrDefault(v => v.Id == id);
        if (villa == null) return NotFound();
        
        return Ok(villa);
    }

    
    [HttpPost]
    public ActionResult<VillaDto> CreateVilla([FromBody]VillaCreateDto villaDto)
    {
        // if (!ModelState.IsValid) return BadRequest(ModelState); // non necessario perchè già incluso nell'attributo [ApiController]  
        // custom validation
        if (_db.Villas.FirstOrDefault(v => v.Name.ToLower() == villaDto.Name.ToLower()) != null )
        {
            ModelState.AddModelError("CustomError", "Villa already exist");
            return BadRequest(ModelState);
        }
        if (villaDto == null) return BadRequest();
        //if (villaDto.Id > 0) return StatusCode(StatusCodes.Status500InternalServerError); // non più necessario visto che l'Id non c'è più nel modello

        Villa model = new()
        {
            Amenity = villaDto.Amenity,
            Details = villaDto.Details,
            ImageUrl = villaDto.ImageUrl,
            Name = villaDto.Name,
            Occupancy = villaDto.Occupancy,
            Rate = villaDto.Rate,
            Sqft = villaDto.Sqft
        };

        _db.Villas.Add(model);
        _db.SaveChanges();
        
        return CreatedAtRoute("GetVilla", new { id = model.Id }, model); // ritorna il 201 con la route dell'oggetto negli headers (location)
        return Created("GetVilla", villaDto); // ritorna il classico 201
    }

    
    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    public IActionResult DeleteVilla(int id)
    {
        if (id == 0) return BadRequest();

        var villa = _db.Villas.FirstOrDefault(v => v.Id == id);
        if (villa == null) return NotFound();

        _db.Villas.Remove(villa);
        _db.SaveChanges();
        return NoContent();
    }

    
    [HttpPut("{id:int}", Name = "UpdateVilla")]
    public IActionResult UpdateVilla(int id, [FromBody] VillaUpdateDto villaDto)
    {
        if (villaDto == null || id != villaDto.Id) return BadRequest();

        Villa model = new()
        {
            Id = villaDto.Id,
            Amenity = villaDto.Amenity,
            Details = villaDto.Details,
            ImageUrl = villaDto.ImageUrl,
            Name = villaDto.Name,
            Occupancy = villaDto.Occupancy,
            Rate = villaDto.Rate,
            Sqft = villaDto.Sqft
        };

        _db.Villas.Update(model);
        _db.SaveChanges();
        return NoContent();
    }

    
    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
    {
        if (patchDto == null || id == 0)
        {
            return BadRequest();
        }

        var villa = _db.Villas.AsNoTracking().FirstOrDefault(v => v.Id == id); // AsNoTracking, la funzione traccia un Id alla volta, essendo che vogliamo modificate il model instanziato sotto, questa istanza non deve essere seguita dal compilatore
        if (villa == null)
        {
            return NotFound();
        }
        
        VillaUpdateDto villaDto = new()
        {
            Amenity = villa.Amenity,
            Details = villa.Details,
            Id = villa.Id,
            ImageUrl = villa.ImageUrl,
            Name = villa.Name,
            Occupancy = villa.Occupancy,
            Rate = villa.Rate,
            Sqft = villa.Sqft
        };
        
        patchDto.ApplyTo(villaDto, ModelState);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        Villa model = new()
        {
            Amenity = villaDto.Amenity,
            Details = villaDto.Details,
            Id = villaDto.Id,
            ImageUrl = villaDto.ImageUrl,
            Name = villaDto.Name,
            Occupancy = villaDto.Occupancy,
            Rate = villaDto.Rate,
            Sqft = villaDto.Sqft
        };

        _db.Villas.Update(model);
        _db.SaveChanges();

        return NoContent();
    }
    
}