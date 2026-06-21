using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerOnRotatingPlatform : MonoBehaviour
{
    public enum MoveControlMode
    {
        WASD,
        ArrowKeys
    }

    public MoveControlMode moveControlMode = MoveControlMode.WASD;
    public float moveSpeed = 5f;
    public float jumpHeight = 1.5f;
    public float gravity = -20f;
    public int extraAirJumps = 1;
    public float platformDetectDistance = 10f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.5f;

    public Transform visualRoot; // 角色模型，可选，用来控制朝向

    private CharacterController controller;

    private Transform currentPlatform;
    private Vector3 localPointOnPlatform;

    private float verticalVelocity;
    private bool isInAir;
    private Vector3 dashDirection;
    private float dashTimer;
    private float dashCooldownTimer;
    private int remainingAirJumps;

    public bool IsInAir => isInAir;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        bool wasGrounded = controller.isGrounded;
        isInAir = !wasGrounded;

        // 1. 如果长射线检测到转盘，地面和空中都使用同一套转盘位移补偿
        if (currentPlatform != null)
        {
            Vector3 expectedWorldPos = currentPlatform.TransformPoint(localPointOnPlatform);
            Vector3 platformDelta = expectedWorldPos - transform.position;

            // 如果转盘只是水平旋转，可以只补偿水平位移
            platformDelta.y = 0f;

            controller.Move(platformDelta);
        }

        // 2. 玩家自己的移动
        Vector3 inputMove = GetMoveInput();
        float x = inputMove.x;
        UpdateDash(inputMove);

        Vector3 move = GetHorizontalMove(inputMove);

        // 3. 跳跃和重力
        if (wasGrounded)
        {
            remainingAirJumps = extraAirJumps;

            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space) && remainingAirJumps > 0)
        {
            remainingAirJumps--;
            Jump();
        }

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        // 4. 移动完成后，重新检测脚下是否是转盘
        DetectPlatformBelow();
        isInAir = !controller.isGrounded;

        // 5. 角色朝向可以独立处理，不受转盘影响
        UpdateVisualDirection(x);
    }

    private void DetectPlatformBelow()
    {
        Vector3 origin = controller.bounds.center;
        float rayDistance = Mathf.Max(platformDetectDistance, controller.bounds.extents.y);

        Debug.DrawRay(origin, Vector3.down * rayDistance, Color.red);

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayDistance))
        {
            RotatingPlatform platform = hit.collider.GetComponentInParent<RotatingPlatform>();

            if (platform != null)
            {
                currentPlatform = platform.transform;
                localPointOnPlatform = currentPlatform.InverseTransformPoint(transform.position);

                // 调试用，确认有没有检测成功
                Debug.Log("站在转盘上：" + platform.name);
            }
            else
            {
                currentPlatform = null;
                Debug.Log("踩到了东西，但不是转盘：" + hit.collider.name);
            }
        }
        else
        {
            currentPlatform = null;
            Debug.Log("脚下没有检测到任何东西");
        }
    }

    private void Jump()
    {
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void UpdateDash(Vector3 inputMove)
    {
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        if (dashTimer > 0f)
        {
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
                dashCooldownTimer = dashCooldown;

            return;
        }

        if (dashCooldownTimer > 0f || inputMove.sqrMagnitude <= 0f)
            return;

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            dashDirection = inputMove;
            dashTimer = dashDuration;
        }
    }

    private Vector3 GetHorizontalMove(Vector3 inputMove)
    {
        if (dashTimer > 0f)
            return dashDirection * dashSpeed;

        return inputMove * moveSpeed;
    }

    private Vector3 GetMoveInput()
    {
        float x = 0f;
        float z = 0f;

        if (moveControlMode == MoveControlMode.WASD)
        {
            if (Input.GetKey(KeyCode.A))
                x -= 1f;
            if (Input.GetKey(KeyCode.D))
                x += 1f;
            if (Input.GetKey(KeyCode.W))
                z += 1f;
            if (Input.GetKey(KeyCode.S))
                z -= 1f;
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftArrow))
                x += 1f;
            if (Input.GetKey(KeyCode.RightArrow))
                x -= 1f;
            if (Input.GetKey(KeyCode.UpArrow))
                z += 1f;
            if (Input.GetKey(KeyCode.DownArrow))
                z -= 1f;
        }

        return new Vector3(x, 0f, z).normalized;
    }

    private void UpdateVisualDirection(float horizontalInput)
    {
        if (visualRoot == null)
            return;

        if (horizontalInput > 0)
        {
            // 朝右
            visualRoot.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else if (horizontalInput < 0)
        {
            // 朝左
            visualRoot.rotation = Quaternion.Euler(0f, -90f, 0f);
        }
    }
}
