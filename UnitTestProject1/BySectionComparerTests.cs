using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WikiDiffSummary;
using static UnitTestProject1.Utility;

namespace UnitTestProject1
{
    [TestClass]
    public class BySectionComparerTests
    {
        [TestMethod]
        public void Simple1()
        {
            var cmp = new WikitextBySectionComparer();
            var diffs = cmp.Compare(WikitextCases.Simple1A, WikitextCases.Simple1B);
            TraceCollection(diffs);
        }
    }
}
