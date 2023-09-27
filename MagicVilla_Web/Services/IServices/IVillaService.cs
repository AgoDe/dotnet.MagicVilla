using MagicVilla_Web.Models.Dto;

namespace MagicVilla_Web.Services.IServices;

public interface IVillaService
{
    Task<T> GetAll<T>();
    Task<T> Get<T>(int id);
    Task<T> Create<T>(VillaCreateDto dto, string token);
    Task<T> Update<T>(VillaUpdateDto dto, string token);
    Task<T> Delete<T>(int id, string token);
    
}