using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndaw.Graphics.Controls
{
    public interface IScrollableArea
    {
        Point ViewPosition { get; set; }
        Point MinimumView { get; set; }
        Point MaximumView { get; set; }

        event EventHandler<EventArgs> ViewPositionChange;
        event EventHandler<EventArgs> MinimumViewChange;
        event EventHandler<EventArgs> MaximumViewChange;
    }
}
