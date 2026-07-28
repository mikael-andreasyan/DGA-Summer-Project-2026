using Unity.VisualScripting;
using UnityEngine;

public class Background : MonoBehaviour
{
    public GameObject UppestBackground;
    public GameObject UpperBackground;
    public GameObject MainBackground;
    private SpriteRenderer UppestSprite;
    private SpriteRenderer UpperSprite;
    private SpriteRenderer MainSprite;
    private ParallaxBackground Uppest;
    private ParallaxBackground Upper;
    private ParallaxBackground Main;
    public SpriteRenderer transition;
    public SpriteRenderer spaceBackground;
    private float transitionSpacing = 103.5f;

    void Start()
    {
        UppestSprite = UppestBackground.gameObject.GetComponent<SpriteRenderer>();
        UpperSprite = UpperBackground.gameObject.GetComponent<SpriteRenderer>();
        MainSprite = MainBackground.gameObject.GetComponent<SpriteRenderer>();
        Uppest = UppestBackground.gameObject.GetComponent<ParallaxBackground>();
        Upper = UpperBackground.gameObject.GetComponent<ParallaxBackground>();
        Main = MainBackground.gameObject.GetComponent<ParallaxBackground>();
    }

    void Update()
    {
        if (GameManager.Instance.triggerTransition)
        {
            Debug.Log("transition");
            UppestSprite.sprite = spaceBackground.sprite;
            Uppest.spacing = transitionSpacing;
            UpperSprite.sprite = spaceBackground.sprite;
            Upper.spacing = transitionSpacing;
            MainSprite.sprite = spaceBackground.sprite;
            Main.spacing = transitionSpacing;
        }
    }
}
