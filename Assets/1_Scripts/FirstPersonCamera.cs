using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;
    
    [Header("Look")]
    public float sensitivity = 100f;
    private Vector2 lookInput;
    private float xRotation;
    
    public GameObject Camera;

    [Header("Grab Settings")] 
    public float grabDistance;
    public float holdDistance;
    public float rotateSpeed;
    public float time;
    
    [Header("About GrabbedObject")]
    private GameObject grabbedObject;
    private Rigidbody objectRb;
    [SerializeField] private bool isHolding;
    [SerializeField] private bool isReturning;
    
    // 입력 값을 저장할 변수들
    private Vector2 rotateInput;
    private bool isRotateMode;
    
    private Vector3 lastConveyorPos;
    private Quaternion lastConveyorRot;
    
    void Update()
    {
        // 만약 잡고 있을경우
        if (isHolding)
        {
            HoldObject();
            
            if (isRotateMode)
            {
                RotateObject();
            }
        }
        // 아니라면 카메라 움직이기 가능
        else
        {
            float mouseX = lookInput.x * sensitivity * Time.deltaTime;
            float mouseY = lookInput.y * sensitivity * Time.deltaTime;
        
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
            player.transform.Rotate(Vector3.up * mouseX); 
        }
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
    
    // E키
    public void OnGrab(InputAction.CallbackContext context)
    {
        // 버튼을 누른 순간(Started)에만 실행
        if (context.started && !isReturning)
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
    }
    
    public void OnRotateMode(InputAction.CallbackContext context)
    {
        if (context.started) isRotateMode = true;
        else if (context.canceled) isRotateMode = false;
    }
    
    public void OnRotateValue(InputAction.CallbackContext context)
    {
        rotateInput = context.ReadValue<Vector2>();
    }

    void TryGrab()
    {
        Ray ray = new Ray(Camera.transform.position, Camera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, grabDistance))
        {
            if (hit.collider.CompareTag("Object"))
            {
                grabbedObject = hit.collider.gameObject;
                objectRb = grabbedObject.GetComponent<Rigidbody>();
                
                objectRb.useGravity = false; 
                objectRb.freezeRotation = true;
                isHolding = true;
                
                lastConveyorPos = grabbedObject.transform.position + new Vector3(0f, 0.5f, 0f);
                lastConveyorRot = grabbedObject.transform.rotation;

                StartCoroutine(CameraUp(time));
            }
        }
    }

    private IEnumerator CameraUp(float time)
    {
        float t = 0f;
        Vector3 currentEuler = Camera.transform.rotation.eulerAngles;
        Quaternion startRot = Camera.transform.rotation;
        Quaternion endRot = Quaternion.Euler(0f, currentEuler.y, currentEuler.z);
        
        while (t <= 1f)
        {
            t += Time.deltaTime / time;
            Camera.transform.rotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }
    }

    void HoldObject()
    {
        Vector3 targetPos = Camera.transform.position + Camera.transform.forward * holdDistance;
        // Vector3 direction = targetPos - grabbedObject.transform.position;
        //
        // objectRb.linearVelocity = direction * moveSpeed;

        grabbedObject.transform.position = targetPos;
    }

    void RotateObject()
    {
        float x = rotateInput.x * rotateSpeed;
        float y = rotateInput.y * rotateSpeed;

        grabbedObject.transform.Rotate(Camera.transform.up, -x, Space.World);
        grabbedObject.transform.Rotate(Camera.transform.right, y, Space.World);
    }

    void DropObject() => StartCoroutine(BackToConveyor(time));

    IEnumerator BackToConveyor(float time)
    {
        isHolding = false;
        isReturning = true;
        
        float t = 0f;
        Vector3 startPos = grabbedObject.transform.position;
        Quaternion startRot = grabbedObject.transform.rotation;

        while (t <= 1f)
        {
            t += Time.deltaTime / time;
            grabbedObject.transform.position = Vector3.Lerp(startPos, lastConveyorPos, t);
            grabbedObject.transform.rotation = Quaternion.Lerp(startRot, lastConveyorRot, t);
            yield return null;
        }
        
        // grabbedObject관련 변수 초기화
        objectRb.useGravity = true;
        objectRb.freezeRotation = false;
        objectRb.linearVelocity = Vector3.zero;
        
        objectRb = null;
        grabbedObject = null;
        isReturning = false;
    }
}
