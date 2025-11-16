using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Header("Configuration")]
    [Tooltip("Nom de la scène Game Over")]
    public string nomSceneGameOver = "GameOver";

    [Tooltip("Prix minimum pour acheter la graine la moins chère (Salade par défaut)")]
    public int prixGraineMoinsChere = 10;

    [Header("Debug")]
    public bool afficherDebug = true;

    private static string raisonGameOver = "";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        VerifierFaillite();
    }

    void VerifierFaillite()
    {
        if (MoneyManager.Instance == null || InventoryManager.Instance == null)
        {
            return;
        }

        int argent = MoneyManager.Instance.argentActuel;
        int legumes = InventoryManager.Instance.ObtenirTotalLegumes();
        int grainesPlantees = CompterGrainesPlantees();
        int legumesPlantes = CompterLegumesPlantes();

        // Faillite si l'argent est insuffisant pour acheter la graine la moins chère ET aucune ressource disponible
        if (argent < prixGraineMoinsChere && legumes == 0 && grainesPlantees == 0 && legumesPlantes == 0)
        {
            if (afficherDebug)
            {
                Debug.Log($"[GameOverManager] Faillite détectée : {argent}$ (< {prixGraineMoinsChere}$), 0 légumes inventaire, 0 graines et 0 légumes plantés");
            }

            DeclenecherGameOver($"FAILLITE !\n\nVous n'avez plus de légumes ni d'argents.\nVous êtes ruiné !");
        }
    }

    int CompterGrainesPlantees()
    {
        int seedLayer = LayerMask.NameToLayer("Seed");

        if (seedLayer == -1)
        {
            Debug.LogWarning("[GameOverManager] Le layer 'Seed' n'existe pas ! Créez-le dans Project Settings > Tags & Layers.");
            return 0;
        }

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int count = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == seedLayer)
            {
                count++;
            }
        }

        if (afficherDebug)
        {
            Debug.Log($"[GameOverManager] Graines plantées détectées : {count}");
        }

        return count;
    }

    int CompterLegumesPlantes()
    {
        int vegetableLayer = LayerMask.NameToLayer("Vegetable");

        if (vegetableLayer == -1)
        {
            Debug.LogWarning("[GameOverManager] Le layer 'Vegetable' n'existe pas ! Créez-le dans Project Settings > Tags & Layers.");
            return 0;
        }

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int count = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == vegetableLayer)
            {
                count++;
            }
        }

        if (afficherDebug)
        {
            Debug.Log($"[GameOverManager] Légumes plantés détectés : {count}");
        }

        return count;
    }

    public void DeclenecherGameOver(string raison)
    {
        if (afficherDebug)
        {
            Debug.Log($"[GameOverManager] GAME OVER - {raison}");
        }

        raisonGameOver = raison;
        Time.timeScale = 1f;
        SceneManager.LoadScene(nomSceneGameOver);
    }

    public static string ObtenirRaisonGameOver()
    {
        return raisonGameOver;
    }
}
