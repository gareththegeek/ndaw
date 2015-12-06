using ndaw.Core.Routing;

namespace ndaw.Graphics.Controls
{
    public class SignalSinkViewModel: IDraggable, INamed
    {
        public ISignalSink Sink { get; set; }
        public bool Offset { get { return true; } }

        public string Name
        { 
            get
            {
                if (Sink == null) return string.Empty;

                return Sink.Name; 
            }
            set { } 
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
