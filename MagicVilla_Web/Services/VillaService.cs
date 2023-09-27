using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services;

public class VillaService : BaseService, IVillaService
{
    private readonly IHttpClientFactory _clientFactory;
    private string _villaUrl;
    
    public VillaService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory, configuration)
    {
        _clientFactory = clientFactory;
        _villaUrl = Uri + "/api/villaApi";
    }

    public Task<T> GetAll<T>()
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = _villaUrl
        });
    }

    public Task<T> Get<T>(int id)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = _villaUrl + $"/{id}"
        });
    }

    public Task<T> Create<T>(VillaCreateDto dto, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = dto,
            Url = _villaUrl,
            Token = token
        });
    }

    public Task<T> Update<T>(VillaUpdateDto dto, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.PUT,
            Data = dto,
            Url = _villaUrl + $"/{dto.Id}",
            Token = token
        });
    }

    public Task<T> Delete<T>(int id, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.DELETE,
            Url = _villaUrl + $"/{id}",
            Token = token
        });
    }
}