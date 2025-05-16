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
    public TMP_InputField fileNameInputField;
    [SerializeField] private GameObject fileNameInputContainer;
    private string filePath;
    [SerializeField] private TMP_Text fileNameDisplayText;

    void OnEnable()
    {
        //filePath = Path.Combine(Application.persistentDataPath, "formData.json");
        // DEPRECATED //
        /*if (PlayerPrefs.HasKey("SavedFileName"))
        {
            string savedName = PlayerPrefs.GetString("SavedFileName");
            fileNameInputField.text = savedName;
            if (fileNameInputContainer != null)
                fileNameInputContainer.SetActive(false); // Oculta el contenedor si ya hay nombre guardado
        }
        else
        {
            if (fileNameInputContainer != null)
                fileNameInputContainer.SetActive(true);
        }
       
        string fileName = fileNameInputField != null && !string.IsNullOrWhiteSpace(fileNameInputField.text) 
        ? fileNameInputField.text 
        : "formData";
        filePath = Path.Combine(Application.persistentDataPath, fileName + ".json");

        fileNameDisplayText.text = "Torneo: " + fileName;

        Debug.Log("File path: " + filePath);

        LoadFromJson();
        DisplayInfo();*/ 

        if (fileNameInputField != null)
            fileNameInputField.text = ""; // Limpia el campo

        if (fileNameInputContainer != null)
            fileNameInputContainer.SetActive(true); // Muestra el contenedor

        fileNameDisplayText.text = "Torneo: ";

        Debug.Log("Esperando nombre de torneo...");
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

    public void ConfirmFileName()
    {
        // DEPRECATED //
        /*string fileName = fileNameInputField.text;
        if (!string.IsNullOrWhiteSpace(fileName))
        {
            PlayerPrefs.SetString("SavedFileName", fileName);
            PlayerPrefs.Save();
            filePath = Path.Combine(Application.persistentDataPath, fileName + ".json");
            if (fileNameInputContainer != null)
                fileNameInputContainer.SetActive(false); // Desactiva el contenedor
            Debug.Log("File name confirmed and input container disabled.");
            fileNameDisplayText.text = "Torneo: " + fileName;
        }
        else
        {
            Debug.LogWarning("File name is empty.");
        }*/

        string fileName = fileNameInputField.text;
        if (!string.IsNullOrWhiteSpace(fileName))
        {
            // Guarda el nombre como key en PlayerPrefs
            PlayerPrefs.SetString("SavedFileName", fileName);
            PlayerPrefs.Save();

            filePath = Path.Combine(Application.persistentDataPath, fileName + ".json");
            if (fileNameInputContainer != null)
                fileNameInputContainer.SetActive(false); // Desactiva el contenedor
            Debug.Log("File name confirmed and input container disabled.");
            fileNameDisplayText.text = "Torneo: " + fileName;

            LoadFromJson();
            DisplayInfo();
        }
        else
        {
            Debug.LogWarning("File name is empty.");
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

        DisplayInfo();
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

        DisplayInfo();
    }

    public void DisplayInfo()
    {
        switch (playerDataList.players.Count)
        {
            case int n when n >= 0 && n < 8:
                displayText.text = "Players en lista de espera hasta completar 8";
                break;

            case int n when n == 8:
                displayText.text = "Octavos completos. Players en lista de espera hasta completar 16";
                break;

            case int n when n >= 9 && n < 16:
                displayText.text = "Octavos completos. Players en lista de espera hasta completar 16";
                break;

            case int n when n == 16:
                displayText.text = "16avos completos. Players en lista de espera hasta completar 32";
                break;

            case int n when n >= 17 && n < 32:
                displayText.text = "16avos completos. Players en lista de espera hasta completar 32";
                break;
            
            case int n when n == 32:
                displayText.text = "32avos completos. Players en lista de espera hasta completar 64";
                break;

            case int n when n >= 33 && n < 64:
                displayText.text = "32avos completos. Players en lista de espera hasta completar 64";
                break;
            
            case int n when n == 64:
                displayText.text = "Torneo completo!";
                break;

            default:
                Debug.Log("Número de jugadores fuera de los rangos manejados.");
                break;
        }
    }
}
