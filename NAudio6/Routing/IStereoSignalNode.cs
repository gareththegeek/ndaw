using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndaw.Routing
{
    public interface IStereoSignalNode: ISignalNode
    {
        ISignalSink LeftIn { get; }
        ISignalSink RightIn { get; }

        ISignalSource LeftOut { get; }
        ISignalSource RightOut { get; }
    }
}
