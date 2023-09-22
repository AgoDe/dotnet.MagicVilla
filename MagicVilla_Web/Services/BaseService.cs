using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;

namespace MagicVilla_Web.Services;

public class BaseService : IBaseService
{
    public ApiResponse responseModel { get; set; }
    public IHttpClientFactory httpClient { get; set; }
    protected String Uri;

    public BaseService(IHttpClientFactory httpClient, IConfiguration configuration)
    {
        this.Uri = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        this.responseModel = new();
        this.httpClient = httpClient;
    }
    
    public async Task<T> SendAsync<T>(ApiRequest apiRequest)
    {
        try
        {
            var client = httpClient.CreateClient("MagicAPI");
            HttpRequestMessage message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new Uri(apiRequest.Url);
            if (apiRequest.Data != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8,
                    "application/json");
            }

            switch (apiRequest.ApiType)
            {
                case SD.ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;
                case SD.ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                case SD.ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            HttpResponseMessage apiResponse = null;

            apiResponse = await client.SendAsync(message);
            var apiContent = await apiResponse.Content.ReadAsStringAsync();

            // TODO: pezzo da rivedere... non ne capisco il senso
            try
            {
                ApiResponse response =  JsonConvert.DeserializeObject<ApiResponse>(apiContent);
                if (apiResponse.StatusCode == HttpStatusCode.BadRequest || apiResponse.StatusCode == HttpStatusCode.NotFound )
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    var res = JsonConvert.SerializeObject(response);
                    var returnObj = JsonConvert.DeserializeObject<T>(res);
                    return returnObj;
                }
            }
            catch (Exception e)
            {
                var ExceptionResponse = JsonConvert.DeserializeObject<T>(apiContent);
                return ExceptionResponse;
            }

            var APIResponse = JsonConvert.DeserializeObject<T>(apiContent);
            return APIResponse;

        }
        catch (Exception e)
        {
            var dto = new ApiResponse
            {
                ErrorMessages = new List<string>{ Convert.ToString(e.Message)},
                IsSuccess = false
            };
            var res = JsonConvert.SerializeObject(dto);
            var APIResponse = JsonConvert.DeserializeObject<T>(res);
            return APIResponse;
        }
    }
    
}