using UnityEngine;

public class GameWinSound : MonoBehaviour
{
    [Header("=== SON VICTOIRE (one-shot) ===")]
    [Tooltip("Son joué une seule fois au début de la victoire")]
    public AudioClip sonVictoire;

    [Header("=== THÈME VICTOIRE (loop) ===")]
    [Tooltip("Musique de victoire jouée en boucle")]
    public AudioClip themeVictoire;

    [Header("=== VOLUMES ===")]
    [Range(0f, 1f)]
    public float volumeSonVictoire = 0.7f;

    [Range(0f, 1f)]
    public float volumeTheme = 0.5f;

    private AudioSource audioSourceTheme;

    void Awake()
    {
        audioSourceTheme = gameObject.AddComponent<AudioSource>();
        audioSourceTheme.loop = true;
        audioSourceTheme.playOnAwake = false;
    }

    void Start()
    {
        JouerSonsVictoire();
    }

    void JouerSonsVictoire()
    {
        if (sonVictoire != null)
        {
            AudioSource.PlayClipAtPoint(sonVictoire, Camera.main.transform.position, volumeSonVictoire);
        }

        if (themeVictoire != null && audioSourceTheme != null)
        {
            audioSourceTheme.clip = themeVictoire;
            audioSourceTheme.volume = volumeTheme;
            audioSourceTheme.Play();
        }
    }

    void OnDestroy()
    {
        if (audioSourceTheme != null) audioSourceTheme.Stop();
    }
}
