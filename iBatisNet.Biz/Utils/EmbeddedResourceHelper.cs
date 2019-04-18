using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace iBatisNet.Biz.Utils
{
    public class EmbeddedResourceHelper
    {
        public static string[] GetAllQueryXML(string directory)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var namespaceName = executingAssembly.GetName().Name;
            string folderName = string.Format("{0}.{1}", namespaceName, directory);
            return executingAssembly
                .GetManifestResourceNames()
                .Where(r => r.StartsWith(folderName) && r.EndsWith(".xml"))
                .Select(r => r.Substring(namespaceName.Length + 1))
                .ToArray();
        }
    }
}
