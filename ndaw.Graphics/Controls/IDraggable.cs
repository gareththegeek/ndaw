using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndaw.Graphics.Controls
{
    public interface IDraggable
    {
        int X { get; set; }
        int Y { get; set; }
        int Width { get; }
        int Height { get; }
    }
}
