using UnityEngine;
using Mirror;

public class BallController : NetworkBehaviour
{
    public static BallController instance;
    public Transform startPoint; // Maç başında topun merkezi

    private Rigidbody rb;
    public PlayerControl currentPlayer; // Burayı 'public' yaptık 🚀

    void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        ResetBall();
    }

    void ResetBall()
    {
        transform.position = startPoint.position;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        currentPlayer = null;
    }

    void Update()
    {
        if (currentPlayer != null)
        {
            transform.position = currentPlayer.transform.position + new Vector3(0, 0.5f, 0.6f);
        }
    }

    public void AssignBall(PlayerControl player)
    {
        currentPlayer = player;
        rb.isKinematic = true;
    }

    public void ReleaseBall()
    {
        currentPlayer = null;
        rb.isKinematic = false;
    }

    public void ShootBall(Vector3 direction)
    {
        ReleaseBall();
        rb.AddForce(direction * 10f, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentPlayer == null && other.CompareTag("Player"))
        {
            AssignBall(other.GetComponent<PlayerControl>());
        }
    }

    public void KnockAway()
    {
        ReleaseBall();
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f)).normalized;
        rb.AddForce(randomDirection * 5f, ForceMode.Impulse);
    }
}
