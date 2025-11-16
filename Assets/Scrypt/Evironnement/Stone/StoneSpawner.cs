using UnityEngine;

public class StoneSpawner : MonoBehaviour
{
    [Header("Prefabs de pierres")]
    [Tooltip("Liste des prefabs de pierres a spawner")]
    public GameObject[] stonePrefabs;

    [Header("Configuration de la zone")]
    [Tooltip("Nombre total de pierres a spawner")]
    public int nombrePierres = 200;

    [Tooltip("Taille de la zone de spawn (X et Z)")]
    public Vector2 tailleZone = new Vector2(350, 350);

    [Tooltip("Hauteur du sol par defaut")]
    public float hauteurSol = 2.6f;

    [Header("Zone degagee au centre")]
    [Tooltip("Rayon du cercle degage au centre (pas de pierres)")]
    public float rayonZoneDegagee = 50f;

    [Tooltip("Rayon de transition (de clairseme a dense)")]
    public float rayonTransition = 100f;

    [Tooltip("Courbe de densite (0 = centre, 1 = bord)")]
    public AnimationCurve courbeDensite;

    [Header("Eviter les obstacles")]
    [Tooltip("Rayon de detection des obstacles")]
    public float rayonDetectionObstacle = 2f;

    [Tooltip("Layer des obstacles (DOIT contenir 'Obstacle')")]
    public LayerMask layerObstacle;

    [Tooltip("Distance minimale entre pierres")]
    public float distanceMinEntrePierres = 2f;

    [Tooltip("Layer des objets crees")]
    public LayerMask layerCreate;

    [Header("Variation")]
    [Tooltip("Scale minimum des pierres")]
    public float scaleMin = 0.5f;

    [Tooltip("Scale maximum des pierres")]
    public float scaleMax = 2.5f;

    [Tooltip("Variation de scale selon la distance")]
    public bool variationScaleParDistance = true;

    [Header("Clustering (groupes naturels)")]
    [Tooltip("Activer le clustering pour effet naturel")]
    public bool activerClustering = true;

    [Tooltip("Nombre de centres de clusters")]
    public int nombreClusters = 15;

    [Tooltip("Rayon des clusters")]
    public float rayonCluster = 15f;

    [Tooltip("Probabilite de spawn dans un cluster (0-1)")]
    public float probabiliteCluster = 0.8f;

    [Header("Rotation")]
    [Tooltip("Rotation aleatoire max sur X")]
    public float rotationMaxX = 20f;

    [Tooltip("Rotation aleatoire max sur Z")]
    public float rotationMaxZ = 20f;

    [Header("Debug")]
    [Tooltip("Afficher les informations de debug")]
    public bool afficherDebug = true;

    [Tooltip("Afficher les gizmos de zones")]
    public bool afficherGizmos = true;

    private Vector3[] centresClusters;

    void Start()
    {
        // Configurer la courbe de densite par defaut
        if (courbeDensite == null || courbeDensite.length == 0)
        {
            ConfigurerCourbeDensiteParDefaut();
        }

        // Generer les centres de clusters
        if (activerClustering)
        {
            GenererCentresClusters();
        }

        // Spawner les pierres
        SpawnerPierres();
    }

    // Configure une courbe de densite par defaut
    // Les pierres sont plus denses aux bords

    void ConfigurerCourbeDensiteParDefaut()
    {
        courbeDensite = new AnimationCurve();
        courbeDensite.AddKey(0f, 0f);    // Centre = 0% de densite
        courbeDensite.AddKey(0.4f, 0.1f); // Transition debut
        courbeDensite.AddKey(0.7f, 0.5f); // Zone intermediaire
        courbeDensite.AddKey(0.85f, 0.8f); // Zone dense commence
        courbeDensite.AddKey(1f, 1f);     // Bord = 100% de densite
    }

