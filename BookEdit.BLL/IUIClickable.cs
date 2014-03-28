using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookEdit.BLL
{
    public interface IUIClickable
    {
        string ParagId { get; set; }

        string NodeId { get; set; }


        int BookId { get; set; }
    }
}
