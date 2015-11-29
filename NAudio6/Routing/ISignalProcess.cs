using NAudio.Wave;

namespace ndaw.Routing
{
    public interface ISignalProcess
    {
        WaveFormat Format { get; set; }

        void Process(float[][] buffers, int count);
    }
}
