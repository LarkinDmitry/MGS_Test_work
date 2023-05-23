using System;
using System.Net;
using UnityEngine;

public class Model
{
    public Action<bool> ChangedConnectedServerStatus { get; set; }
    public Action<bool> ChangedMusicActiveState { get; set; }
    public Action<bool> ChangedEffectsActiveState { get; set; }
    public Action<float> ChangedVolumeValue { get; set; }
    public Action<IPAddress> ChangedServerAddress { get; set; }
    public Action<int> ChangedPort { get; set; }
    public Action<string> ChangedVideoStreamAddress { get; set; }
    public Action<float> ChangedOdometerValue { get; set; }

    private NetworkData _networkData;
    private LocalData _localData;

    public Model(NetworkData networkData, LocalData localData)
    {
        _localData = localData;
        _localData.SetDataPath(Application.persistentDataPath);

        _networkData = networkData;
        _networkData.CreateConnect(_localData.ServerIP, _localData.ServerPort);

        Initialization();
    }

    private void Initialization()
    {
        _networkData.ChangedOdometerValue += (f) => ChangedOdometerValue?.Invoke(f);
        _networkData.ChangedConnectedServerStatus += (b) => ChangedConnectedServerStatus?.Invoke(b);
    }

    public void UpdateAllValue()
    {
        ChangedMusicActiveState?.Invoke(_localData.MusicCheckBox);
        ChangedEffectsActiveState?.Invoke(_localData.EffectsCheckBox);
        ChangedVolumeValue?.Invoke(_localData.Volume);
        ChangedServerAddress?.Invoke(_localData.ServerIP);
        ChangedPort?.Invoke(_localData.ServerPort);
        ChangedVideoStreamAddress?.Invoke(_localData.VideoAddress);

        ChangedConnectedServerStatus?.Invoke(_networkData.ServerIsConnected);
        ChangedOdometerValue?.Invoke(_networkData.OdometerValue);
    }

    public void SetVolume(float volume)
    {
        _localData.Volume = volume;
        ChangedVolumeValue?.Invoke(_localData.Volume);
    }

    public void SetMusicActiveState(bool state)
    {
        _localData.MusicCheckBox = state;
        ChangedMusicActiveState?.Invoke(_localData.MusicCheckBox);
    }

    public void SetEffectsActiveState(bool state)
    {
        _localData.EffectsCheckBox = state;
        ChangedEffectsActiveState?.Invoke(_localData.EffectsCheckBox);
    }

    public void SetServerAddress(string ip)
    {
        if(IPAddress.TryParse(ip, out IPAddress newIP))
        {
            _localData.ServerIP = newIP;
            _networkData.CreateConnect(_localData.ServerIP, _localData.ServerPort);
        }

        ChangedServerAddress?.Invoke(_localData.ServerIP);
    }

    public void SetServerPort(string port)
    {
        if(int.TryParse(port, out int newPort) && newPort >= 0 && newPort < 9999)
        {
            _localData.ServerPort = newPort;
            _networkData.CreateConnect(_localData.ServerIP, _localData.ServerPort);
        }

        ChangedPort?.Invoke(_localData.ServerPort);
    }

    public void SetVideoStreamAddress(string uri)
    {
        _localData.VideoAddress = uri;
        ChangedVideoStreamAddress?.Invoke(_localData.VideoAddress);
    }
}