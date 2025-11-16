using UnityEngine;

public class PlantationUpgradeManager : MonoBehaviour
{
    public static PlantationUpgradeManager Instance { get; private set; }

    [Header("Configuration")]
    [Tooltip("Configuration des prix du shop")]
    public ConfigurationShop configShop;

    [Header("Configuration des paliers")]
    [Tooltip("Palier actuel (0 = 1 graine, 1 = 5 graines, 2 = 10 graines, 3 = 25 graines)")]
    public int palierActuel = 0;

    [Header("Nombre de graines par palier")]
    public int[] grainesParPalier = new int[] { 1, 5, 10, 25 };

    [Header("Distance de plantation")]
    [Tooltip("Distance maximale pour détecter les parcelles vides (en mètres)")]
    public float distanceMaxPlantation = 5f;

    [Header("Debug")]
    public bool afficherDebug = true;

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

    public int ObtenirNombreGrainesSimultanees()
    {
        if (palierActuel >= 0 && palierActuel < grainesParPalier.Length)
        {
            return grainesParPalier[palierActuel];
        }
        return 1; // Par défaut
    }

    public bool AcheterPalierSuivant()
    {
        if (palierActuel >= grainesParPalier.Length - 1)
        {
            if (afficherDebug)
            {
                Debug.Log("[PlantationUpgradeManager] Tous les paliers sont déjà débloqués !");
            }
            return false;
        }

        int prixPalierSuivant = ObtenirPrixPalierSuivant();

        if (MoneyManager.Instance == null || !MoneyManager.Instance.Depenser(prixPalierSuivant))
        {
            if (afficherDebug)
            {
                Debug.Log($"[PlantationUpgradeManager] Pas assez d'argent pour acheter le palier {palierActuel + 1} ({prixPalierSuivant}$)");
            }
            return false;
        }

        palierActuel++;

        if (afficherDebug)
        {
            Debug.Log($"[PlantationUpgradeManager] Palier {palierActuel} débloqué ! Vous pouvez maintenant planter {ObtenirNombreGrainesSimultanees()} graines à la fois.");
        }

        return true;
    }

    public int ObtenirPrixPalierSuivant()
    {
        if (configShop == null)
        {
            Debug.LogWarning("[PlantationUpgradeManager] ConfigurationShop non assigné ! Utilisation des valeurs par défaut.");
            switch (palierActuel)
            {
                case 0: return 500;
                case 1: return 2000;
                case 2: return 10000;
                default: return -1;
            }
        }

        switch (palierActuel)
        {
            case 0: return configShop.prixPlantationPalier1;
            case 1: return configShop.prixPlantationPalier2;
            case 2: return configShop.prixPlantationPalier3;
            default: return -1; // Tous les paliers débloqués
        }
    }

    public bool PeutAcheterPalierSuivant()
    {
        if (palierActuel >= grainesParPalier.Length - 1)
        {
            return false; // Tous les paliers débloqués
        }

        int prix = ObtenirPrixPalierSuivant();
        return MoneyManager.Instance != null && MoneyManager.Instance.argentActuel >= prix;
    }
}
