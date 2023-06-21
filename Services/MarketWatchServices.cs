using AutoMapper;
using Grpc.Core;
using GrpcService1.DAL;
using GrpcService1.Protos;
using System.Collections;
using System.Net.WebSockets;
using System.Text;
using static GrpcService1.Protos.MarketWatchDataServices;

namespace GrpcService1.Services
{
    public class MarketWatchDataServices : MarketWatchDataServicesBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper _Mapper;
        private readonly ILogger<MarketWatchDataServices> _logger;
        
        public MarketWatchDataServices(IHttpClientFactory httpClientFactory, IMapper mapper, ILogger<MarketWatchDataServices> logger)
        {
            _httpClientFactory = httpClientFactory;
            _Mapper = mapper; 
            _logger = logger;
        }

        public override async Task<ScripDetailsResponse> GetScripDetals(ScripDetailsRequest request, ServerCallContext context)
        {
            var httpclient = _httpClientFactory.CreateClient();

            MarketDataManager marketDataManager = new MarketDataManager();

            marketDataManager.CreateList();
            ArrayList arrayList = marketDataManager.GetListOFSymbols();

            //ArrayList arrayList =marketDataManager.GetListOFSymbols();


            //MarketWatchData scripMarketWatchData = arrayList[0] as MarketWatchData;
            MboMbpBroadcast _MboMbpBroadcast = (MboMbpBroadcast)arrayList[0];
            ScripDetailsResponse scripDeatilsResponse  = new ScripDetailsResponse();

            scripDeatilsResponse.Symbol = _MboMbpBroadcast.symbol;
            scripDeatilsResponse.Price = Convert.ToDouble(_MboMbpBroadcast.lastPrice);
            scripDeatilsResponse.Volume = Convert.ToDouble(_MboMbpBroadcast.baseVolume);

            return scripDeatilsResponse;
        }

        public override async Task GetScripDetailsStream(ScripDetailsRequest request, 
            IServerStreamWriter<ScripDetailsResponse> responseStream, 
            ServerCallContext context)
        {
            var httpclient = _httpClientFactory.CreateClient();
            int i = 0;

            while (!context.CancellationToken.IsCancellationRequested)
            {
                MarketDataManager marketDataManager = new MarketDataManager();

                ArrayList arrayList = marketDataManager.GetListOFSymbols();

                MarketWatchData scripMarketWatchData = arrayList[i] as MarketWatchData;

                await responseStream.WriteAsync(
                    _Mapper.Map<ScripDetailsResponse>(scripMarketWatchData));

                //await responseStream.WriteAsync(new ScripDetailsResponse
                //{
                //    Symbol = scripMarketWatchData.Symbol,
                //    Price = scripMarketWatchData.Price,
                //    Volume = scripMarketWatchData.Volume,
                //});
                i++;

                if (arrayList.Count==i)
                {
                    i = 0;
                }
                await Task.Delay(100);
            }

            #region 

            //for (var i = 0; i < 100; i++)
            //{
            //    if (context.CancellationToken.IsCancellationRequested)
            //    {
            //        break;
            //    }

            //    MarketDataManager marketDataManager = new MarketDataManager();

            //    ArrayList arrayList = marketDataManager.GetListOFSymbols();

            //    MarketWatchData scripMarketWatchData = arrayList[0] as MarketWatchData;

            //    await responseStream.WriteAsync(new ScripDetailsResponse
            //    {
            //        Symbol = scripMarketWatchData.Symbol,
            //        Price = scripMarketWatchData.Price,
            //        Volume = scripMarketWatchData.Volume,
            //    }
            //    );

            //    await Task.Delay(1000);
            //}
            #endregion

        }

        public override Task<LoginResponse> GetLoginDetails(LoginRequest request, ServerCallContext context)
        {
            LoginResponse _response = new LoginResponse();
            _response.Status = "Connected";

            return base.GetLoginDetails(request, context);
        }

        public override async Task GetScripBroadcast(ScripBroadcastRequest scripBroadcastRequest, 
            IServerStreamWriter<RealTimeMarketDataResponse> responseStream, 
            ServerCallContext context)
        {
            MarketDataManager marketDataManager = new MarketDataManager();

            marketDataManager.GetBinanceDataOnWebSocket();

            var httpclient = _httpClientFactory.CreateClient();
            int i = 0;
            
            marketDataManager.CreateList();
            ArrayList arrayList = marketDataManager.GetListOFSymbols();

            while (!context.CancellationToken.IsCancellationRequested)
            {

                //foreach (var item in arrayList)
                //{
                    MboMbpBroadcast scripMarketWatchData = (MboMbpBroadcast)arrayList[i];

                    await responseStream.WriteAsync(
                        _Mapper.Map<RealTimeMarketDataResponse>(scripMarketWatchData));
                //}
               

                //await responseStream.WriteAsync(new ScripDetailsResponse
                //{
                //    Symbol = scripMarketWatchData.Symbol,
                //    Price = scripMarketWatchData.Price,
                //    Volume = scripMarketWatchData.Volume,
                //});
                i++;

                if (arrayList.Count == i)
                {
                    i = 0;
                    marketDataManager.CreateList();
                    arrayList = marketDataManager.GetListOFSymbols();
                }
                await Task.Delay(10000);
            }
        }

        public override async Task GetScripBinanceBroadcast(ScripBroadcastReq request, IServerStreamWriter<ScripBroadcastResponse> responseStream, ServerCallContext context)
        {

            MarketDataManager marketDataManager = new MarketDataManager();

            marketDataManager.GetBinanceDataOnWebSocket();

            var httpclient = _httpClientFactory.CreateClient();
            int i = 0;

            marketDataManager.CreateList();
            //ArrayList arrayList = marketDataManager.GetListOFSymbols();
            ArrayList arrayListB = marketDataManager.GetBroadcastStreamList();

            marketDataManager.Clear();

            while (!context.CancellationToken.IsCancellationRequested)
            {

                //foreach (var item in arrayList)
                //{
                

                if (arrayListB != null)
                {
                    if (arrayListB.Count == i)
                    {
                        arrayListB = marketDataManager.GetBroadcastStreamList();
                    }

                    if (arrayListB.Count > i)
                    {
                        String scripMarketWatchData = arrayListB[i].ToString();
                        ScripBroadcastResponse obj = new ScripBroadcastResponse();
                        obj.Message = scripMarketWatchData;

                        await responseStream.WriteAsync(obj);

                        i++;
                        //}
                    }
                   
                }
                else
                {
                    arrayListB = marketDataManager.GetBroadcastStreamList();
                }

                //await responseStream.WriteAsync(new ScripDetailsResponse
                //{
                //    Symbol = scripMarketWatchData.Symbol,
                //    Price = scripMarketWatchData.Price,
                //    Volume = scripMarketWatchData.Volume,
                //});
               

                //if (arrayListB.Count == i)
                //{
                //    i = 0;
                //    marketDataManager.CreateList();
                //    arrayListB = marketDataManager.GetListOFSymbols();
                //}
                await Task.Delay(100);
            }
        }
    }
}
