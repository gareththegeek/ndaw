using NAudio.Wave;
using System;

namespace ndaw.Core.Routing
{
    public class RoutingEventArgs : EventArgs
    {
        public int Index { get; set; }
        public WaveFormat Format { get; set; }
        public int Count { get; set; }
        public float[] Buffer { get; set; }
    }
}
