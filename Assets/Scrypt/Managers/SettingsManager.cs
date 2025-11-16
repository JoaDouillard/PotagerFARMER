using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [Header("Paramètres Audio")]
    [Range(0f, 1f)]
    public float volumeGeneral = 1f;

    [Header("Paramètres Souris")]
    [Range(0.5f, 5f)]
    public float sensibiliteSourisX = 2f;

    [Range(0.5f, 5f)]
    public float sensibiliteSourisY = 2f;

    private const string KEY_VOLUME = "VolumeGeneral";
    private const string KEY_SENS_X = "SensibiliteX";
    private const string KEY_SENS_Y = "SensibiliteY";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ChargerParametres();
    }

    void ChargerParametres()
    {
        volumeGeneral = PlayerPrefs.GetFloat(KEY_VOLUME, 1f);
        sensibiliteSourisX = PlayerPrefs.GetFloat(KEY_SENS_X, 2f);
        sensibiliteSourisY = PlayerPrefs.GetFloat(KEY_SENS_Y, 2f);

        AppliquerParametres();
        Debug.Log($"[SettingsManager] Paramètres chargés - Volume: {volumeGeneral}, Sens: {sensibiliteSourisX}/{sensibiliteSourisY}");
    }

    public void ModifierVolume(float nouveauVolume)
    {
        volumeGeneral = Mathf.Clamp01(nouveauVolume);
        AudioListener.volume = volumeGeneral;
        PlayerPrefs.SetFloat(KEY_VOLUME, volumeGeneral);
        PlayerPrefs.Save();
    }

    public void ModifierSensibiliteX(float nouvelleSensibilite)
    {
        sensibiliteSourisX = Mathf.Clamp(nouvelleSensibilite, 0.5f, 5f);
        PlayerPrefs.SetFloat(KEY_SENS_X, sensibiliteSourisX);
        PlayerPrefs.Save();

        DroneController drone = FindObjectOfType<DroneController>();
        if (drone != null && drone.cameraController != null)
        {
            drone.cameraController.sensibiliteSourisX = sensibiliteSourisX;
        }
    }

    public void ModifierSensibiliteY(float nouvelleSensibilite)
    {
        sensibiliteSourisY = Mathf.Clamp(nouvelleSensibilite, 0.5f, 5f);
        PlayerPrefs.SetFloat(KEY_SENS_Y, sensibiliteSourisY);
        PlayerPrefs.Save();

        DroneController drone = FindObjectOfType<DroneController>();
        if (drone != null && drone.cameraController != null)
        {
            drone.cameraController.sensibiliteSourisY = sensibiliteSourisY;
        }
    }

    void AppliquerParametres()
    {
        AudioListener.volume = volumeGeneral;

        DroneController drone = FindObjectOfType<DroneController>();
        if (drone != null && drone.cameraController != null)
        {
            drone.cameraController.sensibiliteSourisX = sensibiliteSourisX;
            drone.cameraController.sensibiliteSourisY = sensibiliteSourisY;
        }
    }

    public void ReinitialiserParametres()
    {
        ModifierVolume(1f);
        ModifierSensibiliteX(2f);
        ModifierSensibiliteY(2f);
        Debug.Log("[SettingsManager] Paramètres réinitialisés");
    }
}
