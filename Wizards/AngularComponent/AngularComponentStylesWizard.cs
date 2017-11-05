using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TemplateWizard;

namespace AngularWizards.AngularComponent
{
    public abstract class AngularComponentStylesWizard : AngularComponentWizard
    {
        protected override void InitWizard(AngularComponentWizardWindow wnd, object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            base.InitWizard(wnd, automationObject, replacementsDictionary, runKind, customParams);
            wnd.createCompFolder.IsChecked = false;
            wnd.createCompTemplateFile.IsChecked = false;
            wnd.createCompClassFile.IsChecked = false;
            wnd.createCompTestFile.IsChecked = false;
            wnd.UpdateFields();
        }

        protected override void ReadSettings(AngularComponentWizardWindow wnd, string projectFullName)
        {
        }

        protected override void SaveSettings(AngularComponentWizardWindow wnd, string projectFullName)
        {
        }
    }

    public class AngularComponentScssStylesWizard: AngularComponentStylesWizard
    {
        protected override void InitWizard(AngularComponentWizardWindow wnd, object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            wnd.compStyleLanguage.SelectedValue = StylesheetLanguage.SCSS;
            base.InitWizard(wnd, automationObject, replacementsDictionary, runKind, customParams);
        }
    }

    public class AngularComponentCssStylesWizard : AngularComponentStylesWizard
    {
        protected override void InitWizard(AngularComponentWizardWindow wnd, object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            wnd.compStyleLanguage.SelectedValue = StylesheetLanguage.CSS;
            base.InitWizard(wnd, automationObject, replacementsDictionary, runKind, customParams);
        }
    }

    public class AngularComponentLessStylesWizard : AngularComponentStylesWizard
    {
        protected override void InitWizard(AngularComponentWizardWindow wnd, object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            wnd.compStyleLanguage.SelectedValue = StylesheetLanguage.LESS;
            base.InitWizard(wnd, automationObject, replacementsDictionary, runKind, customParams);
        }
    }
}
