using UnityEngine;

public class WallScript : MonoBehaviour
{
    public bool isLeft;

void Start()
    {
        int leftOrRight = 1;
        if (isLeft)
            leftOrRight = -1;
        else if (!isLeft)
            leftOrRight = 1;


        float xPos = leftOrRight * GameManager.Instance.boundaryWidth/2f;
        transform.position = new Vector2(xPos, transform.position.y);
    }


void Update()
    {
        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        transform.position = new Vector2(transform.position.x, cam.transform.position.y);
    }

}
