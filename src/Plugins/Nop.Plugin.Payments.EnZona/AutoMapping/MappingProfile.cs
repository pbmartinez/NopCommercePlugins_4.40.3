using AutoMapper;
using Nop.Plugin.Payments.EnZona.Models;

namespace Nop.Plugin.Payments.EnZona.AutoMapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PaymentsEnZonaSettings, ConfigurationModel>().ReverseMap();
        }
    }
}
