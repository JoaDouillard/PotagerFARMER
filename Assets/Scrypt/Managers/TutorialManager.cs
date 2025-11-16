using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Panels UI")]
    [Tooltip("Panel pour l'histoire (contexte du jeu)")]
    public GameObject panelHistoire;

    [Tooltip("Panel pour les commandes (touches du clavier)")]
    public GameObject panelCommandes;

    [Header("Textes à cacher")]
    [Tooltip("Texte affichant l'argent")]
    public TextMeshProUGUI texteArgent;

    [Tooltip("Texte affichant la graine équipée")]
    public TextMeshProUGUI texteGraineEquipee;

    [Header("Configuration")]
    [Tooltip("Le tutoriel a-t-il déjà été vu ?")]
    public bool tutorielDejavu = false;

    [Tooltip("Afficher le tutoriel au démarrage")]
    public bool afficherAuDemarrage = true;

    // État interne du tutoriel
    private enum EtatTuto { Histoire, Commandes, Termine }
    private EtatTuto etatActuel = EtatTuto.Termine;

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
        if (panelHistoire != null)
        {
            panelHistoire.SetActive(false);
        }

        if (panelCommandes != null)
        {
            panelCommandes.SetActive(false);
        }

        if (afficherAuDemarrage && !tutorielDejavu)
        {
            AfficherTutoriel();
        }
    }

    void Update()
    {
        if (etatActuel != EtatTuto.Termine)
        {
            if (Input.anyKeyDown)
            {
                SkipTuto();
            }
        }
    }

    void SkipTuto()
    {
        if (etatActuel == EtatTuto.Histoire)
        {
            PasserAuxCommandes();
        }
        else if (etatActuel == EtatTuto.Commandes)
        {
            FermerTutoriel();
        }
    }

    public void AfficherTutoriel()
    {
        if (panelHistoire != null)
        {
            panelHistoire.SetActive(true);
        }

        if (panelCommandes != null)
        {
            panelCommandes.SetActive(false);
        }

        if (texteArgent != null)
        {
            texteArgent.gameObject.SetActive(false);
        }

        if (texteGraineEquipee != null)
        {
            texteGraineEquipee.gameObject.SetActive(false);
        }

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        etatActuel = EtatTuto.Histoire;
    }

    void PasserAuxCommandes()
    {

        if (panelHistoire != null)
        {
            panelHistoire.SetActive(false);
        }

        if (panelCommandes != null)
        {
            panelCommandes.SetActive(true);
        }

        etatActuel = EtatTuto.Commandes;
    }

    void FermerTutoriel()
    {
        if (panelHistoire != null)
        {
            panelHistoire.SetActive(false);
        }

        if (panelCommandes != null)
        {
            panelCommandes.SetActive(false);
        }

        if (texteArgent != null)
        {
            texteArgent.gameObject.SetActive(true);
        }

        if (texteGraineEquipee != null)
        {
            texteGraineEquipee.gameObject.SetActive(true);
        }

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        tutorielDejavu = true;
        etatActuel = EtatTuto.Termine;
    }

    public void ReafficherTutoriel()
    {
        AfficherTutoriel();
    }

    public void ResetTutoriel()
    {
        tutorielDejavu = false;

    }
}
