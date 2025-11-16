using System.Collections;
using UnityEngine;


// Script simple pour faire grossir progressivement un objet
// Version corrigée - utilise uniquement une Coroutine
public class Grossir : MonoBehaviour
{
    [Header("Paramètres de croissance")]
    [Tooltip("Vitesse de croissance (scale ajouté par frame)")]
    public float speedGrossir = 0.01f;

    [Tooltip("Délai avant de commencer à grossir (en secondes)")]
    public float timeBeforeScale = 0f;

    [Tooltip("Scale maximum de l'objet")]
    public float maxScale = 10f;

    [Header("Debug")]
    public bool afficherDebug = false;

    void Start()
    {
        // Démarrer la coroutine de croissance (FIX: ajout de StartCoroutine)
        StartCoroutine(Coroutine_ScaleUp());
    }

 
    // Coroutine qui fait grossir l'objet progressivement
    private IEnumerator Coroutine_ScaleUp()
    {
        // Attendre le délai initial si nécessaire
        if (timeBeforeScale > 0)
        {
            yield return new WaitForSeconds(timeBeforeScale);
        }

        if (afficherDebug)
        {
            Debug.Log($"[Grossir] Début de la croissance de {gameObject.name}");
        }

        // Faire grossir progressivement jusqu'à atteindre maxScale
        while (transform.localScale.x < maxScale)
        {
            // Ajouter la croissance
            transform.localScale += Vector3.one * speedGrossir;

            // Attendre la prochaine frame
            yield return null;
        }

        // S'assurer que la taille finale est exactement maxScale
        transform.localScale = Vector3.one * maxScale;

        if (afficherDebug)
        {
            Debug.Log($"[Grossir] {gameObject.name} a atteint sa taille maximale de {maxScale}");
        }
    }
}
