using UnityEngine;
using NativeWebSocket;
using System.Net;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections;

public class NetworkData : MonoBehaviour
{
    public Action<float> ChangedOdometerValue { get; set; }
    public Action<bool> ChangedConnectedServerStatus { get; set; }

    private WebSocket _websocket;
    private IPAddress _currentIP;
    private int _currentPort;

    private readonly int microsecondsToReconnect = 5000;
    private readonly float _maxIdleTime = 10;
    private float _lastUpdateTime;
        
    private bool _isConnected;
    public bool ServerIsConnected
    {
        get
        {
            return _isConnected;
        }

        private set
        {
            _lastUpdateTime = 0;

            if (_isConnected != value)
            {
                _isConnected = value;
                ChangedConnectedServerStatus?.Invoke(value);                
            }
        }
    }

    private float _odometerValue;
    public float OdometerValue
    {
        get
        {
            return _odometerValue;
        }

        private set
        {
            _lastUpdateTime = 0;

            if (_odometerValue != value)
            {                
                _odometerValue = value;
                ChangedOdometerValue?.Invoke(value);                
            }
        }
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        _websocket?.DispatchMessageQueue();
#endif

        _lastUpdateTime += Time.unscaledDeltaTime;

        if (_lastUpdateTime > _maxIdleTime)
        {
            OnError("timeout exceeded");
        }
    }

    private void OnApplicationQuit()
    {
        _websocket.Close();
    }

    public void CreateConnect(IPAddress webSocketServerIPAddress, int port)
    {
        if(_websocket != null) _websocket.Close();

        _currentIP = webSocketServerIPAddress;
        _currentPort = port;        


        _websocket = new WebSocket($"ws://{_currentIP}:{_currentPort:0000}/ws");

        _websocket.OnOpen += OnOpen;
        _websocket.OnError += OnError;
        _websocket.OnClose += OnClose;
        _websocket.OnMessage += OnMessage;

        _websocket.Connect();
    }    

    private void OnOpen()
    {
        ServerIsConnected = true;
        Logger.WriteLog($"websocket open");        

        Dictionary<string, string> request = new()
        {
            { "operation", "getCurrentOdometer" }
        };

        _websocket.SendText(JsonConvert.SerializeObject(request));

        StartCoroutine(CheckConnect(5));        
    }

    private void OnError(string e)
    {
        Logger.WriteLog($"websocket error - {e}");
        ServerIsConnected = false;

        StartCoroutine(ReConnect(microsecondsToReconnect));
    }

    private void OnClose(WebSocketCloseCode closeCode)
    {
        Logger.WriteLog($"websocket close - {closeCode}");
        ServerIsConnected = false;
    }

    private void OnMessage(byte[] bytes)
    {
        ServerIsConnected = true;

        var json = System.Text.Encoding.UTF8.GetString(bytes);
        var message = JsonConvert.DeserializeObject<ServerMessage>(json);
        if (message.odometer != null) OdometerValue = message.odometer.Value;
        if (message.value != null) OdometerValue = message.value.Value;
        if (message.status != null) ServerIsConnected = true;
    }

    IEnumerator CheckConnect(float timeOut)
    {
        while(ServerIsConnected)
        {
            Dictionary<string, string> request = new()
            {
                { "operation", "getRandomStatus" }
            };

            _websocket.SendText(JsonConvert.SerializeObject(request));
            yield return new WaitForSeconds(timeOut);
        }
    }

    IEnumerator ReConnect(float timeOut)
    {
        yield return new WaitForSeconds(timeOut);
        Logger.WriteLog($"reconnect attempt");
        CreateConnect(_currentIP, _currentPort);
    }

    private struct ServerMessage
    {
        public bool? status;
        public float? odometer;
        public float? value;
    }
}