    // Genere les centres des clusters
    void GenererCentresClusters()
    {
        centresClusters = new Vector3[nombreClusters];

        for (int i = 0; i < nombreClusters; i++)
        {
            // Position aleatoire mais pas trop proche du centre
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(rayonZoneDegagee + 15f, tailleZone.x / 2f - 15f);

            float x = Mathf.Cos(angle) * distance;
            float z = Mathf.Sin(angle) * distance;

            centresClusters[i] = new Vector3(x, 0f, z);
        }

        if (afficherDebug)
        {
            Debug.Log($"[StoneSpawner] {nombreClusters} centres de clusters generes");
        }
    }

    // Spawner toutes les pierres avec distribution naturelle
    void SpawnerPierres()
    {
        if (stonePrefabs == null || stonePrefabs.Length == 0)
        {
            Debug.LogWarning("[StoneSpawner] Aucun prefab de pierre assigne !");
            return;
        }

        int spawned = 0;
        int tentatives = 0;
        int maxTentatives = nombrePierres * 50;

        while (spawned < nombrePierres && tentatives < maxTentatives)
        {
            tentatives++;

            // Generer une position
            Vector3 position = GenererPositionNaturelle();

            // Verifier si la position est valide
            if (!PositionEstValide(position))
            {
                continue;
            }

            // Ajuster la hauteur avec raycast
            position = AjusterHauteurAvecRaycast(position);

            // Choisir un prefab aleatoire
            GameObject prefab = stonePrefabs[Random.Range(0, stonePrefabs.Length)];

            // Spawner la pierre
            GameObject pierre = Instantiate(prefab, position, Quaternion.identity, transform);
            pierre.layer = LayerMask.NameToLayer("Create");

            // Rotation aleatoire (pierres peuvent etre inclinees)
            pierre.transform.rotation = Quaternion.Euler(
                Random.Range(-rotationMaxX, rotationMaxX),
                Random.Range(0f, 360f),
                Random.Range(-rotationMaxZ, rotationMaxZ)
            );

            // Scale variable
            float scale = CalculerScale(position);
            pierre.transform.localScale = Vector3.one * scale;

            spawned++;
        }

        if (afficherDebug)
        {
            float taux = (float)spawned / tentatives * 100f;
            Debug.Log($"[StoneSpawner] {spawned}/{nombrePierres} pierres spawnees ({tentatives} tentatives, taux: {taux:F1}%)");
        }
    }


    // Genere une position naturelle basee sur la densite et les clusters
    Vector3 GenererPositionNaturelle()
    {
        Vector3 position;

        // Decide si on utilise un cluster ou une position aleatoire
        if (activerClustering && centresClusters != null && Random.value < probabiliteCluster)
        {
            // Position dans un cluster
            Vector3 centreCluster = centresClusters[Random.Range(0, centresClusters.Length)];
            Vector2 offset = Random.insideUnitCircle * rayonCluster;
            position = centreCluster + new Vector3(offset.x, 0f, offset.y);
        }
        else
        {
            // Position aleatoire ponderee par la densite
            position = GenererPositionPonderee();
        }

        return position;
    }


    // Genere une position ponderee par la courbe de densite
    Vector3 GenererPositionPonderee()
    {
        int maxTentatives = 20;
        for (int i = 0; i < maxTentatives; i++)
        {
            // Position aleatoire dans toute la zone
            float x = Random.Range(-tailleZone.x / 2, tailleZone.x / 2);
            float z = Random.Range(-tailleZone.y / 2, tailleZone.y / 2);
            Vector3 pos = new Vector3(x, 0f, z);

            // Calculer la distance au centre (normalise 0-1)
            float distanceCentre = new Vector2(x, z).magnitude;
            float distanceMax = tailleZone.x / 2f;
            float pourcentageDistance = Mathf.Clamp01(distanceCentre / distanceMax);

            // Obtenir la densite cible depuis la courbe
            float densiteCible = courbeDensite.Evaluate(pourcentageDistance);

            // Accepter ou rejeter selon la densite
            if (Random.value < densiteCible)
            {
                return pos;
            }
        }

        // Fallback: position aleatoire simple
        return new Vector3(
            Random.Range(-tailleZone.x / 2, tailleZone.x / 2),
            0f,
            Random.Range(-tailleZone.y / 2, tailleZone.y / 2)
        );
    }


