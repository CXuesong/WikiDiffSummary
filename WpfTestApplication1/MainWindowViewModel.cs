using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiDiffSummary;

namespace WpfTestApplication1
{
    internal class MainWindowViewModel : BindableBase
    {

        private string _Text1 = "== A ==\nSome text. Some text.";
        private string _Text2 = "== B ==\nSome text. Some other text.";
        private string _Summary = "Diff summary will be shown here.";

        public string Text1
        {
            get { return _Text1; }
            set
            {
                if (SetProperty(ref _Text1, value)) BuildSummary();
            }
        }


        public string Text2
        {
            get { return _Text2; }
            set
            {
                if (SetProperty(ref _Text2, value)) BuildSummary();
            }
        }


        public string Summary
        {
            get { return _Summary; }
            private set { SetProperty(ref _Summary, value); }
        }

        private void BuildSummary()
        {
            var cmp = new WikitextBySectionComparer();
            var diffs = cmp.Compare(_Text1, _Text2);
            Summary = string.Join("\n", diffs);
        }
    }
}
