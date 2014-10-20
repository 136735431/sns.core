using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.SNS.IService.Store
{
    public interface IOnlineStoreable
    {
        object Take();
        object Take(string userId, string appid);
        object Save(string key, object value);


    }
}
