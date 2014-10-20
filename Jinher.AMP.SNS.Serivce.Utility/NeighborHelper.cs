using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.SNS.Serivce.Utility
{
    /// <summary>
    /// 附近的人帮助类
    /// </summary>
    public class NeighborHelper
    {
        private static double earth_radius = 6370856;//地球半径 （米）
        private static double length = 40075.7 * 1000 / 360;//地球周长单位米/360 为一纬度的距离
        private static double lngAngle = 0.9;
        /// <summary>
        /// 弧度计算
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double Rad(double d)
        {
            return d * Math.PI / 180.0;
        }
        public static void DistanceOfTwoPoints(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = Rad(lat1);
            double radLat2 = Rad(lat2);
            double a = radLat1 - radLat2;
            double b = Rad(lng1) - Rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * earth_radius;//earth_radius
            s = Math.Round(s * 10000) / 10000;
            double ss = s * 1.0936132983377;
            //Console.WriteLine("两点间的距离是：" + s + "米" + "," + (int)ss + "码");
            Console.WriteLine("两点间的距离是：" + s + "米" + "," + (int)ss + "码");
        }

        /// <summary>
        ///  计算当前点坐标范围坐标
        /// </summary>
        /// <param name="lat1">当前点纬度</param>
        /// <param name="lng1">当前点经度</param>
        /// <param name="range">矩形范围半径</param>
        /// <param name="lat2">纬度范围值</param>
        /// <param name="lng2">经度范围值</param>
        public static void DistanceOfPoint(double lat1, double lng1, int range, out double[] lat2, out double[] lng2)
        {
            lat2 = LatitudeRange(lat1, range);
            lng2 = LongitudeRange(lng1, range);
            return;
        }
        /// <summary>
        /// 经度范围计算Longitude
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        private static double[] LongitudeRange(double lng, int range)
        {
            double sublng  = range / length;
            return new double[] { lng - sublng, lng + sublng };
        }
        /// <summary>
        /// 纬度范围计算Latitude
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        private static double[] LatitudeRange(double lat, int range)
        {

            double sublat = Math.Abs(range / (length * Math.Cos(lat)));

            return new double[] { lat - sublat, lat + sublat };
        }
        /// <summary>
        /// 获取距离内纬度
        /// </summary>
        /// <returns></returns>
        public static double GetRangeLat(int range)
        {
            return Math.Abs(range / (length * lngAngle));
        }

        /// <summary>
        /// 获取距离内经度
        /// </summary>
        /// <returns></returns>
        public static double GetRangeLng( int range)
        {
            return range / length;
        }
    }
}
