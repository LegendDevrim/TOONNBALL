using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    private Rigidbody rb;
    private bool isGrounded = true;

    [Header("Animations")]
    public GameObject idlePrefab;
    public GameObject walkPrefab;
    public GameObject walkBackPrefab;
    public GameObject walkRightPrefab;
    public GameObject tacklePrefab;
    public GameObject fallPrefab;

    void Start()
    {
        if (!isLocalPlayer) return;

        rb = GetComponent<Rigidbody>();

        SetActiveAnimation(idlePrefab);
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        if (moveDirection.magnitude > 0)
        {
            transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
            transform.forward = moveDirection; // Yönü değiştirme
        }

        // Animasyonları yönet
        if (Input.GetKey(KeyCode.W))
            SetActiveAnimation(walkPrefab);
        else if (Input.GetKey(KeyCode.S))
            SetActiveAnimation(walkBackPrefab);
        else if (Input.GetKey(KeyCode.D))
            SetActiveAnimation(walkRightPrefab);
        else
            SetActiveAnimation(idlePrefab);

        // Zıplama
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        // Çelme atma
        if (Input.GetMouseButtonDown(1)) // Sağ tık
        {
            SetActiveAnimation(tacklePrefab);
            Invoke(nameof(ResetIdle), 2f);
        }

        // Topu atma
        if (Input.GetMouseButtonDown(0)) // Sol tık
        {
            if (BallController.instance.currentPlayer == this)
                BallController.instance.ShootBall(transform.forward);
        }
    }

    void SetActiveAnimation(GameObject animPrefab)
    {
        idlePrefab.SetActive(false);
        walkPrefab.SetActive(false);
        walkBackPrefab.SetActive(false);
        walkRightPrefab.SetActive(false);
        tacklePrefab.SetActive(false);
        fallPrefab.SetActive(false);

        animPrefab.SetActive(true);
    }

    void ResetIdle()
    {
        SetActiveAnimation(idlePrefab);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }
}