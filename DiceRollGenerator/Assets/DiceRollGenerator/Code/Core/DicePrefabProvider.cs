using UnityEngine;

[System.Serializable]
public sealed class DicePrefabProvider {
    [Header("Dice References")]
    [SerializeField] private GameObject d4Prefab;
    [SerializeField] private GameObject d6Prefab;
    [SerializeField] private GameObject d8Prefab;
    [SerializeField] private GameObject d10Prefab;
    [SerializeField] private GameObject d12Prefab;
    [SerializeField] private GameObject d20Prefab;

    public GameObject GetPrefabForType(DiceType type) {
        return type switch {
            DiceType.D4 => d4Prefab,
            DiceType.D6 => d6Prefab,
            DiceType.D8 => d8Prefab,
            DiceType.D10 => d10Prefab,
            DiceType.D12 => d12Prefab,
            DiceType.D20 => d20Prefab,
            _ => d6Prefab
        };
    }
}


