using AngularWizards.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AngularWizards.AngularComponent
{
    /// <summary>
    /// Interaction logic for WizardWindow.xaml
    /// </summary>
    public partial class AngularComponentWizardWindow : Window
    {
        protected readonly NameService _nameSvc = new NameService();

        public AngularComponentWizardWindow()
        {
            InitializeComponent();
            compStyleLanguage.SelectedValue = StylesheetLanguage.SCSS;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void SelectAllOnFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox)?.SelectAll();
        }

        private void compName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFields();
        }

        public void UpdateFields()
        {
            if (this.IsInitialized && compName.Text.Length > 0 && useCustomNames.IsChecked != true)
            {
                IEnumerable<string> nameParts = _nameSvc.SplitName(compName.Text);
                string pascalCase = _nameSvc.ToPascalCase(nameParts);
                compClassName.Text = $"{pascalCase}Component";
                compSelector.Text = $"{compSelectorPrefix.Text}{(compSelectorPrefix.Text.Length > 0 ? "-" : "")}{string.Join("-", nameParts)}";
                compClassFileName.Text = $"{string.Join("-", nameParts)}.component.ts";
                compStyleFileName.Text = $"{string.Join("-", nameParts)}.component.{compStyleLanguage.SelectedValue.ToString().ToLower()}";
                compTemplateFileName.Text = $"{string.Join("-", nameParts)}.component.html";
                compTestFileName.Text = $"{string.Join("-", nameParts)}.component.spec.ts";
                compFolderName.Text = string.Join("-", nameParts);
            }
        }

        private void compStyleLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFields();
        }
    }
}
