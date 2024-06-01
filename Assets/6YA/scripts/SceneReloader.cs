using UnityEngine;
using UnityEngine.SceneManagement; // Import this namespace to manage scenes

public class SceneReloader : MonoBehaviour
{
    // Method to reload the current scene
    public void ReloadScene()
    {
        // Get the currently active scene and reload it
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
