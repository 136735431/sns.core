
using Jinher.AMP.SNS.IService.Service;
using Jinher.AMP.SNS.Serivce.Utility;
using Jinher.AMP.SNS.Service.BP.IService;
using Jinher.AMP.SNS.Service.Deploy;
using Jinher.AMP.SNS.Service.Neighbor;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jinher.AMP.SNS.Service.BP.Service
{
    public class Neighbor : INeighbor
    {
        INeighborServiceable _NeighborService = null;
        Stopwatch sw = new Stopwatch();
        /// <summary>
        /// 获取附近的人
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<OnlineUserModel> NeighborPeople(OnlineUserModel user, int maxNum)
        {
            try
            {
                if (_NeighborService == null)
                {
                    _NeighborService = NeighborService.CreateInterfaceInstance();
                }
                sw.Start();
                var r = _NeighborService.NeighborPeople(user, maxNum);
                sw.Stop();
                Console.WriteLine("{0}:get neighbor people number{1},user time {2} millisecond", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), r.Count, sw.ElapsedMilliseconds);
                
                return r;
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取附近的人出错:" + ex.Message);
                LogHelper.WriteLog("获取附近的人出错", ex);
            }
            return null;
        }
        /// <summary>
        /// 上下线用户通知
        /// </summary>
        /// <param name="data"></param>
        public void OnLineNotification(object data)
        {
            try
            {
                string value = data.ToString();

                //data format : "userId:00000000,appid:000000000,type:on/off"
                if (data != null)
                {
                    if (_NeighborService == null)
                    {
                        _NeighborService = NeighborService.CreateInterfaceInstance();
                    }
                    _NeighborService.Update(data);
                }
                Console.WriteLine("{0}:update sucess! now have {1} perple online", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss "), this.Count());
            }
            catch (Exception ex)
            {
                Console.WriteLine("更新上下线用户出错:" + ex.Message);
                LogHelper.WriteLog("更新上下线用户出错", ex);
            }
        }
        /// <summary>
        /// 当前在线用户数量
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            try
            {
                if (_NeighborService == null)
                {
                    _NeighborService = NeighborService.CreateInterfaceInstance();
                }
                return _NeighborService.Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取在线用户数量出错:" + ex.Message);
                LogHelper.WriteLog("获取在线用户数量出错", ex);
            }
            return 0;
        }
    }
}
