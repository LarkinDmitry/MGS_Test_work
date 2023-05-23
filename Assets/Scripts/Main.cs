using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject Audio;

    private IView _uiView;
    private IView _audioView;
    private Model _model;
    private ViewModel _viewModel;

    void Start()
    {
        Logger.SetLogPath(Application.persistentDataPath);

        NetworkData nwData = GetComponent<NetworkData>();
        LocalData lcData = GetComponent<LocalData>();
        VideoStreamData vsData = GetComponent<VideoStreamData>();

        _model = new(nwData, lcData, vsData);

        _viewModel = new(_model);

        _audioView = Audio.GetComponent<IView>();
        _audioView.Initialization(_viewModel);

        _uiView = UI.GetComponent<IView>();
        _uiView.Initialization(_viewModel);        
    }
}