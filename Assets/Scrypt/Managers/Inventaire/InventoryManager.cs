using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Inventaire")]
    [Tooltip("Stockage des légumes par type et rareté: Type -> (Rareté -> Quantité)")]
    public Dictionary<TypeGraine, Dictionary<RareteLegume, int>> inventaireLegumes = new Dictionary<TypeGraine, Dictionary<RareteLegume, int>>();

    [Tooltip("Stockage du prix unitaire par type et rareté: Type -> (Rareté -> Prix)")]
    public Dictionary<TypeGraine, Dictionary<RareteLegume, int>> prixUnitaireLegumes = new Dictionary<TypeGraine, Dictionary<RareteLegume, int>>();

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
        InitialiserInventaire();
    }

    void InitialiserInventaire()
    {
        foreach (TypeGraine type in System.Enum.GetValues(typeof(TypeGraine)))
        {
            if (!inventaireLegumes.ContainsKey(type))
            {
                inventaireLegumes[type] = new Dictionary<RareteLegume, int>();
                prixUnitaireLegumes[type] = new Dictionary<RareteLegume, int>();
            }

            foreach (RareteLegume rarete in System.Enum.GetValues(typeof(RareteLegume)))
            {
                inventaireLegumes[type][rarete] = 0;
                prixUnitaireLegumes[type][rarete] = 0;
            }
        }
    }

    public void AjouterLegume(TypeGraine type, RareteLegume rarete, int prixVente)
    {
        if (!inventaireLegumes.ContainsKey(type))
        {
            inventaireLegumes[type] = new Dictionary<RareteLegume, int>();
            prixUnitaireLegumes[type] = new Dictionary<RareteLegume, int>();

            foreach (RareteLegume r in System.Enum.GetValues(typeof(RareteLegume)))
            {
                inventaireLegumes[type][r] = 0;
                prixUnitaireLegumes[type][r] = 0;
            }
        }

        if (!inventaireLegumes[type].ContainsKey(rarete))
        {
            inventaireLegumes[type][rarete] = 0;
            prixUnitaireLegumes[type][rarete] = 0;
        }

        inventaireLegumes[type][rarete]++;
        prixUnitaireLegumes[type][rarete] = prixVente;

        if (afficherDebug)
        {
            Debug.Log($"[InventoryManager] +1 {type} {RareteHelper.ObtenirNomRarete(rarete)} ({prixVente}$) | Total: {inventaireLegumes[type][rarete]}");
        }
    }

    public bool RetirerLegume(TypeGraine type, RareteLegume rarete, int quantite = 1)
    {
        if (!inventaireLegumes.ContainsKey(type) || !inventaireLegumes[type].ContainsKey(rarete) || inventaireLegumes[type][rarete] < quantite)
        {
            if (afficherDebug)
            {
                Debug.LogWarning($"[InventoryManager] Pas assez de {type} {RareteHelper.ObtenirNomRarete(rarete)} ! Besoin: {quantite}, Disponible: {ObtenirQuantite(type, rarete)}");
            }
            return false;
        }

        inventaireLegumes[type][rarete] -= quantite;

        if (afficherDebug)
        {
            Debug.Log($"[InventoryManager] -{quantite} {type} {RareteHelper.ObtenirNomRarete(rarete)} | Restant: {inventaireLegumes[type][rarete]}");
        }

        return true;
    }

    public int ObtenirQuantite(TypeGraine type, RareteLegume rarete)
    {
        if (!inventaireLegumes.ContainsKey(type) || !inventaireLegumes[type].ContainsKey(rarete))
        {
            return 0;
        }
        return inventaireLegumes[type][rarete];
    }

    public int ObtenirQuantiteType(TypeGraine type)
    {
        if (!inventaireLegumes.ContainsKey(type))
        {
            return 0;
        }

        int total = 0;
        foreach (var kvp in inventaireLegumes[type])
        {
            total += kvp.Value;
        }
        return total;
    }

    public int ObtenirTotalLegumes()
    {
        int total = 0;
        foreach (var typeDict in inventaireLegumes)
        {
            foreach (var rareteKvp in typeDict.Value)
            {
                total += rareteKvp.Value;
            }
        }
        return total;
    }

    public int ObtenirValeurTotale()
    {
        int total = 0;
        foreach (var typeDict in inventaireLegumes)
        {
            foreach (var rareteKvp in typeDict.Value)
            {
                TypeGraine type = typeDict.Key;
                RareteLegume rarete = rareteKvp.Key;
                int quantite = rareteKvp.Value;
                int prixUnitaire = prixUnitaireLegumes[type][rarete];
                total += quantite * prixUnitaire;
            }
        }
        return total;
    }

    public int ObtenirPrixUnitaire(TypeGraine type, RareteLegume rarete)
    {
        if (!prixUnitaireLegumes.ContainsKey(type) || !prixUnitaireLegumes[type].ContainsKey(rarete))
        {
            return 0;
        }
        return prixUnitaireLegumes[type][rarete];
    }

    public int ObtenirValeurType(TypeGraine type)
    {
        if (!inventaireLegumes.ContainsKey(type))
        {
            return 0;
        }

        int total = 0;
        foreach (var rareteKvp in inventaireLegumes[type])
        {
            RareteLegume rarete = rareteKvp.Key;
            int quantite = rareteKvp.Value;
            int prixUnitaire = prixUnitaireLegumes[type][rarete];
            total += quantite * prixUnitaire;
        }
        return total;
    }


    public Dictionary<TypeGraine, Dictionary<RareteLegume, int>> ObtenirInventaire()
    {
        var copie = new Dictionary<TypeGraine, Dictionary<RareteLegume, int>>();
        foreach (var typeDict in inventaireLegumes)
        {
            copie[typeDict.Key] = new Dictionary<RareteLegume, int>(typeDict.Value);
        }
        return copie;
    }

    public void ViderInventaire()
    {
        InitialiserInventaire();
    }

}
