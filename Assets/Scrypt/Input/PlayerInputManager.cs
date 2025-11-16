using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance { get; private set; }

    private PlayerControls controls;

    // Accès public aux contrôles
    public PlayerControls Controls => controls;

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Initialiser les contrôles
        controls = new PlayerControls();
        Debug.Log("[PlayerInputManager] Initialisé avec succès");
    }

    void OnEnable()
    {
        // Activer l'Action Map Drone
        controls.Drone.Enable();
        Debug.Log("[PlayerInputManager] Contrôles Drone activés");
    }

    void OnDisable()
    {
        // Désactiver l'Action Map Drone
        controls.Drone.Disable();
    }

    // Méthodes utilitaires pour activer/désactiver les contrôles
    public void EnableDroneControls()
    {
        controls.Drone.Enable();
    }

    public void DisableDroneControls()
    {
        controls.Drone.Disable();
    }
}
