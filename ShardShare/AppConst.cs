using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShardShare
{
    public class AppConst
    {
        public class ShardDemo
        {
            public const string ExchangeName = "shard.images";
            //public const string RoutingKey = "r1234"; // no need this one
        }
        public class ConsistantHashingDemo
        {
            public const string ExchangeName = "my.hashing.ex";
        }
    }
}
