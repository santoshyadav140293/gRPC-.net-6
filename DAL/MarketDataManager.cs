using System;
using System.Collections;
using System.Net.WebSockets;
using System.Text;

namespace GrpcService1.DAL
{
    public class MarketDataManager
    {
        ArrayList arrayList;

        ArrayList BcastArrayList;

        public object lockobject=new object();

        List<string> symboldata;
        public MarketDataManager()
        {
            BcastArrayList = new ArrayList();
        }

        public void CreateList()
        {
            symboldata = new List<string> {
                "LTCBTC",
"ETHBTC",
"BNBBTC",
"NEOBTC",
"QTUMETH",
"EOSETH",
"SNTETH",
"BNTETH",
"BCCBTC",
"GASBTC",
"BNBETH",
"BTCUSDT",
"ETHUSDT",
"ETHBUSD",
"BCHABCBUSD",
"LTCBUSD",
 };
            //symboldata[0] = "BTC";
            arrayList = new ArrayList();
            Random random = new Random();
            //int price = .Next(100);
            for (int i = 0; i < symboldata.Count; i++)
            {
                MboMbpBroadcast objMboMbpBroadcast = new MboMbpBroadcast();

                objMboMbpBroadcast.symbol = symboldata[i];
                objMboMbpBroadcast.quoteVolume = "999999999.2323";
                objMboMbpBroadcast.lastPrice = random.Next(40000, 49999).ToString();
                objMboMbpBroadcast.baseVolume = random.Next(900000,999999).ToString();
                objMboMbpBroadcast.openPrice = random.Next(9884484);
                objMboMbpBroadcast.askPrice = objMboMbpBroadcast.lastPrice;
                objMboMbpBroadcast.askQuantity = "100";
                objMboMbpBroadcast.bidQuantity = "100";
                objMboMbpBroadcast.bidPrice = objMboMbpBroadcast.lastPrice;
                objMboMbpBroadcast.openPrice = objMboMbpBroadcast.openPrice;
                objMboMbpBroadcast.lowPrice = 8454512;
                objMboMbpBroadcast.closeTime = DateTime.Now.ToString();
                objMboMbpBroadcast.totalTrades = 0;
                objMboMbpBroadcast.weightedAveragePrice = (7845855 / 100).ToString();
                objMboMbpBroadcast.openTime = DateTime.Now.ToString();
                objMboMbpBroadcast.lastQuantity = "100";
                objMboMbpBroadcast.lastTradeId = "123456";
                objMboMbpBroadcast.firstTradeId = 4561;
                objMboMbpBroadcast.eventType = 0;
                objMboMbpBroadcast.openPrice = random.Next();
                objMboMbpBroadcast.eventTime= DateTime.Now.ToString();
                objMboMbpBroadcast.firstTrade = DateTime.Now.ToString();
                objMboMbpBroadcast.highPrice = "49999";
                objMboMbpBroadcast.priceChange = random.Next(10, 100).ToString();
                objMboMbpBroadcast.priceChangePer = random.Next(10, 100).ToString();
                arrayList.Add(objMboMbpBroadcast);
            }
        }

        public async Task GetBinanceDataOnWebSocket()
        {

            ClientWebSocket clientWebSocket = new ClientWebSocket();

            //WebSocket webSocket = new WebSocket("wss://stream.binance.com:9443/ws/!ticker@arr");
            //Uri.parse("wss://stream.binance.com:9443/ws/!ticker@arr"));
            Uri serverUri = new Uri("wss://stream.binance.com:9443/ws/!ticker@arr");

            await clientWebSocket.ConnectAsync(serverUri, CancellationToken.None);

            Task receiveTask = ReceiveData(clientWebSocket);

            //byte[] receiveBuffer = new byte[2048];
            //var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
            //string receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
            //ArrayList binanceDataArrayList = new ArrayList();
            //binanceDataArrayList.Add(receivedMessage);

            //Console.WriteLine("Received message: " + receivedMessage);

        }

        public async Task ReceiveData(ClientWebSocket clientWebSocket)
        {
            byte[] receiveBuffer = new byte[1024];
            //List<byte> receiveBuffer1 = new List<byte>();

            string receivedMessage=string.Empty;
            
            while (clientWebSocket.State == WebSocketState.Open)
            {
                try
                {
                    var receiveResult = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

                    if (receiveResult.EndOfMessage)
                    {
                        string strReceivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, receiveResult.Count);
                        receivedMessage += strReceivedMessage;
                        //Console.WriteLine("Received message: " + receivedMessage);

                        lock (lockobject)
                        {
                            BcastArrayList.Add(receivedMessage);
                        }

                        receivedMessage = string.Empty;
                    }
                    else
                    {
                        string strReceivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, receiveResult.Count);

                        receivedMessage += strReceivedMessage;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while receiving data: " + ex.Message);
                }
            }
        }



        public ArrayList GetListOFSymbols()
        {
            if (arrayList.Count > 0)
            {
                return arrayList;
            }
            else
            {
                return null;
            }
        }



        public ArrayList GetBroadcastStreamList()
        {
            lock (lockobject)
            {
                if (BcastArrayList.Count > 0)
                {
                    return BcastArrayList;
                }
                else
                {
                    return null;
                }
            }
        }

        public void Clear()
        {
            lock (lockobject)
            {
                BcastArrayList.Clear();
            }
        }
    }
}
