using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private float startPos;
    public float spacing;

    // 0-1 (0=no effect, 1=normal, 0.5=half effect)
    public float parallaxEffect;

    void Start()
    {
        startPos = transform.position.y;
        spacing = 20f;
    }

    void LateUpdate()
    {
        float cameraY = GameManager.Instance.GetCamera().transform.position.y;

        transform.position = new Vector3(
            transform.position.x,
            startPos + cameraY * parallaxEffect,
            transform.position.z
        );

        while (cameraY - transform.position.y > spacing)
        {
            startPos += spacing * 3f;
            transform.position = new Vector3(
                transform.position.x,
                startPos + cameraY * parallaxEffect,
                transform.position.z
            );
        }
    }
}
