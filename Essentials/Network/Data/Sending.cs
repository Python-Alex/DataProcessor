using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.Essentials.Network.Data.Sending
{
    public enum EventType
    {
        INITIALIZED = 0,
        DATA_FED = 1,
        UPDATED = 2,
        QUERY_RESPONSE = 3
    }

    public class Outgoing
    {
        public int Event;
        public string Tag;
    }

    public class QueryResponse
    {
        public int Event;
        public string Tag;
        public List<dynamic> Response;
    }
}
