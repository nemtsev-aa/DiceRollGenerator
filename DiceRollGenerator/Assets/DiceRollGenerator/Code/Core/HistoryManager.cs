using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public sealed class RollData {
    public DiceType Type;
    public int Count;
    public List<int> Results;
    public int Total;

    public RollData(DiceType type,
                       int count,
                       List<int> results,
                       int total) {

        Type = type;
        Count = count;
        Results = results;
        Total = total;
    }
}

public sealed class HistoryManager : MonoBehaviour {
    private const string SAVE_FILE = "dice_history.dat";
    private const int MAX_HISTORY = 100;

    public event Action HistoryChanged;

    public IReadOnlyList<RollRecord> RollHistory => rollHistory;
    private List<RollRecord> rollHistory = new();

    public void AddRoll(RollData data) {
        RollRecord record = new RollRecord(data.Type, data.Count, data.Results, data.Total);
        rollHistory.Insert(0, record); // Добавляем в начало

        // Ограничиваем размер истории
        if (rollHistory.Count > MAX_HISTORY)
            rollHistory.RemoveAt(rollHistory.Count - 1);

        SaveHistory();

        HistoryChanged?.Invoke();
    }

    public void ClearHistory() {
        rollHistory.Clear();
        SaveHistory();
    }

    private void SaveHistory() {
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILE);

        try {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Create)) {
                formatter.Serialize(stream, rollHistory);
            }
        }
        catch (Exception e) {
            Debug.LogError($"Failed to save history: {e.Message}");
        }
    }

    public void TryLoadHistory() {
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILE);

        if (File.Exists(path)) {
            try {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(path, FileMode.Open)) {
                    rollHistory = (List<RollRecord>)formatter.Deserialize(stream);
                }
            }
            catch (System.Exception e) {
                Debug.LogError($"Failed to load history: {e.Message}");
                rollHistory = new List<RollRecord>();
            }
        }
    }
}


