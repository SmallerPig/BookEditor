using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RY.Common
{
    /// <summary>
    /// 可以在CMS里面呈现列表接口
    /// </summary>
    public interface IConsoleListable<T> where T : Console.IConsoleEntity
    {
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="islocked"></param>
        /// <returns></returns>
        int GetCount(bool islocked);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="count"></param>
        /// <param name="isLock"></param>
        /// <returns></returns>
        IList<T> GetList(int from, int count,bool isLock);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetById(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        void ToRecycle(T t);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        void Restore(T t);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        void Delete(T t);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        T Insert(T t);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        void Update(T t);
    }
}
