using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

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

    public void PlaySFX(AudioClip clip, float volume = 1f, bool randomizePitch = true)
    {
        if (clip == null) return;

        sfxSource.pitch = randomizePitch ? Random.Range(0.95f, 1.05f) : 1f;
        sfxSource.PlayOneShot(clip, volume);
    }
}