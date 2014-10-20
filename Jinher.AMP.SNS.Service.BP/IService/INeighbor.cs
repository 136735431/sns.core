using Jinher.AMP.SNS.Service.Deploy;
using System.Collections.Generic;
using System.ServiceModel;

namespace Jinher.AMP.SNS.Service.BP.IService
{
    [ServiceContract]
    public interface INeighbor
    {
        /// <summary>
        /// get 附近的人
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [OperationContract]
        List<OnlineUserModel> NeighborPeople(OnlineUserModel user, int maxNum);

        /// <summary>
        /// 在线用户上线通知
        /// </summary>
        /// <param name="data"></param>
        [OperationContract]
        void OnLineNotification(object data);

        [OperationContract]
        int Count();
    }
}
