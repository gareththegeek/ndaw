using ndaw.Core.Routing;
using System.Collections.Generic;

namespace ndaw.Core.Soundcard
{
    public interface ISoundcard
    {
        void Start();
        void Stop();

        ISignalNode Inputs { get; }
        ISignalNode Outputs { get; }
    }
}
