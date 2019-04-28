using EnvDTE;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace AngularWizards.AngularComponent
{
    public class AngularComponentWizard : IWizard
    {
        protected enum ComponentFile
        {
            Template,
            Class,
            Style,
            Test
        }

        private const string rootSettingsCollection = "Web\\Angular";
        protected string projectSettingsCollection = "";
        private const string node_modules = "node_modules";
        private const string component = "component";
        protected readonly Dictionary<ComponentFile, string> fileNames = new Dictionary<ComponentFile, string>
        {
            { ComponentFile.Template, $"{component}.{component}.html" },
            { ComponentFile.Class, $"{component}.{component}.ts" },
            { ComponentFile.Style, $"{component}.{component}.scss" },
            { ComponentFile.Test, $"{component}.{component}.spec.ts" },
        };
        protected readonly Dictionary<string, bool> files = new Dictionary<string, bool>();

        public void BeforeOpeningFile(global::EnvDTE.ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(global::EnvDTE.Project project)
        {
        }

        public void ProjectItemFinishedGenerating(global::EnvDTE.ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
        }

        protected virtual void InitWizard(AngularComponentWizardWindow wnd, object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            bool? success = false;
            try
            {
                string targetFolderPath = "", projectFolderPath = "", projectFullName = "";
                GetSelectionData(automationObject, ref targetFolderPath, ref projectFolderPath, ref projectFullName);
                var itemName = Regex.Replace(replacementsDictionary["$rootname$"], @"[\.\-\s]?component", "", RegexOptions.IgnoreCase);
                if (string.IsNullOrWhiteSpace(itemName))
                {
                    itemName = "my";
                }
                var wnd = new AngularComponentWizardWindow();
                wnd.compName.Text = itemName;
                this.ReadSettings(wnd, projectFullName);
                this.InitWizard(wnd, automationObject, replacementsDictionary, runKind, customParams);
                success = wnd.ShowDialog();
                if (success == true)
                {
                    this.SaveSettings(wnd, projectFullName);
                    if (wnd.createCompFolder.IsChecked != true)
                    {
                        wnd.compFolderName.Text = "";
                    }
                    else
                    {
                        wnd.compFolderName.Text += "\\";
                    }
                    var compFolderPath = Path.Combine(targetFolderPath, wnd.compFolderName.Text);
                    var nodeModulesRelativePath = new Uri(compFolderPath).MakeRelativeUri(
                         new Uri(Path.Combine(projectFolderPath, node_modules)));
                    replacementsDictionary.Add($"${nameof(wnd.compFolderName)}$", wnd.compFolderName.Text);
                    replacementsDictionary.Add($"${nameof(compFolderPath)}$", compFolderPath);
                    replacementsDictionary.Add($"${nameof(nodeModulesRelativePath)}$", nodeModulesRelativePath.OriginalString);
                    replacementsDictionary.Add($"${nameof(wnd.compName)}$", wnd.compName.Text);
                    replacementsDictionary.Add($"${nameof(wnd.compSelector)}$", wnd.compSelector.Text);
                    replacementsDictionary.Add($"${nameof(wnd.compSelectorPrefix)}$", wnd.compSelectorPrefix.Text);
                    replacementsDictionary.Add($"${nameof(wnd.compClassName)}$", wnd.compClassName.Text);
                    replacementsDictionary.Add($"${nameof(wnd.compTemplateFileName)}$", wnd.compTemplateFileName.Text);
                    replacementsDictionary.Add($"${nameof(wnd.compClassFileName)}$", wnd.compClassFileName.Text);
                    replacementsDictionary.Add($"${nameof(wnd.compClassFileName)}WithoutExtension$", Path.GetFileNameWithoutExtension(wnd.compClassFileName.Text));
                    replacementsDictionary.Add($"${nameof(wnd.compStyleFileName)}$", wnd.compStyleFileName.Text);
                    replacementsDictionary.Add($"${nameof(wnd.compTestFileName)}$", wnd.compTestFileName.Text);

                    this.files.Add(this.fileNames[ComponentFile.Class], wnd.createCompClassFile.IsChecked.GetValueOrDefault());
                    this.files.Add(this.fileNames[ComponentFile.Template], wnd.createCompTemplateFile.IsChecked.GetValueOrDefault());
                    this.files.Add(this.fileNames[ComponentFile.Style], wnd.createCompStyleFile.IsChecked.GetValueOrDefault());
                    this.files.Add(this.fileNames[ComponentFile.Test], wnd.createCompTestFile.IsChecked.GetValueOrDefault());
                }
            }
            catch (Exception ex)
            {
                success = false;
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (success != true)
            {
                throw new WizardCancelledException();
            }
        }

        private static void GetSelectionData(
            object automationObject,
            ref string targetFolderPath,
            ref string projectFolderPath,
            ref string projectFullName)
        {
            var dte = (automationObject as DTE);
            if (dte.SelectedItems.Count > 0)
            {
                var selectedItem = dte.SelectedItems.Item(1);
                Project currentProject = null;

                if (selectedItem.Project != null)
                {
                    currentProject = selectedItem.Project;
                    projectFullName = currentProject.FullName;
                    projectFolderPath = Path.GetDirectoryName(currentProject.FileName);
                    targetFolderPath = projectFolderPath;
                }
                else if (selectedItem.ProjectItem != null)
                {
                    currentProject = selectedItem.ProjectItem.ContainingProject;
                    projectFullName = currentProject.FullName;
                    projectFolderPath = Path.GetDirectoryName(currentProject.FileName);
                    if (selectedItem.ProjectItem.FileCount > 0)
                    {
                        targetFolderPath = selectedItem.ProjectItem.FileNames[0];
                    }
                }
            }
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return this.files.Keys.Contains(filePath) && this.files[filePath];
        }

        protected virtual void ReadSettings(AngularComponentWizardWindow wnd, string projectFullName)
        {
            var store = this.GetProjectSettingsStore(projectFullName);
            if (store.PropertyExists(this.projectSettingsCollection, nameof(wnd.compStyleLanguage)))
            {
                var styleLanguage = store.GetString(this.projectSettingsCollection, nameof(wnd.compStyleLanguage));
                if (Enum.TryParse<StylesheetLanguage>(styleLanguage, out var result))
                {
                    wnd.compStyleLanguage.SelectedValue = result;
                    wnd.UpdateFields();
                }
            }
            if (store.PropertyExists(this.projectSettingsCollection, nameof(wnd.compSelectorPrefix)))
            {
                wnd.compSelectorPrefix.Text = store.GetString(this.projectSettingsCollection, nameof(wnd.compSelectorPrefix));
            }
            if (store.PropertyExists(this.projectSettingsCollection, nameof(wnd.createCompClassFile)))
            {
                wnd.createCompClassFile.IsChecked = store.GetBoolean(this.projectSettingsCollection, nameof(wnd.createCompClassFile));
            }
            if (store.PropertyExists(this.projectSettingsCollection, nameof(wnd.createCompFolder)))
            {
                wnd.createCompFolder.IsChecked = store.GetBoolean(this.projectSettingsCollection, nameof(wnd.createCompFolder));
            }
            if (store.PropertyExists(this.projectSettingsCollection, nameof(wnd.createCompStyleFile)))
            {
                wnd.createCompStyleFile.IsChecked = store.GetBoolean(this.projectSettingsCollection, nameof(wnd.createCompStyleFile));
            }
            if (store.PropertyExists(this.projectSettingsCollection, nameof(wnd.createCompTemplateFile)))
            {
                wnd.createCompTemplateFile.IsChecked = store.GetBoolean(this.projectSettingsCollection, nameof(wnd.createCompTemplateFile));
            }
            if (store.PropertyExists(this.projectSettingsCollection, nameof(wnd.createCompTestFile)))
            {
                wnd.createCompTestFile.IsChecked = store.GetBoolean(this.projectSettingsCollection, nameof(wnd.createCompTestFile));
            }
        }

        protected virtual void SaveSettings(AngularComponentWizardWindow wnd, string projectFullName)
        {
            var store = this.GetProjectSettingsStore(projectFullName);
            store.SetString(this.projectSettingsCollection, nameof(wnd.compStyleLanguage), $"{wnd.compStyleLanguage.SelectedValue}");
            store.SetString(this.projectSettingsCollection, nameof(wnd.compSelectorPrefix), $"{wnd.compSelectorPrefix.Text}");
            store.SetBoolean(this.projectSettingsCollection, nameof(wnd.createCompClassFile), wnd.createCompClassFile.IsChecked.GetValueOrDefault());
            store.SetBoolean(this.projectSettingsCollection, nameof(wnd.createCompFolder), wnd.createCompFolder.IsChecked.GetValueOrDefault());
            store.SetBoolean(this.projectSettingsCollection, nameof(wnd.createCompStyleFile), wnd.createCompStyleFile.IsChecked.GetValueOrDefault());
            store.SetBoolean(this.projectSettingsCollection, nameof(wnd.createCompTemplateFile), wnd.createCompTemplateFile.IsChecked.GetValueOrDefault());
            store.SetBoolean(this.projectSettingsCollection, nameof(wnd.createCompTestFile), wnd.createCompTestFile.IsChecked.GetValueOrDefault());
        }

        protected WritableSettingsStore GetProjectSettingsStore(string projectFullName)
        {
            this.projectSettingsCollection = $"{rootSettingsCollection}\\{projectFullName.GetHashCode()}";
            SettingsManager settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            var userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            userSettingsStore.CreateCollection(this.projectSettingsCollection);
            return userSettingsStore;
        }
    }
}
