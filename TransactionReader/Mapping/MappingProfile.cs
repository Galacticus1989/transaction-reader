using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransactionReader.Models;

namespace TransactionReader.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Transaction, TransactionDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.payment, opt => opt.MapFrom(src => src.Amount.ToString("0.00") + " " + src.CurrencyCode))
                .ForMember(dest => dest.status, opt => opt.MapFrom(src => src.Status));
        }
    }
}
