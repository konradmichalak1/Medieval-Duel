using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private Transform lookAt;
    private Transform camTransform;
    public bool focusOnEnemy; //is camera focused on enemy?
    public Transform enemy;
    public Transform player;
    public bool isAgent;
    private float distance = 6.0f;
    private float currentX = 0.0f;
    private float currentY = 30.0f;

    public float sensitivityX, sensitivityY, sensitivityScroll;

    private const float Y_ANGLE_MIN = 0.0f;
    private const float Y_ANGLE_MAX = 60.0f;

    public float maxDistance, minDistance;

    void Start(){
        lookAt = player;
        focusOnEnemy = false;
        camTransform = transform;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        if (!isAgent)
        {
            if (Input.GetButtonDown("Focus") && !focusOnEnemy)
            {
                focusOnEnemy = true;
                lookAt = enemy;

            }
            else if (Input.GetButtonDown("Focus") && focusOnEnemy)
            {
                focusOnEnemy = false;
                lookAt = player;
            }

            if (!focusOnEnemy)
            {
                currentX += Input.GetAxis("Mouse X") * sensitivityX;
                currentY += Input.GetAxis("Mouse Y") * -sensitivityY;
                currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);

                distance += Input.GetAxis("Mouse ScrollWheel") * -sensitivityScroll;
                distance = Mathf.Clamp(distance, minDistance, maxDistance);
            }
        }
    }
    void LateUpdate () {
        if (!focusOnEnemy)
        {
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            Vector3 dir = new Vector3(0, 0, -distance);
            camTransform.position = player.position + rotation * dir;
            camTransform.LookAt(lookAt.position);
        }
        else if(focusOnEnemy)
        {
            Vector3 dir = CameraDirection();
            camTransform.position =  dir;
            camTransform.LookAt(lookAt.position);
        }
    }

    private Vector3 CameraDirection()
    {
        float z = enemy.position.z - player.position.z;
        float x = enemy.position.x - player.position.x;
        float n = Mathf.Sqrt(Mathf.Abs((x * x) - (z * z)));
        float sin_a = z / n;
        float cos_a = x / n;
        float z_length = sin_a * (distance + n);
        float cam_z = enemy.position.z - z_length;
        float x_length = cos_a * (distance + n);
        float cam_x = enemy.position.x - x_length;

        cam_x = Mathf.Clamp(cam_x, player.position.x - 2.5f, player.position.x + 2.5f);
        cam_z = Mathf.Clamp(cam_z, player.position.z - 2.5f, player.position.z + 2.5f);
        Vector3 dir = new Vector3(cam_x, 2, cam_z);
        return dir;
    }
}
