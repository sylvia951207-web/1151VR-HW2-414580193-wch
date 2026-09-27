using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    // 陣列：依序存放一般移動速度、按住 Shift 時的跑步速度
    public float[] moveSpeeds = { 3f, 5f };
    public float jumpSpeed = 7f;

    private Rigidbody2D body;
    private BoxCollider2D box;
    private bool jumpRequested;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.spaceKey.wasPressedThisFrame && IsGrounded())
            jumpRequested = true;
    }

    void FixedUpdate()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        float direction = 0f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            direction = -1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            direction = 1f;

        int speedIndex = keyboard.leftShiftKey.isPressed ? 1 : 0;
        float speed = moveSpeeds[speedIndex];

        // Vector2：設定水平速度，同時保留目前的垂直速度
        body.linearVelocity = new Vector2(direction * speed, body.linearVelocity.y);

        if (jumpRequested)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpSpeed);
            jumpRequested = false;
        }

        // 在空中按 S 或下方向鍵，加快往下落
        if (!IsGrounded() &&
            (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed))
        {
            body.AddForce(Vector2.down * 12f);
        }
    }

    bool IsGrounded()
    {
        Vector2 feet = new Vector2(box.bounds.center.x, box.bounds.min.y + 0.02f);
        return Physics2D.Raycast(feet, Vector2.down, 0.12f);
    }
    private void OnTriggerEnter2D(Collider2D other)
{
    if (other.gameObject.name == "Spike")
    {
        enabled = false; // 停止接收移動、跳躍按鍵

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(0f, -10f); // 立刻往下墜

        GetComponent<Collider2D>().enabled = false; // 穿過地板
    }
}
}