using UnityEngine;

[System.Serializable]
public class DiceFace {
    public Transform transform;
    public int value;

    public DiceFace(Transform transform, int value) {
        this.transform = transform;
        this.value = value;
    }
}

