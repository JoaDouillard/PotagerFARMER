using UnityEngine;

public enum TypeGraine
{
    Navet,
    Carotte,
    Salade,
    Potiron,
    Potate
}

[System.Serializable]
public class InfoGraine
{
    [Header("Identification")]
    public TypeGraine type;

    [Header("Prefabs")]
    public GameObject prefabGraine;

    [Header("Économie")]
    [Tooltip("Prix d'achat de la graine en dollars")]
    public int prixAchat = 10;

    [Tooltip("Prix de vente du légume récolté en dollars")]
    public int prixVente = 25;

    [Header("Croissance (hérité par Seed si non défini)")]
    public float tempsCroissance = 10f;

    [Header("Visuel (référence)")]
    public float tailleMin = 0.5f;
    public float tailleMax = 2f;
    public Color couleur = Color.green;
}
