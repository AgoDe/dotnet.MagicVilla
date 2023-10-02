using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<Villa, VillaDto>();
        CreateMap<VillaDto, Villa>();
        CreateMap<Villa, VillaCreateDto>().ReverseMap();
        CreateMap<Villa, VillaUpdateDto>().ReverseMap();
        
        CreateMap<VillaNumber, VillaNumberDto>();
        CreateMap<VillaNumberDto, VillaNumber>();
        CreateMap<VillaNumber, VillaNumberCreateDto>().ReverseMap();
        CreateMap<VillaNumber, VillaNumberUpdateDto>().ReverseMap();

        CreateMap<ApplicationUser, UserDTO>().ReverseMap();
    }
}