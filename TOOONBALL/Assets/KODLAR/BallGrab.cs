using UnityEngine;
using Photon.Pun;

public class BallGrab : MonoBehaviourPun
{
    public Transform ballHoldPoint;
    private GameObject attachedBall;

    private void Update()
    {
        if (!photonView.IsMine) return;

        if (attachedBall != null && Input.GetMouseButtonDown(0))
        {
            ThrowBall();
        }
    }

    private void ThrowBall()
    {
        Rigidbody ballRb = attachedBall.GetComponent<Rigidbody>();
        attachedBall.transform.parent = null;
        ballRb.isKinematic = false;
        ballRb.velocity = transform.forward * 15f; // fırlatma hızı
        attachedBall = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine || attachedBall != null) return;

        if (other.CompareTag("Ball"))
        {
            // top zaten başka oyuncuya ait mi kontrol et
            Ball ballScript = other.GetComponent<Ball>();
            if (ballScript.HasOwner()) return;

            attachedBall = other.gameObject;

            // Sıkı yapışma
            attachedBall.transform.position = ballHoldPoint.position;
            attachedBall.transform.rotation = ballHoldPoint.rotation;
            attachedBall.transform.parent = ballHoldPoint;
            Rigidbody rb = attachedBall.GetComponent<Rigidbody>();
            rb.isKinematic = true;

            // Sahiplik bilgisi gir
            ballScript.SetOwner(this);
        }
    }
}
