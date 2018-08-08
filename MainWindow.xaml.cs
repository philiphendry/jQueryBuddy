using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using jQueryBuddy.CodeCompletion;
using jQueryBuddy.InstantHelp;
using jQueryBuddy.Persistence;
using jQueryBuddy.Preferences;
using jQueryBuddy.Properties;
using jQueryBuddy.Rendering;
using jQueryBuddy.UserControls;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using MessageBoxOptions = System.Windows.MessageBoxOptions;

namespace jQueryBuddy
{
    public partial class MainWindow : IUserPreferences
    {
        private const string SnippetFileExtension = "jbs";

        private readonly HtmlRenderer _resultsRenderer;
        private readonly IInstantHelpRenderer _instantHelpRenderer;
        private SnippetDocument _activeTabDocument;
        private Rendering.References _references;

        #region Commands and Binding

        public static readonly RoutedCommand CommandReferences = new RoutedCommand();
        public static readonly RoutedCommand CommandBuildExecute = new RoutedCommand();
        public static readonly RoutedCommand CommandNextPane = new RoutedCommand();
        public static readonly RoutedCommand CommandHideResultsPane = new RoutedCommand();
        public static readonly RoutedCommand CommandHideHelpPane = new RoutedCommand();
        private readonly CanExecuteRoutedEventHandler _canAlwaysExecute = (sender, e) => e.CanExecute = true;

        private void BindCommands()
        {
            MenuReferences.Command = CommandReferences;
            CommandBindings.Add(new CommandBinding(CommandReferences, ExecuteReferences));
            CommandReferences.InputGestures.Add(new KeyGesture(Key.F4));

            MenuExecute.Command = CommandBuildExecute;
            CommandBindings.Add(new CommandBinding(CommandBuildExecute, ExecuteBuild));
            CommandBuildExecute.InputGestures.Add(new KeyGesture(Key.F5));

            MenuNextPane.Command = CommandNextPane;
            CommandBindings.Add(new CommandBinding(CommandNextPane, ExecuteNextPane));
            CommandNextPane.InputGestures.Add(new KeyGesture(Key.F6));

            MenuHideResultsPane.Command = CommandHideResultsPane;
            CommandBindings.Add(new CommandBinding(CommandHideResultsPane, ExecuteHideResultsPane));
            CommandHideResultsPane.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control));

