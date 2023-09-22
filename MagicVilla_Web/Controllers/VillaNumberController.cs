using System.Collections.Specialized;
using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Models.VM;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers;

public class VillaNumberController : Controller
{
    private readonly IVillaNumberService _villaNumberService;
    private readonly IVillaService _villaService;
    private readonly IMapper _mapper;

    public VillaNumberController(IVillaNumberService villaNumberService, IMapper mapper, IVillaService villaService)
    {
        _villaNumberService = villaNumberService;
        _villaService = villaService;
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
        VillaNumberCreateVM villaNumberVM = new();

        var response = await _villaService.GetAll<ApiResponse>();
        if (response != null && response.IsSuccess)
        {
            villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(response.Result))
                .Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString(),
                    });
        }
        return View(villaNumberVM);
    }
    
    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VillaNumberCreateVM model)
    {
        if (ModelState.IsValid)
        {
            var response = await _villaNumberService.Create<ApiResponse>(model.VillaNumber);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if (response.ErrorMessages.Count > 0)
                {
                    ModelState.AddModelError("errorMessages", response.ErrorMessages.FirstOrDefault());
                }
            }

        }
        
        var res = await _villaService.GetAll<ApiResponse>();
        if (res != null && res.IsSuccess)
        {
            model.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(res.Result))
                .Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),
                });
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