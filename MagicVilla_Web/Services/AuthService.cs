using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services;

public class AuthService : BaseService, IAuthService
{
    private readonly IHttpClientFactory _clientFactory;
    private string authUrl;
    
    public AuthService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient, configuration)
    {
        _clientFactory = httpClient;
        authUrl = Uri + "/api/UsersAuth";
    }

    public Task<T> Login<T>(LoginRequestDTO credentials)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = credentials,
            Url = authUrl + "/login"

        });
    }

    public Task<T> Register<T>(RegistrationRequestDto objToCreate)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = objToCreate,
            Url = authUrl + "/register"

        });
    }
    
}