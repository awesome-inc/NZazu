using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NZazuFiddle.Utils
{
    internal static class LoggingUtil
    {

        public static string CreateLogMessage(Object instance, string msg, string detailedMsg = null, string stackTrace = null)
        {
            var message = $"{instance.GetType().FullName} :: {GetCurrentMethod()} => {msg}";
            message = detailedMsg != null ? message + Environment.NewLine + detailedMsg : message;
            message = stackTrace != null ? message + Environment.NewLine + stackTrace : message;

            return message;
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(2);

            return sf.GetMethod().Name;
        }
    }
}
