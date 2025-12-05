using CameraManagmentService;
using Cysharp.Threading.Tasks;
using DiceManagementService;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public sealed class DiceGameManager : MonoBehaviour, IDisposable {
    [SerializeField] private DiceSelectorManager _diceSelector;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private HistoryManager _historyManager;

    private DiceManager _diceManager;
    private CameraManager _cameraManager;

    private NavigationPanel NavigationPanel => _uiManager.GetPanel<NavigationPanel>();
    private RollSettingsPanel RollSettingsPanel => _uiManager.GetPanel<RollSettingsPanel>();
    private HistoryPanel HistoryPanel => _uiManager.GetPanel<HistoryPanel>();
    private ResultsPanel ResultsPanel => _uiManager.GetPanel<ResultsPanel>();
    private AboutPanel AboutPanel => _uiManager.GetPanel<AboutPanel>();

    [Inject]
    public void Construct(DiceManager diceManager, CameraManager cameraManager) {
        _diceManager = diceManager;
        _cameraManager = cameraManager;
    }

    private void Start() {
        _uiManager.Init();

        RollSettingsPanel.Init();
        NavigationPanel.Show();
        NavigationPanel.SetMainMenuState(true);

        NavigationPanel.RollButtonClicked += OnRollButtonClicked;
        NavigationPanel.HistoryButtonClicked += OnHistoryButtonClicked;
        NavigationPanel.AboutButtonClicked += OnAboutButtonClicked;
        NavigationPanel.MultiplayerButtonClicked += OnMultiplayerButtonClicked;

        RollSettingsPanel.DiceCountChanged += OnDiceCountChanged;
        RollSettingsPanel.RollStartButtonClicked += OnRollStartButtonClicked;

        ResultsPanel.Closed += OnResultsPanelClosed;
        HistoryPanel.Closed += OnHistoryClosed;

        _diceSelector.DiceSelected += OnDiceSelected;
        _diceManager.AllDiceStopped += OnAllDiceStopped;
    }

    private void OnDiceSelected(DiceSelectable dice) {

        if (dice != null) {
            _diceManager.SetDiceType(dice.DiceType);
        }
    }

    private void OnAllDiceStopped() {
        var results = _diceManager.CurrentResults;
        int total = results.Sum(r => r.value);

        ResultsPanel.ShowResults(results, total);
    }

    private void OnRollButtonClicked() => ShowRollingState().Forget();
    private void OnHistoryButtonClicked() => ShowHistoryState().Forget();
    private void OnMultiplayerButtonClicked() => ShowMultiplayerState().Forget();
    private void OnAboutButtonClicked() => ShowAboutState().Forget();

    private async UniTask ShowRollingState() {
        _uiManager.HideAllPanels();
        NavigationPanel.SetMainMenuState(false);

        await _cameraManager.SwitchState(CameraPositionsTypes.Dices);

        _diceSelector.Activate(true);
        RollSettingsPanel.Show();
    }

    private async UniTask ShowHistoryState() {
        _uiManager.HideAllPanels();
        NavigationPanel.SetMainMenuState(false);

        await _cameraManager.SwitchState(CameraPositionsTypes.History); 
        HistoryPanel.SetRollRecords(_historyManager.RollHistory.ToList());
    }

    private async UniTask ShowMultiplayerState() {
        _uiManager.HideAllPanels();
        NavigationPanel.SetMainMenuState(false);

        await _cameraManager.SwitchState(CameraPositionsTypes.Multiplayer);
    }

    private async UniTask ShowAboutState() {
        _uiManager.HideAllPanels();
        await UniTask.Yield();

        AboutPanel.Show();
    } 

    private void OnDiceCountChanged(int count) => _diceManager.SetDiceCount(count);

    private void OnRollStartButtonClicked() {
        _cameraManager.SwitchState(CameraPositionsTypes.RollingResult).Forget();

        _diceSelector.Activate(false);
        _diceManager.RollDice();
    }

    private void OnResultsPanelClosed() {

        if (_diceManager.TryGetCurrentRollData(out RollData rollData) == true) 
            _historyManager.AddRoll(rollData);

        RollSettingsPanel.Show();
        _diceSelector.Activate(true);
        _diceManager.Reset();

        _cameraManager.SwitchState(CameraPositionsTypes.Dices).Forget();
    }

    private void OnHistoryClosed() {
        _cameraManager.SwitchState(CameraPositionsTypes.Dices).Forget();
    }

    public void Dispose() {
        NavigationPanel.RollButtonClicked -= OnRollButtonClicked;
        NavigationPanel.HistoryButtonClicked -= OnHistoryButtonClicked;
        NavigationPanel.AboutButtonClicked -= OnAboutButtonClicked;
        NavigationPanel.MultiplayerButtonClicked -= OnMultiplayerButtonClicked;

        RollSettingsPanel.DiceCountChanged -= OnDiceCountChanged;
        RollSettingsPanel.RollStartButtonClicked -= OnRollStartButtonClicked;

        ResultsPanel.Closed -= OnResultsPanelClosed;
        HistoryPanel.Closed -= OnHistoryClosed;

        _diceSelector.DiceSelected -= OnDiceSelected;
        _diceManager.AllDiceStopped -= OnAllDiceStopped;
    }
}
