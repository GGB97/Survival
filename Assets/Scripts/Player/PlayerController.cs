using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed; // 이동 속도
    Vector2 curMovementInput;
    public float jumpForce;
    public LayerMask groundLayerMask;

    [Header("Look")]
    public Transform cameraContrainer;
    public float minXLook;
    public float maxXLook;
    float camCurXRot;
    public float lookSensitivity; // 민감도

    Vector2 mouseDelta;

    [HideInInspector]
    public bool canLook = true;

    Rigidbody _rigidbody;

    public static PlayerController instance;

    private void Awake()
    {
        instance = this;
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 잠궈 놓겠단 소리?

        //GetComponent<PlayerInput>().actions["Move"].performed += OnMoveInput;
        //GetComponent<PlayerInput>().actions["Look"].performed += OnLookInput;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()   // LateUpdate는 모든 처리가 끝나고 동작하기 떄문에 카메라 작업에 많이 사용함
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x; // 캐릭터의 서있는 상태 에서의 forwad, right
        dir *= moveSpeed;
        dir.y = _rigidbody.velocity.y; // y값은 어차피 없애야함 vel.y를 가져오면 그 위치정도를 쓸 수 있다? 바로 아래에 dir=vel 하는걸 봐서는 아마 점프or하강 때의 속도값을 유지 하려고 하는 것 같음.

        _rigidbody.velocity = dir;
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity; // 마우스의 Y가 변동 하면 마우스는 상/하 방향으로 움직임 근데 왜 x값을 변경하는가 -> x축이 회전을 할 때 위/아래 를 바라보는 듯한 회전이 이루어짐.
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook); // 첫 번째 매개변수의 값을 min~max 의 값으로 변형 ex) min보다 작으면 min으로 고정, max보다 크다면 max로 고정
        cameraContrainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0); // 마우스의 x값만큼 y에 +해줌
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        // InputActionPahse enum members
        // Started -> 입력 액션이 시작된 상태
        // Performed -> 입력 액션이 수행 중인 상태 (누르고 있는 상태)
        // Canceld -> 입력 액션이 취소된 상태 
        // Waiting -> 입력 액션이 대기 중인 상태
        // Disabled -> 입력 액션이 비활성화 된 상태

        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        // ForceMode enum Members
        // Force -> 지정된 방향으로 힘을 적용합니다. 힘은 물체에 지속적으로 적용됩니다.
        // Acceleration -> 물체에 가속을 적용합니다. 물체에 적용된 힘이 질량에 따라 가속도로 변환됩니다.
        // Impulse -> 순간적인 힘을 적용합니다. 힘이 한 번 적용되고, 그 이후에는 영향이 없습니다.
        // VelocityChange -> 물체의 속도를 변경하는 데 필요한 힘을 적용합니다. 현재 속도와 관계없이 물체의 속도를 변경합니다.

        if (context.phase == InputActionPhase.Started)
        {
            if (IsGrounded())
                _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
        }
    }


    private bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * .2f) + (Vector3.up * .1f), Vector3.down), // Player 기준 앞/뒤/좌/우 에서 아래로 레이를 쏴서 바닥인지 판단 하는 것 같은데. 이러면 딱 발바닥에 닿는정도의 바닥이면 어떻게 되는가?
            new Ray(transform.position + (-transform.forward * .2f) + (Vector3.up * .1f), Vector3.down), // 씬에서 보면 점프 도중에 기즈모로 그려진 레이가 바닥에 닿아있는데. 왜 점프가 안되는거지?
            new Ray(transform.position + (transform.right* .2f) + (Vector3.up * .1f), Vector3.down),
            new Ray(transform.position + (-transform.right * .2f) + (Vector3.up * .1f), Vector3.down)
        };


        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], .1f, groundLayerMask))
                return true;
        }

        return false;
    }

    private void OnDrawGizmos() // OnDrawGizmosSelected -> 선택되었을 때만 기즈모 그려줌
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position + (transform.forward * .2f) + (Vector3.up * .1f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.forward * .2f) + (Vector3.up * .1f), Vector3.down);
        Gizmos.DrawRay(transform.position + (transform.right * .2f) + (Vector3.up * .1f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.right * .2f) + (Vector3.up * .1f), Vector3.down);
    }

    public void ToggleCursor(bool toggle)
    {
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}
