using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class RollSettingsPanel : UIPanel {
    private int DICE_MIN_COUNT = 1;
    private int DICE_MAX_COUNT = 3;

    public event Action RollStartButtonClicked;
    public event Action<DiceType> DiceTypeChanged;
    public event Action<int> DiceCountChanged;

    [SerializeField] private Slider _diceCountSlider;
    [SerializeField] private TMP_Text _diceCountText;
    [SerializeField] private Button _rollButton;

    public void Init() {
        // Настройка слайдера
        _diceCountSlider.minValue = DICE_MIN_COUNT;
        _diceCountSlider.maxValue = DICE_MAX_COUNT;
    }

    public override void Show() {
        base.Show();

        _diceCountSlider.onValueChanged.AddListener(OnDiceCountChanged);
        _rollButton.onClick.AddListener(OnRollButtonClicked);

        UpdateDiceCountText();
    }

    public override void Hide() {
        base.Hide();

        _diceCountSlider.onValueChanged.RemoveListener(OnDiceCountChanged);
        _rollButton.onClick.RemoveListener(() => RollStartButtonClicked?.Invoke());
    }

    private void OnDiceCountChanged(float value) {
        int count = Mathf.RoundToInt(value);
        UpdateDiceCountText();

        DiceCountChanged?.Invoke(count);
        Debug.Log($"Dice count: {count}");
    }

    private void OnRollButtonClicked() {
        Hide();
        RollStartButtonClicked?.Invoke();
    }

    private void UpdateDiceCountText() => _diceCountText.text = $"Кубиков: {_diceCountSlider.value}";
}
