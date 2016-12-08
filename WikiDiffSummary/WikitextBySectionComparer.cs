using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MwParserFromScratch;
using MwParserFromScratch.Nodes;

namespace WikiDiffSummary
{
    public partial class WikitextBySectionComparer
    {

        public List<SectionDiff> Compare(string text1, string text2)
        {
            if (text1 == null) throw new ArgumentNullException(nameof(text1));
            if (text2 == null) throw new ArgumentNullException(nameof(text2));

            return null;
        }
    }
}
