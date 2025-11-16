using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("TextMeshPro pour le titre GAME OVER")]
    public TextMeshProUGUI texteTitre;

    [Tooltip("TextMeshPro pour afficher la raison du game over")]
    public TextMeshProUGUI texteRaison;

    [Tooltip("Bouton pour recommencer")]
    public Button boutonRecommencer;

    [Tooltip("Bouton pour quitter")]
    public Button boutonQuitter;

    [Header("Configuration")]
    [Tooltip("Nom de la scène de jeu principale")]
    public string nomSceneJeu = "GameScene";

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;

        if (texteTitre != null)
        {
            texteTitre.text = "GAME OVER";
        }

        if (texteRaison != null)
        {
            string raison = GameOverManager.ObtenirRaisonGameOver();
            if (string.IsNullOrEmpty(raison))
            {
                raison = "Vous avez perdu !";
            }
            texteRaison.text = raison;
        }

        if (boutonRecommencer != null)
        {
            boutonRecommencer.onClick.AddListener(Recommencer);
        }

        if (boutonQuitter != null)
        {
            boutonQuitter.onClick.AddListener(Quitter);
        }
    }

    void Recommencer()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nomSceneJeu);
    }

    void Quitter()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
