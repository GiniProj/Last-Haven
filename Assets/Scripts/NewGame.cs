using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour
{
    public void StartNewGame()
    {
        SceneManager.LoadScene("LastHaven");
        Debug.Log("Starting a new game...");
    }
}
