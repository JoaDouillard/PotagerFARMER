using UnityEngine;

public enum RareteLegume
{
    Aucun,       // Pas d'auto-récolte
    Commun,      // 50% → 75%
    Rare,        // 75% → 87.5%
    Epique,      // 87.5% → 95%
    Legendaire   // 95% → 100%
}

[System.Serializable]
public class PrixParRarete
{
    public TypeGraine typeLegume;
    public int prixCommun = 10;
    public int prixRare = 20;
    public int prixEpique = 35;
    public int prixLegendaire = 55;
}

public static class RareteHelper
{
    public static RareteLegume DeterminerRarete(float maturite)
    {
        return DeterminerRarete(maturite, 0.5f, 1.0f);
    }

    public static RareteLegume DeterminerRarete(float maturite, float seuilMin, float seuilMax)
    {
        if (maturite < seuilMin)
        {
            return RareteLegume.Aucun;
        }

        float plageUtile = seuilMax - seuilMin;

        if (plageUtile <= 0f)
        {
            return RareteLegume.Commun;
        }

        float progressionDansPlage = Mathf.Clamp01((maturite - seuilMin) / plageUtile);

        if (progressionDansPlage >= 0.90f) return RareteLegume.Legendaire;
        if (progressionDansPlage >= 0.75f) return RareteLegume.Epique;
        if (progressionDansPlage >= 0.50f) return RareteLegume.Rare;
        return RareteLegume.Commun;
    }

    // Obtenir la couleur selon la rareté
    public static Color ObtenirCouleurRarete(RareteLegume rarete)
    {
        switch (rarete)
        {
            case RareteLegume.Commun:
                return new Color(0.7f, 0.7f, 0.7f); // Gris
            case RareteLegume.Rare:
                return new Color(0.2f, 0.5f, 1f); // Bleu
            case RareteLegume.Epique:
                return new Color(0.6f, 0.3f, 0.9f); // Violet
            case RareteLegume.Legendaire:
                return new Color(1f, 0.6f, 0f); // Orange/Doré
            default:
                return Color.white;
        }
    }

    // Obtenir le nom de la rareté
    public static string ObtenirNomRarete(RareteLegume rarete)
    {
        switch (rarete)
        {
            case RareteLegume.Commun: return "Commun";
            case RareteLegume.Rare: return "Rare";
            case RareteLegume.Epique: return "Épique";
            case RareteLegume.Legendaire: return "Légendaire";
            default: return "Inconnu";
        }
    }
}
