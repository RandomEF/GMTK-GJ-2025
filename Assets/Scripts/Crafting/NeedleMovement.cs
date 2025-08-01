using UnityEngine;
using UnityEngine.InputSystem;

public class NeedleMovement : MonoBehaviour
{
    public PlayerManager playerManager;
    public Camera benchView;
    private InputSystem_Actions inputs;
    public float maxSpeed = 0.5f;
    public float rotationSpeed = 10;
    public Rigidbody body;
    private Vector3 lastPosition;
    private Vector3 currentPosition;
    public LayerMask notNeedlePlane;

    void Start()
    {
        playerManager = PlayerManager.Instance;
        inputs = playerManager.inputs;
        lastPosition = transform.position;
        currentPosition = transform.position;
        notNeedlePlane = LayerMask.GetMask("Needle Plane");
    }

    void FixedUpdate()
    {
        if (inputs.CraftMenu.enabled)
        {
            lastPosition = currentPosition;
            Vector3 mousePos = Mouse.current.position.ReadValue();
            Ray ray = benchView.ScreenPointToRay(mousePos);
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, layerMask: notNeedlePlane, maxDistance: 2))
            {
                currentPosition = hit.point;
                currentPosition.y = lastPosition.y;
                Vector3 change = currentPosition - lastPosition;
                Vector3 currentVelocity = body.linearVelocity;
                Vector3.SmoothDamp(body.position, currentPosition, ref currentVelocity, 0.1f, maxSpeed, Time.fixedDeltaTime);
                body.linearVelocity = currentVelocity;
                if (change.magnitude != 0)
                {
                    body.rotation = Quaternion.Slerp(body.rotation, Quaternion.LookRotation(change.normalized, Vector3.forward), 0.75f);
                    //body.rotation = Quaternion.LookRotation(change.normalized, Vector3.forward);
                }
            }
        }
    }
}
