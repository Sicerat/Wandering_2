using UnityEngine;

public class GrapplingGun : MonoBehaviour {

    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;
    public float defaultPullSpeed, stretchSpeed;
    private float currentPullSpeed;
    private float maxDistance = 100f;
    private SpringJoint joint;
    public float jointSpring = 4.5f, jointDamper = 7f, jointMassScale = 4.5f;

    void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0)) {
            StopGrapple();
        }

        if (Input.GetMouseButton(1))
        {
            PullOnGrapple();
            print("RMB pressed!");
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StretchGrapple();
            print("RMB released!");
        }
    }

    //Called after Update
    void LateUpdate() {
        DrawRope();
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple() {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable)) {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = 0.25f;

            //Set desiredLenght for pulling function to "default value" of start max joint distance
            //Set pull speed to default pull speed
            print("Previous pull speed: " + currentPullSpeed);
            currentPullSpeed = joint.maxDistance * 0.01f * defaultPullSpeed;
            print("Set pulling parameters to default: " + currentPullSpeed);

            //Adjust these values to fit your game.
            joint.spring = jointSpring;
            joint.damper = jointDamper;
            joint.massScale = jointMassScale;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopGrapple() {
        lr.positionCount = 0;
        Destroy(joint);
    }

    /// <summary>
    /// Pull up while grappling
    /// </summary>
    void PullOnGrapple()
    {
        if (joint == null) return;
        //print("Joint exists!");
        currentPullSpeed += 0.01f;
        //print("Default pull speed: " + defaultPullSpeed);
        //print("Current pull speed: " + currentPullSpeed);
        joint.maxDistance -= currentPullSpeed * Time.fixedDeltaTime;
        //joint.maxDistance = Mathf.Lerp(joint.maxDistance, desiredLenght, 0.5f);
        print("Current joint max distance: " + joint.maxDistance);
    }

    void StretchGrapple()
    {

    }

    private Vector3 currentGrapplePosition;
    
    void DrawRope() {
        //If not grappling, don't draw rope
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);
        
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling() {
        return joint != null;
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }
}
