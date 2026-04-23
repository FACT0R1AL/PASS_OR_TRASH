using UnityEngine;
using System.Collections;

public class ObjectGrabber : MonoBehaviour
{
    public Transform cameraTransform;

    [Header("Grab Settings")]
    public float grabDistance = 5f;
    public float holdDistance = 2f;
    public float moveSpeed = 10f;
    public float rotateSpeed = 5f;
    
    [Header("About GrabbedObject")]
    private GameObject grabbedObject;
    private Rigidbody objectRb;
    private bool isHolding = false;
    private bool isBack = false;

    private Vector3 lastConveyorPos;
    private Quaternion lastConveyorRot;

    // FPSController fpsController;

    // void Start()
    // {
    //     fpsController = GetComponent<FPSController>();
    //
    //     if (fpsController == null)
    //         fpsController = GetComponentInParent<FPSController>();
    // }

    void Update()
    {
        // E 키로 잡기 / 놓기
        if (Input.GetKeyDown(KeyCode.E) && !isBack)
        {
            if (isHolding)
            {
                DropObject();
            }
            else
            {
                TryGrab();
            }
        }

        if (isHolding && objectRb is not null)
        {
            HoldObject();
            RotateObject();
        }
        
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * grabDistance, Color.red);
    }

    void TryGrab()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward * grabDistance);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, grabDistance))
        {
            if (hit.collider.CompareTag("Object"))
            {
                grabbedObject = hit.collider.gameObject;
                objectRb = grabbedObject.GetComponent<Rigidbody>();

                if (objectRb is null)
                {
                    Debug.LogWarning("grabbedObject has no Rigidbody");
                }
                else
                {
                    objectRb.useGravity = false;
                    objectRb.freezeRotation = true;
                    isHolding = true;
                    lastConveyorPos = grabbedObject.transform.position + new Vector3(0f, 0.5f, 0f);
                    lastConveyorRot = grabbedObject.transform.rotation;
                    
                    Debug.Log(lastConveyorPos);
                }
                    
                    // // 이동 + 시점 완전 차단
                    // if (fpsController != null)
                    // {
                    //     fpsController.canLook = false;
                    //     fpsController.canMove = false;
                    //     fpsController.ResetView(); // 정면 바라보기
                    // }
            }
        }
    }

    void HoldObject()
    {
        Vector3 targetPos = cameraTransform.position + cameraTransform.forward * holdDistance;
        
        Vector3 direction = targetPos - grabbedObject.transform.position;
        objectRb.linearVelocity = direction * moveSpeed;
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
        StartCoroutine(BackToConveyor());

        // // 다시 이동 + 시점 허용
        // if (fpsController != null)
        // {
        //     fpsController.canLook = true;
        //     fpsController.canMove = true;
        // }
    }

    IEnumerator BackToConveyor()
    {
        float t = 0f;

        Vector3 startPos = grabbedObject.transform.position;
        Vector3 endPos = lastConveyorPos;
        
        Quaternion startRot = grabbedObject.transform.rotation;
        Quaternion endRot = lastConveyorRot;

        isBack = true;
        
        while (t <= 1f)
        {
            t += Time.deltaTime;
            
            grabbedObject.transform.rotation = Quaternion.Lerp(startRot, endRot, t);
            grabbedObject.transform.position = Vector3.Lerp(startPos, endPos, t);
            
            yield return null;
        }
        
        grabbedObject.transform.position = endPos;
        grabbedObject.transform.rotation = endRot;
        
        objectRb.useGravity = true;
        objectRb.freezeRotation = false;
        objectRb.linearVelocity = Vector3.zero;
        objectRb = null;
        grabbedObject = null;
        isHolding = false;
        isBack = false;

        yield return null;
    }
}
