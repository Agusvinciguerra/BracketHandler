using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class Brackets : MonoBehaviour
{
    [SerializeField] private GameObject inputFieldPrefab; // Prefab for InputField
    [SerializeField] private Transform inputFieldParent; // Parent container for InputFields
    [SerializeField] private Transform inputFieldParent2; // Parent container for InputFields

    private string filePath; // Path to the JSON file

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

    void OnEnable()
    {
        filePath = Path.Combine(Application.persistentDataPath, "formData.json");
        Debug.Log("JSON file path: " + filePath);

        LoadFromJson();
        InstantiateInputFields();
        InstantiateHalfInputFields();
    }

    void LoadFromJson()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            playerDataList = JsonUtility.FromJson<PlayerDataList>(jsonData);
            Debug.Log($"Loaded {playerDataList.players.Count} players from JSON.");
        }
        else
        {
            Debug.LogWarning("JSON file not found. Starting with an empty player list.");
        }
    }

    void InstantiateInputFields()
    {
        // Clear existing InputFields (optional, to avoid duplicates)
        foreach (Transform child in inputFieldParent)
        {
            Destroy(child.gameObject);
        }

        // Instantiate an InputField for each player alias
        foreach (var player in playerDataList.players)
        {
            GameObject newInputField = Instantiate(inputFieldPrefab, inputFieldParent);
            newInputField.SetActive(true);

            // Set the InputField's text to the player's alias
            TMP_InputField inputField = newInputField.GetComponent<TMP_InputField>();
            if (inputField != null)
            {
                inputField.text = player.alias; // Set alias as default text
            }
        }
    }

    void InstantiateHalfInputFields()
    {
        // Calcular la mitad de los jugadores (redondeando hacia abajo si es impar)
        int halfCount = playerDataList.players.Count / 2;

        // Limpiar los InputFields existentes (opcional, para evitar duplicados)
        foreach (Transform child in inputFieldParent2)
        {
            Destroy(child.gameObject);
        }

        // Instanciar un InputField para la mitad de los jugadores
        for (int i = 0; i < halfCount; i++)
        {
            GameObject newInputField = Instantiate(inputFieldPrefab, inputFieldParent2);
            newInputField.SetActive(true);

            // Configurar el texto del InputField con el alias del jugador
            TMP_InputField inputField = newInputField.GetComponent<TMP_InputField>();
            if (inputField != null)
            {
                inputField.text = playerDataList.players[i].alias; // Establecer el alias como texto predeterminado
            }
        }

        Debug.Log($"Instanciados {halfCount} InputFields (mitad de los jugadores).");
    }
}