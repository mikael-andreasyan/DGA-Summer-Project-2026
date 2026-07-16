using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    private const string MusicParam = "MusicVolume";
    private const string SFXParam = "SFXVolume";

    private void Start()
    {
        float music = PlayerPrefs.GetFloat(MusicParam, 1f);
        float sfx = PlayerPrefs.GetFloat(SFXParam, 1f);

        musicSlider.SetValueWithoutNotify(music);
        sfxSlider.SetValueWithoutNotify(sfx);
        ApplyVolume(MusicParam, music);
        ApplyVolume(SFXParam, sfx);

        musicSlider.onValueChanged.AddListener(v => SetVolume(MusicParam, v));
        sfxSlider.onValueChanged.AddListener(v => SetVolume(SFXParam, v));
    }

    private void SetVolume(string param, float sliderValue)
    {
        ApplyVolume(param, sliderValue);
        PlayerPrefs.SetFloat(param, sliderValue);
    }

    private void ApplyVolume(string param, float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f;
        mixer.SetFloat(param, dB);
    }
}