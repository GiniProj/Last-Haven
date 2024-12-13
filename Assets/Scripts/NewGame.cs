using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour
{
    // Start a new game
    public void StartNewGame()
    {
        SceneManager.LoadScene("LastHaven");
        Debug.Log("Starting a new game...");
    }
}
