using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services;

public class VillaNumberService : BaseService, IVillaNumberService
{
    private readonly IHttpClientFactory _clientFactory;
    private string _villaNumberUrl;
    
    public VillaNumberService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory, configuration)
    {
        _clientFactory = clientFactory;
        _villaNumberUrl = Uri + "/api/v1/villaNumberApi";
    }

    public Task<T> GetAll<T>()
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = _villaNumberUrl
        });
    }

    public Task<T> Get<T>(int id)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = _villaNumberUrl + $"/{id}"
        });
    }
    

    public Task<T> Create<T>(VillaNumberCreateDto dto, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = dto,
            Url = _villaNumberUrl,
            Token = token
        });
    }

    public Task<T> Update<T>(VillaNumberUpdateDto dto, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.PUT,
            Data = dto,
            Url = _villaNumberUrl + $"/{dto.VillaNo}",
            Token = token
        });
    }

    public Task<T> Delete<T>(int id, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.DELETE,
            Url = _villaNumberUrl + $"/{id}",
            Token = token
        });
    }
}