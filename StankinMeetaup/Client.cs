using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Atentis.Connection;

namespace StankinMeetaup
{
    public class Client
    {
        #region Клиентские методы

        //сущность которая поддерживает соединение
        private Slot slot = new Slot(); 
        private string name="Study";
        private string server = "study.alor.ru";
        private int port=7800;
        private string login = "study39226";
        private string password = "875357";

        public Client() { }

        public void connect()
        {
            slot.SlotID = this.name;
            slot.Server = this.server;
            slot.Port = this.port;
            slot.Login = this.login;
            slot.Password = this.password;

            slot.rqs = new RequestSocket(slot);
            slot.rqs.Init();
            slot.Start();
            setSlotEventHandlers(slot);
            setRSocketEventHandlers(slot);
        }

        public void disconnect()
        {
            slot.Disconnect();
            if (slot.rqs != null)
            {
                removeSlotEventHandlers(slot);
                removeRSocketEventHandlers(slot);
            }
        }

        #endregion

        #region транзакционные методы

        public void makeOrder(String buysell, int lots)
        {
            long orderNo;
            string resultMsg;

            slot.AddMarketOrder(buysell, SECBOARD, SECCODE, lots, "", out orderNo, out resultMsg);

            Logger.AddEvent(slot.SlotID, resultMsg);
        }

        #endregion

        #region портфель

        public int positions = 0;
        public static string SECBOARD = "TQBR";
        public static string SECCODE = "GAZP";

        #endregion

        #region обработчики состояния

        public void slot_evhSlotStateChanged(object sender, SlotEventArgs e)
        {
            if (e.State == SlotState.Denied)
            {
                Logger.AddEvent(slot.SlotID, "Denied");
            }
            if (e.State == SlotState.Disconnected)
            {
                Logger.AddEvent(slot.SlotID, "Disconnected");
            }
            if (e.State == SlotState.Connected)
            {
                Logger.AddEvent(slot.SlotID, "Connected");
            }
            if (e.State == SlotState.Failed)
            {
                Logger.AddEvent(slot.SlotID, "Failed");
            }
        }

        //Внутренний лог
        public void rqs_evhLogLine(object sender, TableEventArgs e)
        {
            Logger.AddEvent(e.RequestSocket.slot.SlotID, e.Message);
        }

        //Информация об успешной авторизации
        public void rqs_evhServiceLoggedIn(object sender, TableEventArgs e)
        {
            Logger.AddEvent("EVENT", "LoggedIn");
        }

        //Требуется создание новой сессии. Работа с текущей сессией далее невозможна
        public void rqs_evhNewSession(object sender, TableEventArgs e)
        {
            Logger.AddEvent("EVENT", "New session required");
            System.Threading.ThreadPool.QueueUserWorkItem(reconnectSlotAsync, e);

        }
    
        // необходима смена пароля
        private void rqs_evhNeedNewPassword(object sender, TableEventArgs e)
        {
            Logger.AddEvent("EVENT", "Need new password");
        }

        /// Клонирование слота
        Slot cloneSlot(Slot cslot)
        {
            Slot newSlot = new Slot();
            newSlot.PublicKeyFile = "";
            newSlot.rqs = new RequestSocket(newSlot);
            newSlot.SlotID = this.name;
            newSlot.Server = this.server;
            newSlot.Port = this.port;
            newSlot.Login = this.login;
            newSlot.Password = this.password;
            setSlotEventHandlers(newSlot);
            setRSocketEventHandlers(newSlot);
            return newSlot;
        }

        /// Переподключение к серверу
        public void reconnectSlotAsync(Object obj)
        {
            //доделать
        }

        /// Добавление обработчиков событий слота
        public void setSlotEventHandlers(Slot slot)
        {
            slot.evhSlotStateChanged += new SlotEventHandler(slot_evhSlotStateChanged);
        }

        /// Удаление обработчиков событий слота
        public void removeSlotEventHandlers(Slot slot)
        {
            slot.evhSlotStateChanged -= slot_evhSlotStateChanged;

        }

        /// Добавление обработчиков событий внутренненго объекта RequestSocket
        public void setRSocketEventHandlers(Slot slot)
        {
            slot.rqs.evhLoggedIn += rqs_evhServiceLoggedIn;
            slot.rqs.evhNewSession += rqs_evhNewSession;
            slot.rqs.evhNeedNewPassword += rqs_evhNeedNewPassword;
            slot.rqs.evhLogLine += rqs_evhLogLine;

        }

        /// Удаление обработчиков событий внутренненго объекта RequestSocket
        public void removeRSocketEventHandlers(Slot slot)
        {
            slot.rqs.evhLoggedIn -= rqs_evhServiceLoggedIn;
            slot.rqs.evhNewSession -= rqs_evhNewSession;
            slot.rqs.evhNeedNewPassword -= rqs_evhNeedNewPassword;
            slot.rqs.evhLogLine -= rqs_evhLogLine;
        }

        #endregion
    }
}
