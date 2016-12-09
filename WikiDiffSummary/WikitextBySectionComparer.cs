using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DiffMatchPatch;
using MwParserFromScratch;
using MwParserFromScratch.Nodes;

namespace WikiDiffSummary
{
    public class WikitextBySectionComparer
    {
        private float _SectionResemblanceThreshold = 0.4f;

        /// <summary>
        /// A threshold used to when determine whether the two sections are similar.
        /// This threshold is used to determine whether a section has been renamed.
        /// </summary>
        /// <value>A float value between 0 and 1. The larger the value, the larger possibility that two sections are treated as similar.</value>
        public float SectionResemblanceThreshold
        {
            get { return _SectionResemblanceThreshold; }
            set
            {
                if (value < 0 || value > 1) throw new ArgumentOutOfRangeException(nameof(value));
                _SectionResemblanceThreshold = value;
            }
        }

        public IList<SectionDiff> Compare(string text1, string text2)
        {
            if (text1 == null) throw new ArgumentNullException(nameof(text1));
            if (text2 == null) throw new ArgumentNullException(nameof(text2));
            var diffs = new List<SectionDiff>();
            var sections1 = WikitextSection.ParseSections(text1);
            var sections2 = WikitextSection.ParseSections(text2);
            var dmp = new diff_match_patch();
            for (int i1 = 0, i2 = 0; i1 < sections1.Count || i2 < sections2.Count;)
            {
                var s1 = i1 < sections1.Count ? sections1[i1] : null;
                var s2 = i2 < sections2.Count ? sections2[i2] : null;
                Debug.Assert(s1 != null || s2 != null);
                if (s1 == null)
                {
                    diffs.Add(new SectionDiff(null, s2, s2.Content.Length, 0));
                    i2++;
                }
                else if (s2 == null)
                {
                    diffs.Add(new SectionDiff(s1, null, 0, s1.Content.Length));
                    i1++;
                    continue;
                }
                else if (s1.Path == s2.Path)
                {
                    // Diff by the same title.
                    diffs.Add(Compare(dmp, s1, s2));
                    i1++;
                    i2++;
                }
                else
                {
                    // The titles of s1 & s2 are different.
                    // Determine whether the section has been renamed.
                    // If we can judge the resemblance simply by length, we'll not perform expensive diff.
                    var resemble = Math.Abs((float) (s1.Length - s2.Length)/Math.Max(s1.Length, s2.Length)) <=
                                   SectionResemblanceThreshold;
                    SectionDiff d = null;
                    if (resemble)
                    {
                        d = Compare(dmp, s1, s2);
                        var removedRatio = (float) d.RemovedChars/s1.Length;
                        var addedRatio = (float) d.RemovedChars/s2.Length;
                        if (addedRatio >= SectionResemblanceThreshold || removedRatio >= SectionResemblanceThreshold)
                            resemble = false;
                    }
                    if (resemble)
                    {
                        Debug.Assert(d != null);
                        // We assume the section has been renamed.
                        diffs.Add(d);
                        i1++;
                        i2++;
                    }
                    else
                    {
                        // Otherwise, 1 or more sections have been added / removed.
                        // We assume we can decide the modifications of sections by the count of sections below the current one.
                        var sectionsLeft1 = sections1.Count - i1;
                        var sectionsLeft2 = sections2.Count - i2;
                        if (sectionsLeft1 == sectionsLeft2)
                        {
                            diffs.Add(new SectionDiff(s1, null, 0, s1.Length));
                            diffs.Add(new SectionDiff(null, s2, s2.Length, 0));
                            i1++;
                            i2++;
                        }
                        else if (sectionsLeft1 < sectionsLeft2)
                        {
                            diffs.Add(new SectionDiff(null, s2, s2.Length, 0));
                            i2++;
                        }
                        else
                        {
                            // sectionsLeft1 > sectionsLeft2
                            diffs.Add(new SectionDiff(s1, null, 0, s1.Length));
                            i1++;
                        }
                    }
                }
            }
            return diffs;
        }

        internal SectionDiff Compare(diff_match_patch comparer, WikitextSection section1, WikitextSection section2)
        {
            Debug.Assert(comparer != null);
            Debug.Assert(section1 != null);
            Debug.Assert(section2 != null);
            var diff = comparer.diff_main(section1.Content, section2.Content);
            comparer.diff_cleanupSemantic(diff);
            int addedChars = 0, removedChars = 0;
            var identical = true;
            foreach (var d in diff)
            {
                switch (d.operation)
                {
                    case Operation.DELETE:
                        removedChars += d.text.OfType<char>().Count(c => !char.IsWhiteSpace(c));
                        identical = false;
                        break;
                    case Operation.INSERT:
                        addedChars += d.text.OfType<char>().Count(c => !char.IsWhiteSpace(c));
                        identical = false;
                        break;
                    case Operation.EQUAL:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return new SectionDiff(section1, section2, addedChars, removedChars, identical);
        }
    }
}
