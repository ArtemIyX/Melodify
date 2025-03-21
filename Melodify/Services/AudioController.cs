namespace Melodify.Services;

public class AudioController(WebView videoWebView)
{
    private readonly WebView _videoWebView = videoWebView;

    private static readonly string _elementName = "media";

    /// <summary>
    /// Sets the volume of the video (0.0 to 1.0).
    /// </summary>
    /// <param name="volume">Volume level between 0.0 (mute) and 1.0 (max).</param>
    public void SetVolume(double volume)
    {
        if (volume < 0.0 || volume > 1.0)
            throw new ArgumentOutOfRangeException(nameof(volume), "Volume must be between 0.0 and 1.0");

        string js = $"document.getElementsByName('{_elementName}')[0].volume = {volume};";
        _videoWebView.Eval(js);
    }

    /// <summary>
    /// Plays the video.
    /// </summary>
    public void Play()
    {
        string js = $"document.getElementsByName('{_elementName}')[0].play();";
        _videoWebView.Eval(js);
    }

    /// <summary>
    /// Pauses the video.
    /// </summary>
    public void Pause()
    {
        string js = $"document.getElementsByName('{_elementName}')[0].pause();";
        _videoWebView.Eval(js);
    }

    /// <summary>
    /// Sets the current playback time of the video in seconds.
    /// </summary>
    /// <param name="seconds">Time in seconds to set.</param>
    public void SetTime(double seconds)
    {
        if (seconds < 0)
            throw new ArgumentOutOfRangeException(nameof(seconds), "Time cannot be negative");

        string js = $"document.getElementsByName('{_elementName}')[0].currentTime = {seconds};";
        _videoWebView.Eval(js);
    }

}
