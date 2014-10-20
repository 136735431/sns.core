
using Jinher.AMP.SNS.Serivce.Utility;
using Jinher.AMP.SNS.Service.Deploy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jinher.AMP.SNS.Service.BP.Service
{
    public class CalculateNeighbor
    {
        public static List<OnlineUserModel> NeighborPeople(List<OnlineUserModel> dataList, OnlineUserModel user, int maxPeople = 100, int maxRange = 10000)
        {
            List<OnlineUserModel> list = new List<OnlineUserModel>();
            //依次获取范围内数据 梯度为（500* 2的平方）

            double[] latRange, lngRange;
            for (int i = 0; i < maxRange; i++)
            {
                double temp = 500 * Math.Pow(2, i) > maxRange ? maxRange : 500 * Math.Pow(2, i);

                if (temp <= maxRange)
                {
                    NeighborHelper.DistanceOfPoint(user.Latitude, user.Longitude, (int)temp, out latRange, out lngRange);

                    //取范围值
                    var result = NeighborPeople(dataList, latRange, lngRange);
                    if (result != null && result.Count > 0)
                    {
                        list = result;
                    }
                    //清空范围
                    latRange = new double[2];
                    lngRange = new double[2];

                    if (list.Count >= maxPeople)
                    {
                        return list;
                    }
                }
            }
            return list;
        }

        public static List<OnlineUserModel> NeighborPeople(List<OnlineUserModel> dataList, double[] lat, double[] lng)
        {

            var query = from a in dataList
                        where a.Longitude >= lng[0] && a.Longitude <= lng[1]
                        && a.Latitude >= lat[0] && a.Latitude <= lat[1]
                        select a;

            return query.ToList();
        }
    }
}
