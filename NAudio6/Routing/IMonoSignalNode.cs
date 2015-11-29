using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndaw.Routing
{
    public interface IMonoSignalNode: ISignalNode
    {
        ISignalSink CentreIn { get; }
        ISignalSource CentreOut { get; }
    }
}
