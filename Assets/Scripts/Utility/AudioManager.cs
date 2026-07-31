using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip powerupPickupSound;
    [SerializeField] private AudioClip pauseSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private int sfxPoolSize = 5;

    private AudioSource[] sfxSources;
    private int nextSfxIndex;

    private static AudioManager instance;
    private Coroutine musicRoutine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        ServiceLocator.Register(this);
        DontDestroyOnLoad(gameObject);

        musicSource.loop = true;

        sfxSources = new AudioSource[sfxPoolSize];
        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = sfxMixerGroup;
            sfxSources[i] = source;
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
            ServiceLocator.Unregister(this);
    }

    public void PlayMusic(AudioClip clip, float volume = 1, float fadeTime = 0.5f)
    {
        if (musicSource.clip == clip) return;

        if (musicRoutine != null) StopCoroutine(musicRoutine);

        musicRoutine = StartCoroutine(PlayMusicRoutine(clip, volume, fadeTime));
    }

    private IEnumerator PlayMusicRoutine(AudioClip clip, float volume, float fadeTime)
    {
        yield return Fade(0, fadeTime);
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.Play();

        yield return Fade(volume, fadeTime);
    }

    private IEnumerator Fade(float endValue, float duration)
    {
        float startValue = musicSource.volume;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startValue, endValue, time / duration);
            yield return null;
        }

        musicSource.volume = endValue;
    }

    private AudioSource GetAvailableSfxSource()
    {
        foreach (var source in sfxSources)
        {
            if (!source.isPlaying) return source;
        }

        // all busy — steal in rotation rather than always cutting off the same one
        AudioSource oldest = sfxSources[nextSfxIndex];
        nextSfxIndex = (nextSfxIndex + 1) % sfxSources.Length;
        return oldest;
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, bool randomizePitch = true)
    {
        if (clip == null) return;

        AudioSource source = GetAvailableSfxSource();
        source.pitch = randomizePitch ? Random.Range(0.95f, 1.05f) : 1f;
        source.PlayOneShot(clip, volume);
    }

    public void PlayPowerupPickup()
    {
        PlaySFX(powerupPickupSound, 1f, false);
    }

    public void PlayPause()
    {
        PlaySFX(pauseSound, 1f, false);
    }

    public void PlayLose()
    {
        PlaySFX(loseSound, 1f, false);
    }

    public void StopAll()
    {
        if (musicRoutine != null)
        {
            StopCoroutine(musicRoutine);
            musicRoutine = null;
        }

        musicSource.Stop();
        foreach (var source in sfxSources)
        {
            source.Stop();
        }
    }
}