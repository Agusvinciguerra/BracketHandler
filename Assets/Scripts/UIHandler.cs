using System.Text;
using UnityEngine;
using TMPro;

public class UIHandler : MonoBehaviour
{
    [System.Serializable]
    public class Wrapper
    {
        public System.Collections.Generic.List<string> parejas;
    }

    [SerializeField] private TMP_Text parejasDisplayText;

    public void OnMessageReceived(string message)
    {
        Debug.Log("Mensaje recibido en UIHandler: " + message);
        
        Wrapper received = JsonUtility.FromJson<Wrapper>(message);
        if (received != null && received.parejas != null && received.parejas.Count > 0)
        {
            parejasDisplayText.text = received.parejas[0]; // Solo la primera pareja
        }
        else
        {
            parejasDisplayText.text = "No hay parejas.";
        }
    }
}
