using ndaw.Core.Routing;
using System.Collections.Generic;

namespace ndaw.Core.Soundcard
{
    public interface ISoundcard: ISignalNode
    {
        void Start();
        void Stop();
    }
}
