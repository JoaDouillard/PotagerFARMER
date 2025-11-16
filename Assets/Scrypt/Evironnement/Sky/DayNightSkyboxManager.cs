using UnityEngine;

// Gère le cycle jour/nuit avec skyboxes, soleil, lune et étoiles
public class DayNightSkyboxManager : MonoBehaviour
{
    #region === REFERENCES ===
    [Header("=== REFERENCES ===")]
    [SerializeField] private Light soleil;
    [SerializeField] private Light lune;
    [SerializeField] private ParticleSystem Etoiles;

    [Header("Visuels Célestes")]
    private GameObject soleilVisuel;
    private GameObject luneVisuel;
    [SerializeField] private float tailleSoleil = 100f;
    [SerializeField] private float tailleLune = 80f;
    [SerializeField] private float distanceCeleste = 900f;
    #endregion

    #region === SKYBOXES ===
    [Header("=== SKYBOXES ===")]
    [SerializeField] private Material skyboxMatin;
    [SerializeField] private Material skyboxMidi;
    [SerializeField] private Material skyboxApresMidi;
    [SerializeField] private Material skyboxSoiree;
    [SerializeField] private Material skyboxNuit;

    [Header("Heures de changement")]
    [Range(0f, 24f)] public float heureDebutMatin = 6f;
    [Range(0f, 24f)] public float heureDebutMidi = 10f;
    [Range(0f, 24f)] public float heureDebutApresMidi = 16f;
    [Range(0f, 24f)] public float heureDebutSoiree = 17.5f;
    [Range(0f, 24f)] public float heureDebutNuit = 19f;
    #endregion

    #region === ROTATION DES NUAGES ===
    [Header("=== ROTATION NUAGES ===")]
    [SerializeField] private float vitesseRotationNuages = 1f;
    #endregion

    #region === CYCLE JOUR/NUIT ===
    [Header("=== CYCLE JOUR/NUIT ===")]
    [SerializeField] private float dureeJourEnSecondes = 180f;
    [SerializeField] private bool cycleActif = true;
    [SerializeField][Range(0f, 24f)] private float heureActuelle = 12f;
    [SerializeField] private float vitesseTemps = 1f;
    #endregion

    #region === LUMINOSITE ===
    [Header("=== LUMINOSITE ===")]
    [SerializeField] private Gradient couleurSoleil;
    [SerializeField] private AnimationCurve intensiteSoleil;
    [SerializeField] private float intensiteMaxSoleil = 1.5f;

    [Header("Lune")]
    [SerializeField] private Color couleurLune = new Color(0.5f, 0.6f, 0.8f, 1f);
    [SerializeField] private float intensiteLune = 0.3f;

    [Header("Ambiante")]
    [SerializeField] private float intensiteAmbianteJour = 1f;
    [SerializeField] private float intensiteAmbianteNuit = 0.2f;
    #endregion

    #region === ETOILES ===
    [Header("=== ETOILES ===")]
    [SerializeField] private bool activerEtoiles = true;
    [SerializeField] private int nombreEtoiles = 100;
    [SerializeField] private float hauteurEtoiles = 200f;
    [SerializeField] private bool formeEtoile = true;
    #endregion

    #region === VARIABLES PRIVEES ===
    private float rotationNuagesAccumulee = 0f;
    private static readonly int RotationPropertyID = Shader.PropertyToID("_Rotation");
    private string skyboxActuelle = "";
    private bool etoilesEmises = false;
    #endregion

    void Start()
    {
        InitialiserSkyboxParDefaut();
        CreerVisuels();
        ConfigurerEtoiles();
        StartCoroutine(CoroutineMiseAJourCiel());
    }

    void InitialiserSkyboxParDefaut()
    {
        if (RenderSettings.skybox == null)
        {
            if (skyboxMidi != null)
            {
                RenderSettings.skybox = skyboxMidi;
            }
            else if (skyboxMatin != null)
            {
                RenderSettings.skybox = skyboxMatin;
            }
        }
    }

