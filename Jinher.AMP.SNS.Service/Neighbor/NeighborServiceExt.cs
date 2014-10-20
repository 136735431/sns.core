using Jinher.AMP.SNS.IService.Service;
using Jinher.AMP.SNS.Serivce.Utility;
using Jinher.AMP.SNS.Service.BE.Neighbor;
using Jinher.AMP.SNS.Service.Deploy;
using System;
using System.Collections.Generic;

namespace Jinher.AMP.SNS.Service.Neighbor
{
    public partial class NeighborService : INeighborServiceable
    {
        #region 实现INeighborServiceable方法
        /// <summary>
        /// get OnlineUserList data number
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            int r = 0;
            lock ("Jinher.AMP.SNS.Service.Neighbor.NeighborService")
            {
                r = neighborStoreable.Count();
            }
            return r;
        }
        /// <summary>
        /// 获取用户附近的人
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<OnlineUserModel> NeighborPeople(OnlineUserModel user,int maxNum)
        {
            List<OnlineUserModel> list = null;
            //lock ("Jinher.AMP.SNS.Service.Neighbor.NeighborService")
            //{
            //    list = CalculateNeighbor.NeighborPeople(OnlineUserList, user);
            //}
            lock ("Jinher.AMP.SNS.Service.Neighbor.NeighborService")
            {
                list = neighborStoreable.Take(user, maxNum);
            }
            return list;
        }
        /// <summary>
        /// Update OnlineUserList
        /// </summary>
        /// <param name="data"></param>
        public void Update(object data)
        {
            //data format : "userId:00000000,appid:000000000,type:on/off"
            string uid = null;
            string appid = null;
            string type = null;
            string value = data.ToString();
            string[] strArray = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length == 3)
            {
                foreach (var item in strArray)
                {
                    string[] arr = item.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length == 2)
                    {
                        if (string.Compare(arr[0], "userId", true) == 0)
                        {
                            uid = arr[1];
                        }
                        else if (string.Compare(arr[0], "appid", true) == 0)
                        {
                            appid = arr[1];
                        }
                        else if (string.Compare(arr[0], "type", true) == 0)
                        {
                            type = arr[1];
                        }
                    }
                }

                if (string.Compare(type, "on", true) == 0)
                {
                    Online(appid, uid);
                }
                else if (string.Compare(type, "off", true) == 0)
                {
                    Outline(appid, uid);
                }
            }
        }
        /// <summary>
        /// 上线
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="userid"></param>
        public void Online(string appid, string userid)
        {
            //var r = neighborStore.Take(userid, appid);
            //if (r != null && r is List<OnlineUserModel>)
            //{
            //    lock ("Jinher.AMP.SNS.Service.Neighbor.NeighborService")
            //    {
            //        OnlineUserList.AddRange(r as List<OnlineUserModel>);
            //    }
            //}
            //console dispaly
            var model = OnlineStore.CreateInstance().Take(userid, appid);
            if (model != null)
            {
                var temp = model as List<OnlineUserModel>;
                foreach (var item in temp)
                {
                    lock ("Jinher.AMP.SNS.Service.Neighbor.NeighborService")
                    {
                        neighborStoreable.UserOnline(item);
                    }
                }
            }
            else
            {
                lock ("Jinher.AMP.SNS.Service.Neighbor.NeighborService")
                {
                    neighborStoreable.UserOnline(new OnlineUserModel() { AppId = appid, UserId = userid });
                }
            }
        }
        /// <summary>
        /// 离线
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="userid"></param>
        public void Outline(string appid, string userid)
        {
            //lock ("Jinher.AMP.SNS.Service.Neighbor.NeighborService.OutlineService")
            //{
            //    OutlineList.Add(new OnlineUserModel() { AppId = appid, UserId = userid, OutlineTime = DateTime.Now.AddMinutes(outlineMinutes) });
            //}
            neighborStoreable.UserOutline(new OnlineUserModel() { AppId = appid, UserId = userid });

        }

        #endregion

    }
}
