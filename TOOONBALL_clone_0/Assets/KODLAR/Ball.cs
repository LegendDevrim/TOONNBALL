using UnityEngine;

public class Ball : MonoBehaviour
{
    private BallGrab currentOwner;

    public void SetOwner(TopGrabber owner)
    {
        currentOwner = owner;
    }

    public bool HasOwner()
    {
        return currentOwner != null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Top yere çarptıysa sahipliği sıfırla
        currentOwner = null;
    }
}
