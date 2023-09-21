using System.Collections.Specialized;
using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers;

public class VillaNumberController : Controller
{
    private readonly IVillaNumberService _villaNumberService;
    private readonly IMapper _mapper;

    public VillaNumberController(IVillaNumberService villaNumberService, IMapper mapper)
    {
        _villaNumberService = villaNumberService;
        _mapper = mapper;
    }
    
    // GET
    public async Task<IActionResult> Index()
    {
        List<VillaNumberDto> list = new();

        var response = await _villaNumberService.GetAll<ApiResponse>();
        if (response != null && response.IsSuccess)
        {
            list = JsonConvert.DeserializeObject<List<VillaNumberDto>>(Convert.ToString((response.Result)));
        }

        return View(list);


    }

    // GET
    public async Task<IActionResult> Create()
    {
        return View();
    }
    
    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VillaNumberCreateDto model)
    {
        if (ModelState.IsValid)
        {
            var response = await _villaNumberService.Create<ApiResponse>(model);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

        }
        return View(model);
    }
    
    // GET
    public async Task<IActionResult> Update(int id)
    {
        var response = await _villaNumberService.Get<ApiResponse>(id);
        if (response != null && response.IsSuccess)
        {
            VillaNumberDto model = JsonConvert.DeserializeObject<VillaNumberDto>(Convert.ToString(response.Result));
            return View(_mapper.Map<VillaNumberUpdateDto>(model));
        }

        return NotFound();
    }
    
    // PUT 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(VillaNumberUpdateDto model)
    {
        if (ModelState.IsValid)
        {
            var response = await _villaNumberService.Update<ApiResponse>(model);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

        }
        return View(model);
    }
    
    // GET
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _villaNumberService.Get<ApiResponse>(id);
        if (response != null && response.IsSuccess)
        {
            VillaNumberDto model = JsonConvert.DeserializeObject<VillaNumberDto>(Convert.ToString(response.Result));
            return View(model);
        }

        return NotFound();
    }
    // DELETE 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(VillaNumberDto model)
    {
       
        var response = await _villaNumberService.Delete<ApiResponse>(model.VillaNo);
        if (response != null && response.IsSuccess)
        {
            return RedirectToAction(nameof(Index));
        }
        
        return View(model);
    }
}