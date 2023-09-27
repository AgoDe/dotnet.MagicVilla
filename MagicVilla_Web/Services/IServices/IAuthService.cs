using MagicVilla_Web.Models.Dto;

namespace MagicVilla_Web.Services.IServices;

public interface IAuthService
{
    Task<T> Login<T>(LoginRequestDTO objToCreate);
    Task<T> Register<T>(RegistrationRequestDto objToCreate);
}