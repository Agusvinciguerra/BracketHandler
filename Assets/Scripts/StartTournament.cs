using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class StartTournament : MonoBehaviour
{
    [System.Serializable]
    public class PlayerData
    {
        public string name;
        public string lastName;
        public string alias;
    }

    [System.Serializable]
    public class PlayerDataList
    {
        public List<PlayerData> players = new List<PlayerData>();
    }
    private string tournamentFilePath;
    private bool coupleSent = false;
    private List<string> parejas = new List<string>();
    private int parejaActual = 0;
    [SerializeField] private TMPro.TMP_Text parejaDisplayText;

    private List<string> ganadores = new List<string>();

    void OnEnable()
    {
        string fileName = PlayerPrefs.HasKey("SavedFileName")
            ? PlayerPrefs.GetString("SavedFileName")
            : "TournamentStarted";
        tournamentFilePath = Path.Combine(Application.persistentDataPath, fileName + "T.json");
        Debug.Log("Tournament file path: " + tournamentFilePath);


        if (File.Exists(tournamentFilePath))
        {
            string jsonData = File.ReadAllText(tournamentFilePath);
            PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(jsonData);

            for (int i = 0; i < playerDataList.players.Count; i += 2)
            {
                string player1 = playerDataList.players[i].alias;
                string player2 = (i + 1 < playerDataList.players.Count) ? playerDataList.players[i + 1].alias : "BYE";
                parejas.Add($"{player1} vs {player2}");
            }

            ShowCurrentPair();
        }
        else
        {
            Debug.LogWarning("Tournament file not found.");
        }
    }

    void ShowCurrentPair()
    {
        if (parejas.Count > 0 && parejaActual < parejas.Count)
        {
            parejaDisplayText.text = parejas[parejaActual];
            SendCouplesToServer();
        }
        else
        {
            parejaDisplayText.text = "No hay más parejas.";
        }
    }

    public void NextPair()
    {
        if (parejaActual < parejas.Count - 1)
        {
            parejaActual++;
            ShowCurrentPair();
        }
        else
        {
            parejaDisplayText.text = "Fin del torneo.";
        }
    }

    public void SendCouplesToServer()
    {
        if (parejas.Count > 0 && parejaActual < parejas.Count)
        {
            string parejaActualStr = parejas[parejaActual];
            string parejaJson = JsonUtility.ToJson(new Wrapper { parejas = new List<string> { parejaActualStr } });
            FindObjectOfType<Client>().SendMessageToServer(parejaJson);
            Debug.Log("Pareja enviada al servidor: " + parejaActualStr);
        }
    }

    [System.Serializable]
    public class Wrapper
    {
        public List<string> parejas;
    }

    public void SeleccionarGanadorActual(int indice)
    {
        if (parejaActual < parejas.Count)
        {
            // Obtén los nombres de los jugadores del par actual
            string[] jugadores = parejas[parejaActual].Split(new string[] { " vs " }, System.StringSplitOptions.None);
            if (indice >= 0 && indice < jugadores.Length)
            {
                string ganador = jugadores[indice];
                ganadores.Add(ganador);
                NextPair();
                Debug.Log("Ganador agregado: " + ganador);
            }
        }
    }

    public void UpdatePairs()
    {
        if (ganadores.Count < 2)
        {
            parejaDisplayText.text = ganadores.Count == 1
                ? $"¡Ganador del torneo: {ganadores[0]}!"
                : "No hay suficientes ganadores para una nueva ronda.";
            return;
        }

        parejas.Clear();
        for (int i = 0; i < ganadores.Count; i += 2)
        {
            string player1 = ganadores[i];
            string player2 = (i + 1 < ganadores.Count) ? ganadores[i + 1] : "BYE";
            parejas.Add($"{player1} vs {player2}");
        }

        ganadores.Clear();
        parejaActual = 0;
        ShowCurrentPair();
    }
}
