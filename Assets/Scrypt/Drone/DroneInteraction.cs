using UnityEngine;
using TMPro;

public class DroneInteraction : MonoBehaviour
{
    [Header("Plantation")]
    public GameObject[] grainesDisponibles;
    public int indexGraineCourante = 0;
    public float rayonDetectionZone = 3f;

    [Header("Récolte")]
    public float rayonDetectionLegume = 3f;

    [Header("Arrosoir")]
    public float rayonDetectionChamp = 5f;
    public float dureeBoostArrosage = 90f;
    public float multiplicateurArrosage = 2f;
    public GameObject particulesArrosoir;

    [Header("UI")]
    public TextMeshProUGUI texteGraineEquipee;

    private Vector3 offsetParticulesArrosoir = new Vector3(2.375499f, 2f, 2.3755f);

    void Start()
    {
        Invoke("MettreAJourAffichageGraine", 0.1f);
    }

    void Update()
    {
        if (Time.timeScale > 0f)
        {
            GererChangementGraine();
            GererPlantation();
            GererRecolte();
            GererArrosoir();
        }
    }

    void GererChangementGraine()
    {
        if (PlayerInputManager.Instance == null) return;

        if (PlayerInputManager.Instance.Controls.Drone.ChangeSeed.WasPressedThisFrame())
        {
            if (grainesDisponibles == null || grainesDisponibles.Length == 0)
            {
                Debug.LogWarning("[DroneInteraction] Aucune graine disponible !");
                return;
            }

            if (ShopManager.Instance == null)
            {
                Debug.LogWarning("[DroneInteraction] ShopManager introuvable !");
                return;
            }

            // Trouver la prochaine graine débloquée
            int tentatives = 0;
            int maxTentatives = grainesDisponibles.Length;

            do
            {
                indexGraineCourante = (indexGraineCourante + 1) % grainesDisponibles.Length;
                tentatives++;

                GameObject grainePrefab = grainesDisponibles[indexGraineCourante];
                Seed seedScript = grainePrefab.GetComponent<Seed>();

                if (seedScript != null)
                {
                    if (ShopManager.Instance.EstGraineDebloquee(seedScript.typeGraine))
                    {
                        MettreAJourAffichageGraine();

                        if (SoundManager.Instance != null)
                        {
                            SoundManager.Instance.JouerSonUI(SoundManager.Instance.sonChangementGraine);
                        }

                        break;
                    }
                }
            }
            while (tentatives < maxTentatives);

            if (tentatives >= maxTentatives)
            {
                Debug.LogWarning("[DroneInteraction] Aucune graine débloquée trouvée !");
            }
        }
    }

    void MettreAJourAffichageGraine()
    {
        if (texteGraineEquipee == null) return;

        if (grainesDisponibles == null || grainesDisponibles.Length == 0)
        {
            texteGraineEquipee.text = "Aucune graine";
            return;
        }

        if (ShopManager.Instance == null)
        {
            Invoke("MettreAJourAffichageGraine", 0.1f);
            return;
        }

        GameObject grainePrefab = grainesDisponibles[indexGraineCourante];
        Seed seedScript = grainePrefab.GetComponent<Seed>();

        if (seedScript != null)
        {
            bool debloquee = ShopManager.Instance.EstGraineDebloquee(seedScript.typeGraine);

            if (debloquee)
            {
                texteGraineEquipee.text = $"Graine : {seedScript.typeGraine} ({seedScript.prixAchat}$)";
                texteGraineEquipee.color = Color.white;
            }
        }
        else
        {
            texteGraineEquipee.text = "Erreur graine";
        }
    }

    void GererPlantation()
    {
        if (PlayerInputManager.Instance == null) return;

        if (PlayerInputManager.Instance.Controls.Drone.Interact.WasPressedThisFrame())
        {
            PlanterGraine();
        }
    }