    // Verifie si une position est valide pour spawner
    bool PositionEstValide(Vector3 position)
    {
        // Verifier si dans la zone degagee
        float distanceCentre = new Vector2(position.x, position.z).magnitude;
        if (distanceCentre < rayonZoneDegagee)
        {
            return false;
        }

        // Verifier collision avec obstacles (layer Obstacle)
        if (Physics.CheckSphere(position, rayonDetectionObstacle, layerObstacle))
        {
            return false;
        }

        // Verifier distance minimale avec autres pierres
        if (Physics.CheckSphere(position, distanceMinEntrePierres, layerCreate))
        {
            return false;
        }

        return true;
    }


    // Ajuste la hauteur avec un raycast vers le sol
    Vector3 AjusterHauteurAvecRaycast(Vector3 position)
    {
        Vector3 positionHaute = position + Vector3.up * 100f;

        RaycastHit hit;
        if (Physics.Raycast(positionHaute, Vector3.down, out hit, 200f))
        {
            position.y = hit.point.y;
        }
        else
        {
            position.y = hauteurSol;
        }

        return position;
    }

    // Calcule le scale selon la distance au centre
    // Les pierres sont plus grosses aux bords
    float CalculerScale(Vector3 position)
    {
        if (!variationScaleParDistance)
        {
            return Random.Range(scaleMin, scaleMax);
        }

        // Plus petit au centre, plus grand aux bords
        float distanceCentre = new Vector2(position.x, position.z).magnitude;
        float distanceMax = tailleZone.x / 2f;
        float pourcentage = Mathf.Clamp01(distanceCentre / distanceMax);

        // Scale augmente avec la distance
        float scaleBase = Mathf.Lerp(scaleMin, scaleMax, pourcentage);

        // Ajouter une variation aleatoire
        float variation = Random.Range(-0.3f, 0.3f);

        return Mathf.Clamp(scaleBase + variation, scaleMin, scaleMax);
    }


    // Regenerer toutes les pierres (pour tests)
    public void RegenerPierres()
    {
        // Detruire toutes les pierres existantes
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Regenerer les clusters
        if (activerClustering)
        {
            GenererCentresClusters();
        }

        // Respawner
        SpawnerPierres();
    }


    // Visualisation dans l'editeur
    void OnDrawGizmos()
    {
        if (!afficherGizmos) return;

        // Zone totale
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f);
        Gizmos.DrawWireCube(transform.position, new Vector3(tailleZone.x, 0.5f, tailleZone.y));

        // Zone degagee (cercle central)
        Gizmos.color = Color.red;
        DrawCircle(transform.position, rayonZoneDegagee, 32);

        // Zone de transition
        Gizmos.color = Color.yellow;
        DrawCircle(transform.position, rayonTransition, 32);

        // Centres de clusters (si actives)
        if (activerClustering && centresClusters != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f); // Orange
            foreach (Vector3 centre in centresClusters)
            {
                Gizmos.DrawWireSphere(transform.position + centre, rayonCluster);
            }
        }
    }

    // Dessine un cercle pour les gizmos
    void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angle = 0f;
        float angleStep = 360f / segments;
        Vector3 lastPoint = center + new Vector3(Mathf.Cos(0) * radius, 0f, Mathf.Sin(0) * radius);

        for (int i = 1; i <= segments; i++)
        {
            angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(lastPoint, newPoint);
            lastPoint = newPoint;
        }
    }

    void OnApplicationQuit()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
