using System;
using UnityEngine;

public class ViewModel
{
    public Action<bool> ChangedConnectedServerStatus { get; set; }
    public Action<bool> ChangedMusicActiveState { get; set; }
    public Action<bool> ChangedEffectsActiveState { get; set; }
    public Action<float> ChangedVolumeValue { get; set; }
    public Action<string> ChangedServerAddress { get; set; }
    public Action<string> ChangedPort { get; set; }
    public Action<string> ChangedVideoStreamAddress { get; set; }
    public Action<float> ChangedOdometerValue { get; set; }
    public Action<Texture2D> ChangeVideoFrame { get; set; }
    public Action<bool> MuteMusic { get; set; }
    public Action Click { get; set; }

    private Model _model;

    public ViewModel(Model model)
    {
        _model = model;

        _model.ChangedConnectedServerStatus += (b) => ChangedConnectedServerStatus?.Invoke(b);
        _model.ChangedMusicActiveState += (b) => ChangedMusicActiveState?.Invoke(b);
        _model.ChangedEffectsActiveState += (b) => ChangedEffectsActiveState?.Invoke(b);
        _model.ChangedVolumeValue += (f) => ChangedVolumeValue?.Invoke(f);

        _model.ChangedServerAddress += (ip) => ChangedServerAddress?.Invoke(ip.ToString());
        _model.ChangedPort += (i) => ChangedPort?.Invoke(i.ToString("0000"));
        _model.ChangedVideoStreamAddress += (s) => ChangedVideoStreamAddress?.Invoke(s);
        _model.ChangedOdometerValue += (f) => ChangedOdometerValue?.Invoke(f);

        _model.ChangeVideoFrame += (t) => ChangeVideoFrame?.Invoke(t);
        _model.MuteMusic += (b) => MuteMusic?.Invoke(b);
    }

    public void UpdateAllValue()
    {
        _model.UpdateAllValue();
    }

    public void ChangeMusicState(bool state)
    {
        _model.SetMusicActiveState(state);
        OnClick();
    }

    public void ChangeEffectsState(bool state)
    {
        _model.SetEffectsActiveState(state);
        OnClick();
    }

    public void ChangeVolume(float value)
    {
        _model.SetVolume(value);
        OnClick();
    }

    public void ChangeServerAddress(string ip)
    {
        _model.SetServerAddress(ip);
        OnClick();
    }

    public void ChangeServerPort(string port)
    {
        _model.SetServerPort(port);
        OnClick();
    }

    public void ChangeVideoStreamAddress(string uri)
    {
        _model.SetVideoStreamAddress(uri);
        OnClick();
    }

    public void ChangeStreamVideoState(bool state)
    {
        _model.ChangeStreamVideoState(state);
        OnClick();
    }

    public void OnClick()
    {
        Click?.Invoke();
    }
}