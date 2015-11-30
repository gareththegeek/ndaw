using ndaw.Core.Routing;

namespace ndaw.Core.Oscillators
{
    public interface IOscillator: ISignalProcess
    {
        float Frequency { get; set; }
        float Amplitude { get; set; }
    }
}
