using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoireUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("TextMeshPro pour le titre VICTOIRE")]
    public TextMeshProUGUI texteTitre;

    [Tooltip("TextMeshPro pour le message de victoire")]
    public TextMeshProUGUI texteMessage;

    [Tooltip("Bouton pour recommencer")]
    public Button boutonRejouer;

    [Tooltip("Bouton pour quitter")]
    public Button boutonQuitter;

    [Header("Configuration")]
    [Tooltip("Nom de la scène de jeu principale")]
    public string nomSceneJeu = "GameScene";

    [Header("Messages")]
    [Tooltip("Message de victoire normale (paiement)")]
    [TextArea(3, 5)]
    public string messageVictoireNormale = "Félicitations !\n\nVous avez acheté votre liberté\net gagné la partie !";

    [Tooltip("Message de victoire par évasion")]
    [TextArea(3, 5)]
    public string messageVictoireEvasion = "Bravo, vous vous êtes échappé sans payer,\n\nmais vous gardez votre corps de drone et bonne chance à vous pour retrouver votre corps.\n\nCourage, mais vous êtes plus malin que la moyenne des gens.";

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;

        if (texteTitre != null)
        {
            texteTitre.text = "VICTOIRE !";
        }

        if (texteMessage != null)
        {
            string winReason = PlayerPrefs.GetString("WinReason", "normal");

            if (winReason == "escape")
            {
                texteMessage.text = messageVictoireEvasion;
            }
            else
            {
                texteMessage.text = messageVictoireNormale;
            }

            PlayerPrefs.DeleteKey("WinReason");
            PlayerPrefs.Save();
        }

        if (boutonRejouer != null)
        {
            boutonRejouer.onClick.AddListener(Rejouer);
        }

        if (boutonQuitter != null)
        {
            boutonQuitter.onClick.AddListener(Quitter);
        }
    }

    void Rejouer()
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
