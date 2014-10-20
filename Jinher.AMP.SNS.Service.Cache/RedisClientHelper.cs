using System.Collections.Generic;
using ServiceStack.Redis;
using Jinher.AMP.SNS.Service.Deploy;

namespace Jinher.AMP.SNS.Service.Cache
{
    public class RedisClientHelper
    {
        private string redisServer = "183.56.131.243";
        private int redisPort = 6379;
        RedisClient redis = null;
        public RedisClientHelper()
        {
            redis = new RedisClient(redisServer, redisPort);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        public RedisClientHelper(string server, int port)
        {
            redis = new RedisClient(server, port);
        }

        public void Init()
        {
        }

        public object Take(string type)
        {
            switch (type)
            {
                case "Neighbor"://附近的人
                    return NeighborTake();
                default:
                    return null;
            }
        }

        public List<OnlineUserModel> NeighborTake()
        {
            List<OnlineUserModel> list = new List<OnlineUserModel>();
            var getSpecificKeys = redis.SearchKeys("UserLoginInfo:*");

            foreach (var getKey in getSpecificKeys)
            {
                string userId = getKey.ToString().Split(':')[1];
                string appId = getKey.ToString().Split(':')[2];

                list.Add(new OnlineUserModel() { AppId = appId, UserId = userId });
            }
            return list;
        }

        public void Save(string key, OnlineUserModel value)
        {
            redis.Add<OnlineUserModel>(key, value);
        }

        public void Remove(string key)
        {
            redis.Remove(key);
        }

        public bool SearchKeys(string p)
        {
            var r = redis.SearchKeys(p);
            if (r == null)
            {
                return false;
            }
            else if (r.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 模糊条件查询
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public List<OnlineUserModel> GetKeysData(string p)
        {
            List<OnlineUserModel> list = new List<OnlineUserModel>();
            var getSpecificKeys = redis.SearchKeys(p);

            foreach (var item in getSpecificKeys)
            {
                list.Add(redis.Get<OnlineUserModel>(item));
            }
            return list;
        }
        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<OnlineUserModel> GetKeysDataForKey(string key)
        {
            return redis.Get<List<OnlineUserModel>>(key);
        }

        public void Save(string key, List<OnlineUserModel> value)
        {
            redis.Add<List<OnlineUserModel>>(key, value);
        }
    }

}
