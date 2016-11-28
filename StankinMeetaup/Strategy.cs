using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atentis.History;

namespace StankinMeetaup
{
    public static class Strategy
    {
        public static List<Client> clients = new List<Client>();

        //для работы с историей
        static HistoryProvider provider = new HistoryProvider();
        static List<RawCandle> res = new List<RawCandle>();

        public static void start()
        {
            //законектились клиентом
            foreach(var client in clients)
             client.connect();

            //подписались на обновление свечей
            HistoryRequest req = new HistoryRequest(Client.SECBOARD, Client.SECCODE, 60, DateTime.Now.AddDays(-5), DateTime.Now.AddDays(2));
            res = provider.LoadHistory(req, true);
            provider.OnCandleFinished += strateg;
        }

        public static void stop()
        {
            //отконектились клиентом
            foreach (var client in clients)
            client.disconnect();

            //отписались от обновления
            provider.OnCandleFinished -= strateg;
        }

        public static bool SMA(int bars, RawCandle candle)
        {
            //обрабатываем историю и считаем SMA
            double sma=0;
            for (int i=0; i<bars; i++){
                sma+=res[i].Close;
            }
            sma = sma/bars;

            //пересекает снизу else сверху
            if(candle.Close<sma)
                return true;
            else
                return false;

        }

        private static void strateg(RawCandle candle)
        {
            if (SMA(20,candle)) 
            {
                foreach (var client in clients)
                {
                    if (client.positions > 0)
                        client.makeOrder("s", 2);
                    else if (client.positions == 0)
                        client.makeOrder("s", 1);
                }
            }
            else
            {
                foreach (var client in clients)
                {
                    if (client.positions < 0)
                        client.makeOrder("b", 2);
                    else if (client.positions == 0)
                        client.makeOrder("b", 1);
                }
            }
        }
    }
}
