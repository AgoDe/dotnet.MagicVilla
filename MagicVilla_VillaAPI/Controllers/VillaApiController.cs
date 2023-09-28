using System.Net;
using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers;

[Route("api/v{version:apiVersion}/[controller]")] // definisce il nome della rotta
[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class VillaApiController : ControllerBase
{
    protected ApiResponse _response;
    private readonly ILogger<VillaApiController> _logger;
    private readonly IMapper _mapper;
    private readonly IVillaRepository _dbVilla;

    public VillaApiController(IVillaRepository dbVilla, ILogger<VillaApiController> logger, IMapper mapper)
    {
        _dbVilla = dbVilla;
        _logger = logger;
        _mapper = mapper;
        this._response = new();
    }

    [HttpGet]
    [MapToApiVersion("2.0")]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }
    
    [HttpGet]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<ApiResponse>> GetVillas()
    {
        try
        {
            _logger.LogInformation("Getting all villas");
            IEnumerable<Villa> villaList = await _dbVilla.GetAll();
            _response.Result = _mapper.Map<List<VillaDto>>(villaList);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }
    
    
    [HttpGet("{id:int}", Name = "GetVilla")]
    public async Task<ActionResult<ApiResponse>> GetVilla(int id)
    {
        try {
            if (id == 0)
            {
                _logger.LogError("Get villa Error with id " + id);
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }

            var villa = await _dbVilla.Get(v => v.Id == id);
            if (villa == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }

            _response.Result = _mapper.Map<VillaDto>(villa);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }

    
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> CreateVilla([FromBody]VillaCreateDto createDto)
    {
        try
        {
            // custom validation
            if (await _dbVilla.Get(v => v.Name.ToLower() == createDto.Name.ToLower()) != null )
            {
                ModelState.AddModelError("errorMessages", "Villa already exist");
                return BadRequest(ModelState);
            }

            if (createDto == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }

            Villa model = _mapper.Map<Villa>(createDto);

            await _dbVilla.Create(model);
            _response.Result = _mapper.Map<VillaDto>(model);
            _response.StatusCode = HttpStatusCode.Created;
            
            return CreatedAtRoute("GetVilla", new { id = model.Id }, _response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }

    
    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse>> DeleteVilla(int id)
    {
        try
        {

            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }

            var villa = await _dbVilla.Get(v => v.Id == id);
            if (villa == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }

            await _dbVilla.Remove(villa);
            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }

    
    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
    {
        try {
            if (updateDto == null || id != updateDto.Id)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }

            Villa model = _mapper.Map<Villa>(updateDto);

            await _dbVilla.Update(model);

            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }

    
    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [Authorize]
    public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
    {
        if (patchDto == null || id == 0)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            return BadRequest(_response);
        }

        var villa = await _dbVilla.Get(v => v.Id == id, tracked:false);
        VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);
        if (villa == null)
        {
            _response.StatusCode = HttpStatusCode.NotFound;
            _response.IsSuccess = false;
            return NotFound(_response);
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