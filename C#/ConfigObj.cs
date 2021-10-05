using System.Collections.Generic;
using System.IO;

namespace Coloryr.Lib
{
    public record RobotConfig
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public long QQ { get; set; }
        public bool Auto { get; set; }
        public int Time { get; set; }
    }
}
