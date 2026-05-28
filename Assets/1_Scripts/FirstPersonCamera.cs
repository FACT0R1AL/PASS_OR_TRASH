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
    public float minHoldDistance = 0.5f; // 카메라 침범 방지 최소 거리
    public float rotateSpeed = 250f;
    
    [Header("About GrabbedObject")]
    [SerializeField] private GameObject grabbedObject;
    [SerializeField] private Rigidbody objectRb;
    [SerializeField] private Collider grabbedCollider; 
    [SerializeField] private bool isHolding;

    private Vector3 objectBoxSize; // 물체의 실제 크기(Box형태 부피) 저장용

    void Update()
    {
        if (isHolding)
        {
            HoldObjectWithBoxCast();
        }

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
                
                // 물체의 실제 콜라이더 크기(바운드 외곽)를 계산해서 저장
                if (grabbedCollider != null)
                {
                    objectBoxSize = grabbedCollider.bounds.extents;
                }
                else
                {
                    objectBoxSize = new Vector3(0.3f, 0.3f, 0.3f);
                }

                // 플레이어를 밀쳐서 날아오르는 버그 방지를 위해 트리거 모드 ON
                if (grabbedCollider != null) grabbedCollider.isTrigger = true;

                objectRb.useGravity = false; 
                objectRb.isKinematic = true; 
                objectRb.freezeRotation = true;
                isHolding = true;
            }
        }
    }

    // ★ 다른 물체(오브젝트)들까지 가로막는 장애물로 인식하도록 수정된 핵심 로직
    void HoldObjectWithBoxCast()
    {
        if (grabbedObject == null) return;

        Vector3 startPos = Camera.transform.position;
        Vector3 dir = Camera.transform.forward;
        
        // 물체의 크기와 회전값을 그대로 반영한 두꺼운 박스 레이 발사 (모든 충돌체 감지)
        RaycastHit[] hits = Physics.BoxCastAll(startPos, objectBoxSize, dir, grabbedObject.transform.rotation, holdDistance);
        
        float targetDistance = holdDistance;

        // 거리가 가까운 순서대로 정렬하여 정밀도 향상
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        foreach (RaycastHit hit in hits)
        {
            // 예외 1: 자기 자신 콜라이더는 패스
            if (hit.collider.gameObject == grabbedObject) continue;
            
            // 예외 2: 플레이어 본인 몸통 패스
            if (player != null && hit.collider.transform.root == player.transform.root) continue;
            
            // 예외 3: 맵에 배치된 투명한 감지 영역(Trigger) 패스 (진짜 물리 콜라이더만 막히게)
            if (hit.collider.isTrigger) continue;

            // [수정 핵심]: 벽, 바닥뿐만 아니라 '다른 물체들'에 부딪혀도 거기를 마지노선으로 잡고 멈춤
            targetDistance = hit.distance;
            break;
        }

        // 최소 마지노선 거리와 최대 유지 거리 사이로 안전하게 고정
        targetDistance = Mathf.Clamp(targetDistance, minHoldDistance, holdDistance);

        // 부드럽고 묵직하게 목적지로 이동 처리
        Vector3 targetPos = startPos + dir * targetDistance;
        grabbedObject.transform.position = Vector3.Lerp(grabbedObject.transform.position, targetPos, Time.deltaTime * 25f);
    }

    void ReleaseObject()
    {
        if (grabbedObject == null || objectRb == null) return;

        // 놓을 때는 원래대로 단단한 고체 콜라이더로 복구
        if (grabbedCollider != null) grabbedCollider.isTrigger = false;

        objectRb.isKinematic = false;
        objectRb.useGravity = true; 
        objectRb.freezeRotation = false;
        
        objectRb.linearVelocity = Vector3.zero;
        objectRb.angularVelocity = Vector3.zero;

        isHolding = false;
    }

    void DropObject()
    {
        ReleaseObject();
        grabbedCollider = null;
        objectRb = null;
        grabbedObject = null;
    }
    
    void ThrowObject()
    {
        Rigidbody rbToThrow = objectRb; 
        ReleaseObject();
        
        Vector3 throwDirection = (Camera.transform.forward + Camera.transform.up * 0.1f).normalized;
        float throwForce = 10f; 
        
        rbToThrow.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        
        grabbedCollider = null;
        objectRb = null;
        grabbedObject = null;
    }
}