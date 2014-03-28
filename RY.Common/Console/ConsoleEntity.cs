using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*==========================================================
*作者：SmallerPig
*时间：2014/1/15 15:36:01
*版权所有:无锡睿阅数字科技有限公司
============================================================*/
namespace RY.Common.Console
{
    public interface  IConsoleEntity
    {
        int Id { get; set; }

        bool IsLock { get; set; }

    }
}
