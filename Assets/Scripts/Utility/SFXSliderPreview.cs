using UnityEngine;
using UnityEngine.EventSystems;

public class SFXSliderPreview : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] private AudioClip testClip;

    public void OnPointerUp(PointerEventData eventData) =>
        ServiceLocator.Get<AudioManager>().PlaySFX(testClip);
}