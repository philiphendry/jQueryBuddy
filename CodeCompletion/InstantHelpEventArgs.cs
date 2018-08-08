using System;
using System.Collections.Generic;

namespace jQueryBuddy.CodeCompletion
{
    public class InstantHelpEventArgs : EventArgs
    {
        public class HtmlExample
        {
            public string Description;
            public string Html;
            public string Css;
            public string Code;
            public IList<string> JavascriptReferences;
        }
        
        public string HtmlHelp;
        public IList<HtmlExample> HtmlExamples;
    }
}