using System.Net;
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
public class VillaNumberApiController : ControllerBase
{
    protected ApiResponse _response;
    private readonly ILogger<VillaNumberApiController> _logger;
    private readonly IMapper _mapper;
    private readonly IVillaNumberRepository _dbVillaNumber;
    private readonly IVillaRepository _dbVilla;

    public VillaNumberApiController(IVillaNumberRepository dbVillaNumber,IVillaRepository dbVilla,  ILogger<VillaNumberApiController> logger, IMapper mapper)
    {
        _dbVillaNumber = dbVillaNumber;
        _dbVilla = dbVilla;
        _logger = logger;
        _mapper = mapper;
        this._response = new();
    }
    
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetVillaNumbers()
    {
        try
        {
            _logger.LogInformation("Getting all villas");
            IEnumerable<VillaNumber> villaNumberList = await _dbVillaNumber.GetAll(includeProperties:"Villa");
            _response.Result = _mapper.Map<List<VillaNumberDto>>(villaNumberList);
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
    
    
    [HttpGet("{id:int}", Name = "GetVillaNumber")]
    public async Task<ActionResult<ApiResponse>> GetVillaNumber(int id)
    {
        try {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }

            var model = await _dbVillaNumber.Get(v => v.VillaNo == id, includeProperties:"Villa");
            if (model == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }

            _response.Result = _mapper.Map<VillaNumberDto>(model);
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
    public async Task<ActionResult<ApiResponse>> CreateVillaNumber([FromBody]VillaNumberCreateDto createDto)
    {
        try
        {
            // custom validation
            if (await _dbVillaNumber.Get(v => v.VillaNo == createDto.VillaNo) != null )
            {
                ModelState.AddModelError("CustomError", "Villa Number already exist");
                return BadRequest(ModelState);
            }

            if (await _dbVilla.Get(m => m.Id == createDto.VillaId) == null )
            {
                ModelState.AddModelError("CustomError", "Villa ID is Invalid");
                return BadRequest(ModelState);
            }
            
            if (createDto == null) return BadRequest(createDto);

            VillaNumber model = _mapper.Map<VillaNumber>(createDto);

            await _dbVillaNumber.Create(model);
            _response.Result = _mapper.Map<VillaNumberDto>(model);
            _response.StatusCode = HttpStatusCode.Created;
            
            return CreatedAtRoute("GetVilla", new { id = model.VillaNo }, _response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }

    
    [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
    public async Task<ActionResult<ApiResponse>> DeleteVillaNumber(int id)
    {
        try
        {

            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }

            var villa = await _dbVillaNumber.Get(v => v.VillaNo == id);
            if (villa == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }

            await _dbVillaNumber.Remove(villa);
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

    
    [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
    public async Task<ActionResult<ApiResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDto updateDto)
    {
        try {
            if (updateDto == null || id != updateDto.VillaNo)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(updateDto);
            }
            if (await _dbVilla.Get(m => m.Id == updateDto.VillaId) == null )
            {
                ModelState.AddModelError("CustomError", "Villa ID is Invalid");
                return BadRequest(ModelState);
            }

            VillaNumber model = _mapper.Map<VillaNumber>(updateDto);

            await _dbVillaNumber.Update(model);

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
    
}