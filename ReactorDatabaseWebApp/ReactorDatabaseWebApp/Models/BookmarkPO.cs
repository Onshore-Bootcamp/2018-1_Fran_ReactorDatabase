using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReactorDatabaseWebApp.Models
{
    public class BookmarkPO
    {
        public int BookmarkID { get; set; }

        public int UserID { get; set; }

        public int ReactorID { get; set; }

        public ReactorPO ReactorInfo { get; set; }
    }
}