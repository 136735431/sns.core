using Jinher.AMP.SNS.Serivce.Utility;
using Jinher.AMP.SNS.Service;
using Jinher.AMP.SNS.Service.Neighbor;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jinher.AMP.SNS.Service.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            //启动服务
            LogHelper.SetConfig();
            Task.Factory.StartNew(() => StartNeighbor());
            Task.Factory.StartNew(() => StartWCF());
            Console.Read();
            while (true)
            {
                Thread.Sleep(10000 * 10);
            }
        }
        //private static string redisServer = "183.56.131.243";
        //private static int redisPort = 6379;
        //static RedisClient redis = null;
        private static void StartNeighbor()
        {

            //redis = new RedisClient(redisServer, redisPort);
            //var getSpecificKeys = redis.SearchKeys("UserLoginInfo:*");
            Console.WriteLine("{0}:Neighbor server starting.......", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss "));
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //附近的人相关服务启动
            ServiceEntry.Run(NeighborService.CreateInstance());
            sw.Stop();
            Console.WriteLine("{0}:Init：{1} millisecond", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss "), sw.ElapsedMilliseconds);
        }

        static void StartWCF()
        {
            //using (ServiceHost host = new ServiceHost(typeof(UserOnline)))
            //{
            //    host.AddServiceEndpoint(typeof(IUserOnline), new WSHttpBinding(), "http://127.0.0.1:8089/useronline");
            //    if (host.Description.Behaviors.Find<ServiceMetadataBehavior>() == null)
            //    {
            //        ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
            //        behavior.HttpGetEnabled = true;
            //        behavior.HttpGetUrl = new Uri("http://127.0.0.1:8089/useronline/metadata");
            //        host.Description.Behaviors.Add(behavior);
            //    }
            //    host.Opened += delegate
            //    {
            //        Console.WriteLine("IUserOnline已经启动，按任意键终止服务！");
            //    };

            //    host.Open();
            //}
            try
            {
                using (ServiceHost host = new ServiceHost(typeof(Jinher.AMP.SNS.Service.BP.Service.Neighbor)))
                {
                    host.Opened += delegate
                    {
                        Console.WriteLine("{0}:UserOnline Service Is Running(WCF)", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss "));
                    };

                    host.Open();
                    
                    while (true)
                    {
                        Console.Read();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}:WCF error :{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ex.Message);
                LogHelper.WriteLog("wcf服务出错", ex);
                Thread.Sleep(500);
                StartWCF();
            }
        }
    }
}
