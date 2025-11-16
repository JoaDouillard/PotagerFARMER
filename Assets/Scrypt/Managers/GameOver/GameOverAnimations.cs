using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameOverAnimations : MonoBehaviour
{
    [Header("Texte Game Over")]
    [Tooltip("TextMeshPro du titre GAME OVER")]
    public TextMeshProUGUI texteGameOver;

    [Tooltip("Durée de l'animation de fade-in (secondes)")]
    public float dureeFadeIn = 2f;

    [Tooltip("Durée de l'animation de zoom (secondes)")]
    public float dureeZoom = 1f;

    [Tooltip("Échelle finale du texte")]
    public float echelleFinal = 1.2f;

    [Header("Bouton Recommencer")]
    [Tooltip("Bouton Recommencer")]
    public Button boutonRecommencer;

    [Tooltip("Vitesse de pulsation")]
    public float vitessePulsation = 2f;

    [Tooltip("Intensité de la pulsation (1.0 à 1.5 recommandé)")]
    public float intensitePulsation = 1.15f;

    [Header("Musique et Sons")]
    [Tooltip("AudioSource pour la musique de fond")]
    public AudioSource musiqueGameOver;

    [Tooltip("AudioClip de la musique dramatique")]
    public AudioClip clipMusiqueGameOver;

    [Tooltip("AudioClip pour le son d'ambiance (vent, feu, etc.)")]
    public AudioClip clipAmbiance;

    [Tooltip("Volume de la musique (0 à 1)")]
    [Range(0f, 1f)]
    public float volumeMusique = 0.5f;

    [Tooltip("Volume de l'ambiance (0 à 1)")]
    [Range(0f, 1f)]
    public float volumeAmbiance = 0.3f;

    private Vector3 echelleBoutonInitiale;
    private AudioSource sourceAmbiance;

    void Start()
    {
        if (boutonRecommencer != null)
        {
            echelleBoutonInitiale = boutonRecommencer.transform.localScale;
        }

        StartCoroutine(AnimerTexteGameOver());
        StartCoroutine(AnimerBouton());
        JouerMusique();
    }

    IEnumerator AnimerTexteGameOver()
    {
        if (texteGameOver == null) yield break;

        Color couleurInitiale = texteGameOver.color;
        couleurInitiale.a = 0f;
        texteGameOver.color = couleurInitiale;

        Vector3 echelleInitiale = texteGameOver.transform.localScale;
        texteGameOver.transform.localScale = echelleInitiale * 0.5f;

        float tempsEcoule = 0f;

        while (tempsEcoule < dureeFadeIn)
        {
            tempsEcoule += Time.unscaledDeltaTime;
            float progression = tempsEcoule / dureeFadeIn;

            Color nouvelleCouleur = texteGameOver.color;
            nouvelleCouleur.a = Mathf.Lerp(0f, 1f, progression);
            texteGameOver.color = nouvelleCouleur;

            yield return null;
        }

        Color couleurFinale = texteGameOver.color;
        couleurFinale.a = 1f;
        texteGameOver.color = couleurFinale;

        tempsEcoule = 0f;
        while (tempsEcoule < dureeZoom)
        {
            tempsEcoule += Time.unscaledDeltaTime;
            float progression = tempsEcoule / dureeZoom;
            float ease = Mathf.Sin(progression * Mathf.PI * 0.5f);

            texteGameOver.transform.localScale = Vector3.Lerp(
                echelleInitiale * 0.5f,
                echelleInitiale * echelleFinal,
                ease
            );

            yield return null;
        }

        texteGameOver.transform.localScale = echelleInitiale * echelleFinal;
    }

    IEnumerator AnimerBouton()
    {
        if (boutonRecommencer == null) yield break;

        yield return new WaitForSecondsRealtime(dureeFadeIn + dureeZoom);

        while (true)
        {
            float temps = Time.unscaledTime * vitessePulsation;
            float facteur = 1f + (Mathf.Sin(temps) * 0.5f + 0.5f) * (intensitePulsation - 1f);

            boutonRecommencer.transform.localScale = echelleBoutonInitiale * facteur;

            yield return null;
        }
    }

    void JouerMusique()
    {
        if (musiqueGameOver != null && clipMusiqueGameOver != null)
        {
            musiqueGameOver.clip = clipMusiqueGameOver;
            musiqueGameOver.loop = true;
            musiqueGameOver.volume = volumeMusique;
            musiqueGameOver.Play();
        }

        if (clipAmbiance != null)
        {
            GameObject objetAmbiance = new GameObject("Ambiance");
            objetAmbiance.transform.SetParent(transform);
            sourceAmbiance = objetAmbiance.AddComponent<AudioSource>();
            sourceAmbiance.clip = clipAmbiance;
            sourceAmbiance.loop = true;
            sourceAmbiance.volume = volumeAmbiance;
            sourceAmbiance.Play();
        }
    }

    void OnDestroy()
    {
        if (musiqueGameOver != null)
        {
            musiqueGameOver.Stop();
        }

        if (sourceAmbiance != null)
        {
            sourceAmbiance.Stop();
        }
    }
}
