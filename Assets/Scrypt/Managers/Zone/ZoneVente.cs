using UnityEngine;

public class ZoneVente : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Tag du drone pour détecter l'entrée")]
    public string tagDrone = "Player";

    private bool droneEstDansLaZone = false;
    private BoxCollider zoneCollider;

    void Start()
    {
        zoneCollider = GetComponent<BoxCollider>();
        if (zoneCollider == null)
        {
            zoneCollider = gameObject.AddComponent<BoxCollider>();
        }

        zoneCollider.isTrigger = true;
    }

    void Update()
    {
        if (PlayerInputManager.Instance == null) return;

        if (droneEstDansLaZone && PlayerInputManager.Instance.Controls.Drone.Interact.WasPressedThisFrame())
        {
            VendreTousLesLegumes();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagDrone))
        {
            droneEstDansLaZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagDrone))
        {
            droneEstDansLaZone = false;
        }
    }

    void VendreTousLesLegumes()
    {
        int valeurTotale = InventoryManager.Instance.ObtenirValeurTotale();
        int nbLegumes = InventoryManager.Instance.ObtenirTotalLegumes();

        if (InventoryManager.Instance == null || MoneyManager.Instance == null || nbLegumes == 0)
        {
            return;
        }

        MoneyManager.Instance.Gagner(valeurTotale);

        InventoryManager.Instance.ViderInventaire();
    }

    void OnGUI()
    {
        if (droneEstDansLaZone)
        {
            int valeurTotale = 0;
            int nbLegumes = 0;

            if (InventoryManager.Instance != null)
            {
                valeurTotale = InventoryManager.Instance.ObtenirValeurTotale();
                nbLegumes = InventoryManager.Instance.ObtenirTotalLegumes();
            }

            float largeur = 800f;
            float hauteur = 200f;
            float posX = (Screen.width - largeur) / 2f;
            float posY = Screen.height - hauteur - 50f;

            GUIStyle styleBox = new GUIStyle(GUI.skin.box);
            styleBox.fontSize = 32;
            styleBox.alignment = TextAnchor.UpperCenter;
            styleBox.fontStyle = FontStyle.Bold;

            GUIStyle styleLabel = new GUIStyle(GUI.skin.label);
            styleLabel.fontSize = 26;
            styleLabel.alignment = TextAnchor.MiddleCenter;

            GUI.Box(new Rect(posX, posY, largeur, hauteur), "ZONE DE VENTE", styleBox);

            GUI.Label(new Rect(posX + 50, posY + 60, largeur - 100, 40), $"Légumes : {nbLegumes} | Valeur : {valeurTotale}$", styleLabel);
            GUI.Label(new Rect(posX + 50, posY + 110, largeur - 100, 40), $"Appuyez sur [E] pour vendre", styleLabel);
        }
    }

}
