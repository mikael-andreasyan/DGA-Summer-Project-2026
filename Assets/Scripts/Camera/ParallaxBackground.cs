using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private float startPos;
    // 0-1 (0=no effect, 1=normal, 0.5=half effect)
    public float parallaxEffect;

    void Start()
    {
        startPos = transform.position.y;
    }

    void FixedUpdate()
    {
        float distance = GameManager.Instance.GetCamera().transform.position.y
            + parallaxEffect;
        transform.position = new Vector3(transform.position.x, startPos + distance, transform.position.z);
    }
}
