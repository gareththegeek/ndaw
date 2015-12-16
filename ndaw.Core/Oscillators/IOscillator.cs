using ndaw.Core.Routing;

namespace ndaw.Core.Oscillators
{
    public interface IOscillator: ISignalProcess
    {
        float Frequency { get; set; }
        float Amplitude { get; set; }
        //float Generate(int time);
        int Time { get; set; }
    }
}
