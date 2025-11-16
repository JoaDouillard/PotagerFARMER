using System.Collections;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

public class Legume : MonoBehaviour
{
    [Header("Paramètres de croissance")]
    [Tooltip("Taille initiale du légume")]
    public float scaleInitial = 1f;

    [Tooltip("Taille maximale avant explosion")]
    public float scaleMax = 2.5f;

    [Tooltip("Vitesse de croissance du légume")]
    public float vitesseCroissance = 0.3f;

    [Tooltip("Offset vertical pour le spawn du légume (négatif = enfoncé dans le sol)")]
    public float offsetHauteurSpawn = 0f;

    [Header("Paramètres de maturité")]
    [Tooltip("Pourcentage de maturité avant de pouvoir récolter (0-1)")]
    [Range(0f, 1f)]
    public float seuilMaturiteRecolte = 0.5f;

    [Tooltip("Pourcentage où la vibration commence (0-1)")]
    [Range(0f, 1f)]
    public float seuilDebutVibration = 0.5f;

    [Header("Paramètres de vibration")]
    [Tooltip("Intensité maximale de la vibration")]
    public float intensiteVibrationMax = 0.3f;

    [Tooltip("Vitesse de vibration (Hz)")]
    public float vitesseVibration = 5f;

    [Header("Paramètres d'explosion")]
    [Tooltip("Temps avant explosion (en secondes)")]
    public float tempsAvantExplosion = 10f;

    [Tooltip("Prefab de l'effet d'explosion")]
    public GameObject prefabExplosion;

    [Header("Paramètres de récolte")]
    [Tooltip("Type de graine/légume pour l'inventaire")]
    public TypeGraine typeGraine;

    [Tooltip("Configuration des prix par rareté")]
    public ConfigurationPrixLegumes configPrix;

    [Header("Effets visuels")]
    [Tooltip("Particules de récolte")]
    public GameObject particulesRecolte;

    [Header("Effets sonores (optionnel)")]
    [Tooltip("Son de récolte")]
    public AudioClip sonRecolte;

    [Tooltip("Son d'explosion")]
    public AudioClip sonExplosion;

    [Header("Changement de couleur")]
    [Tooltip("Activer le changement de couleur progressif")]
    public bool changerCouleur = true;

    [Tooltip("Couleur initiale (jeune)")]
    public Color couleurJeune = Color.green;

    [Tooltip("Couleur finale (mûr/dangereux)")]
    public Color couleurMur = Color.red;

    [Header("Debug")]
    public bool afficherDebug = true;

    private float tempsEcoule = 0f;
    private bool estRecolte = false;
    private bool estRecoltable = false;
    private bool estArrose = false;
    private Renderer rendererLegume;
    private AudioSource audioSource;
    private Vector3 positionInitiale;
    private MotionHandle motionVibration;
    private Coroutine coroutineExplosion;

