using AutoMapper;
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
    private readonly IMapper _mapper;

    public VillaApiController(ApplicationDbContext db, ILogger<VillaApiController> logger, IMapper mapper)
    {
        _logger = logger;
        _db = db;
        _mapper = mapper;
    }
    
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
    {
        _logger.LogInformation("Getting all villas");
        IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
        return Ok(_mapper.Map<List<VillaDto>>(villaList));
    }
    
    
    [HttpGet("{id:int}", Name = "GetVilla")]
    public async Task<ActionResult<VillaDto>> GetVilla(int id)
    {
        if (id == 0)
        {
            _logger.LogError("Get villa Error with id");
            return BadRequest();
        }

        var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);
        if (villa == null) return NotFound();
        
        return Ok(_mapper.Map<VillaDto>(villa));
    }

    
    [HttpPost]
    public async Task<ActionResult<VillaDto>> CreateVilla([FromBody]VillaCreateDto createDto)
    {
        // if (!ModelState.IsValid) return BadRequest(ModelState); // non necessario perchè già incluso nell'attributo [ApiController]  
        // custom validation
        if (await _db.Villas.FirstOrDefaultAsync(v => v.Name.ToLower() == createDto.Name.ToLower()) != null )
        {
            ModelState.AddModelError("CustomError", "Villa already exist");
            return BadRequest(ModelState);
        }
        if (createDto == null) return BadRequest(createDto);
        //if (villaDto.Id > 0) return StatusCode(StatusCodes.Status500InternalServerError); // non più necessario visto che l'Id non c'è più nel modello

        Villa model = _mapper.Map<Villa>(createDto);

        await _db.Villas.AddAsync(model);
        await _db.SaveChangesAsync();
        
        return CreatedAtRoute("GetVilla", new { id = model.Id }, model); // ritorna il 201 con la route dell'oggetto negli headers (location)
        return Created("GetVilla", model); // ritorna il classico 201
    }

    
    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    public async Task<IActionResult> DeleteVilla(int id)
    {
        if (id == 0) return BadRequest();

        var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);
        if (villa == null) return NotFound();

        _db.Villas.Remove(villa);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    
    [HttpPut("{id:int}", Name = "UpdateVilla")]
    public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
    {
        if (updateDto == null || id != updateDto.Id) return BadRequest(updateDto);

        Villa model = _mapper.Map<Villa>(updateDto);

        _db.Villas.Update(model);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    
    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
    {
        if (patchDto == null || id == 0)
        {
            return BadRequest();
        }

        var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id); // AsNoTracking, la funzione traccia un Id alla volta, essendo che vogliamo modificate il model instanziato sotto, questa istanza non deve essere seguita dal compilatore
        VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);
        if (villa == null)
        {
            return NotFound();
        }
        
        patchDto.ApplyTo(villaDto, ModelState);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Villa model = _mapper.Map<Villa>(villaDto);

        _db.Villas.Update(model);
        await _db.SaveChangesAsync();

        return NoContent();
    }
    
}