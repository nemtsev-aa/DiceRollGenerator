using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DiceManagementService {

    public sealed class DiceManager {
        public event Action AllDiceStopped;

        public IReadOnlyList<DiceResult> CurrentResults => _currentResults;

        private readonly DiceSpawner _diceSpawner;

        private DiceType _currentDiceType = DiceType.D6;
        private int _currentDiceCount = 1;
        private List<DicePhysics> _activeDice = new();
        private List<DiceResult> _currentResults = new();

        public DiceManager(DiceSpawner diceSpawner) {
            _diceSpawner = diceSpawner;
        }

        public void SetDiceType(DiceType type) => _currentDiceType = type;

        public void SetDiceCount(int count) => _currentDiceCount = Mathf.Clamp(count, 1, 10);

        public void RollDice() {

            if (_diceSpawner.TrySpawn(_currentDiceType,
                                      _currentDiceCount) == true) {

                _activeDice = _diceSpawner.CreatedDices.ToList();

                foreach (var dice in _activeDice) {
                    dice.ResultReceived += (result) => RegisterDiceResult(dice, result);
                    dice.Stopped += OnDiceStopped;
                }

                WaitForResults().Forget();
            }
        }

        public void Reset() {
            _currentResults.Clear();
            _diceSpawner.ClearCurrentDice();
        }

        private void RegisterDiceResult(DicePhysics dice, int value) {
            DiceResult result = new DiceResult {
                diceObject = dice.gameObject,
                value = value,
                isFinalized = true
            };

            _currentResults.Add(result);
        }

        private void OnDiceStopped() {
            // Проверяем, все ли кубики остановились
            if (_currentResults.Count == _currentDiceCount)
                OnAllDiceStopped();
        }

        private void OnAllDiceStopped() {
            AllDiceStopped?.Invoke();
        }

        private async UniTask WaitForResults() {
            float timeout = 10f; // Максимальное время ожидания
            float elapsed = 0f;

            while (_currentResults.Count < _currentDiceCount && elapsed < timeout) {
                elapsed += Time.deltaTime;
                await UniTask.Yield();
            }

            if (_currentResults.Count < _currentDiceCount) {
                Debug.LogWarning("Some dice didn't report results");
                OnAllDiceStopped();
            }
        }

        #region Public 

        public bool TryGetCurrentRollData(out RollData data) {

            try {
                var currentRollData = new RollData(
                _currentDiceType,
                _currentDiceCount,
                _currentResults.Select(r => r.value).ToList(),
                _currentResults.Sum(r => r.value));

                data = currentRollData;
                return true;
            }
            catch (Exception e) {
                Debug.LogError($"Failed to RollData: {e.Message}");

                data = null;
                return false;
            }
        }

        public void ForceStopAllDice() {
            foreach (var dice in _activeDice) {
                DicePhysics physics = dice?.GetComponent<DicePhysics>();
                if (physics != null) {
                    physics.ForceStopAndCalculate();
                }
            }
        }
        #endregion
    }
}
