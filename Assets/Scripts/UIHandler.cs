using System.Text;
using UnityEngine;
using TMPro;
using System.Collections;

public class UIHandler : MonoBehaviour
{
    [System.Serializable]
    public class Wrapper
    {
        public System.Collections.Generic.List<string> parejas;
    }

    [SerializeField] private TMP_Text playerOne;
    [SerializeField] private TMP_Text playerTwo;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject versusGameObject;
    [SerializeField] private GameObject waitingGameObject;

    public void OnMessageReceived(string message)
    {
        StartCoroutine(ShowMessageWithExit(message));
    }

    private IEnumerator ShowMessageWithExit(string message)
    {
        if (animator != null)
        {
            animator.SetTrigger("exit");
        }
        yield return new WaitForSeconds(1f);

        versusGameObject.SetActive(true);
        waitingGameObject.SetActive(false);

        Debug.Log("Mensaje recibido en UIHandler: " + message);

        Wrapper received = JsonUtility.FromJson<Wrapper>(message);
        if (received != null && received.parejas != null && received.parejas.Count > 0)
        {
            string[] jugadores = received.parejas[0].Split(new string[] { " vs " }, System.StringSplitOptions.None);
            playerOne.text = jugadores.Length > 0 ? jugadores[0].ToUpper() : "";
            playerTwo.text = jugadores.Length > 1 ? jugadores[1].ToUpper() : "";
        }
        else
        {
            playerOne.text = "";
            playerTwo.text = "";
        }
    }
}
