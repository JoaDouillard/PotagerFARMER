using UnityEngine;

public class GameOverSound : MonoBehaviour
{
    [Header("=== MUSIQUE GAME OVER (loop) ===")]
    [Tooltip("Musique de Game Over (loop, jouée dans tous les cas)")]
    public AudioClip musiqueGameOver;

    [Header("=== SON SPÉCIFIQUE (one-shot) ===")]
    [Tooltip("Son de faillite (joué uniquement si faillite)")]
    public AudioClip sonFaillite;

    [Tooltip("Son de crash (joué uniquement si crash) - OPTIONNEL")]
    public AudioClip sonCrash;

    [Header("=== AMBIANCE (loop) ===")]
    [Tooltip("Son 1 (loop, joué dans tous les cas)")]
    public AudioClip ambiance1;

    [Tooltip("Son 2 (loop, joué dans tous les cas)")]
    public AudioClip ambiance2;

    [Header("=== VOLUMES ===")]
    [Range(0f, 1f)]
    public float volumeMusique = 0.4f;

    [Range(0f, 1f)]
    public float volumeSonSpecifique = 0.7f;

    [Range(0f, 1f)]
    public float volumeVent = 0.2f;

    [Range(0f, 1f)]
    public float volumeFeu = 0.2f;

    private AudioSource audioSourceMusique;
    private AudioSource audioSourceVent;
    private AudioSource audioSourceFeu;
    private bool estFaillite = false;

    void Awake()
    {
        audioSourceMusique = gameObject.AddComponent<AudioSource>();
        audioSourceMusique.loop = true;
        audioSourceMusique.playOnAwake = false;

        audioSourceVent = gameObject.AddComponent<AudioSource>();
        audioSourceVent.loop = true;
        audioSourceVent.playOnAwake = false;

        audioSourceFeu = gameObject.AddComponent<AudioSource>();
        audioSourceFeu.loop = true;
        audioSourceFeu.playOnAwake = false;
    }

    void Start()
    {
        string raison = GameOverManager.ObtenirRaisonGameOver();
        estFaillite = !string.IsNullOrEmpty(raison) && raison.Contains("FAILLITE");

        JouerSonsGameOver();
    }

    void JouerSonsGameOver()
    {
        // 1. MUSIQUE GAME OVER (loop, immédiat, tous les cas)
        if (musiqueGameOver != null && audioSourceMusique != null)
        {
            audioSourceMusique.clip = musiqueGameOver;
            audioSourceMusique.volume = volumeMusique;
            audioSourceMusique.Play();
            Debug.Log("[GameOverSound] Musique Game Over lancée");
        }

        // 2. SON SPÉCIFIQUE (one-shot, immédiat, selon le type de mort)
        if (estFaillite && sonFaillite != null)
        {
            AudioSource.PlayClipAtPoint(sonFaillite, Camera.main.transform.position, volumeSonSpecifique);
            Debug.Log("[GameOverSound] Son de faillite joué");
        }
        else if (!estFaillite && sonCrash != null)
        {
            AudioSource.PlayClipAtPoint(sonCrash, Camera.main.transform.position, volumeSonSpecifique);
            Debug.Log("[GameOverSound] Son de crash joué");
        }

        // 3. AMBIANCE 1 (loop, immédiat, tous les cas)
        if (ambiance1 != null && audioSourceVent != null)
        {
            audioSourceVent.clip = ambiance1;
            audioSourceVent.volume = volumeVent;
            audioSourceVent.Play();
            Debug.Log("[GameOverSound] Ambiance vent lancée");
        }

        // 4. AMBIANCE 2 (loop, immédiat, tous les cas)
        if (ambiance2 != null && audioSourceFeu != null)
        {
            audioSourceFeu.clip = ambiance2;
            audioSourceFeu.volume = volumeFeu;
            audioSourceFeu.Play();
            Debug.Log("[GameOverSound] Ambiance feu lancée");
        }
    }

    void OnDestroy()
    {
        if (audioSourceMusique != null) audioSourceMusique.Stop();
        if (audioSourceVent != null) audioSourceVent.Stop();
        if (audioSourceFeu != null) audioSourceFeu.Stop();
    }
}
