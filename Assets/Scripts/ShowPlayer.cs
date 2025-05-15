using System.IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShowPlayer : MonoBehaviour
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

    [SerializeField] private DataEntry dataEntry;

    [SerializeField] private TMP_Text displayText;
    [SerializeField] private TMP_Text displayAlias;
    [SerializeField] private Button deleteButtonPrefab;
    private int playerCount = 0;
    private string filePath;
    [SerializeField] private Transform buttonParentTransform;

    void OnEnable()
    {
        //filePath = Path.Combine(Application.persistentDataPath, "formData.json");

        string fileName = PlayerPrefs.HasKey("SavedFileName")
            ? PlayerPrefs.GetString("SavedFileName")
            : "formData";
        filePath = Path.Combine(Application.persistentDataPath, fileName + ".json");

        LoadAndDisplayPlayerName();
        LoadAndDisplayPlayerAlias();

        CountPlayers();

        InstantiateDeleteButtons();
    }

    void LoadAndDisplayPlayerName()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);

            PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(jsonData);

            string formattedData = "";
            foreach (var player in playerDataList.players)
            {
                formattedData += $"{player.name} {player.lastName}\n";
            }

            displayText.text = formattedData.TrimEnd(); 
        }
        else
        {
            displayText.text = "No player data found.";
        }
    }

    void LoadAndDisplayPlayerAlias()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);

            PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(jsonData);

            string formattedData = "";
            foreach (var player in playerDataList.players)
            {
                formattedData += $"{player.alias}\n";
            }

            displayAlias.text = formattedData.TrimEnd(); 
        }
    }

    void CountPlayers()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);

            PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(jsonData);

            playerCount = playerDataList.players.Count;

            Debug.Log($"Number of players: {playerCount}");
        }
        else
        {
            Debug.LogWarning("No player data found.");
            displayText.text = "No player data found.";
        }
    }

    void InstantiateDeleteButtons()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);

            PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(jsonData);

            // Limpiar botones existentes (opcional, para evitar duplicados)
            foreach (Transform child in buttonParentTransform)
            {
                Destroy(child.gameObject);
            }

            // Instanciar un botón por cada jugador
            for (int i = 0; i < playerDataList.players.Count; i++)
            {
                Button newButton = Instantiate(deleteButtonPrefab, buttonParentTransform);
                newButton.gameObject.SetActive(true);

                // Asignar texto al botón con el número del jugador
                TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    buttonText.text = $"Player {i + 1}"; // Mostrar el número del jugador
                }

                // Capturar el índice actual en una variable local
                int playerIndex = i;

                // Agregar un listener al botón para manejar eventos (por ejemplo, eliminar jugador)
                newButton.onClick.AddListener(() => OnButtonClicked(playerIndex));
            }

            Debug.Log($"Se han instanciado {playerDataList.players.Count} botones.");
        }
        else
        {
            Debug.LogWarning("No se encontraron datos de jugadores.");
        }
    }

    void OnButtonClicked(int playerIndex)
    {
        if (File.Exists(filePath))
        {
            // Leer el archivo JSON
            string jsonData = File.ReadAllText(filePath);

            // Deserializar el JSON en la lista de jugadores
            PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(jsonData);

            // Verificar si el índice es válido
            if (playerIndex >= 0 && playerIndex < playerDataList.players.Count)
            {
                // Obtener el jugador que será eliminado
                PlayerData playerToRemove = playerDataList.players[playerIndex];
                Debug.Log($"Eliminando al jugador: {playerToRemove.name} {playerToRemove.lastName}");

                // Eliminar al jugador de la lista
                playerDataList.players.RemoveAt(playerIndex);

                // Guardar la lista actualizada en el archivo JSON
                jsonData = JsonUtility.ToJson(playerDataList, true);
                File.WriteAllText(filePath, jsonData);

                Debug.Log($"Jugador eliminado: {playerToRemove.name} {playerToRemove.lastName}");

                // Actualizar la interfaz de usuario
                InstantiateDeleteButtons();
                LoadAndDisplayPlayerName();
                LoadAndDisplayPlayerAlias();
                CountPlayers();

                // Eliminar de lista definitiva
                dataEntry.RemovePlayerByIndex(playerIndex);
            }
            else
            {
                Debug.LogWarning("Índice de jugador no válido.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró el archivo JSON.");
        }
    }
}