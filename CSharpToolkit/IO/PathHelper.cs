using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace CSharpToolkit.IO
{
    static class PathHelper
    {
        [DllImport("shlwapi.dll")]
        static extern bool PathRelativePathTo(
                 [Out] StringBuilder pszPath,
                 [In] string pszFrom,
                 [In] FileAttributes dwAttrFrom,
                 [In] string pszTo,
                 [In] FileAttributes dwAttrTo);

        public static bool GetRelativePath(string from, 
            bool isFromDir, 
            string to, 
            bool isToDir,
            out string relPath)
        {
            var sb = new StringBuilder(256);
            var fromAttr = isFromDir ? FileAttributes.Directory : FileAttributes.Normal;
            var toAttr = isToDir ? FileAttributes.Directory : FileAttributes.Normal;

            var result = PathRelativePathTo(sb,
                from,
                fromAttr,
                to,
                toAttr);

            relPath = sb.ToString();

            return result;
        }
    }
}
