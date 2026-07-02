using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMusic()
    {
        audioSource.PlayOneShot(clip);
    }
}
