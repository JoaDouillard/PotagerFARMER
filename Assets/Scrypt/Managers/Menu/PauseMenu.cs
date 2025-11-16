using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("Références UI")]
    public GameObject panelPause;
    public Slider sliderVolume;
    public Slider sliderSensibilitX;
    public Slider sliderSensibilitY;
    public TextMeshProUGUI texteVolume;
    public TextMeshProUGUI texteSensibilitX;
    public TextMeshProUGUI texteSensibilitY;
    public Button boutonReprendre;
    public Button boutonReinitialiser;
    public Button boutonRevoirTuto;
    public Button boutonQuitter;

    private bool menuOuvert = false;

    void Start()
    {
        if (panelPause != null)
        {
            panelPause.SetActive(false);
        }
        if (boutonReprendre != null)
        {
            boutonReprendre.onClick.AddListener(FermerMenu);
        }

        if (boutonReinitialiser != null)
        {
            boutonReinitialiser.onClick.AddListener(ReinitialiserParametres);
        }

        if (boutonRevoirTuto != null)
        {
            boutonRevoirTuto.onClick.AddListener(RevoirTutoriel);
        }

        if (boutonQuitter != null)
        {
            boutonQuitter.onClick.AddListener(QuitterJeu);
        }

        if (sliderVolume != null)
        {
            sliderVolume.minValue = 0f;
            sliderVolume.maxValue = 1f;
            sliderVolume.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (sliderSensibilitX != null)
        {
            sliderSensibilitX.minValue = 0.5f;
            sliderSensibilitX.maxValue = 5f;
            sliderSensibilitX.onValueChanged.AddListener(OnSensibilitXChanged);
        }

        if (sliderSensibilitY != null)
        {
            sliderSensibilitY.minValue = 0.5f;
            sliderSensibilitY.maxValue = 5f;
            sliderSensibilitY.onValueChanged.AddListener(OnSensibilitYChanged);
        }

        ChargerValeursActuelles();
    }

    void Update()
    {
        if (PlayerInputManager.Instance == null) return;

        if (PlayerInputManager.Instance.Controls.Drone.OpenPauseMenu.WasPressedThisFrame())
        {
            if (MenuManager.Instance != null)
            {
                if (MenuManager.Instance.PeutOuvrirMenu(TypeMenu.Pause))
                {
                    ToggleMenu();
                }
            }
            else
            {
                ToggleMenu();
            }
        }
    }

    void ToggleMenu()
    {
        menuOuvert = !menuOuvert;

        if (panelPause != null)
        {
            panelPause.SetActive(menuOuvert);
        }

        if (menuOuvert)
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.JouerSonUI(SoundManager.Instance.sonOuverturePause);
            }
            if (MenuManager.Instance != null)
            {
                MenuManager.Instance.OuvrirMenu(TypeMenu.Pause);
            }
            else
            {
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            ChargerValeursActuelles();
        }
        else
        {
            FermerMenu();
        }
    }

    void FermerMenu()
    {
        menuOuvert = false;

        if (panelPause != null)
        {
            panelPause.SetActive(false);
        }

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

    public void FermerMenuExterne()
    {
        if (menuOuvert)
        {
            menuOuvert = false;

            if (panelPause != null)
            {
                panelPause.SetActive(false);
            }
        }
    }

    void ChargerValeursActuelles()
    {
        if (SettingsManager.Instance == null) return;

        if (sliderVolume != null)
        {
            sliderVolume.value = SettingsManager.Instance.volumeGeneral;
        }

        if (sliderSensibilitX != null)
        {
            sliderSensibilitX.value = SettingsManager.Instance.sensibiliteSourisX;
        }

        if (sliderSensibilitY != null)
        {
            sliderSensibilitY.value = SettingsManager.Instance.sensibiliteSourisY;
        }

        MettreAJourTextes();
    }

    void OnVolumeChanged(float valeur)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.ModifierVolume(valeur);
        }
        MettreAJourTexteVolume();
    }

    void OnSensibilitXChanged(float valeur)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.ModifierSensibiliteX(valeur);
        }
        MettreAJourTexteSensibilitX();
    }

    void OnSensibilitYChanged(float valeur)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.ModifierSensibiliteY(valeur);
        }
        MettreAJourTexteSensibilitY();
    }

    void MettreAJourTextes()
    {
        MettreAJourTexteVolume();
        MettreAJourTexteSensibilitX();
        MettreAJourTexteSensibilitY();
    }

    void MettreAJourTexteVolume()
    {
        if (texteVolume != null && sliderVolume != null)
        {
            texteVolume.text = $"{Mathf.RoundToInt(sliderVolume.value * 100)}%";
        }
    }

    void MettreAJourTexteSensibilitX()
    {
        if (texteSensibilitX != null && sliderSensibilitX != null)
        {
            texteSensibilitX.text = $"{sliderSensibilitX.value:F1}x";
        }
    }

    void MettreAJourTexteSensibilitY()
    {
        if (texteSensibilitY != null && sliderSensibilitY != null)
        {
            texteSensibilitY.text = $"{sliderSensibilitY.value:F1}x";
        }
    }

    void ReinitialiserParametres()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.ReinitialiserParametres();
            ChargerValeursActuelles();
        }
    }

    public void RevoirTutoriel()
    {
        if (TutorialManager.Instance != null)
        {
            // Fermer le menu pause d'abord
            FermerMenu();

            // Réafficher le tutoriel
            TutorialManager.Instance.ReafficherTutoriel();

            Debug.Log("[PauseMenu] Réaffichage du tutoriel");
        }
        else
        {
            Debug.LogWarning("[PauseMenu] TutorialManager introuvable !");
        }
    }

    void QuitterJeu()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
