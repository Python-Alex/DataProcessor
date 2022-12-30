using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.Logging
{
    internal class ByteTest
    {

        public void test()
        {
            byte[] bytes = new byte[4096];
            bytes[0] = 4;
            bytes[1] = 4;
            bytes[2] = 4;
            bytes[3] = 4;
            BitConverter.ToInt32(bytes, 0);            
        }
    }
}
