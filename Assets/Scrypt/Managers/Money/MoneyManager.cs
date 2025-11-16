using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [Header("Économie")]
    [Tooltip("Argent de départ en dollars")]
    public int argentDepart = 1000;

    [Tooltip("Argent actuel du joueur")]
    public int argentActuel = 0;

    [Header("UI")]
    [Tooltip("TextMeshPro pour afficher l'argent")]
    public TextMeshProUGUI texteArgent;

    [Tooltip("Format d'affichage")]
    public string formatAffichage = "{0}$";

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
    }

    void Start()
    {
        argentActuel = argentDepart;
        MettreAJourAffichage();
    }

    public bool PeutAcheter(int prix)
    {
        return argentActuel >= prix;
    }

    public bool Depenser(int montant, bool jouerSon = true)
    {
        if (!PeutAcheter(montant))
        {
            if (afficherDebug)
            {
                Debug.LogWarning($"[MoneyManager] Pas assez d'argent ! Besoin: {montant}$, Disponible: {argentActuel}$");
            }

            return false;
        }

        argentActuel -= montant;
        MettreAJourAffichage();

        if (jouerSon && SoundManager.Instance != null)
        {
            SoundManager.Instance.JouerSon(SoundManager.Instance.sonDepenseArgent);
        }

        if (afficherDebug)
        {
            Debug.Log($"[MoneyManager] -{montant}$ | Restant: {argentActuel}$");
        }

        return true;
    }

    public void Gagner(int montant)
    {
        argentActuel += montant;
        MettreAJourAffichage();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.JouerSon(SoundManager.Instance.sonGainArgent);
        }

        if (afficherDebug)
        {
            Debug.Log($"[MoneyManager] +{montant}$ | Total: {argentActuel}$");
        }
    }

    void MettreAJourAffichage()
    {
        if (texteArgent != null)
        {
            texteArgent.text = string.Format(formatAffichage, argentActuel);
        }
    }

    public int ObtenirArgent()
    {
        return argentActuel;
    }

    void OnGUI()
    {
        if (afficherDebug && texteArgent == null)
        {
            GUI.Label(new Rect(10, 10, 300, 20), $"Argent: {argentActuel}$");
        }
    }
}
