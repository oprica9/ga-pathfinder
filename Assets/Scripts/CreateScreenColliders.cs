using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateScreenColliders : MonoBehaviour
{
    public GameObject wallPrefab;

    Transform leftWall;
    Transform rightWall;
    Transform topWall;
    Transform bottomWall;

    public Camera cam;

    float cam_height;
    float cam_width;

    public RectTransform canvasTransform;

    void Start()
    {
        cam_height = 2f * cam.orthographicSize;
        cam_width = cam_height * cam.aspect;


        // instantiate game objects
        leftWall = Instantiate(wallPrefab).transform;
        rightWall = Instantiate(wallPrefab).transform;
        Destroy(rightWall.GetComponent<Obstacle>());
        topWall = Instantiate(wallPrefab).transform;
        bottomWall = Instantiate(wallPrefab).transform;

        // sizes
        leftWall.localScale = new Vector2(10, cam_height);
        rightWall.localScale = new Vector2(canvasTransform.rect.width, canvasTransform.rect.height);
        topWall.localScale = new Vector2(cam_width, 10);
        bottomWall.localScale = new Vector2(cam_width, 10);

        //positions
        leftWall.position = Camera.main.ViewportToWorldPoint(Vector3.zero) + new Vector3(-leftWall.localScale.x / 2, leftWall.localScale.y / 2);
        topWall.position = Camera.main.ViewportToWorldPoint(Vector3.zero) + new Vector3(cam_width / 2, cam_height + topWall.localScale.y / 2);
        bottomWall.position = Camera.main.ViewportToWorldPoint(Vector3.zero) + new Vector3(cam_width / 2, -topWall.localScale.y / 2);
        rightWall.position = Camera.main.ViewportToScreenPoint(Vector3.zero) + new Vector3(cam_width / 2 - rightWall.localScale.x / 2, 0);



    }

    private void Update()
    {

    }

}