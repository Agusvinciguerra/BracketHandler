using System.IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DataEntry : MonoBehaviour
{
    [System.Serializable]
    public class PlayerData
    {
        public string name;
        public string lastName;
        public string alias;

        public PlayerData(string name, string lastName, string alias)
        {
            this.name = name;
            this.lastName = lastName;
            this.alias = alias;
        }
    }

    [System.Serializable]
    public class PlayerDataList
    {
        public List<PlayerData> players = new List<PlayerData>();
    }

    private PlayerDataList playerDataList = new PlayerDataList();

    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField lastNameInputField;
    [SerializeField] private TMP_InputField aliasInputField;
    [SerializeField] private TMP_Text displayText;

    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "formData.json");
        Debug.Log("File path: " + filePath);

        LoadFromJson();
        DisplayPlayerAlias();
    }

    void LoadFromJson()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);

            playerDataList = JsonUtility.FromJson<PlayerDataList>(jsonData);

            Debug.Log("Player data loaded from JSON file.");
        }
        else
        {
            Debug.LogWarning("No JSON file found. Starting with an empty player list.");
        }
    }

    public void SavePlayerData()
    {
        string nameInput = nameInputField.text;
        string lastNameInput = lastNameInputField.text;
        string aliasInput = aliasInputField.text;

        AddPlayerData(nameInput, lastNameInput, aliasInput);

        nameInputField.text = "";
        lastNameInputField.text = "";
        aliasInputField.text = "";

        DisplayPlayerAlias();
    }

    void AddPlayerData(string nameInput, string lastNameInput, string aliasInput)
    {
        playerDataList.players.Add(new PlayerData(nameInput, lastNameInput, aliasInput));

        SaveToJson();
    }

    void SaveToJson()
    {
        // Serialize the player list to JSON
        string jsonData = JsonUtility.ToJson(playerDataList, true);

        // Write the JSON data to the file
        File.WriteAllText(filePath, jsonData);

        Debug.Log("Player data saved to JSON file.");
    }

    public void RemovePlayerByIndex(int index)
    {
        if (index >= 0 && index < playerDataList.players.Count)
        {
            Debug.Log($"Eliminando jugador de DataEntry: {playerDataList.players[index].name} {playerDataList.players[index].lastName}");
            playerDataList.players.RemoveAt(index);
        }
        else
        {
            Debug.LogWarning("Índice no válido en DataEntry.");
        }
    }

    void DisplayPlayerAlias()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);

            PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(jsonData);

            string formattedData = "";
            for (int i = playerDataList.players.Count - 1; i >= 0; i--)
            {
                formattedData += $"{playerDataList.players[i].alias}\n";
            }

            displayText.text = formattedData.TrimEnd();
        }
        else
        {
            displayText.text = "No player data found.";
            Debug.LogWarning("No JSON file found.");
        }
    }
}
