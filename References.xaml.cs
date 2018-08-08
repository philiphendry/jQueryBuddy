using System.Windows;
using System.Windows.Data;
using Microsoft.Windows.Controls;

namespace jQueryBuddy
{
    /// <summary>
    /// Interaction logic for References.xaml
    /// </summary>
    public partial class References : Window
    {
        private readonly Rendering.References _references;

        public References(Rendering.References references)
        {
            InitializeComponent();
            _references = references;
            referencesGrid.ItemsSource = _references;
        }

        public Rendering.References GetReferences()
        {
            return _references;
        }
    }
}
