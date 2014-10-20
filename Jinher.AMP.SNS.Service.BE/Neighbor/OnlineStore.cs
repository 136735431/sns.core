using Jinher.AMP.SNS.IService.Store;
using Jinher.AMP.SNS.Serivce.Utility;
using Jinher.AMP.SNS.Service.Cache;
using Jinher.AMP.SNS.Service.Deploy;
using System;
using System.Collections.Generic;

namespace Jinher.AMP.SNS.Service.BE.Neighbor
{
    public class OnlineStore : BaseStore, IOnlineStoreable
    {
        private string redisServer = "127.0.0.1";
        private int redisPort = 6379;
        public RedisClientHelper redis = null;//new Cache.RedisClientHelper(redisServer, redisPort);
        #region 单例
        private static OnlineStore _OnlineStore = null;
        private OnlineStore()
        {
            redis = new Cache.RedisClientHelper(redisServer, redisPort);
        }
        public static IOnlineStoreable CreateInstance()
        {
            if (_OnlineStore == null)
            {
                lock ("Jinher.AMP.SNS.DataStore.Neighbor.OnlineStore")
                {
                    if (_OnlineStore == null)
                    {
                        _OnlineStore = new OnlineStore();
                    }
                }
            }
            return _OnlineStore;
        }
        #endregion
        /// <summary>
        /// 取所有在线用户
        /// </summary>
        /// <returns></returns>
        public object Take()
        {
            try
            {
                var r = TakeTwo();
                if (r != null && r.Count > 0)
                {
                    Console.WriteLine("SNS redis取数据");
                    return r;
                }
                else
                {
                    Console.WriteLine("消息中心redis取数据");
                    return TakeOne();
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}初始化数据的错误:{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ex.Message);
                LogHelper.WriteLog("初始化数据的错误", ex);
                return null;
            }
        }
        /// <summary>
        /// 消息中心服务器
        /// </summary>
        /// <returns></returns>
        private List<OnlineUserModel> TakeTwo()
        {
            return redis.GetKeysData("hash:*");
        }
        //private bool IsHaveData()
        //{
        //    return redis.SearchKeys("hash:*");
        //}
        //取在线用户(消息平台服务器)
        private object TakeOne()
        {
            //取在线用户(消息平台服务器)
            RedisClientHelper r = new RedisClientHelper();
            var result = r.Take("Neighbor");

            result = mongo.Take(result, "Neighbor");
            return result;
        }
        MongodbClientHelper mongo = new MongodbClientHelper();
        /// <summary>
        /// 取单个在线用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>返回值是list集合</returns>
        public object Take(string userId, string appid)
        {
            if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(appid))
            {
                //同步经纬度（用户位置）

                var result = mongo.Take(new List<OnlineUserModel>() { new OnlineUserModel() { UserId = userId, AppId = appid } }, "Neighbor");
                return result;
            }
            else
            {
                return null;
            }
        }
        public object Save(string key, object value)
        {
            throw new NotImplementedException();
        }
    }
}
