using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AngularWizards.Services
{
    public class NameService
    {
        public IEnumerable<string> SplitName(string name)
        {
            return Regex
                .Split(Regex.Replace(name, @"[^\w\-\.]", ""), @"((?:\.|-|[A-Z])+[^\.\-A-Z]*)")
                .Select(part => part.Trim('.', '-').ToLower())
                .Where(part => !string.IsNullOrEmpty(part));
        }

        public string ToPascalCase(IEnumerable<string> nameParts)
        {
            return string.Join("",
                nameParts.Select(part => part.Substring(0, 1).ToUpper() + part.Substring(1)));
        }

        public string ToPascalCase(string name)
        {
            return ToPascalCase(SplitName(name));
        }
    }
}
