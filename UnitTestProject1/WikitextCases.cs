using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    /// <summary>
    /// Contains sample wikitext strings.
    /// </summary>
    internal static class WikitextCases
    {
        public const string Simple1A = @"Text text.

== A ==
TextA
=== A1 ===
TextA1
=== A2 ===
TextA2
== B ==
TextB
";
        public const string Simple1B = @"Text text.

== A ==
TextA
=== A1 ===
TextA1
=== A2 ===
Foo
== B ==
TextB
";

    }
}
