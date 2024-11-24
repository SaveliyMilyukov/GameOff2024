using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SteamSystemPart))]
public class SSPEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SteamSystemPart ssp = (SteamSystemPart)target;

        GUILayout.Space(5);
        if (GUILayout.Button("Find Neigbours"))
        {
            ssp.FindNeigbours();
            SaveChanges();
        }
        if (GUILayout.Button("Find Neigbours For System"))
        {
            ssp.FindNeigboursForWholeSystem();
            SaveChanges();
        }
    }
}
