using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Imps.Services.HA
{
    public class EggLog
    {
        [Conditional("DEBUG")]
        public static void Info(string log)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("egg3_{0}_{1}", DateTime.Now.ToString(), log));
        }
    }
}
