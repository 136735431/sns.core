
using Jinher.AMP.SNS.Serivce.Utility;
using Jinher.AMP.SNS.Service.Deploy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jinher.AMP.SNS.Serivce.Utility
{
    public class CalculateNeighbor
    {
        /// <summary>
        /// 直接存储(暂时不适用)
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="user"></param>
        /// <param name="maxPeople"></param>
        /// <param name="maxRange"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 直接存储
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        private static List<OnlineUserModel> NeighborPeople(List<OnlineUserModel> dataList, double[] lat, double[] lng)
        {

            var query = from a in dataList
                        where a.Longitude >= lng[0] && a.Longitude <= lng[1]
                        && a.Latitude >= lat[0] && a.Latitude <= lat[1]
                        select a;

            return query.ToList();
        }

        /// <summary>
        /// 采用geohash算法，请求获取附近的人
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dict"></param>
        /// <param name="num"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static List<OnlineUserModel> NeighborPeople(OnlineUserModel model, Dictionary<string, OnlineUserModel> dict, int num, int precision)
        {
            string key = Geohash.Encode(model.Latitude, model.Longitude);

            if (!string.IsNullOrWhiteSpace(key))
            {
                for (int i = precision - 1; i > 0; i--)
                {
                    //LogHelper.WriteLog(key + "  ==  " + i);
                    string temp = "hash:" + key.Substring(0, i);
                    lock ("Jinher.AMP.SNS.Service.BE.Neighbor.NeighborStore")
                    {
                        var query = from a in dict
                                    where a.Key.Contains(temp)
                                    select a.Value;
                        if (query.Count() >= num)
                        {
                            return query.ToList();
                        }
                        if (i == 1)
                        {
                            var r = query.ToList();

                            var data = NeighborPeopleForNullModel(dict, num - r.Count);
                            r.AddRange(data);
                            return r;
                        }
                    }
                }
            }
            else
            {
                return NeighborPeopleForNullModel(dict, num);
            }
            return null;
        }
        static Random ram = new Random();
        public static List<OnlineUserModel> NeighborPeopleForNullModel(Dictionary<string, OnlineUserModel> dict, int num)
        {
            var data = from a in dict
                       where a.Key.Contains("hash:null;")
                       select a.Value;
            var temp = data.Count() - num;

            if (temp <= 0)
                return data.ToList();
            else
            {
                int index = ram.Next((temp + num) / num);
                return data.Skip(index).Take(num).ToList();
            }
        }
    }
}
