using System;

namespace ReactorDB_DAL.Models
{
    public class LogDO
    {
        public int LogID { get; set; }

        public string Timestamp { get; set; }

        public string Message { get; set; }
    }
}
