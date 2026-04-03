using UnityEngine;
using UnityEditor;

public class PieceReplacer : MonoBehaviour
{
    [MenuItem("Onitama/Replace Dummy Pieces")]
    public static void ReplaceItems()
    {
        // Load the Chess Set prefabs
        var whiteKing = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Chess Set/Prefabs/Chess King White.prefab");
        var blackKing = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Chess Set/Prefabs/Chess King Black.prefab");
        var whitePawn = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Chess Set/Prefabs/Chess Pawn White.prefab");
        var blackPawn = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Chess Set/Prefabs/Chess Pawn Black.prefab");

        if (whiteKing == null || blackKing == null || whitePawn == null || blackPawn == null)
        {
            Debug.LogError("Could not find chess piece prefabs. Make sure they are located in 'Assets/Chess Set/Prefabs/'.");
            return;
        }

        // We assume Minimax will be White and Greedy will be Black.
        // You can easily swap these in the code below if preferred!
        
        int count = 0;
        count += ReplaceGroup("Minimax-master", whiteKing);
        count += ReplaceGroup("Minimax-pupil", whitePawn);
        count += ReplaceGroup("Greedy-master", blackKing);
        count += ReplaceGroup("Greedy-pupil", blackPawn);
        
        Debug.Log($"Successfully replaced {count} dummy pieces with chess pieces!");
    }

    private static int ReplaceGroup(string nameContains, GameObject newPrefab)
    {
        int replacedCount = 0;
        
        // Find all objects in the scene. 
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (var obj in allObjects)
        {
            // Only match the objects holding the names, NOT their visual children (which might be named Sphere etc.)
            if (obj.name.Contains(nameContains))
            {
                // We instantiate the specific chess prefab
                GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(newPrefab);
                
                // Keep transformations
                newObj.transform.position = obj.transform.position;
                newObj.transform.rotation = obj.transform.rotation;
                
                // The loaded chess pieces natively have a scale of 100 on their prefab.
                // We shouldn't overwrite the prefab's scale to 1 unless it looks tiny. 
                // Because unity prefabs retain their local scale from the prefab definition when instantiated using PrefabUtility, 
                // we leave exactly what PrefabUtility provided.
                
                // Important: inherit parent hierarchy
                newObj.transform.SetParent(obj.transform.parent, true);
                
                // Preserve original name so logic won't break
                newObj.name = obj.name;

                // Safely destroy and record for CTRL+Z
                Undo.DestroyObjectImmediate(obj);
                Undo.RegisterCreatedObjectUndo(newObj, "Replace Piece");
                
                replacedCount++;
            }
        }
        
        return replacedCount;
    }
}
