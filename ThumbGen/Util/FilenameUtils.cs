using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbGen.Util
{
    internal class FilenameUtils
    {
        public static string ChangeFilename(string filepath, Func<string, string> replaceFilename)
        {
            var dir = Path.GetDirectoryName(filepath) ?? string.Empty;
            var ext = Path.GetExtension(filepath);
            var oldFilename = Path.GetFileNameWithoutExtension(filepath);

            return Path.Combine(dir, replaceFilename(oldFilename) + ext);
        }
    }
}
