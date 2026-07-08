using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    private float highestY;
    public float smooth = 3f;
    public float scrollSpeed = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       player = GameManager.Instance.GetPlayer();
       highestY = player.position.y;
    }

    void LateUpdate()
    {
        highestY += scrollSpeed * Time.deltaTime;
        highestY = Mathf.Max(highestY, player.position.y);

        Vector3 target = new Vector3(
            player.position.x,
            highestY
        );

        transform.position = Vector3.Lerp(
            transform.position,
            target,
            smooth * Time.deltaTime
        );
    }
}
