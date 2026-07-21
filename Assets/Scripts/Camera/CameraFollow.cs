using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    private float highestY;
    public float smooth = 3f;
    [Header("Camera Speed-Up Effect")]
    public float scrollSpeed = 1f;
    public float speedEffect = 0.5f;
    public float maxSpeed = 10f;
    public float timeInterval = 3f;

    private bool startedMoving = false;
    private float incTime = 1f;

    void Start()
    {
       transform.position = new Vector3(0,0, -10);

       player = GameManager.Instance.GetPlayer();
       highestY = player.position.y;
    }

    void LateUpdate()
    {
        if(GameManager.Instance.CurrentState == GameManager.GameState.Playing)
        {
            if (!startedMoving)
            {
                incTime = Time.time;
                startedMoving = true;
            }
            cameraMove();
        }
    }

    private void cameraMove()
    {
        // If we've capped the time 
        if (scrollSpeed<maxSpeed && Time.time - incTime >= timeInterval) {
                scrollSpeed = scrollSpeed + speedEffect;
                scrollSpeed = Mathf.Min(scrollSpeed, maxSpeed);
                incTime = Time.time;
        }
        
        highestY += scrollSpeed * Time.deltaTime;
        highestY = Mathf.Max(highestY, player.position.y);

        Vector3 target = new Vector3(
            transform.position.x,
            highestY,
            -10f
        );

        transform.position = Vector3.Lerp(
            transform.position,
            target,
            smooth * Time.deltaTime
        );
    }
}
