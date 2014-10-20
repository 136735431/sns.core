/*
 服务入口
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.SNS.Service
{
    /// <summary>
    /// 服务入口
    /// </summary>
    public class ServiceEntry
    {
        /// <summary>
        /// 启动服务
        /// </summary>
        public static void Run(BaseService bs)
        {
            if (bs != null)
            {
                bs.Run();
            }
        }
    }
}
