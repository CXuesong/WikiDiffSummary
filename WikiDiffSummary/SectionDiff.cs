using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MwParserFromScratch.Nodes;

namespace WikiDiffSummary
{
    public enum SectionDiffStatus
    {
        /// <summary>
        /// The is a new section.
        /// </summary>
        Added,
        /// <summary>
        /// The section has been removed.
        /// </summary>
        Removed,
        /// <summary>
        /// The section has been modified.
        /// </summary>
        Modified,
        /// <summary>
        /// The section has hardly been modified.
        /// </summary>
        Same,
        /// <summary>
        /// The section has not been modified at all.
        /// </summary>
        Identical,
    }


    /// <summary>
    /// Gets the path consisting of different levels of headings.
    /// </summary>
    public class SectionPath : IList<string>, IEquatable<SectionPath>
    {
        /// <summary>
        /// An empty path. This usually represents the section before the first heading.
        /// </summary>
        public static readonly SectionPath Empty = new SectionPath(new string[0]);

        private readonly IList<string> _Segments;
        private readonly string _Expression;

        internal static SectionPath FromHeadings(IEnumerable<Heading> headings)
        {
            return new SectionPath(headings.Select(h => h.ToPlainText()).ToArray());
        }

        private SectionPath(string[] segments)
        {
            if (segments == null) throw new ArgumentNullException(nameof(segments));
            _Segments = segments;
            _Expression = string.Join("/", segments);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _Segments.Count ^ _Expression.GetHashCode();
        }

        /// <inheritdoc />
        public bool Equals(SectionPath other)
        {
            if (other == null) return false;
            if (_Expression != other._Expression) return false;
            if (ReferenceEquals(_Segments, other._Segments)) return true;
            if (_Segments.Count != other._Segments.Count) return false;
            return _Segments.SequenceEqual(other._Segments);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as SectionPath);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _Expression;
        }

        public static bool operator==(SectionPath x, SectionPath y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            return x.Equals(y);
        }

        public static bool operator !=(SectionPath x, SectionPath y)
        {
            return !(x == y);
        }

        /// <inheritdoc />
        public IEnumerator<string> GetEnumerator() => _Segments.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        void ICollection<string>.Add(string item)
        {
            throw new InvalidOperationException();
        }

        /// <inheritdoc />
        void ICollection<string>.Clear()
        {
            throw new InvalidOperationException();
        }

        /// <inheritdoc />
        public bool Contains(string item) => _Segments.Contains(item);

        /// <inheritdoc />
        public void CopyTo(string[] array, int arrayIndex) => _Segments.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        bool ICollection<string>.Remove(string item)
        {
            throw new InvalidOperationException();
        }

        /// <inheritdoc />
        public int Count => _Segments.Count;

        /// <inheritdoc />
        bool ICollection<string>.IsReadOnly => true;

        /// <inheritdoc />
        public int IndexOf(string item) => _Segments.IndexOf(item);

        /// <inheritdoc />
        void IList<string>.Insert(int index, string item)
        {
            throw new InvalidOperationException();
        }

        /// <inheritdoc />
        void IList<string>.RemoveAt(int index)
        {
            throw new InvalidOperationException();
        }

        public string this[int index] => _Segments[index];

        /// <inheritdoc />
        string IList<string>.this[int index]
        {
            get { return _Segments[index]; }
            set { throw new InvalidOperationException(); }
        }
    }

    /// <summary>
    /// Gets the difference of a section.
    /// </summary>
    public class SectionDiff
    {
        internal SectionDiff(WikitextSection section1, WikitextSection section2, int addedChars, int removedChars, bool identical = false)
        {
            Debug.Assert(section1 != null || section2 != null);
            Section1 = section1;
            Section2 = section2;
            AddedChars = addedChars;
            RemovedChars = removedChars;
            if (section1 == null) Status = SectionDiffStatus.Removed;
            else if (section2 == null) Status = SectionDiffStatus.Added;
            else if (identical) Status = SectionDiffStatus.Identical;
            else if (addedChars == 0 && removedChars == 0) Status = SectionDiffStatus.Same;
            else Status = SectionDiffStatus.Modified;
        }

        public WikitextSection Section1 { get; }

        public WikitextSection Section2 { get; }

        public int AddedChars { get; }

        public int RemovedChars { get; }

        public SectionDiffStatus Status { get; }

        public bool AreSameOrIdentical => Status == SectionDiffStatus.Same || Status == SectionDiffStatus.Identical;

        /// <inheritdoc />
        public override string ToString()
        {
            string s;
            if (Section1 == null || Section2 == null)
                s = $"[{(Section1 ?? Section2).Path}]";
            else if (Section1.Path == Section2.Path)
                s = $"[{Section1.Path}]";
            else
                s = $"[{Section1.Path}][{Section2.Path}]";
            s += Status;
            if (AreSameOrIdentical) return s;
            if (AddedChars > 0) s += " +" + AddedChars;
            if (RemovedChars > 0) s += " -" + RemovedChars;
            return s;
        }
    }
}
