using TMPro;
using UnityEngine;

public class HistoryItemUI : MonoBehaviour {
    [SerializeField] private TMP_Text diceTypeText;
    [SerializeField] private TMP_Text diceCountText;
    [SerializeField] private TMP_Text resultsText;
    [SerializeField] private TMP_Text totalText;
    [SerializeField] private TMP_Text timeText;

    public void SetData(RollRecord record) {
        diceTypeText.text = record.diceType.ToString();
        diceCountText.text = $"x{record.diceCount}";

        // Форматируем результаты
        string results = string.Join(", ", record.results);
        resultsText.text = results;

        totalText.text = record.total.ToString();
        timeText.text = record.timestamp.ToString("HH:mm:ss");
    }
}
