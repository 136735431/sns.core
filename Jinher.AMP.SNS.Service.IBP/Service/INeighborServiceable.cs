
using Jinher.AMP.SNS.Service.Deploy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.SNS.IService.Service
{
    public interface INeighborServiceable
    {
        /// <summary>
        /// 在线用户数量
        /// </summary>
        /// <returns></returns>
        int Count();
        /// <summary>
        /// 附近的人
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        List<OnlineUserModel> NeighborPeople(OnlineUserModel user, int maxNum);
        /// <summary>
        /// update 
        /// </summary>
        /// <param name="data"></param>
        void Update(object data);
        /// <summary>
        /// 上线
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="userid"></param>
        void Online(string appid, string userid);
        /// <summary>
        /// 下线
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="userid"></param>
        void Outline(string appid, string userid);
    }
}
