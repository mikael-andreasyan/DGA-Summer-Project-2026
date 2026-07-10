using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private float startPos, length;
    // 0-1 (0=no effect, 1=normal, 0.5=half effect)
    public float parallaxEffect;

    void Start()
    {
        startPos = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void FixedUpdate()
    {
        float distance = GameManager.Instance.GetCamera().transform.position.y
            * parallaxEffect;
        float movement = GameManager.Instance.GetCamera().transform.position.y
            * (1 - parallaxEffect);
        transform.position = new Vector3(transform.position.x, startPos + distance, transform.position.z);
    }
}