    void CreerVisuels()
    {
            if (soleil == null || lune == null) return;

            soleilVisuel = GameObject.CreatePrimitive(PrimitiveType.Quad);
            soleilVisuel.name = "SoleilVisuel";
            soleilVisuel.transform.SetParent(soleil.transform);
            soleilVisuel.transform.localPosition = Vector3.zero;
            soleilVisuel.transform.localRotation = Quaternion.identity;
            soleilVisuel.transform.localScale = Vector3.one * tailleSoleil;
            soleilVisuel.layer = LayerMask.NameToLayer("TransparentFX");

            Collider soleilCollider = soleilVisuel.GetComponent<Collider>();
            if (soleilCollider != null)
            {
                Object.Destroy(soleilCollider);
            }

            Material matSoleil = CreerMaterialSoleil();
            Renderer soleilRenderer = soleilVisuel.GetComponent<Renderer>();
            soleilRenderer.material = matSoleil;
            soleilRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            soleilRenderer.receiveShadows = false;
            soleilRenderer.sortingOrder = -1000;

            luneVisuel = GameObject.CreatePrimitive(PrimitiveType.Quad);
            luneVisuel.name = "LuneVisuel";
            luneVisuel.transform.SetParent(lune.transform);
            luneVisuel.transform.localPosition = Vector3.zero;
            luneVisuel.transform.localRotation = Quaternion.identity;
            luneVisuel.transform.localScale = Vector3.one * tailleLune;
            luneVisuel.layer = LayerMask.NameToLayer("TransparentFX");

            Collider luneCollider = luneVisuel.GetComponent<Collider>();
            if (luneCollider != null)
            {
                Object.Destroy(luneCollider);
            }

            Material matLune = CreerMaterialLune();
            Renderer luneRenderer = luneVisuel.GetComponent<Renderer>();
            luneRenderer.material = matLune;
            luneRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            luneRenderer.receiveShadows = false;
            luneRenderer.sortingOrder = -1000;
    }

    Material CreerMaterialSoleil()
    {
        Texture2D textureSoleil = CreerTextureRondLumineux(256, new Color(1f, 0.9f, 0.6f, 1f), 3f);

        Shader shader = Shader.Find("Unlit/Transparent");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Standard");

        Material mat = new Material(shader);
        mat.mainTexture = textureSoleil;
        mat.color = Color.white;
        return mat;
    }

    Material CreerMaterialLune()
    {
        Texture2D textureLune = CreerTextureCroissantLune(256, new Color(0.9f, 0.95f, 1f, 1f), 1.5f);

        Shader shader = Shader.Find("Unlit/Transparent");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Standard");

        Material mat = new Material(shader);
        mat.mainTexture = textureLune;
        mat.color = Color.white;
        return mat;
    }

