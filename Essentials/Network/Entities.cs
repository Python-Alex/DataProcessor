using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace DataProcessor.Essentials.Network.Entities
{
    public abstract class BaseConnectionThread
    {
        private Thread _thread;

        protected BaseConnectionThread()
        {
            _thread = new Thread(new ThreadStart(RunThread));
        }

        // Thread methods / properties
        public void Start() => _thread.Start();
        public void Join() => _thread.Join();
        public bool IsAlive => _thread.IsAlive;

        // Override in base class
        public abstract void RunThread();
    }

    // upper-level class to handle connection
    public class ConnectionThread : BaseConnectionThread
    {
        public static Socket? Connection;

        public static int BytesReceived;
        public static DateTime LastReceive;
        public static bool Errored;
        public Exception Error;
        public static bool NotifiedInterval;

        public Socket? GetConnection() => Connection;
        public int GetBytesReceived() => BytesReceived;
        public DateTime GetLastReceived() => LastReceive;
        public bool ErroredOut() => Errored;
        public Exception GetException() => Error;

        public void SetNotified()
        { NotifiedInterval = true; }
        public void UnsetNotified()
        { NotifiedInterval = false; }
        public bool GetNotified()
        { return NotifiedInterval; }

        public ConnectionThread(Socket client)
            : base()
        {
            if (!Manager.InsertInstance(this))
                return;

            Connection = client;

            BytesReceived = 0;

            client.ReceiveTimeout = Properties.ClientReceiveTimeout * 1000;
            client.SendTimeout = Properties.ClientSendTimeout * 1000;

            Logging.Logging.Info("NetworkHandler", String.Format($"Accepted Connection[{Connection.Handle}]"));

            LastReceive = DateTime.Now;
            Errored = false;
            Error = null;
            NotifiedInterval = false;
        }

        public void HandleData(Data.Receiving.Description descriptor, string raw_data)
        {
            if ((Data.Receiving.EventType)descriptor.Event == Data.Receiving.EventType.INITIALIZE)
            {
                Data.Receiving.InitializeTask initializeTask = JsonConvert.DeserializeObject<Data.Receiving.InitializeTask>(raw_data);
                try
                {
                    EventHandler.Tasks.DataTask dataTask = new EventHandler.Tasks.DataTask(initializeTask.Tag, initializeTask.ModuleName);
                    EventHandler.Tasks.TaskRegistry.Tasks.Add(dataTask);

                    dataTask.Running = true;
                    dataTask.StopTask = false;
                    dataTask.PauseTask = false;

                    Logging.Logging.Info("ConnectionThread", String.Format($"Initialized Task ~Tag: {dataTask.Tag}, ~ModuleName: {dataTask.ModuleName}"));

                    Data.Sending.Outgoing initialized = new Data.Sending.Outgoing();
                    initialized.Event = (int)Data.Sending.EventType.INITIALIZED;
                    initialized.Tag = initializeTask.Tag;

                    Connection.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(initialized)));
                }
                catch (Exception ex)
                {
                    Logging.Logging.Error("ConnectionThread", String.Format($"Failed Initializing Task"), ex);
                    Error = ex;
                }
            }
            else if ((Data.Receiving.EventType)descriptor.Event == Data.Receiving.EventType.FEED)
            {
                Data.Receiving.FeedTask feedTask = JsonConvert.DeserializeObject<Data.Receiving.FeedTask>(raw_data);
                EventHandler.Tasks.DataTask dataTask = EventHandler.Tasks.TaskRegistry.GetTask(feedTask.Tag);

                Thread subTaskThread = new Thread(() => dataTask.Execute(feedTask));
                // feed thread into event loop
                subTaskThread.Start();

                dataTask.CurrentThreads.Add(subTaskThread);

                Logging.Logging.Info("ConnectionThread", String.Format($"Feeding Task ~Tag: {dataTask.Tag}"));

                Data.Sending.Outgoing fed = new Data.Sending.Outgoing();
                fed.Event = (int)Data.Sending.EventType.DATA_FED;
                fed.Tag = dataTask.Tag;

                Connection.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(fed)));

            }
            else if ((Data.Receiving.EventType)descriptor.Event == Data.Receiving.EventType.UPDATE)
            {
                Data.Receiving.UpdateTask updateTask = JsonConvert.DeserializeObject<Data.Receiving.UpdateTask>(raw_data);
                EventHandler.Tasks.DataTask dataTask = EventHandler.Tasks.TaskRegistry.GetTask(updateTask.Tag);

                if (updateTask.PauseTask)
                {
                    dataTask.PauseTask = true;
                    dataTask.Running = false;
                }
                else if (updateTask.ResumeTask)
                {
                    dataTask.PauseTask = false;
                    dataTask.Running = true;
                }
                else if (updateTask.StopTask)
                {
                    dataTask.StopTask = true;
                    dataTask.Running = false;
                }

                Logging.Logging.Info("ConnectionThread", String.Format($"Updated Task ~Tag: {dataTask.Tag} Runtime Paused:{dataTask.PauseTask} Stopped:{dataTask.StopTask} Running:{dataTask.Running}"));

                Data.Sending.Outgoing updated = new Data.Sending.Outgoing();
                updated.Event = (int)Data.Sending.EventType.DATA_FED;
                updated.Tag = dataTask.Tag;

                Connection.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(updated)));
            }
            else if((Data.Receiving.EventType)descriptor.Event == Data.Receiving.EventType.QUERY)
            {
                Data.Receiving.QueryEntry queryEntry = JsonConvert.DeserializeObject<Data.Receiving.QueryEntry>(raw_data);
                if(!EventHandler.Table.EventTableExtensions.QueryTable.ContainsKey(queryEntry.Var))
                {
                    Logging.Logging.Info("ConnectionThread", String.Format($"Query Method Doesn't Exist {queryEntry.Var}"));
                    return;
                }

                Func<string, List<dynamic>> queryFunction = EventHandler.Table.EventTableExtensions.QueryTable[queryEntry.Var];
                List<dynamic> response = queryFunction(queryEntry.Tag);

                Data.Sending.QueryResponse qresponse = new Data.Sending.QueryResponse();
                qresponse.Event = (int)Data.Sending.EventType.QUERY_RESPONSE;
                qresponse.Tag = queryEntry.Tag;
                qresponse.Response = response;

                Connection.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(qresponse)));

            }
            Logging.Logging.Info("ConnectionThread", String.Format($"Shipped Event {(Data.Receiving.EventType)descriptor.Event}"));
        }

        public override void RunThread()
        {
            try
            {
                while (IsAlive && Connection.Connected)
                {
                    string data = "";
                    byte[] bytes = new byte[4096];

                    BytesReceived += Connection.Receive(bytes);
                    data = Encoding.ASCII.GetString(bytes);

                    data = data.Replace('\r', '\0').Replace('\n', '\0');
                    if(data.Length == 2 || data.Length == 0)
                    {
                        continue;
                    }

                    Data.Receiving.Description descriptor = JsonConvert.DeserializeObject<Data.Receiving.Description>(data);
                    Logging.Logging.Info("ConnectionThread", String.Format($"Handling Event: {descriptor.Event} - Descriptor Tag: {descriptor.Tag}"));
                    HandleData(descriptor, data);
                
                    LastReceive = DateTime.Now;
                }
                Logging.Logging.Info("*", "Outside Thread");
            }
            
            catch (SocketException sock_ex)
            {
                Connection.Close(1);

                Logging.Logging.Error("ConnectionThread", String.Format($"Connection[{Connection.Handle}] Socket Error Occured (Likely Timeout/Disconnect)"), sock_ex);
                Errored = true;
            }
            catch (Exception ex)
            {
                Logging.Logging.Error("ConnectionThread", String.Format($"Connection[{Connection.Handle}] Experienced an Error"), ex);
                Errored = true;
            }
            
        }
    }

    public class ListenHandler : BaseConnectionThread
    {
        Socket Handler;
        public IPEndPoint EndPoint;
        public ListenHandler()
            : base()
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(Properties.BindAddress), Properties.BindPort);

            Handler = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                Handler.Bind(EndPoint);
                Handler.Listen();
                Logging.Logging.Info("NetworkInit", String.Format($"Started Listener {EndPoint.Address}:{EndPoint.Port}"));
            } catch (Exception ex)
            {
                Logging.Logging.Error("NetworkInit", "Failed Setting up Handler", ex);
                return;
            }

        }

        public override void RunThread()
        {
            while(Handler.IsBound)
            {
                ConnectionThread connectionThread = new ConnectionThread(Handler.Accept());
                connectionThread.Start();
            }
        }
    }
}
