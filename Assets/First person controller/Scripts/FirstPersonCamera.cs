using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public GameObject playerToFollow;
    Transform playerTransform;
    float xRotation = 0f;
    float sensitivity;
    public Vector3 cameraOffset = new Vector3(0, 0.5f);
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = playerToFollow.transform;
        sensitivity = playerToFollow.GetComponent<FirstPersonMovement>().sensitivity;
    }

    private void LateUpdate()
    {
        //this.transform.position = playerTransform.position + cameraOffset;
        CameraLook();
    }


    private void CameraLook()
    {
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(value: xRotation, min: -90f, max: 90f);
        

        this.transform.rotation = Quaternion.Euler(xRotation, Mathf.LerpAngle(this.transform.eulerAngles.y, playerTransform.eulerAngles.y, 0.25f), z: 0);
        print("Player Y rotation:" + playerTransform.rotation.y);
        //playerBody.Rotate(eulers: Vector3.up * mouseX);
    }
}
