using System.Collections.Generic;
using System.Windows;
using jQueryBuddy.CodeCompletion;
using jQueryBuddy.Preferences;

namespace jQueryBuddy.InstantHelp
{
    public interface IInstantHelpRenderer : IUserPreferences
    {
        UIElement RenderHelp(string htmlHelp, IList<InstantHelpEventArgs.HtmlExample> htmlExamples);
    }
}