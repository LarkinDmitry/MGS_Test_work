using System.IO;
using Newtonsoft.Json;
using System.Net;
using UnityEngine;

public class LocalData : MonoBehaviour
{
    private string _configPath;
    private PlayerConfig config;

    public bool MusicCheckBox
    {
        get
        {
            return config.musicCheckBox;
        }
        set
        {
            config.musicCheckBox = value;
            SaveConfig(config);
        }
    }

    public bool EffectsCheckBox
    {
        get
        {
            return config.effectsCheckBox;
        }
        set
        {
            config.effectsCheckBox = value;
            SaveConfig(config);
        }
    }

    public float Volume
    {
        get
        {
            return config.volume;
        }
        set
        {
            config.volume = value;
            SaveConfig(config);
        }
    }

    public IPAddress ServerIP
    {
        get
        {
            return IPAddress.Parse(config.serverIP);
        }
        set
        {
            config.serverIP = value.ToString();
            SaveConfig(config);
        }
    }

    public int ServerPort
    {
        get
        {
            return config.serverPort;
        }
        set
        {
            config.serverPort = value;
            SaveConfig(config);
        }
    }

    public string VideoAddress
    {
        get
        {
            return config.videoAddress;
        }
        set
        {
            config.videoAddress = value;
            SaveConfig(config);
        }
    }

    public void SetDataPath(string dataPath)
    {
        _configPath = Path.Combine(dataPath, "config.txt");

        if (!File.Exists("READ_ME!.txt"))
        {
            string message = $"path to \"config.txt\"\n{_configPath}";
            File.WriteAllText("READ_ME!.txt", message);
        }

        if (!File.Exists(_configPath))
        {
            PlayerConfig defaultConfig = new()
            {
                musicCheckBox = true,
                effectsCheckBox = true,
                volume = 1,
                serverIP = "185.246.65.199",
                serverPort = 9090,
                videoAddress = "videoAddress"
            };

            SaveConfig(defaultConfig);
        }

        string loadConfig = File.ReadAllText(_configPath);
        config = JsonConvert.DeserializeObject<PlayerConfig>(loadConfig);
    }

    private void SaveConfig(PlayerConfig config)
    {
        string configJson = JsonConvert.SerializeObject(config, Formatting.Indented);
        File.WriteAllText(_configPath, configJson);
    }

    private struct PlayerConfig
    {
        public bool musicCheckBox;
        public bool effectsCheckBox;
        public float volume;
        public string serverIP;
        public int serverPort;
        public string videoAddress;
    }
}