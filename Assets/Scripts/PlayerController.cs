using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float fixedY = 0.5f;
    public bool isWin = false;
    public bool isLose = false;
    public float minX = -5f;
    public float maxX = 5f;
    public float minZ = -10f;
    public float maxZ = -2f;

    private Camera cam;
    private bool isDragging = false;
    private Vector3 dragOffset;

    [Header("PC Smooth Movement")]
    public float smooth = 0.4f;
    private Vector3 velocity = Vector3.zero;

    [Header("Mobile Drag Movement")]
    public float mobileSpeed = 0.02f;
    private Vector2 lastTouchPos;
    private bool isMobile = false;

    public Animator animator;

    public GameObject powerPlayerEffect;

    public static PlayerController Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cam = Camera.main;

#if !UNITY_EDITOR
        isMobile = true; // build ra mobile sẽ tự bật
#endif
    }

    void Update()
    {
#if UNITY_EDITOR
        HandlePC();
#else
        HandleMobile();
#endif
    }

    // -------------------------------------------------
    //                  PC CONTROL
    // -------------------------------------------------

    void HandlePC()
    {
        if (Input.GetMouseButtonDown(0))
            BeginDragPC(Input.mousePosition);

        if (Input.GetMouseButton(0) && isDragging)
            UpdateDragPC(Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
            EndDrag();
    }

    void BeginDragPC(Vector2 screenPos)
    {
        if (RayToPlane(screenPos, out Vector3 hit))
        {
            isDragging = true;
            dragOffset = transform.position - hit;
        }
    }

    void UpdateDragPC(Vector2 screenPos)
    {
        if (!RayToPlane(screenPos, out Vector3 hit))
            return;

        animator.SetBool("PlayerMoving", true);

        Vector3 target = hit + dragOffset;
        target.y = fixedY;

        target.x = Mathf.Clamp(target.x, minX, maxX);
        target.z = Mathf.Clamp(target.z, minZ, maxZ);

        transform.position = Vector3.SmoothDamp(
            transform.position,
            target,
            ref velocity,
            smooth
        );
    }

    // -------------------------------------------------
    //                  MOBILE CONTROL
    // -------------------------------------------------

    void HandleMobile()
    {
        if (Input.touchCount == 0)
        {
            if (isDragging)
                EndDrag();
            return;
        }

        Touch t = Input.GetTouch(0);

        if (t.phase == TouchPhase.Began)
        {
            isDragging = true;
            lastTouchPos = t.position;
        }
        else if (t.phase == TouchPhase.Moved && isDragging)
        {
            MoveByTouchDelta(t);
            lastTouchPos = t.position;
        }

        if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
        {
            EndDrag();
        }
    }

    void MoveByTouchDelta(Touch t)
    {
        Vector2 delta = t.position - lastTouchPos;

        animator.SetBool("PlayerMoving", true);

        Vector3 move = new Vector3(delta.x, 0, delta.y) * mobileSpeed;

        Vector3 target = transform.position + move;

        target.x = Mathf.Clamp(target.x, minX, maxX);
        target.z = Mathf.Clamp(target.z, minZ, maxZ);

        transform.position = Vector3.Lerp(
            transform.position,
            target,
            0.5f
        );
    }


    void EndDrag()
    {
        isDragging = false;
        animator.SetBool("PlayerMoving", false);
    }

    bool RayToPlane(Vector2 pos, out Vector3 hit)
    {
        Ray ray = cam.ScreenPointToRay(pos);
        Plane plane = new Plane(Vector3.up, new Vector3(0, fixedY, 0));

        if (plane.Raycast(ray, out float distance))
        {
            hit = ray.GetPoint(distance);
            return true;
        }

        hit = Vector3.zero;
        return false;
    }
}
