﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ndaw.Core.Routing
{
    public interface ISignalNode : INamed
    {
        IEnumerable<ISignalSource> Sources { get; }
        IEnumerable<ISignalSink> Sinks { get; }
    }
}
