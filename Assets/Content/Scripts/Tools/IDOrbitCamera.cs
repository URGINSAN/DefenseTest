using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// Specifies the unity update method to use.
/// </summary>
public enum UnityUpdateMethod
{
    None,
    FixedUpdate,
    Update,
    LateUpdate
}

/// <summary>
/// Camera that orbits around its target.
/// </summary>
public class IDOrbitCamera : MonoBehaviour
{
    public UnityUpdateMethod updateMethod = UnityUpdateMethod.Update;
    [Tooltip("Enable to have the camera respond to input itself, instead of being updated by another script.")]
    public bool useSimpleInput = false;
    [Tooltip("Should the camera orient itself around the world up or the players up?")]
    public bool usePlayerUp = false;
    [Tooltip("The Target to follow and orbit.")]
    public Transform target;
    [Tooltip("A secondary target that can be activated(for example: aiming")]
    public Transform secondaryTarget;
    [Tooltip("How fast the target transitions from the primary target to the secondary target.")]
    public float transitionToSpeed = 15f;
    [Tooltip("How fast the target transitions back to the primary target from the secondary target.")]
    public float transitionFromSpeed = 5f;
    [Tooltip("Amount of smoothing to be applied to the cameras rotation.")]
    public float rotationSmoothing = 5f;
    [Tooltip("Amount of smoothing to be applied to the cameras position.")]
    public float positionSmoothing = 20f;
    [Tooltip("Collider radius of the camera.")]
    public float cameraCollisionRadius = .3f;
    [Tooltip("What layers the camera collides with.")]
    public LayerMask layerMask;
    [Tooltip("Controls the offset of the camera from the primary target.")]
    public Vector3 positionOffset = Vector3.zero;
    [Tooltip("Will use the offset of the camera in the unity editor as the offset for the target.")]
    public bool useEditorCameraOffset;
    [Tooltip("How far away the camera stays from the target.")]
    public float distance = 2f;
    [Tooltip("The viewing angle limit the camera can get from the forward vector of the player.")]
    public float verticalAngleCap = 60f;
    [Tooltip("Clamp the input values to a maximum range.")]
    public float maxInputRange = 10f;
    [Tooltip("How fast the camera rotates.")]
    public float rotationSpeed = 10f;

    /// <summary>
    /// Is this camera currently accepting input.
    /// </summary>
    public bool acceptInput { get; set; }
    /// <summary>
    /// Rotates the camera around the up axis.
    /// </summary>
    public float inputX { get; set; }
    /// <summary>
    /// Rotates the camera around the right axis.
    /// </summary>
    public float inputY { get; set; }
    public bool isAiming { get; set; }

    private Vector3 position = Vector3.zero;
    private Vector3 desiredForward;
    private Quaternion lookRotation = Quaternion.identity;

    private float transition;
    private float actualDistance;
    private float distanceSmoothing = 5f;

    // Use this for initialization
    void Start()
    {
        if (!target)
        {
            Debug.Log("ERROR: ' " + gameObject.name + " ' has no target!!!");
            return;
        }

        actualDistance = distance;
        desiredForward = transform.forward;

        if (useEditorCameraOffset)
        {
            Vector3 editorOffset = transform.position - target.position;
            distance = editorOffset.z;
            editorOffset.z = 0f;
            positionOffset = editorOffset;
        }

        acceptInput = true;
        transform.SetParent(null);

        PositionCamera();
    }

    private void OnEnable()
    {
        desiredForward = target.forward;
    }

    private void OnDisable()
    {
        inputX = 0f;
        inputY = 0f;
    }

    private void FixedUpdate()
    {
        if (updateMethod != UnityUpdateMethod.FixedUpdate)
            return;

        RotateCamera();
        PositionCamera();
    }

    private void Update()
    {
        if (updateMethod != UnityUpdateMethod.Update)
            return;

        RotateCamera();
        PositionCamera();
    }

    private void LateUpdate()
    {
        if (updateMethod != UnityUpdateMethod.LateUpdate)
            return;

        RotateCamera();
        PositionCamera();
    }

    /// <summary>
    /// Rotates the camera.
    /// </summary>
    public void RotateCamera()
    {
        if (IsPointerOverUIObject())
            return;

        if (useSimpleInput)
        {
            // The tab key can be used to temporarily disable rotation of the camera.
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                acceptInput = !acceptInput;
                inputX = 0f;
                inputY = 0f;
            }

            if (acceptInput)
            {
                inputX = Input.GetAxis("Mouse X") * rotationSpeed;
                inputY = Input.GetAxis("Mouse Y") * rotationSpeed;
                isAiming = Input.GetMouseButton(1);
            }
        }

        inputX = Mathf.Clamp(inputX, -maxInputRange, maxInputRange);
        inputY = Mathf.Clamp(inputY, -maxInputRange, maxInputRange);

        Quaternion rotation = Quaternion.identity;
        Vector3 tempForward = Vector3.zero;
        Vector3 n = Vector3.zero;
        // Do we want to rotate around the world up or the players local up?
        if (usePlayerUp)
        {
            rotation = Quaternion.AngleAxis(inputX, target.up);
            n = Vector3.ProjectOnPlane(rotation * desiredForward, target.up);
        }
        else
        {
            rotation = Quaternion.AngleAxis(inputX, Vector3.up);
            n = Vector3.ProjectOnPlane(rotation * desiredForward, Vector3.up);
        }

        n.Normalize();

        // Rotate the forward vector by the players Vertical axis input.
        rotation *= Quaternion.AngleAxis(-inputY, transform.right);

        // Rotate the desired forward direction.
        tempForward = rotation * desiredForward;
        tempForward.Normalize();


        float angle = Vector3.Angle(n, tempForward);

        // Make sure the the new forward is within our angle limits. Otherwise,
        // constrain the angle of the camera.
        if (angle < verticalAngleCap)
            desiredForward = tempForward;
        else
        {
            desiredForward = Vector3.Slerp(tempForward, n, 1f - (verticalAngleCap / angle));
            desiredForward.Normalize();
        }

        // Get the new look rotation based on the desired forward vector.
        lookRotation = Quaternion.LookRotation(desiredForward);

        // Smoothly transition from the cameras current rotation to the new rotation.
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSmoothing * Time.deltaTime);
        Camera.main.transform.rotation = transform.rotation;
    }

    /// <summary>
    /// Positions the camera.
    /// </summary>
    public void PositionCamera()
    {
        float collisionDistance = distance;
        RaycastHit hitInfo = new RaycastHit();
        Vector3 offset = target.right * positionOffset.x + target.up * positionOffset.y + target.forward * positionOffset.z;
        offset += target.position;

        if (Physics.SphereCast(offset, cameraCollisionRadius, -transform.forward, out hitInfo, distance, layerMask, QueryTriggerInteraction.Ignore))
            collisionDistance = hitInfo.distance;

        if (collisionDistance < actualDistance)
            actualDistance = collisionDistance;
        else
            actualDistance = Mathf.Lerp(actualDistance, collisionDistance, distanceSmoothing * Time.deltaTime);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        position = -transform.forward * actualDistance + offset;

        transition += isAiming ? transitionToSpeed * Time.deltaTime : -transitionFromSpeed * Time.deltaTime;
        transition = Mathf.Clamp01(transition);


        Vector3 newPos = Vector3.Lerp(position, secondaryTarget.position, transition);

        transform.position = Vector3.Lerp(transform.position, newPos, positionSmoothing * Time.deltaTime);
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}