/*==========================================================
*作者：SmallerPig
*时间：2013/8/1 9:34:08
*版权所有:无锡睿阅数字科技有限公司
============================================================*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace RY.Common
{
    /// <summary>
    /// 提供缓存的实现
    /// </summary>
    public class CacheHelper
    {

        Cache cache;
        SqlCacheDependency sqlCacheDependency;

        public CacheHelper()
        {
            cache = HttpRuntime.Cache;
        }

        public object GetByKey(string key)
        {
            if (cache[key] == null)
            {
                return null;
            }
            return cache[key];
        }

        public void SetCahce(string key, object value)
        {
            cache[key] = value;
        }

        public void ClearCacheByKey(string key)
        {
            cache[key] = null;
        }

        public void SetSqlDateTableCache(string key, object value, string table, string connectionName = null )
        {
            string database = connectionName ?? "ImageOfTaiWan";
            sqlCacheDependency = new SqlCacheDependency(database, table);
            cache.Insert(key, value, sqlCacheDependency);
        }


    }
}
