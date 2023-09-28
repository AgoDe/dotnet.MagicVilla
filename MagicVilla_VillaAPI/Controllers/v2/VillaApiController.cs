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

namespace MagicVilla_VillaAPI.Controllers.v2;

[Route("api/v{version:apiVersion}/[controller]")] // definisce il nome della rotta
[ApiController]
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
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }
    
}