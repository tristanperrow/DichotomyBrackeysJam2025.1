using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{

    [Header("Serialized Fields")]
    [SerializeField] private GameObject player;     // player game object

    [Header("Options")]
    [SerializeField] private Bounds _bounds = new Bounds(new Vector2(-1, -1), new Vector2(1, 1));   // world bounds
    [SerializeField] private float cameraDragTime = 0.3f;   // time it takes for camera to catch up

    // properties
    [DoNotSerialize] public Vector2 targetPosition; // position for the camera to try to move to
    [DoNotSerialize] public Bounds cameraBounds;    // actual cameraBounds

    private Vector3 vel = Vector3.zero;

    private void Awake()
    {
        var cam = GetComponent<Camera>();
        var height = cam.orthographicSize;
        var width = cam.aspect * height;

        // creates the actual camera bounds from world bounds
        cameraBounds = new Bounds(
            new Vector2(_bounds.min.x + width, _bounds.min.y + height),
            new Vector2(_bounds.max.x - width, _bounds.max.y - height)
            );
    }

    private void Update()
    {
        // lerp to player position
        targetPosition = player.transform.position;
    }

    private void LateUpdate()
    {
        Vector3 target = GetCameraBounds();

        //transform.position = Vector3.SmoothDamp(transform.position, target, ref vel, cameraDragTime);
        transform.position = target;
    }

    // clamps camera to bounds given
    private Vector3 GetCameraBounds()
    {
        return new Vector3(
            Mathf.Clamp(targetPosition.x, cameraBounds.min.x, cameraBounds.max.x),
            Mathf.Clamp(targetPosition.y, cameraBounds.min.y, cameraBounds.max.y),
            transform.position.z
            );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        // calculate boundaries
        Vector3 bl = new Vector3(_bounds.min.x, _bounds.min.y, transform.position.z);
        Vector3 tl = new Vector3(_bounds.min.x, _bounds.max.y, transform.position.z);
        Vector3 tr = new Vector3(_bounds.max.x, _bounds.max.y, transform.position.z);
        Vector3 br = new Vector3(_bounds.max.x, _bounds.min.y, transform.position.z);

        // draw lines
        Gizmos.DrawLine(bl, tl);
        Gizmos.DrawLine(tl, tr);
        Gizmos.DrawLine(tr, br);
        Gizmos.DrawLine(br, bl);
    }

}

[System.Serializable]
public class Bounds
{

    public Vector2 min;
    public Vector2 max;

    public Bounds(Vector2 min, Vector2 max)
    {
        this.min = min;
        this.max = max;
    }

}
