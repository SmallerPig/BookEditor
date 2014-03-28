using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookEdit.Entity.Interface
{
    interface IDeleteable<T> where T:new()
    {
        void Delete(T t);
    }
}
