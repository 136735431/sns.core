using Jinher.AMP.SNS.Service.Deploy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.SNS.IService.Store
{
    public interface INeighborStoreable
    {
        /// <summary>
        /// 获取附近的人
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        List<OnlineUserModel> Take(OnlineUserModel model, int num);
        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="model"></param>
        void Add(OnlineUserModel model);
        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="model"></param>
        void Update(OnlineUserModel model);
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="model"></param>
        void Remove(OnlineUserModel model);
        /// <summary>
        /// 判断用户是否已经存在
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Equals(OnlineUserModel model);
        /// <summary>
        /// 用户上线
        /// </summary>
        /// <param name="model"></param>
        void UserOnline(OnlineUserModel model);

        /// <summary>
        /// 用户下 线
        /// </summary>
        /// <param name="model"></param>
        void UserOutline(OnlineUserModel model);
        /// <summary>
        /// 服务初始化
        /// </summary>
        void Init();

        int Count();
    }
}
