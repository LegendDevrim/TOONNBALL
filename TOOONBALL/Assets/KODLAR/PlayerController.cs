using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Throwing")]
    public GameObject redBallPrefab;
    public GameObject blueBallPrefab;
    public Transform throwOrigin;
    public float throwForce = 10f;

    [Header("Visual")]
    public Renderer modelRenderer;
    public TextMesh nameText;

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private Rigidbody rb;
    private Camera mainCam;
    private Team myTeam;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCam = Camera.main;
        networkPosition = transform.position;
        networkRotation = transform.rotation;
    }

    private void Start()
    {
        // Takım atanır
        string teamStr = photonView.Owner.CustomProperties["Team"].ToString();
        myTeam = (Team)System.Enum.Parse(typeof(Team), teamStr);

        SetAppearance();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            HandleMovement();
            HandleRotation();
            HandleThrowing();
        }
        else
        {
            // Remote player smooth sync
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
        }
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v).normalized;

        if (dir.magnitude > 0.1f)
            rb.MovePosition(transform.position + dir * moveSpeed * Time.deltaTime);
    }

    void HandleRotation()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 lookDir = hit.point - transform.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 15f);
            }
        }
    }

    void HandleThrowing()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject prefab = myTeam == Team.Red ? redBallPrefab : blueBallPrefab;
            GameObject ball = PhotonNetwork.Instantiate(prefab.name, throwOrigin.position, Quaternion.identity);
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            ballRb.velocity = transform.forward * throwForce;
        }
    }

    void SetAppearance()
    {
        if (myTeam == Team.Red)
        {
            modelRenderer.material.color = Color.red;
            nameText.color = Color.red;
        }
        else
        {
            modelRenderer.material.color = Color.blue;
            nameText.color = Color.blue;
        }

        nameText.text = photonView.Owner.NickName;
    }

    // Network Sync
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
