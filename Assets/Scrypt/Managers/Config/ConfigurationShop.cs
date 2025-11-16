using UnityEngine;

[CreateAssetMenu(fileName = "ConfigShop", menuName = "Potager/Configuration Shop")]
public class ConfigurationShop : ScriptableObject
{
    [Header("Prix des graines à débloquer")]
    [Tooltip("Prix pour débloquer la Carotte")]
    public int prixDeblocageCarotte = 100;

    [Tooltip("Prix pour débloquer la Patate")]
    public int prixDeblocagePotate = 250;

    [Tooltip("Prix pour débloquer le Navet")]
    public int prixDeblocageNavet = 500;

    [Tooltip("Prix pour débloquer le Potiron")]
    public int prixDeblocagePotiron = 1000;

    [Header("Prix des outils")]
    [Tooltip("Prix de l'arrosoir (boost croissance x2 pendant 1min30)")]
    public int prixArrosoir = 300;

    [Tooltip("Prix de l'anti-gravité (supprime la chute lente du drone)")]
    public int prixAntiGravite = 500;

    [Header("Prix Auto-Récolte par Rareté")]
    [Tooltip("Prix pour auto-récolter les légumes Communs (50%-75%)")]
    public int prixAutoRecolteCommun = 200;

    [Tooltip("Prix pour auto-récolter les légumes Rares (75%-87.5%)")]
    public int prixAutoRecolteRare = 500;

    [Tooltip("Prix pour auto-récolter les légumes Épiques (87.5%-95%)")]
    public int prixAutoRecolteEpique = 1000;

    [Tooltip("Prix pour auto-récolter les légumes Légendaires (95%-100%)")]
    public int prixAutoRecolteLegendaire = 2000;

    [Header("Prix Plantation Multiple")]
    [Tooltip("Prix pour passer au palier 1 (5 graines simultanées)")]
    public int prixPlantationPalier1 = 500;

    [Tooltip("Prix pour passer au palier 2 (10 graines simultanées)")]
    public int prixPlantationPalier2 = 2000;

    [Tooltip("Prix pour passer au palier 3 (25 graines simultanées)")]
    public int prixPlantationPalier3 = 10000;

    [Header("Fin du jeu")]
    [Tooltip("Prix pour acheter la victoire (fin du jeu)")]
    public int prixVictoire = 5000;

    public int ObtenirPrixDeblocageGraine(TypeGraine type)
    {
        switch (type)
        {
            case TypeGraine.Salade: return 0;
            case TypeGraine.Carotte: return prixDeblocageCarotte;
            case TypeGraine.Potate: return prixDeblocagePotate;
            case TypeGraine.Navet: return prixDeblocageNavet;
            case TypeGraine.Potiron: return prixDeblocagePotiron;
            default: return 999999;
        }
    }

    public void InitialiserValeursParDefaut()
    {
        prixDeblocageCarotte = 100;
        prixDeblocagePotate = 250;
        prixDeblocageNavet = 500;
        prixDeblocagePotiron = 1000;
        prixArrosoir = 300;
        prixAntiGravite = 500;
        prixAutoRecolteCommun = 200;
        prixAutoRecolteRare = 500;
        prixAutoRecolteEpique = 1000;
        prixAutoRecolteLegendaire = 2000;
        prixPlantationPalier1 = 500;
        prixPlantationPalier2 = 2000;
        prixPlantationPalier3 = 10000;
        prixVictoire = 5000;
    }

    [Header("Descriptions")]
    [TextArea(2, 5)]
    public string descriptionCarotte = "Légume de valeur moyenne";

    [TextArea(2, 5)]
    public string descriptionPotate = "Légume de bonne valeur";

    [TextArea(2, 5)]
    public string descriptionNavet = "Légume de haute valeur";

    [TextArea(2, 5)]
    public string descriptionPotiron = "Légume de très haute valeur";

    [TextArea(2, 5)]
    public string descriptionArrosoir = "Boost la croissance de tous les légumes du champs x2 pendant 1min30";

    [TextArea(2, 5)]
    public string descriptionAutoRecolte = "Récolte automatiquement tous les légumes mûrs du champ";

    [TextArea(2, 5)]
    public string descriptionAntiGravite = "Supprime la chute lente, le drone reste en l'air sans maintenir ESPACE";

    [TextArea(2, 5)]
    public string descriptionAutoRecolteCommun = "Récolte automatique dès 50% de maturité dans la plage récoltable (10$ par légume)";

    [TextArea(2, 5)]
    public string descriptionAutoRecolteRare = "Récolte automatique dès 75% de maturité dans la plage récoltable (20$ par légume)";

    [TextArea(2, 5)]
    public string descriptionAutoRecolteEpique = "Récolte automatique dès 87.5% de maturité dans la plage récoltable (35$ par légume)";

    [TextArea(2, 5)]
    public string descriptionAutoRecolteLegendaire = "Récolte automatique dès 95% de maturité dans la plage récoltable (55$ par légume)";

    [TextArea(2, 5)]
    public string descriptionPlantationPalier1 = "Plantez 5 graines simultanément sur les parcelles les plus proches";

    [TextArea(2, 5)]
    public string descriptionPlantationPalier2 = "Plantez 10 graines simultanément sur les parcelles les plus proches";

    [TextArea(2, 5)]
    public string descriptionPlantationPalier3 = "Plantez 25 graines simultanément sur les parcelles les plus proches";

    [TextArea(2, 5)]
    public string descriptionVictoire = "Achetez votre retraite et terminez le jeu !";
}
