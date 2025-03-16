using UnityEngine;
using Mirror;

public class PlayerControl : NetworkBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    private Rigidbody rb;
    private bool isGrounded = true;
    private bool isDancing = false; // Dans durumu

    [Header("Animations")]
    public GameObject idlePrefab;
    public GameObject walkPrefab;
    public GameObject walkBackPrefab;
    public GameObject walkRightPrefab;
    public GameObject tacklePrefab;
    public GameObject fallPrefab;
    public GameObject runPrefab; // Koşma animasyonu
    public GameObject jumpPrefab; // Zıplama animasyonu
    public GameObject dancePrefab; // Dans animasyonu

    void Start()
    {
        if (!isLocalPlayer) return;

        rb = GetComponent<Rigidbody>();

        SetActiveAnimation(idlePrefab);
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        // Dans animasyonu kontrolü
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isDancing)
        {
            isDancing = true;
            SetActiveAnimation(dancePrefab); // Dans animasyonunu başlat
        }

        if (isDancing && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            isDancing = false; // Hareket etmeye başlandığında dans durur
            SetActiveAnimation(idlePrefab); // Hareket animasyonlarına geç
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // Hareket etme
        if (moveDirection.magnitude > 0)
        {
            transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
            transform.forward = moveDirection; // Yönü değiştirme

            // Koşma animasyonunu aktif et
            if (!isDancing) SetActiveAnimation(runPrefab); // Dans etmiyorsa koşma animasyonu
        }
        else if (!isDancing) // Dans etmiyorsa idle animasyonu oynat
        {
            SetActiveAnimation(idlePrefab);
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
            SetActiveAnimation(jumpPrefab); // Zıplama animasyonunu oynat
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
        runPrefab.SetActive(false); // Koşma animasyonunu kapat
        jumpPrefab.SetActive(false); // Zıplama animasyonunu kapat
        dancePrefab.SetActive(false); // Dans animasyonunu kapat

        animPrefab.SetActive(true); // Aktif animasyonu aç
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
