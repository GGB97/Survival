using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed; // �̵� �ӵ�
    Vector2 curMovementInput;
    public float jumpForce;
    public LayerMask groundLayerMask;

    [Header("Look")]
    public Transform cameraContrainer;
    public float minXLook;
    public float maxXLook;
    float camCurXRot;
    public float lookSensitivity; // �ΰ���

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
        Cursor.lockState = CursorLockMode.Locked; // ��� ���ڴ� �Ҹ�?

        //GetComponent<PlayerInput>().actions["Move"].performed += OnMoveInput;
        //GetComponent<PlayerInput>().actions["Look"].performed += OnLookInput;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()   // LateUpdate�� ��� ó���� ������ �����ϱ� ������ ī�޶� �۾��� ���� �����
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x; // ĳ������ ���ִ� ���� ������ forwad, right
        dir *= moveSpeed;
        dir.y = _rigidbody.velocity.y; // y���� ������ ���־��� vel.y�� �������� �� ��ġ������ �� �� �ִ�? �ٷ� �Ʒ��� dir=vel �ϴ°� ������ �Ƹ� ����or�ϰ� ���� �ӵ����� ���� �Ϸ��� �ϴ� �� ����.

        _rigidbody.velocity = dir;
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity; // ���콺�� Y�� ���� �ϸ� ���콺�� ��/�� �������� ������ �ٵ� �� x���� �����ϴ°� -> x���� ȸ���� �� �� ��/�Ʒ� �� �ٶ󺸴� ���� ȸ���� �̷����.
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook); // ù ��° �Ű������� ���� min~max �� ������ ���� ex) min���� ������ min���� ����, max���� ũ�ٸ� max�� ����
        cameraContrainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0); // ���콺�� x����ŭ y�� +����
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        // InputActionPahse enum members
        // Started -> �Է� �׼��� ���۵� ����
        // Performed -> �Է� �׼��� ���� ���� ���� (������ �ִ� ����)
        // Canceld -> �Է� �׼��� ��ҵ� ���� 
        // Waiting -> �Է� �׼��� ��� ���� ����
        // Disabled -> �Է� �׼��� ��Ȱ��ȭ �� ����

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
        // Force -> ������ �������� ���� �����մϴ�. ���� ��ü�� ���������� ����˴ϴ�.
        // Acceleration -> ��ü�� ������ �����մϴ�. ��ü�� ����� ���� ������ ���� ���ӵ��� ��ȯ�˴ϴ�.
        // Impulse -> �������� ���� �����մϴ�. ���� �� �� ����ǰ�, �� ���Ŀ��� ������ �����ϴ�.
        // VelocityChange -> ��ü�� �ӵ��� �����ϴ� �� �ʿ��� ���� �����մϴ�. ���� �ӵ��� ������� ��ü�� �ӵ��� �����մϴ�.

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
            new Ray(transform.position + (transform.forward * .2f) + (Vector3.up * .1f), Vector3.down), // Player ���� ��/��/��/�� ���� �Ʒ��� ���̸� ���� �ٴ����� �Ǵ� �ϴ� �� ������. �̷��� �� �߹ٴڿ� ��������� �ٴ��̸� ��� �Ǵ°�?
            new Ray(transform.position + (-transform.forward * .2f) + (Vector3.up * .1f), Vector3.down), // ������ ���� ���� ���߿� ������ �׷��� ���̰� �ٴڿ� ����ִµ�. �� ������ �ȵǴ°���?
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

    private void OnDrawGizmos() // OnDrawGizmosSelected -> ���õǾ��� ���� ����� �׷���
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
