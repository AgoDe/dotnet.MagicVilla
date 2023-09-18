using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers;

[Route("api/[controller]")] // definisce il nome della rotta
[ApiController]
public class VillaApiController : ControllerBase
{
    private readonly ILogger<VillaApiController> _logger;
    private readonly IMapper _mapper;
    private readonly IVillaRepository _dbVilla;

    public VillaApiController(IVillaRepository dbVilla, ILogger<VillaApiController> logger, IMapper mapper)
    {
        _dbVilla = dbVilla;
        _logger = logger;
        _mapper = mapper;
    }
    
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
    {
        _logger.LogInformation("Getting all villas");
        IEnumerable<Villa> villaList = await _dbVilla.GetAll();
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

        var villa = await _dbVilla.Get(v => v.Id == id);
        if (villa == null) return NotFound();
        
        return Ok(_mapper.Map<VillaDto>(villa));
    }

    
    [HttpPost]
    public async Task<ActionResult<VillaDto>> CreateVilla([FromBody]VillaCreateDto createDto)
    {
        // if (!ModelState.IsValid) return BadRequest(ModelState); // non necessario perchè già incluso nell'attributo [ApiController]  
        // custom validation
        if (await _dbVilla.Get(v => v.Name.ToLower() == createDto.Name.ToLower()) != null )
        {
            ModelState.AddModelError("CustomError", "Villa already exist");
            return BadRequest(ModelState);
        }
        if (createDto == null) return BadRequest(createDto);
        //if (villaDto.Id > 0) return StatusCode(StatusCodes.Status500InternalServerError); // non più necessario visto che l'Id non c'è più nel modello

        Villa model = _mapper.Map<Villa>(createDto);

        await _dbVilla.Create(model);
        
        
        return CreatedAtRoute("GetVilla", new { id = model.Id }, model); // ritorna il 201 con la route dell'oggetto negli headers (location)
        return Created("GetVilla", model); // ritorna il classico 201
    }

    
    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    public async Task<IActionResult> DeleteVilla(int id)
    {
        if (id == 0) return BadRequest();

        var villa = await _dbVilla.Get(v => v.Id == id);
        if (villa == null) return NotFound();

        await _dbVilla.Remove(villa);
        return NoContent();
    }

    
    [HttpPut("{id:int}", Name = "UpdateVilla")]
    public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
    {
        if (updateDto == null || id != updateDto.Id) return BadRequest(updateDto);

        Villa model = _mapper.Map<Villa>(updateDto);

        await _dbVilla.Update(model);
        
        return NoContent();
    }

    
    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
    {
        if (patchDto == null || id == 0)
        {
            return BadRequest();
        }

        var villa = await _dbVilla.Get(v => v.Id == id, tracked:false);
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

        await _dbVilla.Update(model);
        return NoContent();
    }
    
}