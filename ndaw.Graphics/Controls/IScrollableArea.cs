using SharpDX;
using System;

namespace ndaw.Graphics.Controls
{
    public interface IScrollableArea
    {
        Point ViewPosition { get; set; }
        Point MinimumView { get; set; }
        Point MaximumView { get; set; }
        float Zoom { get; set; }

        event EventHandler<EventArgs> ViewPositionChange;
        event EventHandler<EventArgs> MinimumViewChange;
        event EventHandler<EventArgs> MaximumViewChange;
        event EventHandler<EventArgs> ZoomChange;

        IDraggable BeginDragAt(int x, int y);
        void DragComplete(IDraggable draggable);
    }
}
