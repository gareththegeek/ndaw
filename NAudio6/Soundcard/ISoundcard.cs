using ndaw.Routing;
using System.Collections.Generic;

namespace ndaw.Soundcard
{
    public interface ISoundcard
    {
        IEnumerable<ISignalSource> Inputs { get; }
        IEnumerable<ISignalSink> Outputs { get; }

        void Start();
        void Stop();
    }
}
