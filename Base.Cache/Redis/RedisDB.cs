using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Base.Cache.Redis
{
    class RedisDB
    {
        static void Main(string[] args)
        {
            #region 简单dome
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
            IDatabase db = redis.GetDatabase();
            string value = "name";
            db.StringSet("mykey", value);
            Console.WriteLine(db.StringGet("mykey"));
            #endregion

            #region 使用ConfigurationOptions连接redis服务器
            ConfigurationOptions configurationOptions = new ConfigurationOptions()
            {
                EndPoints = { { "127.0.0.1", 6379 } },
                CommandMap = CommandMap.Create(new HashSet<string>()
          {
                "INFO",
                "CONFIG",
                "CLUSTER",
                "PING",
                "ECHO",
                "CLIENT"
            }, available: false),
                KeepAlive = 180,
                DefaultVersion = new Version(2, 8, 24),
                Password = "CeshiPassword"
            };
            IDatabase db1 = redis.GetDatabase();
            string value2 = "name2";
            db.StringSet("mykey2", value2);
            Console.WriteLine(db.StringGet("mykey2"));
            #endregion

        }
    }
}
