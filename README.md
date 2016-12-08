_For [WikiEdit](https://github.com/CXuesong/WikiEdit), a rudimentary Windows desktop client for MediaWiki sites._

_For [Warriors Wiki](http://warriors.wikia.com), though I've gone too far from the clans, I suppose._

# WikiDiffSummary

An automatic wikitext revision comparer. This is a .NET PCL library targeting at .NET Framework 4.5, ASP.NET Core 1.0, Xamarin.iOS, and Xamarin.Android.

For now it can perform by-section diff, and find out the added/removed/modified/renamed sections. However, it cannot recognize the moved section, and the performance is terrible if there're two many differences between the revisions.

The API is simple, and you can find its usage in `UnitTestProject1`, such as:

```c#
[TestMethod]
public void Simple1()
{
    var cmp = new WikitextBySectionComparer();
    var diffs = cmp.Compare(WikitextCases.Simple1A, WikitextCases.Simple1B);
    TraceCollection(diffs);
}
```

You can also find a GUI demo in `WpfTestApplication1`. Its usage is simple. Just enter the two revisions to the left side and the right side, respectively, wait a moment, and the result is shown at the bottom.