using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    [Header("Master")]
    [SerializeField] private Slider masterSlider;

    [Header("Music")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Toggle musicMuteToggle;

    [Header("SFX")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle sfxMuteToggle;

    private const string MasterParam = "MasterVolume";
    private const string MusicParam = "MusicVolume";
    private const string SFXParam = "SFXVolume";

    private const string MusicMuteKey = "MusicMuted";
    private const string SFXMuteKey = "SFXMuted";

    private void Start()
    {
        // Master has no mute toggle 
        Bind(MasterParam, null, masterSlider, null);
        Bind(MusicParam, MusicMuteKey, musicSlider, musicMuteToggle);
        Bind(SFXParam, SFXMuteKey, sfxSlider, sfxMuteToggle);
    }

    /// <summary>
    /// Restores one channel's saved settings, pushes them to the mixer, then
    /// wires the widgets up to keep doing both. 
    /// </summary>
    private void Bind(string param, string muteKey, Slider slider, Toggle muteToggle)
    {
        if (slider != null)
        {
            // WithoutNotify so seeding from saved settings doesn't fire the listener and re-save
            slider.SetValueWithoutNotify(PlayerPrefs.GetFloat(param, 1f));
        }

        if (muteToggle != null)
        {
            muteToggle.isOn = PlayerPrefs.GetInt(muteKey, 0) == 1;
        }

        Apply(param, slider, muteToggle);

        if (slider != null)
        {
            slider.onValueChanged.AddListener(value =>
            {
                PlayerPrefs.SetFloat(param, value);
                Apply(param, slider, muteToggle);
            });
        }

        if (muteToggle != null)
        {
            muteToggle.onValueChanged.AddListener(muted =>
            {
                PlayerPrefs.SetInt(muteKey, muted ? 1 : 0);
                Apply(param, slider, muteToggle);
            });
        }
    }

    /// <summary>
    /// Writes the channel's level to the mixer. Mute wins over the slider but
    /// doesn't overwrite it, so unmuting returns to whatever level was set
    /// </summary>
    private void Apply(string param, Slider slider, Toggle muteToggle)
    {
        bool muted = muteToggle != null && muteToggle.isOn;
        float level = muted ? 0f : (slider != null ? slider.value : 1f);

        // Log10 of the 0.0001f floor lands on exactly -80dB so makes it silent
        float dB = Mathf.Log10(Mathf.Max(level, 0.0001f)) * 20f;
        mixer.SetFloat(param, dB);
    }
}