    void Start()
    {
        // Assigner le layer "Vegetable" pour la détection de faillite
        int vegetableLayer = LayerMask.NameToLayer("Vegetable");
        if (vegetableLayer != -1)
        {
            gameObject.layer = vegetableLayer;
        }
        else
        {
            Debug.LogWarning("[Legume] Le layer 'Vegetable' n'existe pas ! Créez-le dans Project Settings > Tags & Layers.");
        }

        Vector3 prefabScale = transform.localScale;
        transform.localScale = prefabScale * scaleInitial;
        positionInitiale = transform.position;

        rendererLegume = GetComponent<Renderer>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (sonRecolte != null || sonExplosion != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        coroutineExplosion = StartCoroutine(TimerExplosion());

        if (afficherDebug)
        {
            Debug.Log($"[Legume] Légume créé. Explosion dans {tempsAvantExplosion}s. Récoltable à partir de {seuilMaturiteRecolte * 100}%");
        }
    }

    void Update()
    {
        if (!estRecolte)
        {
            if (transform.localScale.x < scaleMax)
            {
                float nouveauScale = transform.localScale.x + (vitesseCroissance * Time.deltaTime);
                transform.localScale = Vector3.one * Mathf.Min(nouveauScale, scaleMax);
            }

            if (changerCouleur && rendererLegume != null)
            {
                float progression = tempsEcoule / tempsAvantExplosion;
                Color couleurActuelle = Color.Lerp(couleurJeune, couleurMur, progression);
                rendererLegume.material.color = couleurActuelle;
            }

            tempsEcoule += Time.deltaTime;
            float maturite = tempsEcoule / tempsAvantExplosion;

            if (!estRecoltable && maturite >= seuilMaturiteRecolte)
            {
                estRecoltable = true;
                if (afficherDebug)
                {
                    Debug.Log($"[Legume] Légume maintenant récoltable !");
                }
            }

            if (estRecoltable && ShopManager.Instance != null)
            {
                RareteLegume rareteActuelle = RareteHelper.DeterminerRarete(maturite, seuilMaturiteRecolte, 1.0f);
                RareteLegume niveauAutoRecolte = ShopManager.Instance.niveauAutoRecolte;

                if (niveauAutoRecolte != RareteLegume.Aucun && rareteActuelle >= niveauAutoRecolte)
                {
                    if (afficherDebug)
                    {
                        Debug.Log($"[Legume] Auto-récolte déclenchée ! Rareté: {RareteHelper.ObtenirNomRarete(rareteActuelle)}");
                    }
                    Recolter();
                }
            }

            if (maturite >= seuilDebutVibration)
            {
                DemarrerVibration(maturite);
            }
        }
    }

    void ActiverRecolte()
    {
        estRecoltable = true;
       
        if (afficherDebug)
        {
            Debug.Log($"[Legume] Légume maintenant récoltable !");
        }
    }

    void DemarrerVibration(float maturite)
    {
        if (motionVibration.IsActive()) return;

        if (SoundManager.Instance != null && SoundManager.Instance.sonVibrationLegume != null)
        {
            AudioSource.PlayClipAtPoint(SoundManager.Instance.sonVibrationLegume, transform.position, 0.5f);
        }

        float progressionVibration = (maturite - seuilDebutVibration) / (1f - seuilDebutVibration);
        float intensiteActuelle = Mathf.Lerp(0.05f, intensiteVibrationMax, progressionVibration);

        Vector3 positionCible = positionInitiale + new Vector3(
            Random.Range(-intensiteActuelle, intensiteActuelle),
            0f,
            Random.Range(-intensiteActuelle, intensiteActuelle)
        );

        motionVibration = LMotion.Create(positionInitiale, positionCible, 1f / vitesseVibration)
            .WithLoops(-1, LoopType.Yoyo)
            .WithEase(Ease.InOutSine)
            .Bind(value => transform.position = value);

        StartCoroutine(MettreAJourVibration());
    }

    System.Collections.IEnumerator MettreAJourVibration()
    {
        while (motionVibration.IsActive() && !estRecolte)
        {
            yield return new WaitForSeconds(1f / vitesseVibration);

            if (motionVibration.IsActive())
            {
                motionVibration.Cancel();
            }

            float maturite = tempsEcoule / tempsAvantExplosion;
            float progressionVibration = (maturite - seuilDebutVibration) / (1f - seuilDebutVibration);
            float intensiteActuelle = Mathf.Lerp(0.05f, intensiteVibrationMax, progressionVibration);

            Vector3 positionCible = positionInitiale + new Vector3(
                Random.Range(-intensiteActuelle, intensiteActuelle),
                0f,
                Random.Range(-intensiteActuelle, intensiteActuelle)
            );

            motionVibration = LMotion.Create(transform.position, positionCible, 1f / vitesseVibration)
                .WithEase(Ease.InOutSine)
                .Bind(value => transform.position = value);
        }
    }

    // Coroutine qui gère le timer d'explosion
    IEnumerator TimerExplosion()
    {
        yield return new WaitForSeconds(tempsAvantExplosion);

        // Si le légume n'a pas été récolté, il explose
        if (!estRecolte)
        {
            Exploser();
        }
    }


    public void Recolter()
    {
        if (estRecolte)
        {
            if (afficherDebug)
            {
                Debug.LogWarning("[Legume] Déjà récolté !");
            }
            return;
        }

        if (!estRecoltable)
        {
            if (afficherDebug)
            {
                Debug.LogWarning("[Legume] Pas encore récoltable ! (pas assez mûr)");
            }
            return;
        }

        estRecolte = true;

        if (motionVibration.IsActive())
        {
            motionVibration.Cancel();
        }

        float maturite = tempsEcoule / tempsAvantExplosion;
        RareteLegume rarete = RareteHelper.DeterminerRarete(maturite, seuilMaturiteRecolte, 1.0f);
        int prixVente = CalculerPrixVente(rarete);

        if (afficherDebug)
        {
            Debug.Log($"[Legume] Récolté : {typeGraine} à {maturite * 100:F0}% de maturité - Rareté: {RareteHelper.ObtenirNomRarete(rarete)} - Prix: {prixVente}$");
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AjouterLegume(typeGraine, rarete, prixVente);
        }
        else
        {
            Debug.LogWarning("[Legume] InventoryManager introuvable !");
        }

        if (SoundManager.Instance != null && SoundManager.Instance.sonRecolteLegume != null)
        {
            AudioSource.PlayClipAtPoint(SoundManager.Instance.sonRecolteLegume, transform.position, 0.7f);
        }

        if (particulesRecolte != null)
        {
            GameObject particules = Instantiate(particulesRecolte, transform.position, Quaternion.identity);
            Destroy(particules, 2f);
        }

        Destroy(gameObject, 0.1f);
    }

    public bool EstRecoltable()
    {
        return estRecoltable && !estRecolte;
    }

    int CalculerPrixVente(RareteLegume rarete)
    {
        if (configPrix == null)
        {
            Debug.LogWarning("[Legume] ConfigurationPrixLegumes non assigné !");
            return 10; // Prix par défaut
        }

        return configPrix.ObtenirPrix(typeGraine, rarete);
    }


    void Exploser()
    {
        if (motionVibration.IsActive())
        {
            motionVibration.Cancel();
        }

        if (afficherDebug)
        {
            Debug.Log($"[Legume] EXPLOSION ! Le légume était trop mûr.");
        }

        if (sonExplosion != null)
        {
            AudioSource.PlayClipAtPoint(sonExplosion, transform.position, 0.7f);
        }

        if (prefabExplosion != null)
        {
            GameObject explosion = Instantiate(prefabExplosion, transform.position, Quaternion.identity);
            Destroy(explosion, 3f);
        }

        Destroy(gameObject, 0.1f);
    }

    void OnDestroy()
    {
        if (motionVibration.IsActive())
        {
            motionVibration.Cancel();
        }
    }

    public void Arroser()
    {
        if (estArrose)
        {
            return;
        }

        estArrose = true;

        vitesseCroissance *= 2f;
        tempsAvantExplosion *= 0.5f;

        if (coroutineExplosion != null)
        {
            StopCoroutine(coroutineExplosion);
        }
        coroutineExplosion = StartCoroutine(TimerExplosion());

        if (afficherDebug)
        {
            Debug.Log($"[Legume] Légume arrosé ! Vitesse: {vitesseCroissance}, Temps explosion restant: {tempsAvantExplosion - tempsEcoule}s");
        }
    }

    public void ArreterArrosage()
    {
        if (!estArrose)
        {
            return;
        }

        estArrose = false;

        vitesseCroissance /= 2f;
        tempsAvantExplosion *= 2f;

        if (coroutineExplosion != null)
        {
            StopCoroutine(coroutineExplosion);
        }
        coroutineExplosion = StartCoroutine(TimerExplosion());

        if (afficherDebug)
        {
            Debug.Log($"[Legume] Arrosage terminé ! Vitesse: {vitesseCroissance}, Temps explosion restant: {tempsAvantExplosion - tempsEcoule}s");
        }
    }

}
