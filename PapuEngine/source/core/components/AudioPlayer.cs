using NAudio.Wave;

public class AudioPlayer
{
    private readonly AudioFileReader _reader;
    private readonly WaveOutEvent _output;
    private readonly bool _loop;
    private bool _stopping = false;

    public float Volume
    {
        get => _reader.Volume;
        set => _reader.Volume = value;
    }

    public AudioPlayer(string filePath, bool loop = false, float volume = 1.0f)
    {
        _reader = new AudioFileReader(filePath);
        _reader.Volume = volume;
        _loop = loop;

        _output = new WaveOutEvent();
        _output.Init(_reader);

        if (_loop)
        {
            _output.PlaybackStopped += (s, e) =>
            {
                if (_stopping) return; // no reinicies si fue Stop()
                _reader.Position = 0;
                _output.Play();
            };
        }
    }

    public void Play()
    {
        _stopping = false;
        _output.Play();
    }

    public void Stop()
    {
        _stopping = true;
        _output.Stop();
    }

    public void Dispose()
    {
        _output.Dispose();
        _reader.Dispose();
    }
}