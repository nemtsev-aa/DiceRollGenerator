using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class HistoryPanel : UIPanel {
    public event Action Closed;

    [SerializeField] private Transform _historyContainer;
    [SerializeField] private HistoryItemUI _historyItemPrefab;
    [SerializeField] private Button _closeButton;

    public override void Show() {
        base.Show();

        _closeButton.onClick.AddListener(OnCloseButtonClicked);
    }

    public void SetRollRecords(IReadOnlyList<RollRecord> history) {
        Create(history);
        Show();
    }

    private void Create(IReadOnlyList<RollRecord> history) {
        ClearHistoryItems();

        foreach (var record in history) {
            var itemUI = Instantiate(_historyItemPrefab, _historyContainer);

            if (itemUI != null)
                itemUI.SetData(record);
        }
    }

    private void ClearHistoryItems() {

        foreach (Transform child in _historyContainer) {
            Destroy(child.gameObject);
        }
    }

    private void OnCloseButtonClicked() {
        Hide();
        Closed?.Invoke();
    }
}
