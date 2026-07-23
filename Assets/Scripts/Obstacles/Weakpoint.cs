using UnityEngine;

// This script registers when the player touches this weakpoint
public class Weakpoint : MonoBehaviour
{

    public enum WeakpointType
    {
        Left,
        Middle,
        Right
    }

    [SerializeField] private WeakpointType type;
    private BasicCloud cloudScript;
    private bool playerInside;

    void Start()
    {
        cloudScript = GetComponentInParent<BasicCloud>();
        switch (type)
        {
            case WeakpointType.Left:
                if (!cloudScript.isActiveWeakpoint(0))
                {
                    
                    Destroy(gameObject);
                }
                break;
            case WeakpointType.Middle:
                if (!cloudScript.isActiveWeakpoint(1))
                {
                    
                    Destroy(gameObject);
                }
                break;
            case WeakpointType.Right:
                if (!cloudScript.isActiveWeakpoint(2))
                {
                   
                    Destroy(gameObject);
                }
                break;
        }

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
 
        if (collision.gameObject.CompareTag("Player") && !playerInside)
        {
            playerInside = true;
            switch (type)
            {
                case WeakpointType.Left:
                    cloudScript.registerChildCollision(0);
                    break;
                case WeakpointType.Middle:
                    cloudScript.registerChildCollision(1);
                    break;
                case WeakpointType.Right:
                    cloudScript.registerChildCollision(2);
                    break;
            }
        }
    }
    void OnTriggerStay2D(Collider2D collision)
    {
 
        if (collision.gameObject.CompareTag("Player") && !playerInside)
        {
            playerInside = true;
            switch (type)
            {
                case WeakpointType.Left:
                    cloudScript.registerChildCollision(0);
                    break;
                case WeakpointType.Middle:
                    cloudScript.registerChildCollision(1);
                    break;
                case WeakpointType.Right:
                    cloudScript.registerChildCollision(2);
                    break;
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInside = false;
            switch (type)
            {
                case WeakpointType.Left:
                    cloudScript.registerChildCollisionExit(0);
                    break;
                case WeakpointType.Middle:
                    cloudScript.registerChildCollisionExit(1);
                    break;
                case WeakpointType.Right:
                    cloudScript.registerChildCollisionExit(2);
                    break;
            }
        }
    }
}
