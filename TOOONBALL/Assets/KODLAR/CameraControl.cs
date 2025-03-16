using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform player; // Takip edilecek oyuncu
    public Vector3 offset = new Vector3(0, 7, -5); // Kamera açısı (ayarlanabilir)
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(player.position + Vector3.up * 1.5f); // Oyuncunun üstüne hafif odaklanma
    }
}