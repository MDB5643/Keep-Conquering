using UnityEditor;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CustomEditor(typeof(GenerateBackground))]
public class MeshSaverEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GenerateBackground myGenerateBackground = (GenerateBackground)target;
        if (GUILayout.Button("Save Generated Mesh"))
        {
            MeshFilter mf = myGenerateBackground.gameObject.GetComponent<MeshFilter>();
            Mesh m = mf.sharedMesh;
            try
            {
                SaveMesh(m, m.name, false, true);
            }
            catch
            {
                Debug.LogWarning("Invalid Object saved. Have you generated a mesh yet?");
            }
        }
    }
    public static void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
    {
        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);

        Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

        if (optimizeMesh)
            MeshUtility.Optimize(meshToSave);

        AssetDatabase.CreateAsset(meshToSave, path);
        AssetDatabase.SaveAssets();
    }

}