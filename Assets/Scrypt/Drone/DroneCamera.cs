using UnityEngine;

public class DroneCamera : MonoBehaviour
{
    [Header("Caméra")]
    public Camera cameraJoueur;

    [Header("Sensibilité souris")]
    public float sensibiliteSourisX = 2f;
    public float sensibiliteSourisY = 2f;

    [Header("Offsets caméra")]
    public Vector3 offsetCameraFPS = new Vector3(0f, 0f, 1f);
    public Vector3 offsetCameraTPS = new Vector3(0f, 3f, -8f);
    public float vitesseTransitionCamera = 25f;

    [Header("Limites d'inclinaison")]
    public float limiteInclinaisonCameraHaut = 30f;
    public float limiteInclinaisonCameraBas = 10f;

    private float rotationCameraX = 0f;
    private float rotationCameraY = 0f;
    private bool estEnFPS = true;
    private Vector3 offsetCameraCible;
    private Vector3 velociteCamera;

    public float RotationCameraY => rotationCameraY;

    void Start()
    {
        if (cameraJoueur == null)
        {
            cameraJoueur = Camera.main;
        }

        offsetCameraCible = offsetCameraFPS;

        if (cameraJoueur != null)
        {
            cameraJoueur.transform.SetParent(null);
        }
    }

    void Update()
    {
        if (Time.timeScale <= 0f || PlayerInputManager.Instance == null) return;

        GererRotationCamera();
        GererSwitchCamera();
    }

    void LateUpdate()
    {
        if (cameraJoueur != null)
        {
            PositionnerCamera();
        }
    }

    void GererRotationCamera()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        Vector2 lookInput = PlayerInputManager.Instance.Controls.Drone.Look.ReadValue<Vector2>();
        float mouseX = lookInput.x * sensibiliteSourisX * Time.deltaTime;
        float mouseY = lookInput.y * sensibiliteSourisY * Time.deltaTime;

        rotationCameraY += mouseX;
        rotationCameraX -= mouseY;
        rotationCameraX = Mathf.Clamp(rotationCameraX, limiteInclinaisonCameraBas, limiteInclinaisonCameraHaut);
    }

    void PositionnerCamera()
    {
        offsetCameraCible = estEnFPS ? offsetCameraFPS : offsetCameraTPS;

        Vector3 positionCible = transform.position + transform.TransformDirection(offsetCameraCible);

        cameraJoueur.transform.position = Vector3.SmoothDamp(
            cameraJoueur.transform.position,
            positionCible,
            ref velociteCamera,
            1f / vitesseTransitionCamera
        );

        cameraJoueur.transform.rotation = Quaternion.Euler(rotationCameraX, rotationCameraY, 0f);
    }

    void GererSwitchCamera()
    {
        if (PlayerInputManager.Instance.Controls.Drone.SwitchCamera.WasPressedThisFrame())
        {
            estEnFPS = !estEnFPS;
        }
    }
}
