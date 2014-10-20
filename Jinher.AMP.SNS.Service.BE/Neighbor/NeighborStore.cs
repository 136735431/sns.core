using Jinher.AMP.SNS.IService.Store;
using Jinher.AMP.SNS.Serivce.Utility;
using Jinher.AMP.SNS.Service.Deploy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jinher.AMP.SNS.Service.BE.Neighbor
{
    /// <summary>
    /// 附近的人 存储结构
    /// </summary>
    public partial class NeighborStore : BaseStore, INeighborStoreable
    {
        private static int outlineMinutes = 20;//20Minutes 后踢出下线
        public static Hashtable hash = new Hashtable();
        Dictionary<string, OnlineUserModel> dict = new Dictionary<string, OnlineUserModel>();
        //计算gethash精度
        static int precision = 10;
        //存到redis的对象初始化
        Jinher.AMP.SNS.Service.Cache.RedisClientHelper _RedisClientHelper = null;
        private string redisServer = "127.0.0.1";
        private int redisPort = 6379;
        //private static double[] lngRange = { 73.5500, 135.0833 };
        //private static double[] latRange = { 3.8500, 53.5500 };
        ////经度范围：73°33′E(73.5500)至135°05′(135.0833)E
        ////纬度范围：3°51′(3.8500)N至53°33′(53.5500)N
        //private static int maxNet_X = 0;//纬度为x
        //private static int maxNet_Y = 0;//经度为Y

        #region 存储结构初始化(采用geohash算法)
        /// <summary>
        /// 服务初始化
        /// </summary>
        public void Init()
        {
            var r = OnlineStore.CreateInstance().Take();
            _RedisClientHelper = (OnlineStore.CreateInstance() as OnlineStore).redis;//new Cache.RedisClientHelper(redisServer, redisPort);
            //NeighborHelper
            //maxNet_X = Math.Abs((int)((latRange[0] - latRange[1]) / NeighborHelper.GetRangeLat(minMeter)));
            //maxNet_Y = Math.Abs((int)((lngRange[0] - lngRange[1]) / NeighborHelper.GetRangeLat(minMeter)));
            //int max = maxNet_X * maxNet_Y;  Geohash

            if (r != null && r is List<OnlineUserModel>)
            {
                List<OnlineUserModel> list = r as List<OnlineUserModel>;
                for (int i = 0; i < list.Count; i++)
                {
                    string key = Geohash.Encode(list[i].Latitude, list[i].Longitude, precision);
                    var key1 = string.Format("hash:{2};appid:{0};userid:{1}", list[i].AppId, list[i].UserId, key ?? "null");
                    //list[i].GeoHashString = key;
                    //hash.Add(key, list[i]);
                    lock ("Jinher.AMP.SNS.Service.BE.Neighbor.NeighborStore")
                    {
                        dict.Add(key1, list[i]);
                    }
                    _RedisClientHelper.Save(key1, list[i]);
                }
            }

            //启动离线服务线程
            UserOutlineList.Clear();
            InitUserOutlineList();
            this.UserOutlineServerThread();
        }


        #endregion

        #region 实现INeighborStoreable  存储操作的方法
        /// <summary>
        /// 获取附近的人
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<OnlineUserModel> Take(OnlineUserModel model, int num)
        {
            List<OnlineUserModel> result = new List<OnlineUserModel>();
            //模型为空的时候，直接返回没有坐标的用户
            if (model == null)
                result = CalculateNeighbor.NeighborPeopleForNullModel(dict, num);
            else
            {
                //请求数量大于总数
                if (this.Count() <= num)
                {
                    result = dict.Values.ToList();
                }
                else
                    result = CalculateNeighbor.NeighborPeople(model, dict, num, precision);
            }

            if (result != null)
            {
                var r = result.Select(x => new OnlineUserModel() { UserId = x.UserId, AppId = x.AppId, Longitude = x.Longitude, Latitude = x.Latitude }).ToList();
                return r;
            }
            else
            {
                return result;
            }
        }


        /// <summary>
        /// 新增用户
        /// lock : Jinher.AMP.SNS.Service.BE.Neighbor.NeighborStore
        /// </summary>
        /// <param name="model"></param>
        public void Add(OnlineUserModel model)
        {
            if (!Equals(model) && model != null)
            {
                string key = Geohash.Encode(model.Latitude, model.Longitude);
                var key1 = string.Format("hash:{2};appid:{0};userid:{1}", model.AppId, model.UserId, key ?? "null");
                lock ("Jinher.AMP.SNS.Service.BE.Neighbor.NeighborStore")
                {
                    dict.Add(key1, model);
                }
                _RedisClientHelper.Save(key1, model);
            }
        }
        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="model"></param>
        public void Update(OnlineUserModel model)
        {
            string key = Geohash.Encode(model.Latitude, model.Longitude);
            var key1 = string.Format("hash:{2};appid:{0};userid:{1}", model.AppId, model.UserId, key ?? "null");

            lock ("Jinher.AMP.SNS.Service.BE.Neighbor.NeighborStore")
            {
                var query = from a in dict
                            where a.Value.AppId == model.AppId && a.Value.UserId == model.UserId
                            select a.Key;
                if (query != null && query.Count() > 0)
                {
                    var tem = query.ToList();
                    for (int i = 0; i < tem.Count; i++)
                    {
                        dict.Remove(tem[i]);
                        _RedisClientHelper.Remove(tem[i]);
                    }
                }
                dict.Add(key1, model);
                _RedisClientHelper.Save(key1, model);
            }
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="model"></param>
        public void Remove(OnlineUserModel model)
        {
            string key = string.Format("appid:{0};userid:{1}", model.AppId, model.UserId);

            lock ("Jinher.AMP.SNS.Service.BE.Neighbor.NeighborStore")
            {
                var query = from a in dict
                            where a.Key.Contains(key)
                            select a.Key;
                //var query = from a in dict
                //            where a.Value.AppId == model.AppId && a.Value.UserId == model.UserId
                //            select a.Key;

                foreach (var item in query)
                {
                    dict.Remove(item);
                    _RedisClientHelper.Remove(item);
                }
            }
        }
        /// <summary>
        /// 判断用户是否已经存在
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Equals(OnlineUserModel model)
        {
            if (model == null)
            {
                return false;
            }

            lock ("Jinher.AMP.SNS.Service.BE.Neighbor.NeighborStore")
            {
                return dict.Values.Any(x => x.UserId == model.UserId && x.AppId == model.AppId);
            }
        }
        /// <summary>
        /// 用户上线
        /// </summary>
        /// <param name="model"></param>
        public void UserOnline(OnlineUserModel model)
        {
            if (Equals(model))
            {
                Update(model);
            }
            else
            {
                Add(model);
            }
            this.ClearUserOutline(model);
        }
        public int Count()
        {
            lock ("Jinher.AMP.SNS.Service.BE.Neighbor.NeighborStore")
            {
                return dict.Keys.Count;
            }
        }
        /// <summary>
        /// 用户下 线
        /// </summary>
        /// <param name="model"></param>
        public void UserOutline(OnlineUserModel model)
        {
            model.OutlineTime = DateTime.Now.AddMinutes(outlineMinutes);
            lock ("Jinher.AMP.SNS.Service.BE.Neighbor.NeighborStore.UserOutline")
            {
                UserOutlineList.Add(model);
                OutlineListSaveToRedis();
            }
        }

        #endregion

        #region 单例
        private static NeighborStore _NeighborStore = null;
        private NeighborStore()
        {

        }
        public static INeighborStoreable CreateInstance()
        {
            if (_NeighborStore == null)
            {
                lock ("Jinher.AMP.SNS.Service.BE.Neighbor.NeighborStore")
                {
                    if (_NeighborStore == null)
                    {
                        _NeighborStore = new NeighborStore();
                    }
                }
            }
            return _NeighborStore;
        }
        #endregion

        #region 离线服务线程
        /// <summary>
        /// 离线用户队列
        /// </summary>
        private List<OnlineUserModel> UserOutlineList = new List<OnlineUserModel>();

        private void UserOutlineServerThread()
        {
            Task.Factory.StartNew(() =>
            {
                List<OnlineUserModel> list = new List<OnlineUserModel>();
                while (true)
                {
                    try
                    {
                        lock ("Jinher.AMP.SNS.Service.BE.Neighbor.NeighborStore.UserOutline")
                        {
                            var query = UserOutlineList.ToList().Where(x => x.OutlineTime >= DateTime.Now).ToList();
                            if (query != null)
                            {
                                foreach (var item in list)
                                {
                                    //this.Remove(item);
                                    UserOutlineList.Remove(item);
                                }
                            }
                        }
                        foreach (var item in list)
                        {
                            this.Remove(item);
                        }
                        list.Clear();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog("离线服务线程error", ex);
                        Console.WriteLine("{0}:离线服务线程error: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ex.Message);
                    }
                    Thread.Sleep(60 * 1000);//1分钟执行一次
                }
            });
        }
        /// <summary>
        /// 踢出离线用户
        /// </summary>
        /// <param name="model"></param>
        private void ClearUserOutline(OnlineUserModel model)
        {
            lock ("Jinher.AMP.SNS.Service.BE.Neighbor.NeighborStore.UserOutline")
            {
                UserOutlineList.RemoveAll(x => x.UserId == model.UserId && x.AppId == x.AppId);
                OutlineListSaveToRedis();
            }
        }
        /// <summary>
        /// 把离线列表存储到redis中
        /// </summary>
        private void OutlineListSaveToRedis()
        {
            _RedisClientHelper.Save("NeighborStore-UserOutline", UserOutlineList);
        }

        private void InitUserOutlineList()
        {
            var r = _RedisClientHelper.GetKeysDataForKey("NeighborStore-UserOutline");
            if (r != null && r.Count != 0)
            {
                lock ("Jinher.AMP.SNS.Service.BE.Neighbor.NeighborStore.UserOutline")
                {
                    UserOutlineList.InsertRange(0, r);
                }
            }
        }
        #endregion
    }
}
