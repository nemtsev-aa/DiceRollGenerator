using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class DiceSelectorManager : MonoBehaviour {
    public event Action<DiceSelectable> DiceSelected;
    public event Action<DiceSelectable> DiceDeselected;

    [SerializeField] private Transform _dicesParent;
    [SerializeField] private DiceSelectable[] allDice;

    private DiceSelectable currentHoveredDice;
    private DiceSelectable currentSelectedDice;
    private List<DiceSelectable> selectedDice = new List<DiceSelectable>();

    public void Activate(bool status) {
        _dicesParent.gameObject.SetActive(status);
    }

    private void Start() {

        foreach (DiceSelectable dice in allDice) {
            dice.OnDiceSelected += HandleDiceSelected;
        }
    }

    private void HandleDiceSelected(DiceSelectable dice) {
        // Обработка выбора кости через ее собственные события
        // (если используется клик напрямую на объекте)

        if (currentSelectedDice != null)
            currentSelectedDice.DeselectDice();

        currentSelectedDice = dice;
        SelectDice(currentSelectedDice);
    }

    private void SelectDice(DiceSelectable dice) {

        if (dice == null)
            return;

        // Снимаем выделение с других костей
        foreach (DiceSelectable selected in selectedDice) {
            
            if (selected != dice) {
                selected.DeselectDice();
            }
        }

        selectedDice.Clear();

        // Если кость уже выбрана, снимаем выделение
        if (selectedDice.Contains(dice)) {
            DeselectDice(dice);
            return;
        }

        // Выбираем кость
        selectedDice.Add(dice);
        currentSelectedDice = dice;

        DiceSelected?.Invoke(dice);

        Debug.Log($"Selected dice: {dice.DiceType}");
    }

    private void DeselectDice(DiceSelectable dice) {

        if (dice == null || !selectedDice.Contains(dice))
            return;

        dice.DeselectDice();
        selectedDice.Remove(dice);

        if (currentSelectedDice == dice) 
            currentSelectedDice = null;

        DiceDeselected?.Invoke(dice);
    }
}
