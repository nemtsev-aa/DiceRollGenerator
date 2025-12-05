using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DicePhysics : MonoBehaviour {
    public event Action<int> ResultReceived;
    public event Action StartedRolling;
    public event Action Stopped;

    [Header("Dice Configuration")]
    [SerializeField] private DiceType diceType = DiceType.D6;
    [SerializeField] private DiceFaceMarker[] faces;

    [Header("Detection Settings")]
    [SerializeField] private float velocityThreshold = 0.05f;
    [SerializeField] private float angularThreshold = 0.05f;
    [SerializeField] private float requiredStableTime = 0.3f;
    [SerializeField] private float stabilityDotThreshold = 0.98f;

    // Компоненты
    private Rigidbody _rb;

    // Состояние
    private bool _isRolling = false;
    private bool _resultCalculated = false;
    private float _stableTimer = 0f;
    private Coroutine detectionCoroutine;

    // Структура для хранения информации о грани
    private struct DiceFaceInfo {
        public Transform transform;
        public int value;
        public Vector3 worldNormal;
    }

    private List<DiceFaceInfo> diceFaces = new List<DiceFaceInfo>();

    private void Awake() {
        _rb = GetComponent<Rigidbody>();

        // Настраиваем физику
        _rb.maxAngularVelocity = 50f;
        _rb.sleepThreshold = velocityThreshold;

        // Инициализируем список граней
        InitializeFaces();
    }

    private void InitializeFaces() {
        diceFaces.Clear();

        if (faces == null && faces.Length == 0)
            return;

        foreach (DiceFaceMarker face in faces) {

            if (face != null && face.transform != null) {
                diceFaces.Add(new DiceFaceInfo {
                    transform = face.transform,
                    value = face.faceValue,
                    worldNormal = Vector3.zero // Будет вычисляться в Update
                });
            }
        }
    }

    private void Update() {
        // Обновляем мировые нормали граней
        UpdateFaceNormals();
    }

    private void UpdateFaceNormals() {

        for (int i = 0; i < diceFaces.Count; i++) {
            if (diceFaces[i].transform != null) {
                DiceFaceInfo face = diceFaces[i];
                face.worldNormal = diceFaces[i].transform.forward;
                diceFaces[i] = face;
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        // Начинаем отслеживание при первом столкновении
        if (!_isRolling && detectionCoroutine == null) {
            detectionCoroutine = StartCoroutine(DetectionRoutine());
            StartedRolling?.Invoke();
        }
    }

    private IEnumerator DetectionRoutine() {
        _isRolling = true;
        _resultCalculated = false;

        // Ждем пока кубик не начнет замедляться
        yield return new WaitForSeconds(0.5f);

        while (_isRolling) {
            if (IsMoving()) {
                _stableTimer = 0f; // Сбрасываем таймер если движется
            } else {
                _stableTimer += Time.deltaTime;

                // Проверяем стабильность положения
                if (_stableTimer >= requiredStableTime && IsStablePosition()) {
                    CalculateAndReportResult();
                    break;
                }
            }

            yield return null;
        }

        detectionCoroutine = null;
    }

    private bool IsMoving() {
        return _rb.linearVelocity.magnitude > velocityThreshold ||
               _rb.angularVelocity.magnitude > angularThreshold;
    }

    private bool IsStablePosition() {
        // Проверяем, достаточно ли стабильно положение кубика
        DiceFaceInfo? topFace = GetTopFace();
        
        if (topFace.HasValue) {
            float dot = Vector3.Dot(topFace.Value.worldNormal, Vector3.up);
            return dot >= stabilityDotThreshold;
        }

        return false;
    }

    private DiceFaceInfo? GetTopFace() {
        DiceFaceInfo? bestFace = null;
        float bestDot = -1f;

        foreach (DiceFaceInfo face in diceFaces) {
            // Для определения верхней грани смотрим скалярное произведение
            // между нормалью грани и вектором вверх
            float dot = Vector3.Dot(face.worldNormal, Vector3.up);

            // Ищем грань с максимальным скалярным произведением
            if (dot > bestDot) {
                bestDot = dot;
                bestFace = face;
            }
        }

        return bestFace;
    }

    private void OnDrawGizmosSelected() {
        if (diceFaces.Count == 0) return;

        foreach (DiceFaceInfo face in diceFaces) {
            if (face.transform == null) continue;

            Gizmos.color = Color.yellow;
            Vector3 worldPosition = face.transform.position;

            // Рисуем нормаль грани
            Vector3 normal = face.transform.forward;
            Gizmos.DrawLine(worldPosition, worldPosition + normal * 0.3f);

            // Рисуем сферу на конце нормали
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(worldPosition + normal * 0.3f, 0.02f);

            // Отображаем значение
#if UNITY_EDITOR
            UnityEditor.Handles.Label(worldPosition + normal * 0.35f, $"Value: {face.value}");
#endif
        }
    }

    // Метод для принудительной остановки (если кубик застрял)
    public void ForceStopAndCalculate() {
        if (detectionCoroutine != null) {
            StopCoroutine(detectionCoroutine);
            detectionCoroutine = null;
        }

        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        // Даем время на стабилизацию
        StartCoroutine(ForceCalculate());
    }

    private IEnumerator ForceCalculate() {
        yield return new WaitForSeconds(0.1f);
        CalculateAndReportResult();
    }

    private void CalculateAndReportResult() {
        _isRolling = false;
        _resultCalculated = true;

        DiceFaceInfo? topFace = GetTopFace();
        int result;

        if (topFace.HasValue) {
            result = topFace.Value.value;
            Debug.Log($"Dice result calculated: {result} (Face value: {result})");
        }
        else {
            // Если не можем определить грань, используем альтернативные методы
            result = GetFallbackResult();
            Debug.LogWarning($"Using fallback result: {result}");
        }

        ResultReceived?.Invoke(result);
        Stopped?.Invoke();
    }

    private int GetFallbackResult() {
        // 1. Проверяем, есть ли грани с достаточной близостью
        float fallbackThreshold = stabilityDotThreshold - 0.1f; // Немного ниже порога

        DiceFaceInfo? bestFace = null;
        float bestDot = -1f;

        foreach (DiceFaceInfo face in diceFaces) {
            float dot = Vector3.Dot(face.worldNormal, Vector3.up);
            if (dot > bestDot) {
                bestDot = dot;
                bestFace = face;
            }
        }

        // 2. Если есть грань с разумной близостью, используем её
        if (bestFace.HasValue && bestDot > fallbackThreshold) {
            return bestFace.Value.value;
        }

        // 3. Если кубик стоит на ребре, используем физику для определения результата
        return GetResultFromPhysics();
    }

    private int GetResultFromPhysics() {
        // Метод 1: Определение по ближайшей грани к вертикали
        DiceFaceInfo? closestFace = null;
        float smallestAngle = float.MaxValue;

        foreach (DiceFaceInfo face in diceFaces) {
            float angle = Vector3.Angle(face.worldNormal, Vector3.up);
            if (angle < smallestAngle) {
                smallestAngle = angle;
                closestFace = face;
            }
        }

        if (closestFace.HasValue) {
            return closestFace.Value.value;
        }

        // Метод 2: Определение по Raycast
        return GetResultFromRaycast();
    }

    private int GetResultFromRaycast() {
        // Пускаем луч сверху и смотрим, в какую грань попадаем
        Ray ray = new Ray(transform.position + Vector3.up * 2f, Vector3.down);
        RaycastHit[] hits = Physics.RaycastAll(ray, 3f);

        foreach (var hit in hits) {
            DiceFaceMarker marker = hit.collider.GetComponent<DiceFaceMarker>();
            if (marker != null) {
                return marker.faceValue;
            }
        }

        // Последний вариант - случайное значение
        return UnityEngine.Random.Range(1, (int)diceType + 1);
    }

    //// Вспомогательный метод для визуализации в режиме Play
    //void OnGUI() {
    //    if (Application.isPlaying && diceFaces.Count > 0) {
    //        GUIStyle style = new GUIStyle(GUI.skin.label);
    //        style.fontSize = 14;
    //        style.normal.textColor = Color.white;

    //        // Показываем нормали всех граней
    //        for (int i = 0; i < diceFaces.Count; i++) {
    //            Vector3 screenPos = Camera.main.WorldToScreenPoint(diceFaces[i].transform.position);
    //            screenPos.y = Screen.height - screenPos.y;

    //            float dot = Vector3.Dot(diceFaces[i].transform.forward, Vector3.up);
    //            GUI.Label(new Rect(screenPos.x, screenPos.y, 200, 30),
    //                     $"Face {diceFaces[i].value}: dot={dot:F2}", style);
    //        }
    //    }
    //}
}


