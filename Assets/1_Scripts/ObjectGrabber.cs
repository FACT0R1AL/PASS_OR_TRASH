using UnityEngine;

public class ObjectGrabber : MonoBehaviour
{
    public Transform cameraTransform;

    [Header("Grab Settings")]
    public float grabDistance = 5f;
    public float holdDistance = 2f;
    public float moveSpeed = 10f;

    [Header("Rotate Settings")]
    public float rotateSpeed = 5f;

    Rigidbody grabbedObject;
    bool isHolding = false;

    FPSController fpsController;

    void Start()
    {
        fpsController = GetComponent<FPSController>();

        if (fpsController == null)
            fpsController = GetComponentInParent<FPSController>();
    }

    void Update()
    {
        // E 키로 잡기 / 놓기
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isHolding)
                DropObject();
            else
                TryGrab();
        }

        if (isHolding && grabbedObject != null)
        {
            HoldObject();
            RotateObject();
        }
    }

    void TryGrab()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, grabDistance))
        {
            if (hit.collider.CompareTag("Object"))
            {
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    grabbedObject = rb;
                    grabbedObject.useGravity = false;
                    grabbedObject.linearVelocity = Vector3.zero;
                    grabbedObject.angularVelocity = Vector3.zero;

                    isHolding = true;

                    // 이동 + 시점 완전 차단
                    if (fpsController != null)
                    {
                        fpsController.canLook = false;
                        fpsController.canMove = false;
                        fpsController.ResetView(); // 정면 바라보기
                    }
                }
            }
        }
    }

    void HoldObject()
    {
        Vector3 targetPos = cameraTransform.position + cameraTransform.forward * holdDistance;

        Vector3 direction = targetPos - grabbedObject.position;
        grabbedObject.linearVelocity = direction * moveSpeed;
    }

    void RotateObject()
    {
        // 우클릭 시 물체만 회전
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotateSpeed * 100f * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed * 100f * Time.deltaTime;

            grabbedObject.transform.Rotate(cameraTransform.up, -mouseX, Space.World);
            grabbedObject.transform.Rotate(cameraTransform.right, mouseY, Space.World);
        }
    }

    void DropObject()
    {
        grabbedObject.useGravity = true;
        grabbedObject = null;
        isHolding = false;

        // 다시 이동 + 시점 허용
        if (fpsController != null)
        {
            fpsController.canLook = true;
            fpsController.canMove = true;
        }
    }
}
