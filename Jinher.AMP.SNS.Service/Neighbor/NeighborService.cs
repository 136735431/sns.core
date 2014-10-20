using Jinher.AMP.SNS.IService.Service;
using Jinher.AMP.SNS.IService.Store;
using Jinher.AMP.SNS.Service.BE.Neighbor;
using Jinher.AMP.SNS.Service.Deploy;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Jinher.AMP.SNS.Service.Neighbor
{
    /// <summary>
    /// 附近的人服务类
    /// </summary>
    public partial class NeighborService : BaseService, INeighborServiceable
    {
        //离线滑动时间 分钟单位
        private static double outlineMinutes = 20;
        private static NeighborService _NeighborService = null;
        //private static List<OnlineUserModel> onlineList = new List<OnlineUserModel>();
        IOnlineStoreable neighborStore = null;
        INeighborStoreable neighborStoreable = null;
        /// <summary>
        /// 离线队列
        /// </summary>
        //public static List<OnlineUserModel> OutlineList { get; set; }
        #region 初始化
        private NeighborService()
        {
            //OutlineList = new List<OnlineUserModel>();
        }
        /// <summary>
        /// 在线用户（附近的人）
        /// </summary>
        //public List<OnlineUserModel> OnlineUserList
        //{
        //    private set
        //    {
        //        onlineList = value;
        //    }
        //    get
        //    {
        //        return onlineList;
        //    }
        //}
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <returns></returns>
        public static BaseService CreateInstance()
        {
            if (_NeighborService == null)
            {
                lock ("Jinher.AMP.SNS.Service.Neighbor.NeighborService")
                {
                    if (_NeighborService == null)
                    {
                        _NeighborService = new NeighborService();
                    }
                }
            }
            return _NeighborService;
        }

        /// <summary>
        /// 创建接口对象
        /// </summary>
        /// <returns></returns>
        public static INeighborServiceable CreateInterfaceInstance()
        {
            if (_NeighborService == null)
            {
                lock ("Jinher.AMP.SNS.Service.Neighbor.NeighborService")
                {
                    if (_NeighborService == null)
                    {
                        _NeighborService = new NeighborService();
                    }
                }
            }
            return _NeighborService;
        }
        #endregion

        #region 服务启动
        /// <summary>
        /// 启动服务
        /// </summary>
        public override void Run()
        {
            base.Run();
        }
        /// <summary>
        /// 服务初始化
        /// </summary>
        public override void Init()
        {
            //取在线用户数据(简单存储模式)
            //neighborStore = OnlineStore.CreateInstance();
            //lock ("Jinher.AMP.SNS.Service.Neighbor.NeighborService")
            //{
            //    OnlineUserList = neighborStore.Take() as List<OnlineUserModel>;
            //}

            //geohash model
            neighborStoreable = NeighborStore.CreateInstance();
            lock ("Jinher.AMP.SNS.Service.Neighbor.NeighborService")
            {
                neighborStoreable.Init();
            }
            Console.WriteLine("Init sucess! now have {0} perple online", this.Count());
        }

        public override void Dispose()
        {
            //onlineList.Clear();
        }

        /// <summary>
        /// 离线存储服务
        /// </summary>
        public void OutlineService()
        {
            //Task.Factory.StartNew(() =>
            //{
            //    while (true)
            //    {
            //        if (OutlineList.Count > 0)
            //        {
            //            var query = OutlineList.FindAll(x => x.OutlineTime >= DateTime.Now);
            //            lock ("Jinher.AMP.SNS.Service.Neighbor.NeighborService.OutlineService")
            //            {
            //                OutlineList.RemoveAll(x => x.OutlineTime >= DateTime.Now);
            //            }
            //            if (query != null && query.Count > 0)
            //            {

            //                foreach (var item in query)
            //                {
            //                    lock ("Jinher.AMP.SNS.Service.Neighbor.NeighborService")
            //                    {
            //                        OnlineUserList.RemoveAll(x => x.AppId == item.AppId && x.UserId == item.UserId);
            //                    }
            //                }
            //            }
            //        }
            //        //休眠一分钟
            //        Thread.Sleep(60 * 1000);
            //    }
            //});
        }
        #endregion

    }
}
