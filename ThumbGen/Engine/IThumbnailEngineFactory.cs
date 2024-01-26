using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbGen.Engine
{
    public interface IThumbnailEngineFactory
    {
        IThumbnailEngine CreateNew();
    }
}
