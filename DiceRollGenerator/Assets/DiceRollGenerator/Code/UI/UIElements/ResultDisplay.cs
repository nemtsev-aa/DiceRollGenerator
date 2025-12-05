using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultDisplay : MonoBehaviour {
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private Image background;
    [SerializeField] private Color[] diceColors;

    public void SetResult(int value) {
        valueText.text = value.ToString();

        // Цвет в зависимости от значения
        if (diceColors.Length > 0) {
            int colorIndex = Mathf.Clamp(value - 1, 0, diceColors.Length - 1);
            background.color = diceColors[colorIndex];
        }

        // Анимация появления
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.3f)
            .SetDelay(Random.Range(0f, 0.2f))
            .SetEase(Ease.OutBack);

        // Небольшая анимация
        transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 1, 0.5f)
            .SetDelay(0.5f);
    }
}
