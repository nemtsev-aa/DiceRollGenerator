using UnityEngine;


namespace DiceManagementService {
    [System.Serializable]
    public sealed class DiceSpawnerConfig {
        [field: SerializeField] public Transform DiceSpawnArea { get; private set; }
        [field: SerializeField] public float ThrowForce { get; private set; } = 15f;
        [field: SerializeField] public float TorqueForce { get; private set; } = 8f;
        [field: SerializeField] public float DiceSpacing { get; private set; } = 0.8f;
    }
}
