using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BouncingBall : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Bounce Settings")]
    [Tooltip("Başlangıç zıplama kuvveti")]
    public float initialBounceForce = 5f;

    [Tooltip("Sekme kuvveti azalma oranı (0-1)")]
    [Range(0.1f, 1f)]
    public float bounceDamping = 0.8f;

    [Tooltip("Zıplamanın duracağı minimum kuvvet eşiği")]
    public float minBounceThreshold = 0.2f;

    private float currentBounceForce;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentBounceForce = initialBounceForce;
        Bounce();
    }

    void FixedUpdate()
    {
        if (isGrounded && Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            if (currentBounceForce > minBounceThreshold)
            {
                currentBounceForce *= bounceDamping;
                Bounce();
            }
            else
            {
                currentBounceForce = 0f;
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
    }

    void Bounce()
    {
        rb.velocity = new Vector3(0, currentBounceForce, 0);
        isGrounded = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }
}
