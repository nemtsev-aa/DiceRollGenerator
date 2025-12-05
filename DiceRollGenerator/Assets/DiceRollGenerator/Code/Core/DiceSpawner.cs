using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


namespace DiceManagementService {
    public sealed class DiceSpawner {
        public IReadOnlyList<DicePhysics> CreatedDices => _createdDices;

        private readonly DicePrefabProvider _prefabProvider;
        private readonly DiceSpawnerConfig _config;
        private List<DicePhysics> _createdDices;

        private Transform DiceSpawnArea => _config.DiceSpawnArea;
        private float DiceSpacing => _config.DiceSpacing;
        private float ThrowForce => _config.ThrowForce;
        private float TorqueForce => _config.TorqueForce;

        public DiceSpawner(DicePrefabProvider prefabProvider,
                           DiceSpawnerConfig config) {

            _prefabProvider = prefabProvider;
            _config = config;
            _createdDices = new List<DicePhysics>();
        }

        public bool TrySpawn(DiceType type, int count) {
            ClearCurrentDice();

            for (int i = 0; i < count; i++) {
                Vector3 spawnPos = CalculateSpawnPosition(count, i);
                var dicePrefab = _prefabProvider.GetPrefabForType(type);
                var dice = Object.Instantiate(dicePrefab, spawnPos, Random.rotation);

                // Настройка физики
                Rigidbody rb = dice.GetComponent<Rigidbody>();
                if (rb != null) {
                    Vector3 force = new Vector3(
                        Random.Range(-0.3f, 0.3f),
                        1f,
                        Random.Range(-0.3f, 0.3f)
                    ).normalized * ThrowForce;

                    rb.AddForce(force, ForceMode.Impulse);
                    rb.AddTorque(Random.insideUnitSphere * TorqueForce, ForceMode.Impulse);
                }

                if (dice.TryGetComponent(out DicePhysics physics) == true)
                    _createdDices.Add(physics);
            }

            return _createdDices.Count > 0;
        }

        public void ClearCurrentDice() {
            foreach (var dice in _createdDices) {
                if (dice != null)
                    Object.Destroy(dice.gameObject);
            }

            _createdDices.Clear();
        }

        private Vector3 CalculateSpawnPosition(int diceCount, int index) {
            Vector3 basePos = DiceSpawnArea.position;
            int perRow = Mathf.CeilToInt(Mathf.Sqrt(diceCount));

            int row = index / perRow;
            int col = index % perRow;

            return basePos + new Vector3(
                col * DiceSpacing - (perRow - 1) * DiceSpacing * 0.5f,
                0,
                row * DiceSpacing
            );
        }
    }
}
