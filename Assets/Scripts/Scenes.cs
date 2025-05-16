using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public void StartManager()
    {
        SceneManager.LoadScene("Manager", LoadSceneMode.Single);
    }

    public void StartDisplay()
    {
        SceneManager.LoadScene("Display", LoadSceneMode.Single);
    }

    public void StartServer()
    {
        SceneManager.LoadScene("SERVER", LoadSceneMode.Single);
    }
}
