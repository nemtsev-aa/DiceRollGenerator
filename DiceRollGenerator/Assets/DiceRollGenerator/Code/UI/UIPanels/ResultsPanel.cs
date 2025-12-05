using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class ResultsPanel : UIPanel {
    public event Action Closed;

    [SerializeField] private Transform _resultsContainer;
    [SerializeField] private TMP_Text _totalText;
    [SerializeField] private ResultDisplay _resultPrefab;
    [SerializeField] private Button _closeResultsButton;

    private List<ResultDisplay> _resultDisplays = new();

    public void ShowResults(IReadOnlyList<DiceResult> results, int total) {
        // Очищаем старые результаты
        foreach (var display in _resultDisplays) {
            Destroy(display.gameObject);
        }

        _resultDisplays.Clear();

        // Создаем новые
        foreach (var result in results) {
            var display = Instantiate(_resultPrefab, _resultsContainer);

            if (display != null)
                display.SetResult(result.value);

            _resultDisplays.Add(display);
        }

        // Анимация появления
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.5f)
                 .SetEase(Ease.OutBack);

        // Показываем сумму
        _totalText.text = $"Сумма: {total}";

        Show();
    }

    public override void Show() {
        base.Show();

        _closeResultsButton.onClick.AddListener(OnCloseButtonClicked);
    }

    private void OnCloseButtonClicked() {
        Closed?.Invoke();
        Hide();
    }

    public override void Hide() {

        transform.DOScale(Vector3.zero, 0.3f)
                 .SetEase(Ease.InBack)
                 .OnComplete(() => base.Hide());

        _closeResultsButton.onClick.RemoveListener(OnCloseButtonClicked);
    }
}
