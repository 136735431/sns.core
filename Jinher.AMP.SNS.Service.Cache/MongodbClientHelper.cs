using Jinher.AMP.SNS.Service.Deploy;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Jinher.AMP.SNS.Service.Cache
{
    public class MongodbClientHelper
    {
        private string mongodbServer = "183.56.132.10";
        private string mongodbPort = "27017";
        //<Mongodb Server="183.56.132.10" Port="27017" Database="BRC"></Mongodb>
        MongoServer mongo = null;
        private string mongodbDatabase = "BRC";
        MongoDatabase db = null;
        MongoCollection<BsonDocument> collection = null;
        public MongodbClientHelper()
        {

        }

        public object Take(object list, string type)
        {
            switch (type)
            {
                case "Neighbor"://附近的人
                    return NeighborTake(list);
                default:
                    return null;
            }
        }
        /// <summary>
        /// 更新附近的人
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private object NeighborTake(object list)
        {
            List<OnlineUserModel> dataList = null;
            if (list is List<OnlineUserModel>)
            {
                dataList = list as List<OnlineUserModel>;
            }
            else
                return list;
            mongo = MongoServer.Create("mongodb://" + mongodbServer + ":" + mongodbPort);
            ////选择数据库
            db = mongo.GetDatabase(mongodbDatabase);
            ////选择表
            collection = db.GetCollection<BsonDocument>("BehaviourUseLocationDTO");
            //List<IMongoQuery> mqList = new List<IMongoQuery>();

            //for (int i = 0; i < dataList.Count; i++)
            //{
            //    IMongoQuery mq = Query.EQ("UserId", dataList[i].UserId.ToLower());
            //    mqList.Add(mq);
            //}
            //var query = Query.Or(mqList);
            //var cursor = collection.FindAs<BsonDocument>(query);
            Stopwatch totalsw = new Stopwatch();
            totalsw.Restart();
            Stopwatch sw = new Stopwatch();
            for (int i = 0; i < dataList.Count; i++)
            {
                sw.Restart();
                string uid = dataList[i].UserId.ToLower();
                ////查询条件
                //var query = Query.And(Query.GTE("CreateTime", DateTime.Now.AddDays(-3)), Query.LT("CreateTime", DateTime.Now));
                var query = Query.EQ("UserId", uid);
                ////查询数据
                //var cursor = collection.FindAs<BsonDocument>(query);
                //var doc = cursor.LastOrDefault();
                var cursor = collection.FindAs<BsonDocument>(query).SetSortOrder(SortBy.Descending("LocalTime"));
                cursor.Limit = 1;
                var doc = cursor.FirstOrDefault();
              
                if (doc != null)
                {
                    //var temp = dataList[i];
                    //temp.Latitude = doc["Latitude"].ToDouble();
                    //temp.Longitude = doc["Longitude"].ToDouble();
                    //temp.LocationDesc = doc["LocationInfoDescription"].ToString();
                    //dataList[i] = temp;
                    dataList[i].Latitude = doc["Latitude"].ToDouble();
                    dataList[i].Longitude = doc["Longitude"].ToDouble();
                   // dataList[i].LocationDesc = doc["LocationInfoDescription"].ToString();
                }
                //}
                sw.Stop();
                System.Console.WriteLine("get location：{0} millisecond", sw.ElapsedMilliseconds);
            }

            totalsw.Stop();
            System.Console.WriteLine("get location total：{0} millisecond", totalsw.ElapsedMilliseconds);
            return dataList;
        }
    }
}
