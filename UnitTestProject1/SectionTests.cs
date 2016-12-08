using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WikiDiffSummary;

namespace UnitTestProject1
{
    [TestClass]
    public class SectionTests
    {
        [TestMethod]
        public void ParseSectionsTest()
        {
            var sections = WikitextSection.ParseSections(WikitextCases.Simple1A);
            foreach (var s in sections)
            {
                Trace.WriteLine(s.Path);
                Trace.WriteLine(s.Content);
                Trace.WriteLine("----");
            }
        }
    }
}
