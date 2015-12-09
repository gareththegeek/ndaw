using NAudio.Dsp;

namespace ndaw.Core.Fourier
{
    public interface IFastFourierTransformProvider
    {
        void FFT(bool forward, int m, Complex[] data);
        double BlackmannHarrisWindow(int n, int frameSize);
    }
}
