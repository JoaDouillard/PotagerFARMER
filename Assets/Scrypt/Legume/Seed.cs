using System.Collections;
using UnityEngine;

public class Seed : MonoBehaviour
{
    [Header("Identification")]
    [Tooltip("Type de légume que cette graine va produire")]
    public TypeGraine typeGraine;

    [Header("Économie")]
    [Tooltip("Prix d'achat de cette graine en dollars")]
    public int prixAchat = 10;

    [Header("Parametres de croissance")]
    [Tooltip("Temps avant transformation en légume (secondes)")]
    public float tempsCroissance = 5f;

    [Tooltip("Taille initiale de la graine")]
    public float scaleInitial = 0.1f;

    [Tooltip("Taille finale de la graine avant transformation")]
    public float scaleFinal = 1f;

    [Tooltip("Vitesse de croissance visuelle")]
    public float vitesseCroissance = 0.5f;

    [Tooltip("Offset vertical pour le spawn de la graine (négatif = enfoncé dans le sol)")]
    public float offsetHauteurSpawn = 0f;

    [Header("Transformation")]
    [Tooltip("Prefab du légume qui va apparaître après maturation")]
    public GameObject prefabLegume;

    [Header("Effets visuels")]
    public GameObject particulesTransformation;

    [Header("Debug")]
    public bool afficherDebug = true;

    private float tempsEcoule = 0f;
    private bool enCroissance = true;
    private Vector3 scaleTarget;
    private ZonePlantation zonePlantation;

    void Start()
    {
        // Assigner le layer "Seed" pour la détection de faillite
        int seedLayer = LayerMask.NameToLayer("Seed");
        if (seedLayer != -1)
        {
            gameObject.layer = seedLayer;
        }
        else
        {
            Debug.LogWarning("[Seed] Le layer 'Seed' n'existe pas ! Créez-le dans Project Settings > Tags & Layers.");
        }

        // Initialiser la taille de la graine (garder le scale du prefab si défini)
        if (scaleInitial > 0)
        {
            Vector3 prefabScale = transform.localScale;
            transform.localScale = prefabScale * scaleInitial;
            scaleTarget = prefabScale * scaleFinal;
        }

        // Démarrer la coroutine de transformation
        StartCoroutine(CroissanceEtTransformation());
    }

    void Update()
    {
        // Croissance progressive visuelle
        if (enCroissance && transform.localScale.x < scaleFinal)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                scaleTarget,
                Time.deltaTime * vitesseCroissance
            );
        }
    }


    // Coroutine qui gère la croissance et la transformation en légume
    IEnumerator CroissanceEtTransformation()
    {
        // Attendre le temps de croissance
        while (tempsEcoule < tempsCroissance)
        {
            tempsEcoule += Time.deltaTime;
            yield return null;
        }

        // Transformation en légume
        TransformerEnLegume();
    }

    void TransformerEnLegume()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.JouerSon(SoundManager.Instance.sonTransformationGraine);
        }

        if (prefabLegume == null)
        {
            Destroy(gameObject);
            return;
        }

        Legume legumeScriptPrefab = prefabLegume.GetComponent<Legume>();
        float offsetHauteur = 0f;
        if (legumeScriptPrefab != null)
        {
            offsetHauteur = legumeScriptPrefab.offsetHauteurSpawn;
        }

        Vector3 positionLegume = transform.position + Vector3.up * offsetHauteur;
        GameObject legume = Instantiate(prefabLegume, positionLegume, transform.rotation);

        int vegetableLayer = LayerMask.NameToLayer("Vegetable");
        if (vegetableLayer != -1)
        {
            legume.layer = vegetableLayer;
        }

        Legume legumeScript = legume.GetComponent<Legume>();
        if (legumeScript != null)
        {
            legumeScript.typeGraine = typeGraine;
        }

        if (zonePlantation != null)
        {
            zonePlantation.planteCourante = legume;

            if (afficherDebug)
            {
                Debug.Log($"[Seed] Zone mise à jour avec le légume {typeGraine}");
            }
        }

        if (particulesTransformation != null)
        {
            GameObject particules = Instantiate(particulesTransformation, transform.position, Quaternion.identity);
            Destroy(particules, 2f);
        }

        Destroy(gameObject);
    }


    public void Arroser(float accelerationMultiplier = 2f)
    {
        vitesseCroissance *= accelerationMultiplier;
        tempsCroissance /= accelerationMultiplier;

        if (afficherDebug)
        {
            Debug.Log($"[Seed] Graine arrosée ! Croissance accélérée.");
        }
    }

    public void AssignerZone(ZonePlantation zone)
    {
        zonePlantation = zone;
    }


    // Affichage visuel pour le debug
    void OnDrawGizmos()
    {
        if (afficherDebug && Application.isPlaying)
        {
            // Afficher une sphère pour indiquer la zone de la graine
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}
