using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class VictoireAnimations : MonoBehaviour
{
    [Header("Texte Victoire")]
    [Tooltip("TextMeshPro du titre VICTOIRE")]
    public TextMeshProUGUI texteVictoire;

    [Tooltip("Durée de l'animation de fade-in (secondes)")]
    public float dureeFadeIn = 1.5f;

    [Tooltip("Durée de l'animation de zoom (secondes)")]
    public float dureeZoom = 0.8f;

    [Tooltip("Échelle finale du texte")]
    public float echelleFinal = 1.3f;

    [Header("Boutons")]
    [Tooltip("Bouton Rejouer")]
    public Button boutonRejouer;

    [Tooltip("Bouton Quitter")]
    public Button boutonQuitter;

    [Tooltip("Vitesse de pulsation du bouton Rejouer")]
    public float vitessePulsation = 2f;

    [Tooltip("Intensité de la pulsation")]
    public float intensitePulsation = 1.1f;

    [Header("Effets Visuels")]
    [Tooltip("Couleur du texte Victoire (doré par défaut)")]
    public Color couleurVictoire = new Color(1f, 0.84f, 0f);

    private Vector3 echelleBoutonInitiale;

    void Start()
    {
        if (boutonRejouer != null)
        {
            echelleBoutonInitiale = boutonRejouer.transform.localScale;
        }

        StartCoroutine(AnimerTexteVictoire());
        StartCoroutine(AnimerBouton());
    }

    IEnumerator AnimerTexteVictoire()
    {
        if (texteVictoire == null) yield break;

        Color couleurInitiale = couleurVictoire;
        couleurInitiale.a = 0f;
        texteVictoire.color = couleurInitiale;

        Vector3 echelleInitiale = texteVictoire.transform.localScale;
        texteVictoire.transform.localScale = echelleInitiale * 0.3f;

        float tempsEcoule = 0f;

        while (tempsEcoule < dureeFadeIn)
        {
            tempsEcoule += Time.unscaledDeltaTime;
            float progression = tempsEcoule / dureeFadeIn;

            Color nouvelleCouleur = couleurVictoire;
            nouvelleCouleur.a = Mathf.Lerp(0f, 1f, progression);
            texteVictoire.color = nouvelleCouleur;

            yield return null;
        }

        Color couleurFinale = couleurVictoire;
        couleurFinale.a = 1f;
        texteVictoire.color = couleurFinale;

        tempsEcoule = 0f;
        while (tempsEcoule < dureeZoom)
        {
            tempsEcoule += Time.unscaledDeltaTime;
            float progression = tempsEcoule / dureeZoom;
            float ease = 1f - Mathf.Pow(1f - progression, 3f);

            texteVictoire.transform.localScale = Vector3.Lerp(
                echelleInitiale * 0.3f,
                echelleInitiale * echelleFinal,
                ease
            );

            yield return null;
        }

        texteVictoire.transform.localScale = echelleInitiale * echelleFinal;

        StartCoroutine(AnimerScintillement());
    }

    IEnumerator AnimerScintillement()
    {
        if (texteVictoire == null) yield break;

        while (true)
        {
            float temps = Time.unscaledTime * 3f;
            float facteur = 0.8f + Mathf.Sin(temps) * 0.2f;

            Color couleur = couleurVictoire;
            couleur.r *= facteur;
            couleur.g *= facteur;
            couleur.b *= facteur;
            texteVictoire.color = couleur;

            yield return null;
        }
    }

    IEnumerator AnimerBouton()
    {
        if (boutonRejouer == null) yield break;

        yield return new WaitForSecondsRealtime(dureeFadeIn + dureeZoom);

        while (true)
        {
            float temps = Time.unscaledTime * vitessePulsation;
            float facteur = 1f + (Mathf.Sin(temps) * 0.5f + 0.5f) * (intensitePulsation - 1f);

            boutonRejouer.transform.localScale = echelleBoutonInitiale * facteur;

            yield return null;
        }
    }
}
