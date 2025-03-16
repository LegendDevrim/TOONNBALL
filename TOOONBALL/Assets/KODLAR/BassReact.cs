using UnityEngine;

public class BassReact : MonoBehaviour
{
    public AudioSource audioSource; // AudioSource'u bağla
    public ParticleSystem particleSystem; // Particle System'i bağla
    private ParticleSystem.Particle[] particles; // Partikülleri tutmak için
    public int bassRangeStart = 20; // Bass frekansı başlangıç (Hz)
    public int bassRangeEnd = 200; // Bass frekansı bitiş (Hz)
    public float bassMultiplier = 2f; // Bass efekti çarpanı
    public float smoothTime = 0.3f; // Geçişin ne kadar yumuşak olacağı
    private float currentBassValue = 0f; // Şu anki bass değeri
    private float velocity = 0f; // Geçişin hızını kontrol et

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>(); // AudioSource yoksa, kendi AudioSource'u kullan
        }

        if (particleSystem == null)
        {
            particleSystem = GetComponent<ParticleSystem>(); // ParticleSystem yoksa, kendi ParticleSystem'i kullan
        }

        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles]; // Partiküller için dizi oluştur
    }

    void Update()
    {
        // Spectrum verilerini almak için
        float[] spectrum = new float[256];
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        // Bass frekanslarını al
        float bassValue = 0f;
        for (int i = bassRangeStart; i < bassRangeEnd; i++)
        {
            bassValue += spectrum[i];
        }

        // Bass verisini çarpanla artır
        bassValue *= bassMultiplier;

        // Bass değerini smooth bir şekilde geçiş yapacak şekilde güncelle
        currentBassValue = Mathf.SmoothDamp(currentBassValue, bassValue, ref velocity, smoothTime);

        // ParticleSystem'deki tüm partikülleri güncelle
        int numParticlesAlive = particleSystem.GetParticles(particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            // Her bir parçacığa bass efektini uygula
            particles[i].size = Mathf.Lerp(0.1f, 1.5f, currentBassValue); // Bass'a göre büyüklük
            particles[i].velocity = new Vector3(0, currentBassValue, 0); // Yön değiştirme (Y ekseninde yukarı doğru)
        }

        // Partikülleri tekrar ParticleSystem'e uygula
        particleSystem.SetParticles(particles, numParticlesAlive);
    }
}
