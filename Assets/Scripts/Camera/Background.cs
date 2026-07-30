using System.Linq;
using UnityEngine;

public class Background : MonoBehaviour
{
    public GameObject[] backgrounds;
    public SpriteRenderer transitionBackground;
    public SpriteRenderer spaceBackground;
    private GameObject justTransitioned;
    private int transitionStage;

    void Start()
    {
        transitionStage = 0;
    }

    void Update()
    {
        if (transitionStage==1)
        {
            GameObject highest = backgrounds
            .OrderByDescending(bg => bg.transform.position.y)
            .First();
            if (highest!=justTransitioned)
            {
                // First space
                SpriteRenderer transitionedSprite = highest.GetComponent<SpriteRenderer>();
                transitionedSprite.sprite = spaceBackground.sprite;
                justTransitioned = highest.gameObject;
                transitionStage = 2;
            }
            
        } else if (transitionStage==2)
        {
            GameObject highest = backgrounds
            .OrderByDescending(bg => bg.transform.position.y)
            .First();
            if (highest!=justTransitioned)
            {
                // Second space
                SpriteRenderer transitionedSprite = highest.GetComponent<SpriteRenderer>();
                transitionedSprite.sprite = spaceBackground.sprite;
                justTransitioned = highest.gameObject;
                transitionStage = 3;
            }
        } else if (transitionStage==3)
        {
            GameObject highest = backgrounds
            .OrderByDescending(bg => bg.transform.position.y)
            .First();
            if (highest!=justTransitioned)
            {
                // Third space
                SpriteRenderer transitionedSprite = highest.GetComponent<SpriteRenderer>();
                transitionedSprite.sprite = spaceBackground.sprite;
                transitionStage = 4;
            }
        }
        if (transitionStage==0 && GameManager.Instance.triggerTransition)
        {
            // Transition sprite first
            justTransitioned = backgrounds
            .OrderByDescending(bg => bg.transform.position.y)
            .First();

            SpriteRenderer transitionedSprite = justTransitioned.GetComponent<SpriteRenderer>();
            transitionedSprite.sprite = transitionBackground.sprite;
            transitionStage = 1;
        }
    }
}
