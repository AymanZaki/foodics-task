using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Configs
{
    public class EmailConfig
    {
        public Smtp Smtp { get; set; }
        public string From { get; set; }
        public string To { get; set; }

    }
    public class Smtp
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
    }
}