    void PlanterGraine()
    {
        if (grainesDisponibles == null || grainesDisponibles.Length == 0)
        {
            Debug.LogWarning("[DroneInteraction] Aucune graine disponible !");
            return;
        }

        GameObject grainePrefab = grainesDisponibles[indexGraineCourante];
        Seed seedScript = grainePrefab.GetComponent<Seed>();

        if (seedScript == null)
        {
            Debug.LogError("[DroneInteraction] Le prefab n'a pas de script Seed !");
            return;
        }

        if (ShopManager.Instance != null && !ShopManager.Instance.EstGraineDebloquee(seedScript.typeGraine))
        {
            Debug.LogWarning($"[DroneInteraction] Graine {seedScript.typeGraine} non débloquée !");
            return;
        }

        // Obtenir le nombre de graines à planter selon l'upgrade
        int nombreGrainesAPlanter = 1;
        float distanceMax = rayonDetectionZone;

        if (PlantationUpgradeManager.Instance != null)
        {
            nombreGrainesAPlanter = PlantationUpgradeManager.Instance.ObtenirNombreGrainesSimultanees();
            distanceMax = PlantationUpgradeManager.Instance.distanceMaxPlantation;
        }

        // Trouver toutes les zones de plantation vides dans le rayon
        ZonePlantation[] zonesDisponibles = TrouverZonesPlantationProches(distanceMax, nombreGrainesAPlanter);

        if (zonesDisponibles.Length == 0)
        {
            Debug.LogWarning("[DroneInteraction] Aucune zone de plantation proche et vide !");
            return;
        }

        // Calculer combien de graines on peut effectivement planter
        int grainesAPlanter = Mathf.Min(zonesDisponibles.Length, nombreGrainesAPlanter);

        // Vérifier l'argent disponible
        int coutTotal = seedScript.prixAchat * grainesAPlanter;

        if (MoneyManager.Instance != null)
        {
            // Ajuster le nombre de graines selon l'argent disponible
            int argentDisponible = MoneyManager.Instance.argentActuel;
            if (argentDisponible < coutTotal)
            {
                grainesAPlanter = Mathf.Max(0, argentDisponible / seedScript.prixAchat);

                if (grainesAPlanter == 0)
                {
                    Debug.LogWarning($"[DroneInteraction] Pas assez d'argent ! Besoin: {seedScript.prixAchat}$");
                    return;
                }

                coutTotal = seedScript.prixAchat * grainesAPlanter;
                Debug.Log($"[DroneInteraction] Argent insuffisant pour {nombreGrainesAPlanter} graines. Plantation de {grainesAPlanter} graines seulement.");
            }
        }

        // Planter les graines
        int grainesPlantees = 0;
        for (int i = 0; i < grainesAPlanter; i++)
        {
            if (zonesDisponibles[i] != null && zonesDisponibles[i].PeutPlanter())
            {
                // Dépenser l'argent pour chaque graine
                if (MoneyManager.Instance != null && MoneyManager.Instance.Depenser(seedScript.prixAchat, false))
                {
                    zonesDisponibles[i].PlanterGraine(seedScript.typeGraine, grainePrefab);
                    grainesPlantees++;
                }
            }
        }

        // Son de plantation
        if (grainesPlantees > 0 && SoundManager.Instance != null)
        {
            SoundManager.Instance.JouerSon(SoundManager.Instance.sonPlantation);
        }

        Debug.Log($"[DroneInteraction] {grainesPlantees} graine(s) plantée(s) !");
    }

