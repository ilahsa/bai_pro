using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enda
{
    public class NLog
    {
        [Conditional("DEBUG")]
        public static void Info(string log)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("nlog_{0}_{1}", DateTime.Now.ToString(), log));
        }
    }
}
