using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactorDB_DAL.Models
{
    public class BookmarkDO
    {
        public int BookmarkID { get; set; }

        public int UserID { get; set; }

        public int ReactorID { get; set; }
    }
}
