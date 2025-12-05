using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class DiceSelectable : MonoBehaviour,
                              IPointerEnterHandler,
                              IPointerExitHandler,
                              IPointerClickHandler {

    [Header("Selection Settings")]
    [SerializeField] private float hoverHeight = 0.2f; // Насколько поднимается кость
    [SerializeField] private float hoverScale = 1.2f; // Насколько увеличивается
    [SerializeField] private float animationSpeed = 5f; // Скорость анимации
    [SerializeField] private DiceType diceType = DiceType.D6; // Тип кости

    [Header("Effects")]
    [SerializeField] private ParticleSystem hoverParticles;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip selectSound;

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private Material[] originalMaterials;
    private bool isHovered = false;
    private bool isSelected = false;

    // События
    public event System.Action<DiceSelectable> OnDiceSelected;
    public event System.Action<DiceType> OnDiceHovered;
    public event System.Action<DiceType> OnDiceUnhovered;

    // Свойства
    public DiceType DiceType => diceType;
    public bool IsSelected => isSelected;
    public bool IsHovered => isHovered;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();

        // Сохраняем оригинальные материалы
        if (meshRenderer != null) {
            originalMaterials = meshRenderer.materials;
        }

        // Сохраняем оригинальные трансформации
        originalPosition = transform.position;
        originalScale = transform.localScale;
        originalRotation = transform.rotation;

        // Отключаем физику по умолчанию
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void Start() {
        // Инициализируем эффекты
        if (hoverParticles != null) {
            hoverParticles.Stop();
        }
    }

    private void Update() {
        // Плавная анимация при наведении
        AnimateHover();
    }

    private void AnimateHover() {
        Vector3 targetPosition = originalPosition;
        Vector3 targetScale = originalScale;
        Quaternion targetRotation = originalRotation;

        if (isHovered) {
            targetPosition += Vector3.up * hoverHeight;
            targetScale *= hoverScale;

            // Медленное вращение при наведении
            targetRotation *= Quaternion.Euler(0, 30f * Time.deltaTime, 0);
        }

        // Плавная интерполяция
        transform.position = Vector3.Lerp(transform.position, targetPosition, animationSpeed * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, animationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, animationSpeed * Time.deltaTime);
    }

    // Метод вызывается при наведении курсора (через EventSystem)
    public void OnPointerEnter(PointerEventData eventData) {
        if (!isSelected) // Не реагируем, если уже выбрано
        {
            isHovered = true;
            ApplyHoverEffects();
            OnDiceHovered?.Invoke(diceType);
        }
    }

    // Метод вызывается при уходе курсора
    public void OnPointerExit(PointerEventData eventData) {
        if (!isSelected) // Не сбрасываем, если выбрано
        {
            isHovered = false;
            RemoveHoverEffects();
            OnDiceUnhovered?.Invoke(diceType);
        }
    }

    // Метод вызывается при клике
    public void OnPointerClick(PointerEventData eventData) {

        if (eventData.button == PointerEventData.InputButton.Left) {

            if (isSelected == true) {
                DeselectDice();
                return;
            }

            SelectDice();
        }
    }

    public void SelectDice() {
        isSelected = true;
        isHovered = true; // Оставляем hover эффекты для выбранного

        ApplySelectedEffects();
        OnDiceSelected?.Invoke(this);

        // Проигрываем звук
        if (selectSound != null) {
            AudioSource.PlayClipAtPoint(selectSound, transform.position);
        }
    }

    public void DeselectDice() {
        isSelected = false;
        isHovered = false;

        RemoveSelectedEffects();
        RemoveHoverEffects();
        RestoreNormalEffects();
    }

    private void ApplyHoverEffects() {

        // Проигрываем звук
        if (hoverSound != null) {
            AudioSource.PlayClipAtPoint(hoverSound, transform.position, 0.5f);
        }
    }

    private void ApplySelectedEffects() {

        // Включаем частицы
        if (hoverParticles != null && !hoverParticles.isPlaying) {
            hoverParticles.Play();
        }
    }

    private void RemoveSelectedEffects() {

        // Включаем частицы
        if (hoverParticles != null && hoverParticles.isPlaying) {
            hoverParticles.Stop();
        }

        if (hoverParticles != null) {
            var main = hoverParticles.main;
            main.startColor = Color.white;
        }
    }

    private void RemoveHoverEffects() {
        // Останавливаем частицы
        if (hoverParticles != null && hoverParticles.isPlaying) {
            hoverParticles.Stop();
        }
    }

    private void RestoreNormalEffects() {
        // Восстанавливаем оригинальные материалы
        if (meshRenderer != null && originalMaterials != null) {
            meshRenderer.materials = originalMaterials;
        }
    }

    // Метод для ручного вызова выделения (например, через UI)
    public void HoverDice() {
        if (!isSelected) {
            isHovered = true;
            ApplyHoverEffects();
            OnDiceHovered?.Invoke(diceType);
        }
    }

    // Метод для ручного снятия выделения
    public void UnhoverDice() {
        if (!isSelected) {
            isHovered = false;
            RemoveHoverEffects();
            RestoreNormalEffects();
            OnDiceUnhovered?.Invoke(diceType);
        }
    }

    // Вспомогательный метод для сброса состояния
    public void ResetDice() {
        isSelected = false;
        isHovered = false;

        transform.position = originalPosition;
        transform.localScale = originalScale;
        transform.rotation = originalRotation;

        RestoreNormalEffects();

        if (hoverParticles != null) {
            hoverParticles.Stop();
        }
    }

    // Метод для установки нового типа кости
    public void SetDiceType(DiceType type) {
        diceType = type;
        // Можно добавить логику обновления внешнего вида
    }

    // Визуализация для отладки
    private void OnDrawGizmosSelected() {
        if (isHovered) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }

        if (isSelected) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.35f);
        }
    }
}
