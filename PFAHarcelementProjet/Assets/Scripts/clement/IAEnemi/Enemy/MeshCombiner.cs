using UnityEngine;
using UnityEditor;

public class MeshCombiner : MonoBehaviour
{
    [ContextMenu("Combine")]
    void Combine()
    {
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[filters.Length];

        for (int i = 0; i < filters.Length; i++)
        {
            combine[i].mesh      = filters[i].sharedMesh;
            combine[i].transform = filters[i].transform.localToWorldMatrix;
        }

        Mesh combined = new Mesh();
        combined.CombineMeshes(combine);

        // Sauvegarde le mesh dans Assets
        AssetDatabase.CreateAsset(combined, "Assets/PiranaMesh.asset");
        AssetDatabase.SaveAssets();

        Debug.Log("Mesh combiné sauvegardé dans Assets/PiranaMesh.asset");
    }
}