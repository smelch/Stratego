using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stratego.Net.WCF
{
    public class SessionInfo
    {
        public Guid SessionID { get; set; }
        public string RedPlayerName { get; set; }
        public string BluePlayerName { get; set; }
    }
}
