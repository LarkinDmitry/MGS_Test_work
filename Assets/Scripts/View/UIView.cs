using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class UIView : MonoBehaviour, IView
{
    private ViewModel _viewModel;
    private VisualElement _root;

    private VisualElement _connectionIndicator;
    private Button _menuBtn;
    private Button _videoBtn;

    private VisualElement _settingsContainer;
    private bool settingContainerActiveState;
    private Toggle _music;
    private Toggle _effects;
    private Slider _volume;
    private TextField _serverAddress;
    private TextField _serverPort;
    private TextField _videoStreamAddress;

    private Label _odometerValue;

    private bool videoContainerActiveState;
    private VisualElement _videoPanel;

    private readonly float openCloseWindowTimeSecond = 0.3f;
    private readonly float odometerAnimationTimeSecond = 3.0f;
    private readonly float oneStepTimeSecond = 0.05f;
    private readonly string odometerFormat = "00000000000.00";

    private void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _connectionIndicator = _root.Q("ServerConnectIndicator");
        _menuBtn = _root.Q<Button>("Menu");
        _videoBtn = _root.Q<Button>("Video");
        _settingsContainer = _root.Q<VisualElement>("SettingContainer");
        _music = _root.Q<Toggle>("Music");
        _effects = _root.Q<Toggle>("Effects");
        _volume = _root.Q<Slider>("Volume");
        _serverAddress = _root.Q<TextField>("ServerAddress");
        _serverPort = _root.Q<TextField>("PortNumber");
        _videoStreamAddress = _root.Q<TextField>("VideoStreamAddress");
        _odometerValue = _root.Q<Label>("OdometerValue");
        _videoPanel = _root.Q("VideoPanel");

        _settingsContainer.style.transitionDuration = new List<TimeValue> { openCloseWindowTimeSecond };

        ChangeSettingsWindowActiveState(false);
        ChangeVideoPanelActiveState(false);
    }

    public void Initialization(ViewModel viewModel)
    {
        _viewModel = viewModel;

        _viewModel.ChangedConnectedServerStatus += ShowServerStatus;
        _viewModel.ChangedMusicActiveState += (b) => _music.value = b;
        _viewModel.ChangedEffectsActiveState += (b) => _effects.value = b;
        _viewModel.ChangedVolumeValue += (f) => _volume.value = f;
        _viewModel.ChangedServerAddress += (s) => _serverAddress.value = s;
        _viewModel.ChangedPort += (s) => _serverPort.value = s;
        _viewModel.ChangedVideoStreamAddress += (s) => _videoStreamAddress.value = s;
        _viewModel.ChangedOdometerValue += (f) => TryOdometerValue = f;

        _viewModel.UpdateAllValue();

        _menuBtn.clicked += () => ChangeSettingsWindowActiveState(!settingContainerActiveState);
        _videoBtn.clicked += () => ChangeVideoPanelActiveState(!videoContainerActiveState);
        _music.RegisterValueChangedCallback((evt) => _viewModel.ChangeMusicState(evt.newValue));
        _effects.RegisterValueChangedCallback((evt) => _viewModel.ChangeEffectsState(evt.newValue));
        _volume.RegisterValueChangedCallback((evt) => _viewModel.ChangeVolume(evt.newValue));        
        _serverAddress.RegisterValueChangedCallback((evt) => _viewModel.ChangeServerAddress(evt.newValue));
        _serverPort.RegisterValueChangedCallback((evt) => _viewModel.ChangeServerPort(evt.newValue));
        _videoStreamAddress.RegisterValueChangedCallback((evt) => _viewModel.ChangeVideoStreamAddress(evt.newValue));

        _menuBtn.clicked += () => _viewModel.OnClick();
        _videoBtn.clicked += () => _viewModel.OnClick();
    }

    private async void ChangeSettingsWindowActiveState(bool state)
    {
        settingContainerActiveState = state;
        _menuBtn.text = state ? "Close menu" : "Open menu";

        _settingsContainer.style.opacity = state ? 1 : 0;
        if(!state)
        {
            await Task.Delay((int)(openCloseWindowTimeSecond * 1000));
        }        
        _settingsContainer.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void ChangeVideoPanelActiveState(bool state)
    {
        _videoBtn.text = state ? "Stop video" : "Start video";

        _videoPanel.style.backgroundColor = state ? Color.blue : Color.black;

        videoContainerActiveState = state;
    }

    private void ShowServerStatus(bool status)
    {
        _connectionIndicator.Q<Label>("Info").text = status ? "Connected" : "Disconnected";
        _connectionIndicator.style.backgroundColor = status ? Color.green : Color.red;
    }

    private float _lastOdometer = 0f;
    private float _odometer = 0f;
    private float TryOdometerValue
    {
        get
        {
            return _odometer;
        }
        set
        {
            _lastOdometer = _odometer;
            _odometer = value;
            UpdateOdometer();
        }
    }

    private async void UpdateOdometer()
    {
        float newOdometer = TryOdometerValue;
        float currentOdometer = _lastOdometer;

        int steps = (int)(odometerAnimationTimeSecond / oneStepTimeSecond);
        float stepDelta = (TryOdometerValue - currentOdometer) / steps;

        for (int i = 0; i < steps; i++)
        {
            if (newOdometer != TryOdometerValue) return; // if odometer value changes before the end of the animation

            currentOdometer += stepDelta;
            _odometerValue.text = currentOdometer.ToString(odometerFormat);
            await Task.Delay((int)(oneStepTimeSecond * 1000));
        }

        _odometerValue.text = TryOdometerValue.ToString(odometerFormat);
    }
}