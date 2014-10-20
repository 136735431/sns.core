using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.SNS.Service.Deploy
{
    public class OnlineUserModel
    {
        public string AppId { get; set; }
        public string UserId { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// 位置信息
        /// </summary>
        public string LocationDesc { get; set; }

        /// <summary>
        /// 上线时间
        /// </summary>
        public DateTime? OnlineTime { get; set; }
        /// <summary>
        /// 下线时间
        /// </summary>
        public DateTime? OutlineTime { get; set; }

        public string GeoHashString { get; set; }
    }
}
