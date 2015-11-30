
namespace ndaw.Core.Routing
{
    public interface ISignalNode
    {
        bool Bypass { get; set; }

        ISignalProcess SignalProcess { get; }
    }
}
