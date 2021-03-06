﻿using System;
using System.Collections.Generic;

namespace ndaw.Core.Routing
{
    public interface ISignalSource: INamed
    {
        ISignalNode Owner { get; }

        bool IsMapped { get; }

        /// <summary>
        /// Do not modify this collection directly, instead set the Source 
        /// property of the corresponding ISignalSink
        /// </summary>
        IEnumerable<ISignalSink> Sinks { get; }

        /// <summary>
        /// Do not call this directly, instead set the Source property of the 
        /// corresponding ISignalSink
        /// </summary>
        void AddSink(ISignalSink sink);

        /// <summary>
        /// Do not call this directly, instead set the Source property of the 
        /// corresponding ISignalSink
        /// </summary>
        void RemoveSink(ISignalSink sink);

        void RaiseBufferReady(RoutingEventArgs e);
    }
}
