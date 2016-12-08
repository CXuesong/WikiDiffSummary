using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MwParserFromScratch;
using MwParserFromScratch.Nodes;

namespace WikiDiffSummary
{
    /// <summary>
    /// Represents a section in wikitext string.
    /// </summary>
    public class WikitextSection : IWikitextSpanInfo
    {
        private static readonly IEnumerable<Heading> GetSections_Sentinel = new Heading[] {null};

        public static IList<WikitextSection> ParseSections(string text)
        {
            return ParseSections(new WikitextParser(), text);
        }

        private static IList<WikitextSection> ParseSections(WikitextParser parser, string text)
        {
            Debug.Assert(parser != null);
            var root = parser.Parse(text);
            var sections = new List<WikitextSection>();
            var lastPath = new List<Heading>();
            foreach (var heading in root.Lines.OfType<Heading>().Concat(GetSections_Sentinel))
            {
                // Push last section.
                var lastSectionStartsAt = ((IWikitextSpanInfo) lastPath.LastOrDefault())?.Start ?? 0;
                var thisSectionStartsAt = ((IWikitextSpanInfo) heading)?.Start ?? text.Length;
                var lastSection = new WikitextSection(SectionPath.FromHeadings(lastPath),
                    text.Substring(lastSectionStartsAt, thisSectionStartsAt - lastSectionStartsAt),
                    lastSectionStartsAt, thisSectionStartsAt - lastSectionStartsAt);
                sections.Add(lastSection);
                // Prepare next section.
                if (heading != null)
                {
                    while (lastPath.Count > 0 && heading.Level <= lastPath.Last().Level)
                    {
                        lastPath.RemoveAt(lastPath.Count - 1);
                    }
                    lastPath.Add(heading);
                }
            }
            return sections;
        }

        public WikitextSection(SectionPath path, string content, int start, int length)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (content == null) throw new ArgumentNullException(nameof(content));
            Path = path;
            Content = content;
            Start = start;
            Length = length;
        }

        /// <summary>
        /// Path of titles.
        /// </summary>
        public SectionPath Path { get; }

        /// <summary>
        /// Section titles.
        /// </summary>
        public string Content { get; }

        /// <inheritdoc />
        public int Start { get; }

        /// <inheritdoc />
        public int Length { get; }

        /// <inheritdoc />
        bool IWikitextSpanInfo.HasSpanInfo => true;
    }
}