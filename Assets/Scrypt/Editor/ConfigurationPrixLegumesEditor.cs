using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ConfigurationPrixLegumes))]
public class ConfigurationPrixLegumesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ConfigurationPrixLegumes config = (ConfigurationPrixLegumes)target;

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Cliquez sur le bouton ci-dessous pour initialiser les valeurs par défaut recommandées.", MessageType.Info);

        if (GUILayout.Button("Initialiser Valeurs Par Défaut", GUILayout.Height(40)))
        {
            Undo.RecordObject(config, "Initialiser valeurs par défaut");
            config.InitialiserValeursParDefaut();
            EditorUtility.SetDirty(config);
            Debug.Log("[ConfigPrixLegumes] Valeurs par défaut initialisées !");
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Prix recommandés par légume :\n" +
            "Salade (moins cher) : 8 → 40$\n" +
            "Carotte : 10 → 50$\n" +
            "Potate : 12 → 60$\n" +
            "Navet : 15 → 75$\n" +
            "Potiron (plus cher) : 20 → 100$",
            MessageType.None
        );
    }
}
