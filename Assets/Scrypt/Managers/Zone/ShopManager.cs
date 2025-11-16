using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("Configuration")]
    [Tooltip("Configuration des prix du shop")]
    public ConfigurationShop configShop;

    [Header("Débloqués par défaut")]
    [Tooltip("Graine de départ (Salade)")]
    public TypeGraine graineDepart = TypeGraine.Salade;

    [Header("Scène de Victoire")]
    [Tooltip("Nom de la scène de victoire")]
    public string nomSceneVictoire = "Victoire";

    [Header("État du jeu")]
    public bool arrosoirAchete = false;
    public bool autoRecolteAchete = false;
    public bool antiGraviteAchete = false;
    public bool victoire = false;

    [Header("Auto-récolte par rareté")]
    public RareteLegume niveauAutoRecolte = RareteLegume.Aucun;

    [Header("Graines débloquées")]
    private HashSet<TypeGraine> grainesDebloquees = new HashSet<TypeGraine>();

    [Header("Debug")]
    public bool afficherDebug = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("[ShopManager] Une instance existe déjà, destruction de ce doublon.");
            return;
        }

        Instance = this;
        InitialiserGraines();
    }

    void InitialiserGraines()
    {
        // Au départ, seule la graine de départ est débloquée
        grainesDebloquees.Add(graineDepart);

        if (afficherDebug)
        {
            Debug.Log($"[ShopManager] Graine de départ débloquée : {graineDepart}");
        }
    }

    public bool EstGraineDebloquee(TypeGraine type)
    {
        return grainesDebloquees.Contains(type);
    }

    public bool DebloquerGraine(TypeGraine type)
    {
        if (grainesDebloquees.Contains(type))
        {
            if (afficherDebug)
            {
                Debug.LogWarning($"[ShopManager] {type} est déjà débloquée !");
            }
            return false;
        }

        grainesDebloquees.Add(type);

        if (afficherDebug)
        {
            Debug.Log($"[ShopManager] {type} débloquée !");
        }

        return true;
    }

    public HashSet<TypeGraine> ObtenirGrainesDebloquees()
    {
        return new HashSet<TypeGraine>(grainesDebloquees);
    }

    public bool AcheterArrosoir(int prix)
    {
        if (arrosoirAchete)
        {
            if (afficherDebug)
            {
                Debug.LogWarning("[ShopManager] Arrosoir déjà acheté !");
            }
            return false;
        }

        if (MoneyManager.Instance != null && MoneyManager.Instance.PeutAcheter(prix))
        {
            MoneyManager.Instance.Depenser(prix);
            arrosoirAchete = true;

            if (afficherDebug)
            {
                Debug.Log($"[ShopManager] Arrosoir acheté pour {prix}$ !");
            }
            return true;
        }

        return false;
    }

    public bool AcheterAutoRecolte(int prix)
    {
        if (autoRecolteAchete)
        {
            if (afficherDebug)
            {
                Debug.LogWarning("[ShopManager] Auto-récolte déjà achetée !");
            }
            return false;
        }

        if (MoneyManager.Instance != null && MoneyManager.Instance.PeutAcheter(prix))
        {
            MoneyManager.Instance.Depenser(prix);
            autoRecolteAchete = true;

            if (afficherDebug)
            {
                Debug.Log($"[ShopManager] Auto-récolte achetée pour {prix}$ !");
            }
            return true;
        }

        return false;
    }

    public bool AcheterAntiGravite(int prix)
    {
        if (antiGraviteAchete)
        {
            if (afficherDebug)
            {
                Debug.LogWarning("[ShopManager] Anti-gravité déjà achetée !");
            }
            return false;
        }

        if (MoneyManager.Instance != null && MoneyManager.Instance.PeutAcheter(prix))
        {
            MoneyManager.Instance.Depenser(prix);
            antiGraviteAchete = true;

            // Désactiver la gravité sur le drone
            DroneController drone = FindObjectOfType<DroneController>();
            if (drone != null && drone.movement != null)
            {
                drone.movement.chuteLente = 0f;
                if (afficherDebug)
                {
                    Debug.Log($"[ShopManager] Anti-gravité activée ! Chute lente = 0");
                }
            }

            if (afficherDebug)
            {
                Debug.Log($"[ShopManager] Anti-gravité achetée pour {prix}$ !");
            }
            return true;
        }

        return false;
    }

    public bool AmeliorerAutoRecolte(RareteLegume nouveauNiveau, int prix)
    {
        if (niveauAutoRecolte >= nouveauNiveau)
        {
            if (afficherDebug)
            {
                Debug.LogWarning($"[ShopManager] Niveau auto-récolte {nouveauNiveau} déjà acheté ou dépassé !");
            }
            return false;
        }

        if (MoneyManager.Instance != null && MoneyManager.Instance.PeutAcheter(prix))
        {
            MoneyManager.Instance.Depenser(prix);
            niveauAutoRecolte = nouveauNiveau;

            if (afficherDebug)
            {
                Debug.Log($"[ShopManager] Auto-récolte améliorée au niveau {nouveauNiveau} pour {prix}$ !");
            }
            return true;
        }

        return false;
    }

    public bool AcheterPlantationUpgrade()
    {
        if (PlantationUpgradeManager.Instance == null)
        {
            if (afficherDebug)
            {
                Debug.LogWarning("[ShopManager] PlantationUpgradeManager introuvable !");
            }
            return false;
        }

        if (!PlantationUpgradeManager.Instance.PeutAcheterPalierSuivant())
        {
            if (afficherDebug)
            {
                Debug.LogWarning("[ShopManager] Impossible d'acheter le palier suivant (déjà max ou pas assez d'argent)");
            }
            return false;
        }

        bool succes = PlantationUpgradeManager.Instance.AcheterPalierSuivant();

        if (succes && afficherDebug)
        {
            int palier = PlantationUpgradeManager.Instance.palierActuel;
            int nbGraines = PlantationUpgradeManager.Instance.ObtenirNombreGrainesSimultanees();
            Debug.Log($"[ShopManager] Plantation upgrade acheté ! Palier {palier} → {nbGraines} graines simultanées");
        }

        return succes;
    }

    public bool AcheterVictoire(int prix)
    {
        if (victoire)
        {
            if (afficherDebug)
            {
                Debug.LogWarning("[ShopManager] Victoire déjà achetée !");
            }
            return false;
        }

        if (MoneyManager.Instance != null && MoneyManager.Instance.PeutAcheter(prix))
        {
            MoneyManager.Instance.Depenser(prix);
            victoire = true;

            if (afficherDebug)
            {
                Debug.Log($"[ShopManager] VICTOIRE ! Le joueur a acheté sa liberté pour {prix}$ !");
            }

            // Déclencher la fin du jeu
            DeclenecherFinDeJeu();
            return true;
        }

        return false;
    }

    void DeclenecherFinDeJeu()
    {
        Debug.Log("[ShopManager] Fin du jeu - Victoire");

        Time.timeScale = 1f;
        SceneManager.LoadScene(nomSceneVictoire);
    }

    public void ResetProgression()
    {
        grainesDebloquees.Clear();
        InitialiserGraines();
        arrosoirAchete = false;
        autoRecolteAchete = false;
        antiGraviteAchete = false;
        niveauAutoRecolte = RareteLegume.Aucun;
        victoire = false;

        if (afficherDebug)
        {
            Debug.Log("[ShopManager] Progression réinitialisée");
        }
    }
}
