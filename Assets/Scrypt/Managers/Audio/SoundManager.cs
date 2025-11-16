using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("=== SONS GAMEPLAY ===")]
    [Header("Drone")]
    public AudioClip sonPlantation;
    public AudioClip sonChangementGraine;
    public AudioClip sonExplosionDrone;
    public AudioClip sonArrosoir;

    [Header("Légumes")]
    public AudioClip sonTransformationGraine;
    public AudioClip sonVibrationLegume;
    public AudioClip sonRecolteLegume;

    [Header("Argent")]
    public AudioClip sonGainArgent;
    public AudioClip sonDepenseArgent;

    [Header("=== SONS ZONES ===")]
    public AudioClip sonOuvertureShop;

    [Header("=== SONS UI ===")]
    public AudioClip sonOuverturePause;
    public AudioClip sonOuvertureInventaire;

    [Header("=== MUSIQUES ET AMBIANCE ===")]
    public AudioClip musiqueDeFond;
    public AudioClip sonMoteurDrone;

    [Header("=== CONFIGURATION ===")]
    [Range(0f, 1f)] public float volumeSFX = 0.7f;
    [Range(0f, 1f)] public float volumeUI = 0.5f;
    [Range(0f, 1f)] public float volumeMusique = 0.15f;

    private AudioSource audioSourceMusique;
    private AudioSource audioSourceMoteurDrone;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        ConfigurerAudioSources();
    }

    void ConfigurerAudioSources()
    {
        audioSourceMusique = gameObject.AddComponent<AudioSource>();
        audioSourceMusique.loop = true;
        audioSourceMusique.volume = volumeMusique;
        audioSourceMusique.playOnAwake = false;

        audioSourceMoteurDrone = gameObject.AddComponent<AudioSource>();
        audioSourceMoteurDrone.loop = true;
        audioSourceMoteurDrone.volume = 0f;
        audioSourceMoteurDrone.playOnAwake = false;
    }

    void Start()
    {
        if (musiqueDeFond != null)
        {
            audioSourceMusique.clip = musiqueDeFond;
            audioSourceMusique.Play();
        }

        if (sonMoteurDrone != null)
        {
            audioSourceMoteurDrone.clip = sonMoteurDrone;
            audioSourceMoteurDrone.Play();
        }
    }

    public void JouerSon(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume * volumeSFX);
        }
    }

    public void JouerSonUI(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volumeUI);
        }
    }

    public void AjusterVolumeMoteurDrone(float vitesse, float vitesseMax)
    {
        if (audioSourceMoteurDrone != null && sonMoteurDrone != null)
        {
            float volumeCible = Mathf.Clamp01(vitesse / vitesseMax) * 0.3f;
            audioSourceMoteurDrone.volume = Mathf.Lerp(audioSourceMoteurDrone.volume, volumeCible, Time.deltaTime * 3f);
        }
    }

    public void StopperMoteurDrone()
    {
        if (audioSourceMoteurDrone != null)
        {
            audioSourceMoteurDrone.volume = 0f;
        }
    }

    public void StopperMusique()
    {
        if (audioSourceMusique != null)
        {
            audioSourceMusique.Stop();
        }
    }
}