    Texture2D CreerTextureRondLumineux(int taille, Color couleur, float intensite)
    {
        Texture2D texture = new Texture2D(taille, taille, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[taille * taille];
        Vector2 centre = new Vector2(taille / 2f, taille / 2f);
        float rayon = taille / 2f;

        for (int y = 0; y < taille; y++)
        {
            for (int x = 0; x < taille; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, centre);
                float alpha = 1f - Mathf.Clamp01(distance / rayon);
                alpha = Mathf.Pow(alpha, 0.5f);
                Color pixelCouleur = couleur * intensite;
                pixelCouleur.a = alpha;
                pixels[y * taille + x] = pixelCouleur;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        texture.wrapMode = TextureWrapMode.Clamp;
        return texture;
    }

    Texture2D CreerTextureCroissantLune(int taille, Color couleur, float intensite)
    {
        Texture2D texture = new Texture2D(taille, taille, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[taille * taille];
        Vector2 centre = new Vector2(taille / 2f, taille / 2f);
        float rayon = taille / 2.2f;
        float decalageOmbre = taille * 0.15f;

        for (int y = 0; y < taille; y++)
        {
            for (int x = 0; x < taille; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float distanceCentre = Vector2.Distance(pos, centre);
                Vector2 centreOmbre = new Vector2(centre.x + decalageOmbre, centre.y);
                float distanceOmbre = Vector2.Distance(pos, centreOmbre);

                bool dansLune = distanceCentre < rayon;
                bool dansOmbre = distanceOmbre < rayon * 0.95f;

                float alpha = 0f;
                if (dansLune && !dansOmbre)
                {
                    float bordure = 1f - Mathf.Clamp01((rayon - distanceCentre) / (rayon * 0.2f));
                    alpha = 1f - Mathf.Pow(bordure, 2f);
                }

                Color pixelCouleur = couleur * intensite;
                pixelCouleur.a = alpha;
                pixels[y * taille + x] = pixelCouleur;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        texture.wrapMode = TextureWrapMode.Clamp;
        return texture;
    }

    void ConfigurerEtoiles()
    {
        if (Etoiles == null) return;

        Etoiles.transform.position = new Vector3(0, hauteurEtoiles, 0);

        var main = Etoiles.main;
        main.loop = false;
        main.playOnAwake = false;
        main.startLifetime = Mathf.Infinity;
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(2f, 4f);
        main.startColor = new ParticleSystem.MinMaxGradient(Color.white, new Color(1f, 1f, 0.8f));
        main.maxParticles = nombreEtoiles;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = Etoiles.emission;
        emission.enabled = false;

        var shape = Etoiles.shape;
        shape.shapeType = ParticleSystemShapeType.Hemisphere;
        shape.radius = 1000f;
        shape.radiusThickness = 1f;
        shape.rotation = new Vector3(0, 0, 0);

        var renderer = Etoiles.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.sortingOrder = -500;
        }

        MeshRenderer meshRenderer = Etoiles.GetComponent<MeshRenderer>();
        if (meshRenderer != null) Object.Destroy(meshRenderer);
    }

    void Update()
    {
        if (!cycleActif) return;

        heureActuelle += (Time.deltaTime / dureeJourEnSecondes) * 24f * vitesseTemps;

        if (heureActuelle >= 24f)
        {
            heureActuelle -= 24f;
        }

        FaireFaceALaCamera();
    }

    System.Collections.IEnumerator CoroutineMiseAJourCiel()
    {
        while (true)
        {
            FaireTournerLeMonde();
            MettreAJourSkybox();
            MettreAJourRotationNuages();
            MettreAJourLumiereAmbiante();
            MettreAJourEtoiles();

            yield return new WaitForSeconds(0.05f);
        }
    }

    void FaireFaceALaCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        if (soleilVisuel != null)
        {
            Vector3 directionVersCamera = cam.transform.position - soleilVisuel.transform.position;
            soleilVisuel.transform.rotation = Quaternion.LookRotation(-directionVersCamera);
        }

        if (luneVisuel != null)
        {
            Vector3 directionVersCamera = cam.transform.position - luneVisuel.transform.position;
            luneVisuel.transform.rotation = Quaternion.LookRotation(-directionVersCamera);
        }
    }

    void FaireTournerLeMonde()
    {
        float pourcentageJour = heureActuelle / 24f;
        float angleSoleil = (heureActuelle / 12f) * 180f - 90f;

        // === SOLEIL ===
        if (soleil != null)
        {
            Vector3 directionSoleil = new Vector3(
                Mathf.Cos(angleSoleil * Mathf.Deg2Rad),
                Mathf.Sin(angleSoleil * Mathf.Deg2Rad),
                0f
            );

            soleil.transform.position = directionSoleil * 1000f;
            soleil.transform.LookAt(Vector3.zero);

            // Couleur et intensité
            if (couleurSoleil != null)
            {
                soleil.color = couleurSoleil.Evaluate(pourcentageJour);
            }

            if (intensiteSoleil != null)
            {
                float intensiteNormalisee = intensiteSoleil.Evaluate(pourcentageJour);
                soleil.intensity = intensiteNormalisee * intensiteMaxSoleil;
            }

            // Activer/désactiver
            bool estNuit = heureActuelle < heureDebutMatin || heureActuelle >= heureDebutNuit;
            soleil.enabled = !estNuit;
        }

        // === LUNE === (180° opposé au soleil)
        if (lune != null)
        {
            float angleLune = angleSoleil + 180f;

            Vector3 directionLune = new Vector3(
                Mathf.Cos(angleLune * Mathf.Deg2Rad),
                Mathf.Sin(angleLune * Mathf.Deg2Rad),
                0f
            );

            lune.transform.position = directionLune * 1000f;
            lune.transform.LookAt(Vector3.zero);

            lune.color = couleurLune;

            bool estNuit = heureActuelle < heureDebutMatin || heureActuelle >= heureDebutNuit;
            lune.intensity = estNuit ? intensiteLune : 0f;
            lune.enabled = estNuit;
        }

    }

    void MettreAJourLumiereAmbiante()
    {
        bool estNuit = heureActuelle < heureDebutMatin || heureActuelle >= heureDebutNuit;
        float intensiteCible = estNuit ? intensiteAmbianteNuit : intensiteAmbianteJour;
        RenderSettings.ambientIntensity = Mathf.Lerp(
            RenderSettings.ambientIntensity,
            intensiteCible,
            Time.deltaTime * 2f
        );
    }

    void MettreAJourEtoiles()
    {
        if (!activerEtoiles || Etoiles == null) return;

        bool estNuit = heureActuelle < heureDebutMatin || heureActuelle >= heureDebutNuit;

        if (estNuit && !etoilesEmises)
        {
            Etoiles.Clear();
            Etoiles.Emit(nombreEtoiles);
            etoilesEmises = true;
        }
        else if (!estNuit && etoilesEmises)
        {
            Etoiles.Clear();
            etoilesEmises = false;
        }
    }

    void MettreAJourSkybox()
    {
        string nouvelleSkybox = "";
        Material skyboxMaterial = null;

        // Déterminer quelle skybox utiliser selon l'heure
        if (heureActuelle >= heureDebutMatin && heureActuelle < heureDebutMidi)
        {
            skyboxMaterial = skyboxMatin;
            nouvelleSkybox = "Matin";
        }
        else if (heureActuelle >= heureDebutMidi && heureActuelle < heureDebutApresMidi)
        {
            skyboxMaterial = skyboxMidi;
            nouvelleSkybox = "Midi";
        }
        else if (heureActuelle >= heureDebutApresMidi && heureActuelle < heureDebutSoiree)
        {
            skyboxMaterial = skyboxApresMidi;
            nouvelleSkybox = "Apres-Midi";
        }
        else if (heureActuelle >= heureDebutSoiree && heureActuelle < heureDebutNuit)
        {
            skyboxMaterial = skyboxSoiree;
            nouvelleSkybox = "Soiree";
        }
        else
        {
            skyboxMaterial = skyboxNuit;
            nouvelleSkybox = "Nuit";
        }

        if (skyboxActuelle != nouvelleSkybox && skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
            DynamicGI.UpdateEnvironment();
            skyboxActuelle = nouvelleSkybox;
        }
    }

    void MettreAJourRotationNuages()
    {
        if (RenderSettings.skybox == null) return;

        rotationNuagesAccumulee += vitesseRotationNuages * Time.deltaTime;

        if (rotationNuagesAccumulee >= 360f)
        {
            rotationNuagesAccumulee -= 360f;
        }

        if (RenderSettings.skybox.HasProperty(RotationPropertyID))
        {
            RenderSettings.skybox.SetFloat(RotationPropertyID, rotationNuagesAccumulee);
        }
    }
}