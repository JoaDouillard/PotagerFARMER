using UnityEngine;

public class ZoneShop : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Tag du drone pour détecter l'entrée")]
    public string tagDrone = "Player";

    [Header("Configuration des prix")]
    [Tooltip("Configuration du shop (prix, descriptions)")]
    public ConfigurationShop configShop;

    private bool droneEstDansLaZone = false;
    private bool shopOuvert = false;
    private BoxCollider zoneCollider;
    private Rect rectShop;
    private Vector2 scrollPosition = Vector2.zero;

    void Start()
    {
        zoneCollider = GetComponent<BoxCollider>();
        if (zoneCollider == null)
        {
            zoneCollider = gameObject.AddComponent<BoxCollider>();
        }

        zoneCollider.isTrigger = true;

        float largeur = 1000f;
        float hauteur = 900f;

        if (hauteur > Screen.height - 100)
        {
            hauteur = Screen.height - 100;
        }

        rectShop = new Rect(
            (Screen.width - largeur) / 2f,
            (Screen.height - hauteur) / 2f,
            largeur,
            hauteur
        );
    }

    void Update()
    {
        if (PlayerInputManager.Instance == null) return;

        if (droneEstDansLaZone && PlayerInputManager.Instance.Controls.Drone.Interact.WasPressedThisFrame())
        {
            if (MenuManager.Instance != null && !MenuManager.Instance.PeutOuvrirMenu(TypeMenu.Shop))
            {
                return;
            }

            ToggleShop();
        }

        if (shopOuvert && PlayerInputManager.Instance.Controls.Drone.Cancel.WasPressedThisFrame())
        {
            FermerShop();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagDrone))
        {
            droneEstDansLaZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagDrone))
        {
            droneEstDansLaZone = false;
            if (shopOuvert)
            {
                FermerShop();
            }
        }
    }

    void ToggleShop()
    {
        shopOuvert = !shopOuvert;

        if (shopOuvert)
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.JouerSon(SoundManager.Instance.sonOuvertureShop);
            }

            if (MenuManager.Instance != null)
            {
                MenuManager.Instance.OuvrirMenu(TypeMenu.Shop);
            }
            else
            {
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else
        {
            FermerShop();
        }
    }

    void FermerShop()
    {
        shopOuvert = false;

        if (MenuManager.Instance != null)
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

    public void FermerShopExterne()
    {
        if (shopOuvert)
        {
            shopOuvert = false;
        }
    }

    void OnGUI()
    {
        if (droneEstDansLaZone && !shopOuvert)
        {
            GUIStyle styleBox = new GUIStyle(GUI.skin.box);
            styleBox.fontSize = 32;
            styleBox.alignment = TextAnchor.UpperCenter;
            styleBox.fontStyle = FontStyle.Bold;

            GUIStyle styleLabel = new GUIStyle(GUI.skin.label);
            styleLabel.fontSize = 26;
            styleLabel.alignment = TextAnchor.MiddleCenter;

            float largeur = 1000f;
            float hauteur = 200f;
            float posX = (Screen.width - largeur) / 2f;
            float posY = Screen.height - hauteur - 50f;

            GUI.Box(new Rect(posX, posY, largeur, hauteur), "ZONE SHOP", styleBox);
            GUI.Label(new Rect(posX + 100, posY + 80, largeur - 200, 60), $"Appuyez sur [E] pour ouvrir le shop", styleLabel);
        }

        if (shopOuvert)
        {
            AfficherShop();
        }
    }

    void AfficherShop()
    {
        if (configShop == null || ShopManager.Instance == null || MoneyManager.Instance == null)
        {
            GUI.Window(0, rectShop, (id) =>
            {
                GUI.Label(new Rect(10, 30, 580, 30), "Erreur : ConfigShop, ShopManager ou MoneyManager manquant !");
            }, "SHOP - ERREUR");
            return;
        }

        GUI.Window(0, rectShop, ShopWindow, "");
    }

    void ShopWindow(int windowID)
    {
        int argentActuel = MoneyManager.Instance.argentActuel;

        GUIStyle styleLabelGros = new GUIStyle(GUI.skin.label);
        styleLabelGros.fontSize = 36;
        styleLabelGros.fontStyle = FontStyle.Bold;

        GUIStyle styleLabelMoyen = new GUIStyle(GUI.skin.label);
        styleLabelMoyen.fontSize = 30;

        GUIStyle styleButton = new GUIStyle(GUI.skin.button);
        styleButton.fontSize = 28;

        GUI.Label(new Rect(15, 40, 970, 45), $"Votre argent : {argentActuel}$", styleLabelGros);

        scrollPosition = GUI.BeginScrollView(
            new Rect(15, 100, 970, 690),
            scrollPosition,
            new Rect(0, 0, 940, 3200)
        );

        int y = 15;

        GUIStyle styleTitre = new GUIStyle(GUI.skin.label);
        styleTitre.fontSize = 40;
        styleTitre.fontStyle = FontStyle.Bold;
        styleTitre.alignment = TextAnchor.MiddleCenter;

        GUI.Label(new Rect(15, y, 910, 50), "DÉBLOCAGE DE GRAINES", styleTitre);
        y += 65;

        // Carotte
        y = AfficherItemGraine(TypeGraine.Carotte, configShop.prixDeblocageCarotte, configShop.descriptionCarotte, y);

        // Patate
        y = AfficherItemGraine(TypeGraine.Potate, configShop.prixDeblocagePotate, configShop.descriptionPotate, y);

        // Navet
        y = AfficherItemGraine(TypeGraine.Navet, configShop.prixDeblocageNavet, configShop.descriptionNavet, y);

        // Potiron
        y = AfficherItemGraine(TypeGraine.Potiron, configShop.prixDeblocagePotiron, configShop.descriptionPotiron, y);

        y += 30;

        GUI.Label(new Rect(15, y, 910, 50), "OUTILS", styleTitre);
        y += 65;

        // Arrosoir
        y = AfficherItemArrosoir(y, styleLabelMoyen, styleButton);

        // Anti-Gravité
        y = AfficherItemAntiGravite(y, styleLabelMoyen, styleButton);

        y += 30;

        GUI.Label(new Rect(15, y, 910, 50), "AUTO-RÉCOLTE PAR RARETÉ", styleTitre);
        y += 65;

        // Auto-Récolte Commun
        y = AfficherItemAutoRecolteRarete(RareteLegume.Commun, y, styleLabelMoyen, styleButton);

        // Auto-Récolte Rare
        y = AfficherItemAutoRecolteRarete(RareteLegume.Rare, y, styleLabelMoyen, styleButton);

        // Auto-Récolte Épique
        y = AfficherItemAutoRecolteRarete(RareteLegume.Epique, y, styleLabelMoyen, styleButton);

        // Auto-Récolte Légendaire
        y = AfficherItemAutoRecolteRarete(RareteLegume.Legendaire, y, styleLabelMoyen, styleButton);

        y += 30;

        GUI.Label(new Rect(15, y, 910, 50), "PLANTATION MULTIPLE", styleTitre);
        y += 65;

        // Plantation x5
        y = AfficherItemPlantationUpgrade(1, y, styleLabelMoyen, styleButton);

        // Plantation x10
        y = AfficherItemPlantationUpgrade(2, y, styleLabelMoyen, styleButton);

        // Plantation x25
        y = AfficherItemPlantationUpgrade(3, y, styleLabelMoyen, styleButton);

        y += 30;

        GUI.Label(new Rect(15, y, 910, 50), "FIN DU JEU", styleTitre);
        y += 65;

        y = AfficherItemVictoire(y, styleLabelMoyen, styleButton);

        GUI.EndScrollView();

        if (GUI.Button(new Rect(15, 810, 970, 60), "Fermer", styleButton))
        {
            FermerShop();
        }
    }

    int AfficherItemGraine(TypeGraine type, int prix, string description, int y)
    {
        bool debloque = ShopManager.Instance.EstGraineDebloquee(type);
        int argentActuel = MoneyManager.Instance.argentActuel;

        GUIStyle styleLabel = new GUIStyle(GUI.skin.label);
        styleLabel.fontSize = 30;

        GUIStyle styleButton = new GUIStyle(GUI.skin.button);
        styleButton.fontSize = 28;

        int hauteurBox = 150;
        if (description.Length > 60) hauteurBox = 180;
        if (description.Length > 100) hauteurBox = 210;

        Color couleurBox = debloque ? new Color(0.3f, 0.6f, 1f, 0.3f) : new Color(1f, 1f, 1f, 0.1f);

        GUI.Box(new Rect(15, y, 910, hauteurBox), "");

        string statut = debloque ? "✅ DÉBLOQUÉE" : $"{prix}$";
        GUI.Label(new Rect(30, y + 15, 700, 35), $"{type}", styleLabel);
        GUI.Label(new Rect(750, y + 15, 150, 35), statut, styleLabel);
        GUI.Label(new Rect(30, y + 55, 850, hauteurBox - 95), description, styleLabel);

        if (!debloque)
        {
            bool peutAcheter = argentActuel >= prix;

            GUIStyle styleBoutonPersonnalise = new GUIStyle(styleButton);
            styleBoutonPersonnalise.normal.textColor = Color.white;
            styleBoutonPersonnalise.hover.textColor = Color.white;

            Color couleurBouton = peutAcheter ? new Color(0.0f, 0.823f, 0.415f) : new Color(0.5f, 0.5f, 0.5f);
            GUI.backgroundColor = couleurBouton;
            GUI.enabled = peutAcheter;

            if (GUI.Button(new Rect(30, y + hauteurBox - 40, 850, 35), peutAcheter ? $"Acheter ({prix}$)" : $"Pas assez d'argent (besoin {prix}$)", styleBoutonPersonnalise))
            {
                if (MoneyManager.Instance.Depenser(prix))
                {
                    ShopManager.Instance.DebloquerGraine(type);
                }
            }

            GUI.enabled = true;
            GUI.backgroundColor = Color.white;
        }

        return y + hauteurBox + 15;
    }

    int AfficherItemArrosoir(int y, GUIStyle styleLabel, GUIStyle styleButton)
    {
        bool achete = ShopManager.Instance.arrosoirAchete;
        int prix = configShop.prixArrosoir;
        int argentActuel = MoneyManager.Instance.argentActuel;

        int hauteurBox = 150;
        if (configShop.descriptionArrosoir.Length > 60) hauteurBox = 180;
        if (configShop.descriptionArrosoir.Length > 100) hauteurBox = 210;

        Color couleurBox = achete ? new Color(0.3f, 0.6f, 1f, 0.3f) : new Color(1f, 1f, 1f, 0.1f);
        GUI.Box(new Rect(15, y, 910, hauteurBox), "");

        string statut = achete ? "✅ ACHETÉ" : $"{prix}$";
        GUI.Label(new Rect(30, y + 15, 700, 35), "💧 Arrosoir", styleLabel);
        GUI.Label(new Rect(750, y + 15, 150, 35), statut, styleLabel);
        GUI.Label(new Rect(30, y + 55, 850, hauteurBox - 95), configShop.descriptionArrosoir, styleLabel);

        if (!achete)
        {
            bool peutAcheter = argentActuel >= prix;

            GUIStyle styleBoutonPersonnalise = new GUIStyle(styleButton);
            styleBoutonPersonnalise.normal.textColor = Color.white;
            styleBoutonPersonnalise.hover.textColor = Color.white;

            Color couleurBouton = peutAcheter ? new Color(0.0f, 0.823f, 0.415f) : new Color(0.5f, 0.5f, 0.5f);
            GUI.backgroundColor = couleurBouton;
            GUI.enabled = peutAcheter;

            if (GUI.Button(new Rect(30, y + hauteurBox - 40, 850, 35), peutAcheter ? $"Acheter ({prix}$)" : $"Pas assez d'argent (besoin {prix}$)", styleBoutonPersonnalise))
            {
                ShopManager.Instance.AcheterArrosoir(prix);
            }

            GUI.enabled = true;
            GUI.backgroundColor = Color.white;
        }

        return y + hauteurBox + 15;
    }


    int AfficherItemVictoire(int y, GUIStyle styleLabel, GUIStyle styleButton)
    {
        bool achete = ShopManager.Instance.victoire;
        int prix = configShop.prixVictoire;
        int argentActuel = MoneyManager.Instance.argentActuel;

        int hauteurBox = 150;
        if (configShop.descriptionVictoire.Length > 60) hauteurBox = 180;
        if (configShop.descriptionVictoire.Length > 100) hauteurBox = 210;

        Color couleurBox = achete ? new Color(1f, 0.84f, 0f, 0.3f) : Color.white;
        Color ancienneCouleur = GUI.backgroundColor;
        GUI.backgroundColor = couleurBox;
        GUI.Box(new Rect(15, y, 910, hauteurBox), "");
        GUI.backgroundColor = ancienneCouleur;

        string statut = achete ? "🏆 VICTOIRE" : $"{prix}$";
        GUI.Label(new Rect(30, y + 15, 700, 35), "👑 LIBERTÉ - Fin du Jeu", styleLabel);
        GUI.Label(new Rect(750, y + 15, 150, 35), statut, styleLabel);
        GUI.Label(new Rect(30, y + 55, 850, hauteurBox - 95), configShop.descriptionVictoire, styleLabel);

        if (!achete)
        {
            bool peutAcheter = argentActuel >= prix;

            GUIStyle styleBoutonPersonnalise = new GUIStyle(styleButton);
            styleBoutonPersonnalise.normal.textColor = Color.white;
            styleBoutonPersonnalise.hover.textColor = Color.white;

            Color couleurBouton = peutAcheter ? new Color(0.0f, 0.823f, 0.415f) : new Color(0.5f, 0.5f, 0.5f);
            GUI.backgroundColor = couleurBouton;
            GUI.enabled = peutAcheter;

            if (GUI.Button(new Rect(30, y + hauteurBox - 40, 850, 35), peutAcheter ? $"ACHETER LA VICTOIRE ({prix}$)" : $"Pas assez d'argent (besoin {prix}$)", styleBoutonPersonnalise))
            {
                ShopManager.Instance.AcheterVictoire(prix);
            }

            GUI.enabled = true;
            GUI.backgroundColor = Color.white;
        }

        return y + hauteurBox + 15;
    }

    int AfficherItemAntiGravite(int y, GUIStyle styleLabel, GUIStyle styleButton)
    {
        bool achete = ShopManager.Instance.antiGraviteAchete;
        int prix = configShop.prixAntiGravite;
        int argentActuel = MoneyManager.Instance.argentActuel;

        int hauteurBox = 150;
        if (configShop.descriptionAntiGravite.Length > 60) hauteurBox = 180;
        if (configShop.descriptionAntiGravite.Length > 100) hauteurBox = 210;

        Color couleurBox = achete ? new Color(0.3f, 0.6f, 1f, 0.3f) : new Color(1f, 1f, 1f, 0.1f);
        GUI.Box(new Rect(15, y, 910, hauteurBox), "");

        string statut = achete ? "✅ ACHETÉ" : $"{prix}$";
        GUI.Label(new Rect(30, y + 15, 700, 35), "🚁 Anti-Gravité", styleLabel);
        GUI.Label(new Rect(750, y + 15, 150, 35), statut, styleLabel);
        GUI.Label(new Rect(30, y + 55, 850, hauteurBox - 95), configShop.descriptionAntiGravite, styleLabel);

        if (!achete)
        {
            bool peutAcheter = argentActuel >= prix;

            GUIStyle styleBoutonPersonnalise = new GUIStyle(styleButton);
            styleBoutonPersonnalise.normal.textColor = Color.white;
            styleBoutonPersonnalise.hover.textColor = Color.white;

            Color couleurBouton = peutAcheter ? new Color(0.0f, 0.823f, 0.415f) : new Color(0.5f, 0.5f, 0.5f);
            GUI.backgroundColor = couleurBouton;
            GUI.enabled = peutAcheter;

            if (GUI.Button(new Rect(30, y + hauteurBox - 40, 850, 35), peutAcheter ? $"Acheter ({prix}$)" : $"Pas assez d'argent (besoin {prix}$)", styleBoutonPersonnalise))
            {
                ShopManager.Instance.AcheterAntiGravite(prix);
            }

            GUI.enabled = true;
            GUI.backgroundColor = Color.white;
        }

        return y + hauteurBox + 15;
    }

    int AfficherItemAutoRecolteRarete(RareteLegume rarete, int y, GUIStyle styleLabel, GUIStyle styleButton)
    {
        RareteLegume niveauActuel = ShopManager.Instance.niveauAutoRecolte;
        int argentActuel = MoneyManager.Instance.argentActuel;

        // Déterminer le prix et la description selon la rareté
        int prix = 0;
        string description = "";
        string nomRarete = "";
        string icone = "";

        switch (rarete)
        {
            case RareteLegume.Commun:
                prix = configShop.prixAutoRecolteCommun;
                description = configShop.descriptionAutoRecolteCommun;
                nomRarete = "COMMUN";
                icone = "";
                break;
            case RareteLegume.Rare:
                prix = configShop.prixAutoRecolteRare;
                description = configShop.descriptionAutoRecolteRare;
                nomRarete = "RARE";
                icone = "";
                break;
            case RareteLegume.Epique:
                prix = configShop.prixAutoRecolteEpique;
                description = configShop.descriptionAutoRecolteEpique;
                nomRarete = "ÉPIQUE";
                icone = "";
                break;
            case RareteLegume.Legendaire:
                prix = configShop.prixAutoRecolteLegendaire;
                description = configShop.descriptionAutoRecolteLegendaire;
                nomRarete = "LÉGENDAIRE";
                icone = "";
                break;
            default:
                return y;
        }

        bool achete = niveauActuel >= rarete;

        int hauteurBox = 150;
        if (description.Length > 60) hauteurBox = 180;
        if (description.Length > 100) hauteurBox = 210;

        Color couleurBox;
        if (achete)
        {
            couleurBox = new Color(0.3f, 0.6f, 1f, 0.3f);
        }
        else
        {
            Color couleurRarete = RareteHelper.ObtenirCouleurRarete(rarete);
            couleurBox = new Color(couleurRarete.r, couleurRarete.g, couleurRarete.b, 0.2f);
        }

        GUI.Box(new Rect(15, y, 910, hauteurBox), "");

        string statut = achete ? "✅ ACHETÉ" : $"{prix}$";
        GUI.Label(new Rect(30, y + 15, 700, 35), $"🔄 Auto-Récolte {nomRarete}", styleLabel);
        GUI.Label(new Rect(750, y + 15, 150, 35), statut, styleLabel);
        GUI.Label(new Rect(30, y + 55, 850, hauteurBox - 95), description, styleLabel);

        if (!achete)
        {
            bool peutAcheter = argentActuel >= prix;

            GUIStyle styleBoutonPersonnalise = new GUIStyle(styleButton);
            styleBoutonPersonnalise.normal.textColor = Color.white;
            styleBoutonPersonnalise.hover.textColor = Color.white;

            Color couleurBouton = peutAcheter ? new Color(0.0f, 0.823f, 0.415f) : new Color(0.5f, 0.5f, 0.5f);
            GUI.backgroundColor = couleurBouton;
            GUI.enabled = peutAcheter;

            if (GUI.Button(new Rect(30, y + hauteurBox - 40, 850, 35), peutAcheter ? $"Acheter ({prix}$)" : $"Pas assez d'argent (besoin {prix}$)", styleBoutonPersonnalise))
            {
                ShopManager.Instance.AmeliorerAutoRecolte(rarete, prix);
            }

            GUI.enabled = true;
            GUI.backgroundColor = Color.white;
        }

        return y + hauteurBox + 15;
    }

    int AfficherItemPlantationUpgrade(int palier, int y, GUIStyle styleLabel, GUIStyle styleButton)
    {
        if (PlantationUpgradeManager.Instance == null)
        {
            GUI.Label(new Rect(15, y, 1110, 35), "⚠️ PlantationUpgradeManager introuvable !", styleLabel);
            return y + 50;
        }

        int palierActuel = PlantationUpgradeManager.Instance.palierActuel;
        int argentActuel = MoneyManager.Instance.argentActuel;

        // Déterminer le prix et la description selon le palier
        int prix = 0;
        string description = "";
        string nomPalier = "";
        int nbGraines = 0;

        switch (palier)
        {
            case 1:
                prix = configShop.prixPlantationPalier1;
                description = configShop.descriptionPlantationPalier1;
                nomPalier = "x5";
                nbGraines = 5;
                break;
            case 2:
                prix = configShop.prixPlantationPalier2;
                description = configShop.descriptionPlantationPalier2;
                nomPalier = "x10";
                nbGraines = 10;
                break;
            case 3:
                prix = configShop.prixPlantationPalier3;
                description = configShop.descriptionPlantationPalier3;
                nomPalier = "x25";
                nbGraines = 25;
                break;
            default:
                return y;
        }

        bool achete = palierActuel >= palier;
        bool peutAcheter = palierActuel == palier - 1;

        int hauteurBox = 150;
        if (description.Length > 60) hauteurBox = 180;
        if (description.Length > 100) hauteurBox = 210;

        Color couleurBox = achete ? new Color(0.3f, 0.6f, 1f, 0.3f) : new Color(1f, 1f, 1f, 0.1f);
        GUI.Box(new Rect(15, y, 910, hauteurBox), "");

        string statut = achete ? "✅ ACHETÉ" : $"{prix}$";
        GUI.Label(new Rect(30, y + 15, 700, 35), $"🌱 Plantation {nomPalier} ({nbGraines} graines)", styleLabel);
        GUI.Label(new Rect(750, y + 15, 150, 35), statut, styleLabel);
        GUI.Label(new Rect(30, y + 55, 850, hauteurBox - 95), description, styleLabel);

        if (!achete)
        {
            bool argentSuffisant = argentActuel >= prix;
            bool peutAcheterMaintenant = peutAcheter && argentSuffisant;

            string texteBouton;
            Color couleurBouton;

            if (!peutAcheter)
            {
                texteBouton = $"Achetez d'abord le palier précédent";
                couleurBouton = new Color(0.5f, 0.5f, 0.5f);
            }
            else if (!argentSuffisant)
            {
                texteBouton = $"Pas assez d'argent (besoin {prix}$)";
                couleurBouton = new Color(0.5f, 0.5f, 0.5f);
            }
            else
            {
                texteBouton = $"Acheter ({prix}$)";
                couleurBouton = new Color(0.0f, 0.823f, 0.415f);
            }

            GUIStyle styleBoutonPersonnalise = new GUIStyle(styleButton);
            styleBoutonPersonnalise.normal.textColor = Color.white;
            styleBoutonPersonnalise.hover.textColor = Color.white;

            GUI.backgroundColor = couleurBouton;
            GUI.enabled = peutAcheterMaintenant;

            if (GUI.Button(new Rect(30, y + hauteurBox - 40, 850, 35), texteBouton, styleBoutonPersonnalise))
            {
                ShopManager.Instance.AcheterPlantationUpgrade();
            }

            GUI.enabled = true;
            GUI.backgroundColor = Color.white;
        }

        return y + hauteurBox + 15;
    }
}
