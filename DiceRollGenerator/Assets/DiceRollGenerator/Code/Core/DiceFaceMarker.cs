using UnityEngine;
// Вспомогательный компонент для разметки граней в редакторе
public class DiceFaceMarker : MonoBehaviour {
    public int faceValue = 1;
    public Color debugColor = Color.white;

    void OnDrawGizmos() {
        Gizmos.color = debugColor;
        Gizmos.DrawSphere(transform.position, 0.05f);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.1f, faceValue.ToString());
#endif
    }
}

