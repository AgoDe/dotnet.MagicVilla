using System.Collections.Specialized;
using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers;

public class VillaController : Controller
{
    private readonly IVillaService _villaService;
    private readonly IMapper _mapper;

    public VillaController(IVillaService villaService, IMapper mapper)
    {
        _villaService = villaService;
        _mapper = mapper;
    }
    
    // GET
    public async Task<IActionResult> Index()
    {
        List<VillaDto> list = new();

        var response = await _villaService.GetAll<ApiResponse>();
        if (response != null && response.IsSuccess)
        {
            list = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString((response.Result)));
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
    public async Task<IActionResult> Create(VillaCreateDto model)
    {
        if (ModelState.IsValid)
        {
            var response = await _villaService.Create<ApiResponse>(model);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Villa created successfully";
                return RedirectToAction(nameof(Index));
            }

        }
        TempData["error"] = "Error encountered";
        return View(model);
    }
    
    // GET
    public async Task<IActionResult> Update(int id)
    {
        var response = await _villaService.Get<ApiResponse>(id);
        if (response != null && response.IsSuccess)
        {
            VillaDto model = JsonConvert.DeserializeObject<VillaDto>(Convert.ToString(response.Result));
            return View(_mapper.Map<VillaUpdateDto>(model));
        }

        return NotFound();
    }
    
    // PUT 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(VillaUpdateDto model)
    {
        if (ModelState.IsValid)
        {
            var response = await _villaService.Update<ApiResponse>(model);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Villa updated successfully";
                return RedirectToAction(nameof(Index));
            }

        }
        TempData["error"] = "Error encountered";
        return View(model);
    }
    
    // GET
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _villaService.Get<ApiResponse>(id);
        if (response != null && response.IsSuccess)
        {
            VillaDto model = JsonConvert.DeserializeObject<VillaDto>(Convert.ToString(response.Result));
            return View(model);
        }

        return NotFound();
    }
    // DELETE 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(VillaDto model)
    {
       
        var response = await _villaService.Delete<ApiResponse>(model.Id);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Villa deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        TempData["error"] = "Error encountered";
        return View(model);
    }
}