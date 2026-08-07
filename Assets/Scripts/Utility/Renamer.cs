#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Renamer : EditorWindow
{
    private string baseName = "";

    [MenuItem("Tools/Rename Selected Objects")]
    private static void Open() => GetWindow<Renamer>("Rename");

    private void OnGUI()
    {
        baseName = EditorGUILayout.TextField("Base Name", baseName);

        if (GUILayout.Button("Rename"))
        {
            var objects = Selection.gameObjects.OrderBy(o => o.transform.GetSiblingIndex()).ToArray();
            for (int i = 0; i < objects.Length; i++) { objects[i].name = $"{baseName} {i + 1}"; }
            Close();
        }
    }
}
#endif