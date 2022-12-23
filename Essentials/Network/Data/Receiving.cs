using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Generic;


namespace DataProcessor.Essentials.Network.Data.Receiving
{
    public enum EventType
    {
        INITIALIZE = 0,
        FEED = 1,
        UPDATE = 2,
        QUERY = 3
    }


    public class Description
    {
        public int Event;
        public string Tag;
    }

    // turns into an event
    public class InitializeTask
    {
        public int Event;
        public string Tag;
        public string ModuleName;

    }

    public class FeedTask
    {
        public int Event;
        public string Tag;
        public Dictionary<string, dynamic> Arguments;
        //public List<string> Instructions; // API Callback List
        //public List<dynamic> Stack;       // Value Stack for Callbacks
                                          // Instructions[0] && Stack[0]
    }

    // actions depend on bools set
    // no need to set bool consistantly after update packet
    public class UpdateTask
    {
        public int Event;
        public string Tag;
        public bool VerboseOutput;
        public bool StopTask;
        public bool PauseTask;
        public bool ResumeTask;
    }

    public class QueryEntry
    {
        public int Event;
        public string Tag;
        public string Var;
    }
}
