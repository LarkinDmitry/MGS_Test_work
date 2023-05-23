using LibVLCSharp;
using System;
using UnityEngine;

public class VideoStreamData : MonoBehaviour
{
    private Texture2D _videoTexture;
    private Texture2D _plugTexture;
    private Texture2D _texture;
    public Texture2D CurrentTexture
    { 
        get
        { 
            return _texture; 
        } 
        private set 
        { 
            _texture = value;
            ChangeVideoFrame?.Invoke(_texture);
        } 
    }
    public Action<Texture2D> ChangeVideoFrame { get; set; }
    public Action<bool> MuteMusic { get; set; }

    LibVLC _libVLC;
    MediaPlayer _mediaPlayer;
    bool _playing = false;

    void Awake()
    {
        Core.Initialize(Application.dataPath);
        _libVLC = new LibVLC(enableDebugLogs: true);
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);

        _mediaPlayer = new MediaPlayer(_libVLC)
        {
            Media = new Media(new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"))
        };

        _plugTexture = Resources.Load<Texture2D>("Image/plug");
        CurrentTexture = _plugTexture;
    }

    public void Play()
    {
        _playing = true;
        _mediaPlayer.Play();
        CurrentTexture = _videoTexture;
        MuteMusic?.Invoke(true);
    }

    public void Pause()
    {
        _playing = false;
        _mediaPlayer.Pause();
        CurrentTexture = _plugTexture;
        MuteMusic?.Invoke(false);
    }

    void Update()
    {
        if (!_playing) return;

        if (_videoTexture == null)
        {
            uint i_videoHeight = 0;
            uint i_videoWidth = 0;

            _mediaPlayer.Size(0, ref i_videoWidth, ref i_videoHeight);
            var texptr = _mediaPlayer.GetTexture(i_videoWidth, i_videoHeight, out bool updated);
            if (i_videoWidth != 0 && i_videoHeight != 0 && updated && texptr != IntPtr.Zero)
            {
                _videoTexture = Texture2D.CreateExternalTexture((int)i_videoWidth,
                    (int)i_videoHeight,
                    TextureFormat.RGBA32,
                    false,
                    true,
                    texptr);

                CurrentTexture = _videoTexture;
            }
        }
        else if (_videoTexture != null)
        {
            var texptr = _mediaPlayer.GetTexture((uint)_videoTexture.width, (uint)_videoTexture.height, out bool updated);
            if (updated)
            {
                _videoTexture.UpdateExternalTexture(texptr);
            }
        }
    }

    void OnDisable()
    {
        _mediaPlayer?.Stop();
        _mediaPlayer?.Dispose();
        _mediaPlayer = null;

        _libVLC?.Dispose();
        _libVLC = null;
    }
}
