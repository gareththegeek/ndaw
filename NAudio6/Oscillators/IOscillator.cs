using ndaw.Routing;

namespace ndaw.Oscillators
{
    public interface IOscillator: ISignalProcess
    {
        float Frequency { get; set; }
        float Amplitude { get; set; }
    }
}
