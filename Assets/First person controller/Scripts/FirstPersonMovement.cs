using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5;
    bool grounded;
    bool readyToJump;
    public int jumpCooldown;
    LayerMask whatIsGround;
    public GameObject groundChecker;
    public float moveSpeed = 5f;
    public float sensitivity = 5f;
    Rigidbody rb;
    Camera playerCam;
    float xRotation;
    Transform playerBody;
    SpringJoint joint;
    Vector3 grapplePoint;
    public Rope rope;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;
    /// <summary> Functions to override movement speed. Will use the last added override. </summary>
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerBody = this.transform;
        xRotation = 0f;
        playerCam = GetComponentInChildren<Camera>();
        readyToJump = true;
        whatIsGround = LayerMask.GetMask("Default");
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Movement();
        Look();
    }

    private void Movement()
    {
        grounded = Physics.OverlapSphere(groundChecker.transform.position, radius: 0.1f, whatIsGround).Length > 0;
        print(grounded);

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        bool jumping = Input.GetButton("Jump");

        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        CounterMovement(x, y, mag);

        if (x > 0 && xMag > 5) x = 0;
        if (x < 0 && xMag < -5) x = 0;
        if (y > 0 && yMag > 5) y = 0;
        if (y < 0 && yMag < -5) y = 0;

        rb.AddForce(transform.forward * y * moveSpeed * Time.deltaTime);
        rb.AddForce(transform.right * x * moveSpeed * Time.deltaTime);

        if(grounded && readyToJump && jumping)
        {
            readyToJump = false;
            rb.AddForce(Vector3.up * 200);
            Invoke(nameof(ResetJump), jumpCooldown);
        }


        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartGrapple();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            StopGrapple();
        }

        
    }

    private void StartCrouch()
    {
        float slideForce = 400;
        transform.localScale = new Vector3(1, 0.7f, 1);
        rb.AddForce(transform.forward * slideForce);
    }

    private void StopCrouch()
    {
        transform.localScale = new Vector3(1, 1.5f, 1);
    }

    private void StopGrapple()
    {
        Destroy(joint);

        rope.Hide();
    }

    private void StartGrapple()
    {
        print("grapple");
        RaycastHit[] hits = Physics.RaycastAll(playerCam.transform.position, playerCam.transform.forward, Mathf.Infinity, whatIsGround);
        if (hits.Length < 1) return;
        grapplePoint = hits[0].point;
        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        //joint.connectedAnchor = hits[0].rigidbody;
        joint.spring = 3.5f;
        joint.damper = 0.25f;

        rope.Show(grapplePoint);
    }

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(value: xRotation, min: -90f, max: 90f);

        playerCam.transform.localRotation = Quaternion.Euler(xRotation, y: 0, z: 0);
        playerBody.Rotate(eulers: Vector3.up * mouseX);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded) return;
        //float threshold = 0.3f;
        float multiplier = 0.3f;
        print(mag.x);

        if (x == 0)
        {
            rb.AddForce(moveSpeed * transform.right * Time.deltaTime * -mag.x * multiplier);
        }
        if (y == 0)
        {
            rb.AddForce(moveSpeed * transform.forward * Time.deltaTime * -mag.y * multiplier);
        }
    }

    private Vector2 FindVelRelativeToLook()
    {
        float lookAngle = transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(y: rb.velocity.x, x: rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(current: lookAngle, target: moveAngle);
        float v = 90 - u;

        float magnitude = rb.velocity.magnitude;
        float yMag = magnitude * Mathf.Cos(f: u * Mathf.Deg2Rad);
        float xMag = magnitude * Mathf.Cos(f: v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }
}