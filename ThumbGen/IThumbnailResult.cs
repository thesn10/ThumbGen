using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbGen
{
    public interface IThumbnailResult
    {
        void SaveToFile(string filePath);
    }
}
