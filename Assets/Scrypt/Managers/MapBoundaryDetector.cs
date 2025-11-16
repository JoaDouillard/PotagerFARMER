using UnityEngine;
using UnityEngine.SceneManagement;

public class MapBoundaryDetector : MonoBehaviour
{
    [Header("Limites de la carte")]
    [Tooltip("Distance maximale sur l'axe X")]
    public float limiteX = 100f;

    [Tooltip("Distance maximale sur l'axe Z")]
    public float limiteZ = 100f;

    [Tooltip("Hauteur minimale (chute)")]
    public float limiteYMin = -50f;

    [Tooltip("Hauteur maximale")]
    public float limiteYMax = 200f;

    [Header("Configuration")]
    [Tooltip("Objet à surveiller (généralement le drone)")]
    public Transform objetASurveiller;

    [Tooltip("Nom de la scène Win")]
    public string nomSceneWin = "GameWin";

    void Update()
    {
        if (objetASurveiller == null) return;

        Vector3 pos = objetASurveiller.position;

        if (Mathf.Abs(pos.x) > limiteX ||
            Mathf.Abs(pos.z) > limiteZ ||
            pos.y < limiteYMin ||
            pos.y > limiteYMax)
        {
            SortieDeMap();
        }
    }

    void SortieDeMap()
    {
        PlayerPrefs.SetString("WinReason", "escape");
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        SceneManager.LoadScene(nomSceneWin);
    }

}
