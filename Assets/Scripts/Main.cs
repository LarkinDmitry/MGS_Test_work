using UnityEngine;

public class Main : MonoBehaviour
{
    // ƒл€ разделени€ бизнес логики и UI применен паттерн MVVM.

    // –уководсту€сь принципами чистого кода комментарии оставлены по минимуму,
    // только в необходимых, по моему мнению, местах.

    // ¬ программе реализовано простое логирование (хот€ это не требовалось).

    // UI реализован с использованием Unity UI Toolkit (в соответствии с
    // рекомендаци€ми Unity, т.к. они постепенно планируют уходить от "класического" UI).

    // ¬ цел€х упрощени€ проверки, при запуске программы в корневом каталоге
    // создаетс€ файл "READ_ME!.txt" указывающий путь до "config.txt".

    // ƒл€ работы с видео используетс€ пробна€ верси€ плагина от VLC.

    // класс VideoStreamData не полностью выполн€ет тебовани€ “«, но может
    // работать в качестве "демонстратора". Ќа правильную реализацию, увы, не
    // хватило времени. –анее мне не доводилось работать с потоковыми видео,
    // но € готов ускоренно осваивать ранее не известные мне технологии.

    // ѕрошу дать обратную св€зь касательно моего кода. «аранее благодарю
    // за уделенное врем€.

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