            MenuHideHelpPane.Command = CommandHideHelpPane;
            CommandBindings.Add(new CommandBinding(CommandHideHelpPane, ExecuteHideHelpPane));
            CommandHideHelpPane.InputGestures.Add(new KeyGesture(Key.I, ModifierKeys.Control));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, ExecuteNew, _canAlwaysExecute));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, ExecuteSave, _canAlwaysExecute));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, ExecuteOpen, _canAlwaysExecute));
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Icon = BitmapFrame.Create(new Uri("pack://application:,,,/Resources/Images/DollarBlue.ico", UriKind.RelativeOrAbsolute));

            _references = new Rendering.References();
            _resultsRenderer = new HtmlRenderer(grdResults);
            _resultsRenderer.OnError += OnRenderError;

            var newSnippet = CreateNewDocument("New Query");
            _activeTabDocument = newSnippet;
            
            BindCommands();

            Closing += OnClosing;

            _instantHelpRenderer = new WebBrowserInstantHelpRenderer(); // or ... new FlowDocumentInstantHelpRenderer();
            _instantHelpRenderer.LoadPreferences(Settings.Default);
            LoadPreferences(Settings.Default);
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var editedCount = DocumentTab.Items.Cast<TabItem>().Select(tab => tab.Content as SnippetDocument).Where(snippet => snippet.IsModified).Count();
            if (editedCount <= 0) 
                return;

            if (MessageBox.Show(String.Format("There {0} with unsaved changes. Are you sure you want to exit?", 
                editedCount == 1 ? "is a tab" : "are " + editedCount + " tabs"),
                "Unsaved Changes", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private SnippetDocument CreateNewDocument(string tabName)
        {
            var newSnippet = new SnippetDocument();
            newSnippet.JavascriptCompletion += OnPreviewJavascriptCompletion;
            var newTab = new TabItem { Content = newSnippet, Header = tabName };
            DocumentTab.Items.Add(newTab);
            newTab.IsSelected = true;
            return newSnippet;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            _instantHelpRenderer.SavePreferences(Settings.Default);
            SavePreferences(Settings.Default);
            Settings.Default.Save();
        }

        private void OnRenderError(object sender, HtmlRenderer.RenderErrorEventArgs e)
        {
            var errorInfo = e.Error;

            txtStatusMessage.Text = String.Format("Error : {0} at line {1} in the {2}", 
                errorInfo.Message, errorInfo.LineNumber, errorInfo.Source);

            _activeTabDocument.RenderError(errorInfo);
        }

        private void OnPreviewJavascriptCompletion(object sender, InstantHelpEventArgs e)
        {
            grdInstantHelp.Children.Clear();
            grdInstantHelp.Children.Add(_instantHelpRenderer.RenderHelp(e.HtmlHelp, e.HtmlExamples));
        }

        private void OnDocumentTabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            _activeTabDocument = ((TabItem) e.AddedItems[0]).Content as SnippetDocument;
        }

        private void ClearStatusBar()
        {
            txtStatusMessage.Text = "";
        }

        #region Command Execute Events

        private void ExecuteReferences(object sender, ExecutedRoutedEventArgs e)
        {
            var referencesDialog = new References(_references);
            referencesDialog.ShowDialog();
            _references = referencesDialog.GetReferences();
           System.Windows.Forms.MessageBox.Show(_references.GetHtml());
        }

        private void ExecuteBuild(object sender, ExecutedRoutedEventArgs e)
        {
            ClearStatusBar();
            _activeTabDocument.ClearErrors();
            _resultsRenderer.Render(_activeTabDocument.Javascript, _activeTabDocument.Html, _references);
        }

        private static void ExecuteNextPane(object sender, ExecutedRoutedEventArgs e)
        {
            //if (txtHtml.IsKeyboardFocusWithin)
            //    txtJavascript.Focus();
            //else if (txtJavascript.IsKeyboardFocusWithin)
            //    txtHtml.Focus();
            //else if (grdResults.IsKeyboardFocusWithin)
            //    grdResults.Focus();
            //else if (grdInstantHelp.IsKeyboardFocusWithin)
            //    grdInstantHelp.Focus();
            //else
            //    txtHtml.Focus();
        }

        private void ExecuteHideResultsPane(object sender, ExecutedRoutedEventArgs e)
        {
            ResultDefinition.Height = (ResultDefinition.Height.IsStar ? new GridLength(0) : new GridLength(CalculateNewPaneHeight(), GridUnitType.Star));
            ResultSplitterDefinition.Height = (ResultSplitterDefinition.Height.Value == 0 ? new GridLength(5) : new GridLength(0));
            ResultSplitterDefinition.MinHeight = ResultSplitterDefinition.MinHeight == 0 ? 5 : 0;
            MenuHideResultsPane.Header = ResultDefinition.Height.Value == 0 ? "Sh_ow Results Pane" : "Hi_de Results Pane";
        }

        private void ExecuteHideHelpPane(object sender, ExecutedRoutedEventArgs e)
        {
            HelpDefinition.Height = (HelpDefinition.Height.IsStar ? new GridLength(0) : new GridLength(CalculateNewPaneHeight(), GridUnitType.Star));
            HelpSplitterDefinition.Height = (HelpSplitterDefinition.Height.Value == 0 ? new GridLength(5) : new GridLength(0));
            HelpSplitterDefinition.MinHeight = HelpSplitterDefinition.MinHeight == 0 ? 5 : 0;
            MenuHideHelpPane.Header = HelpDefinition.Height.Value == 0 ? "Sho_w Help Pane" : "Hi_de Help Pane";
        }

        /// <summary>
        /// When the results or help panes are made visible after being hidden we need a way to re-calculate the space they should
        /// occupy since, when they were hidden, the others would have filled up the space that remained.
        /// </summary>
        /// <returns></returns>
        private double CalculateNewPaneHeight()
        {
            if (ResultDefinition.Height.Value == 0 && HelpDefinition.Height.Value == 0)
            {
                return EditorsDefinition.Height.Value / 2;
            }
            return ResultDefinition.Height.Value == 0 ? HelpDefinition.Height.Value/2 : ResultDefinition.Height.Value/2;
        }

        private void ExecuteNew(object sender, ExecutedRoutedEventArgs e)
        {
            CreateNewDocument("New Query");
        }

        private void ExecuteSave(object sender, ExecutedRoutedEventArgs e)
        {
            var snippet = new Snippet(_activeTabDocument.Html, _activeTabDocument.Javascript);
            var dialog = new SaveFileDialog {
                     AddExtension = true,
                     DefaultExt = SnippetFileExtension,
                     Filter = String.Format("jQueryBuddy Snippet (*.{0})|*.{0}", SnippetFileExtension)
                 };
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            var serializer = new XmlSerializer(typeof(Snippet));
            serializer.Serialize(dialog.OpenFile(), snippet);
            _activeTabDocument.IsModified = false;
        }

        private void ExecuteOpen(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog {
                     AddExtension = true,
                     DefaultExt = SnippetFileExtension,
                     Filter = String.Format("jQueryBuddy Snippet (*.{0})|*.{0}", SnippetFileExtension)
                };
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            var serializer = new XmlSerializer(typeof(Snippet));
            var snippet = serializer.Deserialize(dialog.OpenFile()) as Snippet;
            if (snippet != null)
            {
                var newSnippet = new SnippetDocument {Html = snippet.Html, Javascript = snippet.Javascript};
                newSnippet.JavascriptCompletion += OnPreviewJavascriptCompletion;
                newSnippet.IsModified = false;
                var newTab = new TabItem { Content = newSnippet, Header = "New Tab" };
                DocumentTab.Items.Add(newTab);
                newTab.IsSelected = true;
            }
        }

        #endregion

        #region IUserPreferences

        public void LoadPreferences(Settings settings)
        {
            Top = settings.WindowTop;
            Left = settings.WindowLeft;
            Width = settings.WindowWidth;
            Height = settings.WindowHeight;
            WindowState = settings.WindowState;
        }

        public void SavePreferences(Settings settings)
        {
            if (WindowState != WindowState.Minimized)
            {
                settings.WindowTop = Top;
                settings.WindowLeft = Left;
                settings.WindowWidth = Width;
                settings.WindowHeight = Height;
                settings.WindowState = WindowState;
            }
        }

        #endregion
    }
}
