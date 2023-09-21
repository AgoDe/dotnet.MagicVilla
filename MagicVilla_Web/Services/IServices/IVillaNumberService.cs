using MagicVilla_Web.Models.Dto;

namespace MagicVilla_Web.Services.IServices;

public interface IVillaNumberService
{
    Task<T> GetAll<T>();
    Task<T> Get<T>(int id);
    Task<T> Create<T>(VillaNumberCreateDto dto);
    Task<T> Update<T>(VillaNumberUpdateDto dto);
    Task<T> Delete<T>(int id);
    
}