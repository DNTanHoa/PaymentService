using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Ultils.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToQueryString(this object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + WebUtility.UrlEncode(p.GetValue(obj, null)?.ToString());
            string queryString = string.Join("&", properties.ToArray());
            return queryString;
        }
    }
}
