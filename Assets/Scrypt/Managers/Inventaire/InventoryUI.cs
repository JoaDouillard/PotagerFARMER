using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Panel principal de l'inventaire (à désactiver/activer)")]
    public GameObject panelInventaire;

    [Tooltip("TextMeshPro pour le titre")]
    public TextMeshProUGUI texteTitre;

    [Header("Tableau UI")]
    [Tooltip("Container (GameObject vide avec Vertical Layout Group) qui contiendra le tableau généré")]
    public Transform conteneurTableau;

    [Header("Statistiques")]
    [Tooltip("TextMeshPro pour afficher le total de légumes")]
    public TextMeshProUGUI texteTotalLegumes;

    [Tooltip("TextMeshPro pour afficher la valeur totale")]
    public TextMeshProUGUI texteValeurTotale;

    [Header("Style")]
    [Tooltip("Couleur de fond pour les lignes paires")]
    public Color couleurLignePaire = new Color(40f/255f, 40f/255f, 40f/255f, 200f/255f);

    [Tooltip("Couleur de fond pour les lignes impaires")]
    public Color couleurLigneImpaire = new Color(40f/255f, 40f/255f, 40f/255f, 200f/255f);

    private bool inventaireOuvert = false;

    void Start()
    {
        if (panelInventaire != null)
        {
            panelInventaire.SetActive(false);
        }
    }

    void Update()
    {
        if (PlayerInputManager.Instance == null) return;

        if (PlayerInputManager.Instance.Controls.Drone.OpenInventory.WasPressedThisFrame())
        {
            if (MenuManager.Instance != null)
            {
                if (MenuManager.Instance.unMenuEstOuvert && MenuManager.Instance.menuActuel != TypeMenu.Inventaire)
                {
                    TypeMenu menuAFermer = MenuManager.Instance.menuActuel;
                    MenuManager.Instance.FermerMenu();

                    NotifierFermeture(menuAFermer);

                    Debug.Log($"[InventoryUI] Menu {menuAFermer} fermé pour ouvrir l'inventaire");
                }

                if (MenuManager.Instance.PeutOuvrirMenu(TypeMenu.Inventaire))
                {
                    ToggleInventaire();
                }
            }
            else
            {
                ToggleInventaire();
            }
        }
    }

    void ToggleInventaire()
    {
        inventaireOuvert = !inventaireOuvert;

        if (inventaireOuvert && SoundManager.Instance != null)
        {
            SoundManager.Instance.JouerSonUI(SoundManager.Instance.sonOuvertureInventaire);
        }

        if (panelInventaire != null)
        {
            panelInventaire.SetActive(inventaireOuvert);
        }

        if (inventaireOuvert)
        {
            if (MenuManager.Instance != null)
            {
                MenuManager.Instance.OuvrirMenu(TypeMenu.Inventaire);
            }
            else
            {
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            MettreAJourAffichage();
        }
        else
        {
            FermerInventaire();
        }

        Debug.Log($"[InventoryUI] Inventaire : {(inventaireOuvert ? "OUVERT" : "FERMÉ")}");
    }

    public void FermerInventaire()
    {
        inventaireOuvert = false;

        if (panelInventaire != null)
        {
            panelInventaire.SetActive(false);
        }

        if (MenuManager.Instance != null && MenuManager.Instance.menuActuel == TypeMenu.Inventaire)
        {
            MenuManager.Instance.FermerMenu();
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void NotifierFermeture(TypeMenu menuFerme)
    {
        // Notifier les autres scripts que leur menu a été fermé
        if (menuFerme == TypeMenu.Shop)
        {
            ZoneShop shop = FindObjectOfType<ZoneShop>();
            if (shop != null)
            {
                shop.FermerShopExterne();
            }
        }
        else if (menuFerme == TypeMenu.Pause)
        {
            PauseMenu pause = FindObjectOfType<PauseMenu>();
            if (pause != null)
            {
                pause.FermerMenuExterne();
            }
        }
    }

    void MettreAJourAffichage()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("[InventoryUI] InventoryManager introuvable !");
            return;
        }

        if (texteTitre != null)
        {
            texteTitre.text = "INVENTAIRE";
        }

        GenererTableauUI();
        MettreAJourStatistiques();
    }

    void GenererTableauUI()
    {
        if (conteneurTableau == null)
        {
            Debug.LogWarning("[InventoryUI] ConteneurTableau non assigné !");
            return;
        }

        // Nettoyer le tableau existant
        foreach (Transform child in conteneurTableau)
        {
            Destroy(child.gameObject);
        }

        var inventaire = InventoryManager.Instance.ObtenirInventaire();

        // Créer l'en-tête
        CreerLigneEntete();

        // Ordre d'affichage personnalisé : Salades → Carottes → Patates → Navets → Potirons
        TypeGraine[] ordreAffichage = new TypeGraine[]
        {
            TypeGraine.Salade,
            TypeGraine.Carotte,
            TypeGraine.Potate,
            TypeGraine.Navet,
            TypeGraine.Potiron
        };

        // Créer une ligne pour chaque type de légume dans l'ordre
        int indexLigne = 0;
        foreach (TypeGraine type in ordreAffichage)
        {
            if (!inventaire.ContainsKey(type)) continue;

            bool estLignePaire = (indexLigne % 2 == 0);
            CreerLigneLegume(type, inventaire[type], estLignePaire);
            indexLigne++;
        }
    }

    void CreerLigneEntete()
    {
        GameObject ligneObj = new GameObject("EnteteTableau");
        ligneObj.transform.SetParent(conteneurTableau, false);

        // Ajouter un LayoutGroup horizontal
        HorizontalLayoutGroup layout = ligneObj.AddComponent<HorizontalLayoutGroup>();
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.spacing = 5f;
        layout.padding = new RectOffset(10, 10, 10, 10);

        // Colonne Type
        CreerCelluleTexte(ligneObj.transform, "TYPE", Color.white, 40, TextAlignmentOptions.Center, true);

        // Colonnes Raretés avec couleurs
        CreerCelluleTexte(ligneObj.transform, "COMMUN", RareteHelper.ObtenirCouleurRarete(RareteLegume.Commun), 36, TextAlignmentOptions.Center, true);
        CreerCelluleTexte(ligneObj.transform, "RARE", RareteHelper.ObtenirCouleurRarete(RareteLegume.Rare), 36, TextAlignmentOptions.Center, true);
        CreerCelluleTexte(ligneObj.transform, "ÉPIQUE", RareteHelper.ObtenirCouleurRarete(RareteLegume.Epique), 36, TextAlignmentOptions.Center, true);
        CreerCelluleTexte(ligneObj.transform, "LÉGENDAIRE", RareteHelper.ObtenirCouleurRarete(RareteLegume.Legendaire), 36, TextAlignmentOptions.Center, true);

        // Ajouter un LayoutElement pour la hauteur
        LayoutElement le = ligneObj.AddComponent<LayoutElement>();
        le.preferredHeight = 50f;
    }

    void CreerLigneLegume(TypeGraine type, System.Collections.Generic.Dictionary<RareteLegume, int> raretes, bool estLignePaire)
    {
        GameObject ligneObj = new GameObject($"Ligne_{type}");
        ligneObj.transform.SetParent(conteneurTableau, false);

        // Ajouter un LayoutGroup horizontal
        HorizontalLayoutGroup layout = ligneObj.AddComponent<HorizontalLayoutGroup>();
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.spacing = 5f;
        layout.padding = new RectOffset(10, 10, 5, 5);

        // Colonne Type (nom du légume)
        CreerCelluleTexte(ligneObj.transform, type.ToString(), Color.white, 36, TextAlignmentOptions.Center, true);

        // Colonnes Raretés
        CreerCelluleRarete(ligneObj.transform, type, RareteLegume.Commun, raretes);
        CreerCelluleRarete(ligneObj.transform, type, RareteLegume.Rare, raretes);
        CreerCelluleRarete(ligneObj.transform, type, RareteLegume.Epique, raretes);
        CreerCelluleRarete(ligneObj.transform, type, RareteLegume.Legendaire, raretes);

        // Ajouter un LayoutElement pour la hauteur
        LayoutElement le = ligneObj.AddComponent<LayoutElement>();
        le.preferredHeight = 80f;
    }

    void CreerCelluleRarete(Transform parent, TypeGraine type, RareteLegume rarete, System.Collections.Generic.Dictionary<RareteLegume, int> raretes)
    {
        int quantite = raretes.ContainsKey(rarete) ? raretes[rarete] : 0;
        int prix = InventoryManager.Instance.ObtenirPrixUnitaire(type, rarete);

        string texte = "-";
        if (quantite > 0)
        {
            texte = $"{quantite} × {prix}$";
        }

        Color couleur = RareteHelper.ObtenirCouleurRarete(rarete);
        CreerCelluleTexte(parent, texte, couleur, 32, TextAlignmentOptions.Center, true);
    }

    GameObject CreerCelluleTexte(Transform parent, string texte, Color couleur, int fontSize, TextAlignmentOptions alignement, bool bold)
    {
        GameObject celluleObj = new GameObject($"Cellule_{texte}");
        celluleObj.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = celluleObj.AddComponent<TextMeshProUGUI>();
        tmp.text = texte;
        tmp.color = couleur;
        tmp.fontSize = fontSize;
        tmp.alignment = alignement;
        if (bold) tmp.fontStyle = FontStyles.Bold;

        return celluleObj;
    }

    void MettreAJourStatistiques()
    {
        if (texteTotalLegumes != null)
        {
            int total = InventoryManager.Instance.ObtenirTotalLegumes();
            texteTotalLegumes.text = $"Total de légumes : {total}";
        }

        if (texteValeurTotale != null)
        {
            int valeur = InventoryManager.Instance.ObtenirValeurTotale();
            texteValeurTotale.text = $"Valeur totale : {valeur}$";
        }
    }

    void OnDestroy()
    {
        // Remettre le temps à 1 si l'objet est détruit pendant que l'inventaire est ouvert
        if (inventaireOuvert)
        {
            Time.timeScale = 1f;
        }
    }
}
