using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using TMPro;

public class Brackets : MonoBehaviour
{
    [SerializeField] private GameObject inputFieldPrefab;
    [SerializeField] private Transform inputFieldParent; 
    [SerializeField] private Transform[] inputFieldParents;

    private string filePath; 

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

    private PlayerDataList playerDataList = new PlayerDataList();
    public int playersSent;

    void OnEnable()
    {
        //filePath = Path.Combine(Application.persistentDataPath, "formData.json");

        // Obtiene el nombre guardado por el usuario, o usa "formData" si no hay ninguno
        string fileName = PlayerPrefs.HasKey("SavedFileName")
            ? PlayerPrefs.GetString("SavedFileName")
            : "formData";
        filePath = Path.Combine(Application.persistentDataPath, fileName + ".json");
        Debug.Log("JSON file path: " + filePath);

        LoadFromJson();
        CheckPlayerCount();
    }

    void CheckPlayerCount()
    {
        switch (playerDataList.players.Count)
        {
            case int n when n >= 0 && n < 8:
                Debug.Log("Menos de 8 jugadores. No se puede iniciar el torneo.");
                break;

            case int n when n == 8:
                InstantiateInputFields(8);
                StartBracket(8);
                break;

            case int n when n >= 9 && n < 16:
                InstantiateInputFields(8);
                StartBracket(8);
                break;

            case int n when n == 16:
                InstantiateInputFields(16);
                StartBracket(16);
                break;

            case int n when n >= 17 && n < 32:
                InstantiateInputFields(16);
                StartBracket(16);
                break;
            
            case int n when n == 32:
                InstantiateInputFields(32);
                StartBracket(32);
                break;

            case int n when n >= 33 && n < 64:
                InstantiateInputFields(32);
                StartBracket(32);
                break;
            
            case int n when n == 64:
                InstantiateInputFields(64);
                StartBracket(64);
                break;

            default:
                Debug.Log("Número de jugadores fuera de los rangos manejados.");
                break;
        }
    }

    void LoadFromJson()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            playerDataList = JsonUtility.FromJson<PlayerDataList>(jsonData);
        }
        else
        {
            Debug.LogWarning("JSON file not found. Starting with an empty player list.");
        }
    }

    void InstantiateInputFields(int count)
    {
        foreach (Transform child in inputFieldParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < count && i < playerDataList.players.Count; i++)
        {
            GameObject newInputField = Instantiate(inputFieldPrefab, inputFieldParent);
            newInputField.SetActive(true);

            TMP_InputField inputField = newInputField.GetComponent<TMP_InputField>();
            if (inputField != null)
            {
                inputField.text = playerDataList.players[i].alias;
            }
        }
    }

    void InstantiateHalfInputFields(int players, int divisor, int index)
    {
        int halfCount = players / divisor;

        foreach (Transform child in inputFieldParents[index])
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < halfCount; i++)
        {
            GameObject newInputField = Instantiate(inputFieldPrefab, inputFieldParents[index]);
            newInputField.SetActive(true);
        }
    }

    void StartBracket(int playerTrigger)
    {
        switch (playerTrigger)
        {
            case 8:
                foreach (Transform parent in inputFieldParents)
                {
                    parent.gameObject.SetActive(false);
                }

                inputFieldParents[0].gameObject.SetActive(true);
                inputFieldParents[1].gameObject.SetActive(true);

                InstantiateHalfInputFields(8, 2, 0);
                InstantiateHalfInputFields(8, 4, 1);

                playersSent = 8;
                break;

            case 16:
                foreach (Transform parent in inputFieldParents)
                {
                    parent.gameObject.SetActive(false);
                }

                inputFieldParents[0].gameObject.SetActive(true);
                inputFieldParents[1].gameObject.SetActive(true);
                inputFieldParents[2].gameObject.SetActive(true);

                InstantiateHalfInputFields(16, 2, 0);
                InstantiateHalfInputFields(16, 4, 1);
                InstantiateHalfInputFields(16, 8, 2);

                playersSent = 16;
                break;

            case 32:
                foreach (Transform parent in inputFieldParents)
                {
                    parent.gameObject.SetActive(false);
                }

                inputFieldParents[0].gameObject.SetActive(true);
                inputFieldParents[1].gameObject.SetActive(true);
                inputFieldParents[2].gameObject.SetActive(true);
                inputFieldParents[3].gameObject.SetActive(true);

                InstantiateHalfInputFields(32, 2, 0);
                InstantiateHalfInputFields(32, 4, 1);
                InstantiateHalfInputFields(32, 8, 2);
                InstantiateHalfInputFields(32, 16, 3);

                playersSent = 32;
                break;

            case 64:
                foreach (Transform parent in inputFieldParents)
                {
                    parent.gameObject.SetActive(false);
                }

                inputFieldParents[0].gameObject.SetActive(true);
                inputFieldParents[1].gameObject.SetActive(true);
                inputFieldParents[2].gameObject.SetActive(true);
                inputFieldParents[3].gameObject.SetActive(true);
                inputFieldParents[4].gameObject.SetActive(true);

                InstantiateHalfInputFields(64, 2, 0);
                InstantiateHalfInputFields(64, 4, 1);
                InstantiateHalfInputFields(64, 8, 2);
                InstantiateHalfInputFields(64, 16, 3);
                InstantiateHalfInputFields(64, 32, 4);

                playersSent = 64;
                break;

            default:
                Debug.LogWarning("Número de jugadores no válido para iniciar el bracket.");
                break;
        }
    }

    public void TrimAndSavePlayers()
    {
        if (playerDataList.players.Count > playersSent)
        {
            playerDataList.players = playerDataList.players.Take(playersSent).ToList();
        }
        else
        {
            Debug.Log("No trimming needed, player count is within limit.");
        }

        // Usa el mismo nombre personalizado para guardar el archivo recortado
        string fileName = PlayerPrefs.HasKey("SavedFileName")
            ? PlayerPrefs.GetString("SavedFileName")
            : "TournamentStarted";
        string tournamentFilePath = Path.Combine(Application.persistentDataPath, fileName + "T.json");
        string jsonData = JsonUtility.ToJson(playerDataList, true);
        File.WriteAllText(tournamentFilePath, jsonData);
        Debug.Log("Player data saved to: " + tournamentFilePath);
    }
}