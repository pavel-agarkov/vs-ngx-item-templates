using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TemplateWizard;

namespace AngularWizards.AngularComponent
{
    public class AngularComponentSpecWizard : AngularComponentWizard
    {
        protected override void InitWizard(AngularComponentWizardWindow wnd, object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            base.InitWizard(wnd, automationObject, replacementsDictionary, runKind, customParams);
            wnd.createCompFolder.IsChecked = false;
            wnd.createCompStyleFile.IsChecked = false;
            wnd.createCompClassFile.IsChecked = false;
            wnd.createCompTemplateFile.IsChecked = false;
        }

        protected override void ReadSettings(AngularComponentWizardWindow wnd, string projectFullName)
        {
        }

        protected override void SaveSettings(AngularComponentWizardWindow wnd, string projectFullName)
        {
        }
    }
}
