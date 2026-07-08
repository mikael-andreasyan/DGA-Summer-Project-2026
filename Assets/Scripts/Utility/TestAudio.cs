using UnityEngine;

public class TestMusic : MonoBehaviour
{
    [SerializeField] private AudioClip menuTrack;

    private void Start() =>
        ServiceLocator.Get<AudioManager>().PlayMusic(menuTrack);
}