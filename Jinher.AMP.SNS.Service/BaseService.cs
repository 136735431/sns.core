using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.SNS.Service
{
    public abstract class BaseService
    {
        /// <summary>
        /// 启动
        /// </summary>
        public virtual void Run()
        {
            Init();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public abstract void Init();
        
        /// <summary>
        /// 释放
        /// </summary>
        public abstract void Dispose();
    }
}
