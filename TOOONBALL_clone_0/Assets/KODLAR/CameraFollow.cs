using UnityEngine;
using Photon.Pun;

public class CameraFollow : MonoBehaviourPun
{
    public Transform target; // Set to local player transform on start
    public Vector3 offset = new Vector3(0, 3, -5);
    public float smoothSpeed = 5f;

    private void Start()
    {
        if (photonView.IsMine)
        {
            Camera.main.transform.SetParent(null); // Detach main camera if any in scene
            target = transform;
            Camera.main.gameObject.AddComponent<CameraFollow>().enabled = true;
            enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}