using System;

namespace ReactorDatabaseWebApp.Models
{
    public class LogPO
    {
        public int LogID { get; set; }

        public string Timestamp { get; set; }

        public string Message { get; set; }
    }
}