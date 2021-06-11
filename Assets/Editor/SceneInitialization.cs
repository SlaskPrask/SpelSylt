using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

[InitializeOnLoad]
public class SceneInitialization
{
    static SceneInitialization()
    {
        EditorSceneManager.activeSceneChangedInEditMode += (_, __) => SetupScene();
    }

    static void SetupScene()
    {
        if (!Object.FindObjectOfType<GameManager>())
        {
            string path = "Assets/Prefabs/GameManager.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) == null)
            {
                path = "Assets/BasicSetup/Prefabs/GameManager.prefab";
            }

            GameObject manager = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path)) as GameObject;

            MonoScript script = MonoScript.FromMonoBehaviour(manager.GetComponent<GameManager>());

            if (script != null && MonoImporter.GetExecutionOrder(script) != -50)
            {
                MonoImporter.SetExecutionOrder(script, -50);
            }
        }
    }
}
