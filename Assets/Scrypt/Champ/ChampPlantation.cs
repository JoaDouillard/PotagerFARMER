using UnityEngine;
using System.Collections;

public class ChampPlantation : MonoBehaviour
{
    [Header("Configuration du champ")]
    public int nombreLignes = 5;
    public int nombreColonnes = 5;
    public float espacementX = 2f;
    public float espacementZ = 2f;
    public Vector3 tailleZone = new Vector3(1.5f, 2f, 1.5f);

    [Header("Position et hauteur")]
    [Tooltip("Offset vertical pour ajuster la hauteur des zones (0 = sur le prefab)")]
    public float offsetHauteur = 0f;

    [Header("Ajustement de position du champ")]
    [Tooltip("Décalage manuel en X pour centrer les zones sur le champ")]
    public float offsetPositionX = 2.375499f;

    [Tooltip("Décalage manuel en Z pour centrer les zones sur le champ")]
    public float offsetPositionZ = 2.3755f;

    [Header("Visibilité")]
    [Tooltip("Afficher les zones en debug (vert/rouge)")]
    public bool afficherZonesDebug = false;

    private ZonePlantation[] zones;
    private bool estBoostActif = false;
    private float multiplicateurBoost = 1f;

    void Start()
    {
        GenererZones();
        StartCoroutine(VerifierEtatArrosageEnContinu());
    }

    void GenererZones()
    {
        zones = new ZonePlantation[nombreLignes * nombreColonnes];

        // Calcul du centrage automatique
        float offsetX = -(nombreColonnes - 1) * espacementX / 2f;
        float offsetZ = -(nombreLignes - 1) * espacementZ / 2f;

        offsetX += offsetPositionX;
        offsetZ += offsetPositionZ;

        int index = 0;
        for (int ligne = 0; ligne < nombreLignes; ligne++)
        {
            for (int colonne = 0; colonne < nombreColonnes; colonne++)
            {
                float x = offsetX + colonne * espacementX;
                float z = offsetZ + ligne * espacementZ;

                // Position de base = position du ChampPlantation + offset hauteur
                Vector3 position = transform.position + new Vector3(x, offsetHauteur, z);

                // Raycast pour détecter le sol
                RaycastHit hit;
                if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down, out hit, 20f))
                {
                    position = hit.point + Vector3.up * offsetHauteur;
                }

                GameObject zoneObj = new GameObject($"Zone_{ligne}_{colonne}");
                zoneObj.transform.position = position;
                zoneObj.transform.SetParent(transform);

                BoxCollider box = zoneObj.AddComponent<BoxCollider>();
                box.isTrigger = true;
                box.size = tailleZone;

                ZonePlantation zone = zoneObj.AddComponent<ZonePlantation>();
                zone.champParent = this;
                zone.afficherZoneDebug = afficherZonesDebug;

                MeshRenderer renderer = zoneObj.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.enabled = afficherZonesDebug;
                }

                zones[index] = zone;
                index++;
            }
        }
    }

    public ZonePlantation ObtenirZoneProche(Vector3 position, float distanceMax = 2f)
    {
        ZonePlantation zonePlusProche = null;
        float distanceMin = float.MaxValue;

        foreach (ZonePlantation zone in zones)
        {
            if (zone == null) continue;

            float distance = Vector3.Distance(position, zone.transform.position);
            if (distance < distanceMin && distance <= distanceMax)
            {
                distanceMin = distance;
                zonePlusProche = zone;
            }
        }

        return zonePlusProche;
    }

    public void AppliquerBoostArrosage(float duree, float multiplicateur)
    {
        if (estBoostActif)
        {
            return;
        }

        StartCoroutine(ActiverArrosagePendant(duree, multiplicateur));
    }

    public bool EstBoostActif()
    {
        return estBoostActif;
    }

    IEnumerator ActiverArrosagePendant(float duree, float multiplicateur)
    {
        estBoostActif = true;
        multiplicateurBoost = multiplicateur;

        Debug.Log($"[ChampPlantation] Arrosage activé pour {duree}s");

        yield return new WaitForSeconds(duree);

        estBoostActif = false;
        multiplicateurBoost = 1f;

        Debug.Log($"[ChampPlantation] Arrosage terminé");
    }

    IEnumerator VerifierEtatArrosageEnContinu()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (zones == null) continue;

            foreach (ZonePlantation zone in zones)
            {
                if (zone != null && zone.planteCourante != null)
                {
                    Seed seed = zone.planteCourante.GetComponent<Seed>();
                    if (seed != null && estBoostActif)
                    {
                        seed.vitesseCroissance *= multiplicateurBoost;
                        seed.tempsCroissance /= multiplicateurBoost;
                    }
                    else if (seed != null && !estBoostActif)
                    {
                        seed.vitesseCroissance /= multiplicateurBoost;
                        seed.tempsCroissance *= multiplicateurBoost;
                    }

                    Legume legume = zone.planteCourante.GetComponent<Legume>();
                    if (legume != null)
                    {
                        if (estBoostActif)
                        {
                            legume.Arroser();
                        }
                        else
                        {
                            legume.ArreterArrosage();
                        }
                    }
                }
            }
        }
    }
}
