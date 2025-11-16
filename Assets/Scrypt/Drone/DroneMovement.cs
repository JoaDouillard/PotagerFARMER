using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DroneMovement : MonoBehaviour
{
    [Header("Vitesses de déplacement")]
    public float vitesseDeplacement = 5f;
    public float vitesseSprint = 10f;
    public float vitesseMonteeDescente = 3f;
    public float chuteLente = 0.5f;

    [Header("Limites d'altitude")]
    public float altitudeMin = 0.5f;
    public float altitudeMax = 50f;

    [Header("Inclinaison du drone")]
    public float angleInclinaisonMax = 15f;
    public float vitesseInclinaison = 8f;

    private CharacterController characterController;
    private float rotationCameraY;
    private Vector2 inputActuel;
    private float inclinaisonAvantActuelle = 0f;
    private float inclinaisonCoteActuelle = 0f;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("[DroneMovement] CharacterController manquant !");
        }
    }

    void Start()
    {
        StartCoroutine(GererAltitudeCoroutine());
        StartCoroutine(LimiterAltitudeCoroutine());
    }

    void Update()
    {
        if (Time.timeScale > 0f)
        {
            GererDeplacement();
            SynchroniserRotationAvecCamera();
            GererInclinaisonMesh();
        }
    }

    void SynchroniserRotationAvecCamera()
    {
        DroneCamera droneCamera = GetComponent<DroneCamera>();
        if (droneCamera != null)
        {
            rotationCameraY = droneCamera.RotationCameraY;
        }
    }

    void GererDeplacement()
    {
        if (characterController == null || PlayerInputManager.Instance == null) return;

        inputActuel = PlayerInputManager.Instance.Controls.Drone.Move.ReadValue<Vector2>();

        bool isSprinting = PlayerInputManager.Instance.Controls.Drone.Sprint.IsPressed();
        float vitesseActuelle = isSprinting ? vitesseSprint : vitesseDeplacement;

        DroneCamera droneCamera = GetComponent<DroneCamera>();
        if (droneCamera != null)
        {
            rotationCameraY = droneCamera.RotationCameraY;
        }

        Vector3 directionCamera = Quaternion.Euler(0f, rotationCameraY, 0f) * Vector3.forward;
        Vector3 droiteCamera = Quaternion.Euler(0f, rotationCameraY, 0f) * Vector3.right;

        Vector3 direction = (directionCamera * inputActuel.y + droiteCamera * inputActuel.x).normalized;
        Vector3 mouvement = direction * vitesseActuelle * Time.deltaTime;
        characterController.Move(mouvement);

        bool estEnMouvementHorizontal = inputActuel.magnitude > 0.1f;
        bool estEnMouvementVertical = PlayerInputManager.Instance.Controls.Drone.Ascend.IsPressed() ||
                                      PlayerInputManager.Instance.Controls.Drone.Descend.IsPressed();
        bool estEnMouvement = estEnMouvementHorizontal || estEnMouvementVertical;

        if (SoundManager.Instance != null)
        {
            if (isSprinting && estEnMouvement)
            {
                float vitesseNormalisee = mouvement.magnitude / (vitesseSprint * Time.deltaTime);
                SoundManager.Instance.AjusterVolumeMoteurDrone(vitesseNormalisee, 1f);
            }
            else
            {
                SoundManager.Instance.AjusterVolumeMoteurDrone(0f, 1f);
            }
        }
    }

    System.Collections.IEnumerator GererAltitudeCoroutine()
    {
        while (true)
        {
            if (Time.timeScale > 0f && characterController != null && PlayerInputManager.Instance != null)
            {
                float deplVertical = 0f;

                if (PlayerInputManager.Instance.Controls.Drone.Ascend.IsPressed())
                {
                    deplVertical = vitesseMonteeDescente * Time.deltaTime;
                }
                else if (PlayerInputManager.Instance.Controls.Drone.Descend.IsPressed())
                {
                    deplVertical = -vitesseMonteeDescente * Time.deltaTime;
                }
                else
                {
                    deplVertical = -chuteLente * Time.deltaTime;
                }

                characterController.Move(new Vector3(0, deplVertical, 0));
            }

            yield return null;
        }
    }

    System.Collections.IEnumerator LimiterAltitudeCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (Time.timeScale > 0f && characterController != null)
            {
                Vector3 pos = transform.position;

                if (pos.y > altitudeMax)
                {
                    pos.y = altitudeMax;
                    characterController.Move(pos - transform.position);
                }
                else if (pos.y < altitudeMin)
                {
                    pos.y = altitudeMin;
                    characterController.Move(pos - transform.position);
                }
            }
        }
    }

    void GererInclinaisonMesh()
    {
        float inclinaisonAvantCible = inputActuel.y * angleInclinaisonMax;
        float inclinaisonCoteCible = -inputActuel.x * angleInclinaisonMax;

        inclinaisonAvantActuelle = Mathf.Lerp(inclinaisonAvantActuelle, inclinaisonAvantCible, Time.deltaTime * vitesseInclinaison);
        inclinaisonCoteActuelle = Mathf.Lerp(inclinaisonCoteActuelle, inclinaisonCoteCible, Time.deltaTime * vitesseInclinaison);

        Quaternion rotationBase = Quaternion.Euler(0, rotationCameraY, 0f);
        Quaternion inclinaison = Quaternion.Euler(inclinaisonAvantActuelle, 0f, inclinaisonCoteActuelle);
        transform.rotation = rotationBase * inclinaison;
    }
}
