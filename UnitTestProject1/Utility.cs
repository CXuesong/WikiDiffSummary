using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    internal static class Utility
    {
        public static void TraceCollection<T>(T collection) where T : IEnumerable
        {
            var c = collection as ICollection;
            if (c != null) Trace.WriteLine("Count = " + c.Count);
            foreach (var item in collection)
            {
                Trace.WriteLine("\t" + item);
            }
        }
    }
}
