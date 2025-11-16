using UnityEngine;

[RequireComponent(typeof(DroneMovement))]
[RequireComponent(typeof(DroneCamera))]
[RequireComponent(typeof(DroneInteraction))]
public class DroneController : MonoBehaviour
{
    [Header("Composants du drone")]
    public DroneMovement movement;
    public DroneCamera cameraController;
    public DroneInteraction interaction;

    [Header("Collision et Explosion")]
    public float vitesseExplosion = 15f;
    public GameObject particulesExplosion;

    private Vector3 dernierePosition;
    private float vitesseActuelle = 0f;
    private bool aExplose = false;
    private MeshRenderer[] meshRenderers;
    private CharacterController characterController;

    void Awake()
    {
        // Récupérer les composants
        movement = GetComponent<DroneMovement>();
        cameraController = GetComponent<DroneCamera>();
        interaction = GetComponent<DroneInteraction>();
        characterController = GetComponent<CharacterController>();

        if (movement == null)
        {
            Debug.LogError("[DroneController] DroneMovement manquant !");
        }

        if (cameraController == null)
        {
            Debug.LogError("[DroneController] DroneCamera manquant !");
        }

        if (interaction == null)
        {
            Debug.LogError("[DroneController] DroneInteraction manquant !");
        }

        // Récupérer les MeshRenderer pour l'explosion
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        dernierePosition = transform.position;
    }

    void Update()
    {
        CalculerVitesse();
    }

    void CalculerVitesse()
    {
        float distance = Vector3.Distance(transform.position, dernierePosition);
        vitesseActuelle = distance / Time.deltaTime;
        dernierePosition = transform.position;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (aExplose) return;

        if (PlayerInputManager.Instance == null) return;

        bool estEnMouvement = PlayerInputManager.Instance.Controls.Drone.Move.ReadValue<Vector2>().magnitude > 0.1f;
        bool estEnSprint = PlayerInputManager.Instance.Controls.Drone.Sprint.IsPressed();

        if (estEnMouvement && estEnSprint && vitesseActuelle >= vitesseExplosion)
        {
            Exploser();
        }
    }

    void Exploser()
    {
        if (aExplose) return;
        aExplose = true;

        Debug.Log($"[DroneController] Explosion - Vitesse: {vitesseActuelle:F1} m/s");

        // Désactiver ce script
        this.enabled = false;

        // Désactiver les autres composants
        if (movement != null) movement.enabled = false;
        if (cameraController != null) cameraController.enabled = false;
        if (interaction != null) interaction.enabled = false;

        // Cacher le mesh du drone
        if (meshRenderers != null)
        {
            foreach (MeshRenderer mr in meshRenderers)
            {
                if (mr != null)
                {
                    mr.enabled = false;
                }
            }
        }

        // Spawn des particules d'explosion
        if (particulesExplosion != null)
        {
            GameObject explosion = Instantiate(particulesExplosion, transform.position, Quaternion.identity);
            Destroy(explosion, 3f);
        }

        // Jouer le son d'explosion
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.JouerSon(SoundManager.Instance.sonExplosionDrone);
        }

        // Attendre 2 secondes avant le Game Over
        StartCoroutine(GameOverApresDelai(2f));
    }

    System.Collections.IEnumerator GameOverApresDelai(float delai)
    {
        yield return new WaitForSeconds(delai);

        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.DeclenecherGameOver($"CRASH FATAL !\n\nVotre drone a explosé en percutant un obstacle\nà {vitesseActuelle:F1} m/s !");
        }

        Destroy(gameObject);
    }
}
