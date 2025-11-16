using UnityEngine;

[CreateAssetMenu(fileName = "ConfigPrixLegumes", menuName = "Potager/Configuration Prix Légumes")]
public class ConfigurationPrixLegumes : ScriptableObject
{
    [Header("Prix par type de légume et rareté")]
    [Tooltip("Ordre : Salade < Carotte < Potate < Navet < Potiron")]
    public PrixLegume[] prixLegumes;

    [System.Serializable]
    public class PrixLegume
    {
        public TypeGraine type;
        public int prixCommun;
        public int prixRare;
        public int prixEpique;
        public int prixLegendaire;
    }

    // Fonction helper pour obtenir le prix
    public int ObtenirPrix(TypeGraine type, RareteLegume rarete)
    {
        foreach (var prix in prixLegumes)
        {
            if (prix.type == type)
            {
                switch (rarete)
                {
                    case RareteLegume.Commun: return prix.prixCommun;
                    case RareteLegume.Rare: return prix.prixRare;
                    case RareteLegume.Epique: return prix.prixEpique;
                    case RareteLegume.Legendaire: return prix.prixLegendaire;
                }
            }
        }
        return 10; // Prix par défaut
    }

    // Valeurs par défaut recommandées
    public void InitialiserValeursParDefaut()
    {
        prixLegumes = new PrixLegume[]
        {
            new PrixLegume { type = TypeGraine.Salade, prixCommun = 8, prixRare = 15, prixEpique = 25, prixLegendaire = 40 },
            new PrixLegume { type = TypeGraine.Carotte, prixCommun = 10, prixRare = 18, prixEpique = 30, prixLegendaire = 50 },
            new PrixLegume { type = TypeGraine.Potate, prixCommun = 12, prixRare = 22, prixEpique = 38, prixLegendaire = 60 },
            new PrixLegume { type = TypeGraine.Navet, prixCommun = 15, prixRare = 28, prixEpique = 45, prixLegendaire = 75 },
            new PrixLegume { type = TypeGraine.Potiron, prixCommun = 20, prixRare = 35, prixEpique = 60, prixLegendaire = 100 }
        };
    }
}
