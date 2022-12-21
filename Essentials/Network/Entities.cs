using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Timers;

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

        public void HandleData(Data.Description descriptor, string raw_data)
        {
            if ((Data.EventType)descriptor.Event == Data.EventType.INITIALIZE)
            {
                Data.InitializeTask initializeTask = JsonConvert.DeserializeObject<Data.InitializeTask>(raw_data);
                try
                {
                    EventHandler.Tasks.DataTask dataTask = new EventHandler.Tasks.DataTask(initializeTask.Tag, initializeTask.ModuleName);
                    EventHandler.Tasks.TaskRegistry.Tasks.Add(dataTask);

                    Logging.Logging.Info("ConnectionThread", String.Format($"Initialized Task ~Tag: {dataTask.Tag}, ~ModuleName: {dataTask.ModuleName}"));
                }
                catch (Exception ex)
                {
                    Logging.Logging.Error("ConnectionThread", String.Format($"Failed Initializing Task"), ex);
                    Error = ex;
                }
            }
            else if ((Data.EventType)descriptor.Event == Data.EventType.FEED)
            {
                Data.FeedTask feedTask = JsonConvert.DeserializeObject<Data.FeedTask>(raw_data);
                EventHandler.Tasks.DataTask dataTask = EventHandler.Tasks.TaskRegistry.GetTask(feedTask.Tag);

                Logging.Logging.Info("ConnectionThread", String.Format($"Feeding Task ~Tag: {dataTask.Tag}"));
                
            }
            else if ((Data.EventType)descriptor.Event == Data.EventType.UPDATE)
            {
                Data.UpdateTask updateTask = JsonConvert.DeserializeObject<Data.UpdateTask>(raw_data);
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
            }

            Logging.Logging.Info("ConnectionThread", String.Format($"Shipped Event {(Data.EventType)descriptor.Event}"));
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

                    Data.Description descriptor = JsonConvert.DeserializeObject<Data.Description>(data);
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
