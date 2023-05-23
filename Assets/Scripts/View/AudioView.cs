using UnityEngine;
using UnityEngine.Audio;

public class AudioView : MonoBehaviour, IView
{
    [SerializeField] private AudioMixer Mixer;
    [SerializeField] private AudioMixerGroup Music;
    [SerializeField] private AudioMixerGroup Effects;

    private ViewModel _viewModel;

    private AudioSource tap;
    private AudioSource backgroundMusic;

    private void Awake()
    {
        tap = gameObject.AddComponent<AudioSource>();
        tap.loop = false;
        tap.outputAudioMixerGroup = Effects;
        tap.clip = Resources.Load<AudioClip>("Audio/pop");

        backgroundMusic = gameObject.AddComponent<AudioSource>();
        backgroundMusic.loop = true;
        backgroundMusic.outputAudioMixerGroup = Music;
        backgroundMusic.clip = Resources.Load<AudioClip>("Audio/background_music");

        backgroundMusic.Play();
    }

    public void Initialization(ViewModel viewModel)
    {
        _viewModel = viewModel;

        _viewModel.ChangedVolumeValue += SetVolume;
        _viewModel.ChangedMusicActiveState += SetMusicActiveState;
        _viewModel.ChangedEffectsActiveState += SetEffectsActiveState;
        _viewModel.Click += Click;

        _viewModel.UpdateAllValue();
    }

    private void Click()
    {
        tap.Play();
    }

    private void SetVolume(float volume)
    {
        Mixer.SetFloat("Master", ConvertToDecibel(volume));
    }

    private void SetMusicActiveState(bool active)
    {
        Mixer.SetFloat("Music", ConvertToDecibel(active ? 1 : 0));
    }

    private void SetEffectsActiveState(bool active)
    {
        Mixer.SetFloat("Effects", ConvertToDecibel(active ? 1 : 0));
    }


    /// <summary>
    /// convert the volume value 0...1 to -80...0 dB
    /// </summary>
    /// <returns></returns>
    private float ConvertToDecibel(float value)
    {
        float vol = Mathf.Clamp(value, 0.0001f, 1);
        return Mathf.Log10(vol) * 20;
    }
}