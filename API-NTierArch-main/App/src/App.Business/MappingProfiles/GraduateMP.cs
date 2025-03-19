using AutoMapper;
using App.Business.DTOs;
using App.Core.Entities;

namespace App.Business.Mappers
{
    public class GraduateMapperProfile : Profile
    {
        public GraduateMapperProfile()
        {
            CreateMap<Graduate, GraduateDTO>().ReverseMap();
            CreateMap<CreateGraduateDTO, Graduate>();
            CreateMap<UpdateGraduateDTO, Graduate>();
            CreateMap<DeleteGraduateDTO, Graduate>();
        }
    }
}
