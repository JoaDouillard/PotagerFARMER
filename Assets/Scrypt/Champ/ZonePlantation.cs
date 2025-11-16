using UnityEngine;
using System.Collections;

public class ZonePlantation : MonoBehaviour
{
    public bool estOccupee = false;
    public GameObject planteCourante = null;
    public ChampPlantation champParent;

    private BoxCollider zoneCollider;
    private TypeGraine typeGraineActuelle;

    [Header("Visibilité")]
    public bool afficherZoneDebug = false;

    void Start()
    {
        zoneCollider = GetComponent<BoxCollider>();
        if (zoneCollider == null)
        {
            zoneCollider = gameObject.AddComponent<BoxCollider>();
        }

        zoneCollider.isTrigger = true;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.enabled = afficherZoneDebug;
        }
    }

    void Update()
    {
        if (estOccupee && planteCourante == null)
        {
            LibererZone();
        }
    }

    public bool PeutPlanter()
    {
        return !estOccupee && planteCourante == null;
    }

    public void PlanterGraine(TypeGraine typeGraine, GameObject prefabGraine)
    {
        if (!PeutPlanter())
        {
            return;
        }

        typeGraineActuelle = typeGraine;

        Seed seedScriptPrefab = prefabGraine.GetComponent<Seed>();
        float offsetHauteur = 0f;
        if (seedScriptPrefab != null)
        {
            offsetHauteur = seedScriptPrefab.offsetHauteurSpawn;
        }

        Vector3 positionPlantation = transform.position + Vector3.up * offsetHauteur;

        if (prefabGraine != null)
        {
            Transform parent = transform.parent;

            GameObject graine = Instantiate(prefabGraine, positionPlantation, prefabGraine.transform.rotation, parent);
            graine.name = $"Seed_{typeGraine}_{name}";
            planteCourante = graine;
            estOccupee = true;

            Seed seedScript = graine.GetComponent<Seed>();
            if (seedScript != null)
            {
                seedScript.AssignerZone(this);
            }
        }
    }

    public void LibererZone()
    {
        estOccupee = false;
        planteCourante = null;
        typeGraineActuelle = 0;
    }

    void OnDrawGizmos()
    {
        if (!afficherZoneDebug) return;

        Gizmos.color = estOccupee ? Color.red : Color.green;
        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.3f);

        if (GetComponent<BoxCollider>() != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, GetComponent<BoxCollider>().size);
        }
    }
}
