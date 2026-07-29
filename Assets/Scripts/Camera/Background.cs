using System.Linq;
using UnityEngine;

public class Background : MonoBehaviour
{
    public GameObject UppestBackground;
    public GameObject UpperBackground;
    public GameObject MainBackground;
    private SpriteRenderer UppestSprite;
    private SpriteRenderer UpperSprite;
    private SpriteRenderer MainSprite;
    public GameObject[] backgrounds;
    private ParallaxBackground Uppest;
    private ParallaxBackground Upper;
    private ParallaxBackground Main;
    public SpriteRenderer transition;
    public SpriteRenderer spaceBackground;
    private GameObject transitionedSp;
    private int transitioned;

    void Start()
    {
        UppestSprite = UppestBackground.gameObject.GetComponent<SpriteRenderer>();
        UpperSprite = UpperBackground.gameObject.GetComponent<SpriteRenderer>();
        MainSprite = MainBackground.gameObject.GetComponent<SpriteRenderer>();
        Uppest = UppestBackground.gameObject.GetComponent<ParallaxBackground>();
        Upper = UpperBackground.gameObject.GetComponent<ParallaxBackground>();
        Main = MainBackground.gameObject.GetComponent<ParallaxBackground>();
        transitioned = 0;
    }

    void Update()
    {
        if (transitioned==1)
        {
            GameObject highest = backgrounds
            .OrderByDescending(bg => bg.transform.position.y)
            .First();
            if (highest!=transitionedSp)
            {
                SpriteRenderer transitionedSprite = highest.GetComponent<SpriteRenderer>();
            transitionedSprite.sprite = spaceBackground.sprite;
            transitioned = 2;
            }
            
        }
        if (transitioned==0 && GameManager.Instance.triggerTransition)
        {
            // Transition sprite first
            transitionedSp = backgrounds
            .OrderByDescending(bg => bg.transform.position.y)
            .First();

            SpriteRenderer transitionedSprite = transitionedSp.GetComponent<SpriteRenderer>();
            transitionedSprite.sprite = transition.sprite;
            transitioned = 1;
        }
    }
}
