using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorLib.Log
{
    public interface ILog
    {
        void LogError(string a);
        void LogError(Exception a);
        void LogOut(string a);
    }
}
