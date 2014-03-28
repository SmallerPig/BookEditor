/*==========================================================
*作者：SmallerPig
*时间：2013/8/1 14:37:52
*版权所有:无锡睿阅数字科技有限公司
============================================================*/
using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Caching;


namespace RY.Common
{
    public static class LinqToSqlCache
    {
        /// <summary>
        /// 快取 LINQ Query 的結果（僅適用於 LINQ to SQL 環境）
        /// 使用的的限制跟使用 SqlCacheDependency 的限制一樣
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <param name="dc">你的 LINQ to SQL DataContext</param>
        /// <param name="CacheId">Cache ID，需為每一個不同的 IQueryable 物件設定一個唯一的 ID</param>
        /// <returns></returns>
        public static List<T> LinqCache<T>(this IQueryable<T> q, DataContext dc, string CacheId)
        {
            List<T> objCache = (List<T>)System.Web.HttpRuntime.Cache.Get(CacheId);

            if (objCache == null)
            {
                #region 从数据库查找

                ///////// 尚未快取，實做 new SqlCacheDependeny //////////

                // 1. 透過 DataContext 取得連線字串
                string connStr = dc.Connection.ConnectionString;

                // 2. 透過 DataContext 與 IQueryable 物件取得 SqlCommand 物件
                SqlCommand sqlCmd = dc.GetCommand(q) as SqlCommand;

                // 3. 建立要給 SqlCacheDependency 使用的 SqlConnection 物件
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // 4. 建立要給 SqlCacheDependency 使用的 SqlCommand 物件
                    using (SqlCommand cmd = new SqlCommand(sqlCmd.CommandText, conn))
                    {
                        // 5.0 將 sqlCmd 中的所有參數傳遞給 cmd 物件
                        foreach (System.Data.Common.DbParameter dbp in sqlCmd.Parameters)
                        {
                            cmd.Parameters.Add(new SqlParameter(dbp.ParameterName, dbp.Value));
                        }

                        // 5.1 啟用資料庫的 Query notifications 功能
                        SqlCacheDependencyAdmin.EnableNotifications(connStr);

                        // 5.2 取得要進行異動通知的表格名稱(ElementType)
                        string NotificationTable = q.ElementType.Name;

                        // 5.3 將取得的 NotificationTable 啟用通知功能
                        if (!SqlCacheDependencyAdmin.GetTablesEnabledForNotifications(connStr).Contains(NotificationTable))
                            SqlCacheDependencyAdmin.EnableTableForNotifications(connStr, NotificationTable);

                        // 6. 建立 SqlCacheDependency物件
                        SqlCacheDependency sqlDep = new SqlCacheDependency(cmd);

                        // 7. 刷新 LINQ to SQL 的值（取得資料庫中的最新資料）
                        dc.Refresh(RefreshMode.OverwriteCurrentValues, q);

                        // 8. 執行 SqlCacheDepency 查詢
                        cmd.ExecuteNonQuery();

                        // 9. 執行 LINQ to SQL 的查詢，並將結果轉成 List<T> 型別，避免延遲查詢(Delayed Query)立即將資料取回
                        objCache = q.ToList();

                        //10. 將結果插入到 System.Web.HttpRuntime.Cache 物件中，並且指定 SqlCacheDependency 物件
                        System.Web.HttpRuntime.Cache.Insert(CacheId, objCache, sqlDep);
                    }
                }
                #endregion
            }

            // 回傳查詢結果（或快取的結果）
            return objCache;
        }
    }
}