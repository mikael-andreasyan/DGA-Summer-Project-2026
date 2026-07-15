using UnityEngine;

// This script registers when the player touches this weakpoint
public class Weakpoint : MonoBehaviour
{
    BasicCloud cloudScript;

    void Start()
    {
        cloudScript = GetComponentInParent<BasicCloud>();
        switch (this.gameObject.name)
        {
            case "LeftWeakpoint":
                if (!cloudScript.isActiveWeakpoint(0))
                {
                    
                    Destroy(gameObject);
                }
                break;
            case "MidWeakpoint":
                if (!cloudScript.isActiveWeakpoint(1))
                {
                    
                    Destroy(gameObject);
                }
                break;
            case "RightWeakpoint":
                if (!cloudScript.isActiveWeakpoint(2))
                {
                   
                    Destroy(gameObject);
                }
                break;
        }

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (this.gameObject.name)
            {
                case "LeftWeakpoint":
                    cloudScript.registerChildCollision(0);
                    break;
                case "MidWeakpoint":
                    cloudScript.registerChildCollision(1);
                    break;
                case "RightWeakpoint":
                    cloudScript.registerChildCollision(2);
                    break;
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (this.gameObject.name)
            {
                case "LeftWeakpoint":
                    cloudScript.registerChildCollisionExit(0);
                    break;
                case "MidWeakpoint":
                    cloudScript.registerChildCollisionExit(1);
                    break;
                case "RightWeakpoint":
                    cloudScript.registerChildCollisionExit(2);
                    break;
            }
        }
    }
}
