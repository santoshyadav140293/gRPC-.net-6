using AutoMapper;
using GrpcService1.DAL;
using GrpcService1.Protos;

namespace GrpcService1.Mapper
{
    public class MarketWatchProfile : Profile
    {

        public MarketWatchProfile()
        {
            CreateMap<RealTimeMarketDataResponse, MboMbpBroadcast>();

            CreateMap<MboMbpBroadcast, RealTimeMarketDataResponse>();
        }
    }
}
