using AutoMapper;

namespace Consensus.Mappings
{
    public class PipeProfile : Profile
    {
        public PipeProfile()
        {
            CreateMap<Data.Entities.Pipe, ApiContracts.Pipe>();
        }
    }
}
