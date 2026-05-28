using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;
    private Collider playerCollider; 
    
    [Header("Look")]
    public float sensitivity = 100f;
    private Vector2 lookInput;
    private float xRotation;
    
    public GameObject Camera;

    [Header("Grab Settings")] 
    public float grabDistance;
    public float holdDistance;
    public float rotateSpeed = 250f; 
    public float lerpSpeed = 20f; // 물체가 카메라 앞을 쫓아오는 속도
    
    [Header("About GrabbedObject")]
    [SerializeField] private GameObject grabbedObject;
    [SerializeField] private Rigidbody objectRb;
    [SerializeField] private Collider grabbedCollider; 
    [SerializeField] private bool isHolding;
    [SerializeField] private bool isReturning;
    
    void Start()
    {
        if (player != null)
        {
            playerCollider = player.GetComponent<Collider>();
            if (playerCollider == null)
            {
                playerCollider = player.GetComponentInParent<Collider>();
            }
        }
    }

    void Update()
    {
        if (Mouse.current != null)
        {
            if (isHolding && Mouse.current.leftButton.isPressed)
            {
                RotateObject();
            }
            else if (Mouse.current.rightButton.isPressed)
            {
                RotateCamera();
            }
        }
    }

    // 물리 이동은 FixedUpdate에서 처리해야 벽 관통과 떨림이 방지됩니다.
    void FixedUpdate()
    {
        if (isHolding)
        {
            HoldObjectPhysics();
        }
    }
    
    void RotateCamera()
    {
        float mouseX = lookInput.x * sensitivity * Time.deltaTime;
        float mouseY = lookInput.y * sensitivity * Time.deltaTime;
    
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f); 
    
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.transform.Rotate(Vector3.up * mouseX); 
    }

    void RotateObject()
    {
        float x = lookInput.x * rotateSpeed * Time.deltaTime;
        float y = lookInput.y * rotateSpeed * Time.deltaTime;

        grabbedObject.transform.Rotate(Camera.transform.up, -x, Space.World);
        grabbedObject.transform.Rotate(Camera.transform.right, y, Space.World);
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
    
    public void OnGrab(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (isHolding)
            {
                if (Keyboard.current != null && Keyboard.current.shiftKey.isPressed)
                {
                    ThrowObject();
                }
                else 
                {
                    DropObject();
                }
            }
            else
            {
                TryGrab();
            }
        }
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
                grabbedCollider = grabbedObject.GetComponent<Collider>();
                
                // 잡고 있을 때는 중력을 끄고, 힘으로 제어합니다.
                objectRb.useGravity = false; 
                objectRb.isKinematic = false; 
                objectRb.freezeRotation = true;
                
                // 공중에서 덜덜거리며 진동하는 것을 막기 위해 공기 저항을 높입니다.
                objectRb.linearDamping = 10f; 
                objectRb.angularDamping = 10f;

                isHolding = true;

                // 플레이어와 물체 간의 물리적 충돌만 꺼서 플레이어가 밀리는 현상 방지
                if (playerCollider != null && grabbedCollider != null)
                {
                    Physics.IgnoreCollision(playerCollider, grabbedCollider, true);
                }
            }
        }
    }

    void HoldObjectPhysics()
    {
        if (grabbedObject == null || objectRb == null) return;

        // 카메라 앞 목표 좌표
        Vector3 targetPos = Camera.transform.position + Camera.transform.forward * holdDistance;
        
        // 방향 벡터 계산
        Vector3 moveDirection = targetPos - grabbedObject.transform.position;
        
        // Rigidbody 속도로 밀어주어 벽이나 바닥에 부딪히면 물리적으로 멈추게 합니다.
        objectRb.linearVelocity = moveDirection * lerpSpeed;
    }

    // ★ 중력 및 모든 물리 설정을 완벽하게 되돌리는 리셋 함수
    void ResetObjectPhysics()
    {
        if (playerCollider != null && grabbedCollider != null)
        {
            Physics.IgnoreCollision(playerCollider, grabbedCollider, false);
        }

        if (objectRb != null)
        {
            objectRb.useGravity = true;     // ★ 중력 다시 켜기 복구
            objectRb.isKinematic = false;
            objectRb.linearDamping = 0f;    // 저항값 원래대로 복구
            objectRb.angularDamping = 0.05f;
            objectRb.freezeRotation = false;
        }
    }

    void DropObject()
    {
        ResetObjectPhysics();
        isHolding = false;
        
        objectRb = null;
        grabbedCollider = null;
        grabbedObject = null;
    }
    
    void ThrowObject()
    {
        ResetObjectPhysics();
        isHolding = false;
        
        Vector3 throwDirection = (Camera.transform.forward + Camera.transform.up * 0.2f).normalized;
        float throwForce = 12f; 
        
        objectRb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        
        objectRb = null;
        grabbedCollider = null;
        grabbedObject = null;
    }
}