    /// <summary>
    /// Trouve les zones de plantation vides les plus proches du joueur
    /// </summary>
    ZonePlantation[] TrouverZonesPlantationProches(float distanceMax, int nombreMax)
    {
        // Trouver toutes les zones dans le rayon
        Collider[] colliders = Physics.OverlapSphere(transform.position, distanceMax);

        System.Collections.Generic.List<ZonePlantation> zonesDisponibles = new System.Collections.Generic.List<ZonePlantation>();

        foreach (Collider col in colliders)
        {
            ZonePlantation zone = col.GetComponent<ZonePlantation>();
            if (zone != null && zone.PeutPlanter())
            {
                zonesDisponibles.Add(zone);
            }
        }

        // Trier par distance
        zonesDisponibles.Sort((a, b) =>
        {
            float distA = Vector3.Distance(transform.position, a.transform.position);
            float distB = Vector3.Distance(transform.position, b.transform.position);
            return distA.CompareTo(distB);
        });

        // Prendre seulement les N plus proches
        int count = Mathf.Min(zonesDisponibles.Count, nombreMax);
        ZonePlantation[] resultat = new ZonePlantation[count];

        for (int i = 0; i < count; i++)
        {
            resultat[i] = zonesDisponibles[i];
        }

        return resultat;
    }

    void GererRecolte()
    {
        if (PlayerInputManager.Instance == null) return;

        if (PlayerInputManager.Instance.Controls.Drone.Harvest.WasPressedThisFrame())
        {
            RecolterLegumePlusProche();
        }
    }

    void RecolterLegumePlusProche()
    {
        Collider[] objetsProches = Physics.OverlapSphere(transform.position, rayonDetectionLegume);

        Legume legumePlusProche = null;
        float distanceMin = float.MaxValue;

        foreach (Collider col in objetsProches)
        {
            Legume legume = col.GetComponent<Legume>();
            if (legume != null && legume.EstRecoltable())
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < distanceMin)
                {
                    distanceMin = distance;
                    legumePlusProche = legume;
                }
            }
        }

        if (legumePlusProche != null)
        {
            legumePlusProche.Recolter();

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.JouerSon(SoundManager.Instance.sonRecolteLegume);
            }

            Debug.Log($"[DroneInteraction] Légume récolté : {legumePlusProche.typeGraine}");
        }
        else
        {
            Debug.LogWarning("[DroneInteraction] Aucun légume récoltable à proximité");
        }
    }

    void GererArrosoir()
    {
        if (PlayerInputManager.Instance == null) return;

        if (PlayerInputManager.Instance.Controls.Drone.UseWateringCan.WasPressedThisFrame())
        {
            UtiliserArrosoir();
        }
    }

    void UtiliserArrosoir()
    {
        if (ShopManager.Instance == null || !ShopManager.Instance.arrosoirAchete)
        {
            Debug.LogWarning("[DroneInteraction] Arrosoir non acheté !");
            return;
        }

        Collider[] champsProches = Physics.OverlapSphere(transform.position, rayonDetectionChamp);

        ChampPlantation champTrouve = null;
        float distanceMin = float.MaxValue;

        foreach (Collider col in champsProches)
        {
            ChampPlantation champ = col.GetComponent<ChampPlantation>();
            if (champ != null)
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < distanceMin)
                {
                    distanceMin = distance;
                    champTrouve = champ;
                }
            }
        }

        if (champTrouve != null)
        {
            if (champTrouve.EstBoostActif())
            {
                Debug.LogWarning($"[DroneInteraction] Le champ {champTrouve.name} a déjà un boost actif !");
                return;
            }

            champTrouve.AppliquerBoostArrosage(dureeBoostArrosage, multiplicateurArrosage);

            if (particulesArrosoir != null)
            {
                Vector3 positionParticules = champTrouve.transform.position + offsetParticulesArrosoir;
                GameObject pluie = Instantiate(particulesArrosoir, positionParticules, Quaternion.identity);
                Destroy(pluie, dureeBoostArrosage);
            }

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.JouerSon(SoundManager.Instance.sonArrosoir);
            }

            Debug.Log($"[DroneInteraction] Arrosoir utilisé sur {champTrouve.name}");
        }
        else
        {
            Debug.LogWarning("[DroneInteraction] Aucun champ détecté à proximité !");
        }
    }
}
