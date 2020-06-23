using AutoMapper;
using Predict.Dtos;
using Predict.Models;
using Predict.ViewModels;

namespace Predict.App_Start
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PlayerDto, Player>();
            CreateMap<Player, PlayerDto>();
            CreateMap<Pool, Pool>();
            CreateMap<Fixture, FixtureViewModel>();
            CreateMap<FixtureViewModel, Fixture>();
            CreateMap<KoFixtureViewModel, KoFixture>();
            CreateMap<KoFixture, KoFixtureViewModel>();
        }
    }
}