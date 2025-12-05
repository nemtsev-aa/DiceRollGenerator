using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class NavigationPanel : UIPanel {
    public event Action RollButtonClicked;
    public event Action MultiplayerButtonClicked;
    public event Action AboutButtonClicked;
    public event Action HistoryButtonClicked;

    public bool IsMainMenuState => _isMainMenuState;

    [Header("Transform Settings")]
    [SerializeField] private RectTransform _mainMenuParent;
    [SerializeField] private RectTransform _gameplayParent;

    [Header("Animation Settings")]
    [SerializeField] private float _animationDuration = 0.3f;
    [SerializeField] private Ease _positionEase = Ease.OutBack;
    [SerializeField] private Ease _scaleEase = Ease.OutBack;

    [Header("Scale Settings")]
    [SerializeField] private float _mainMenuScale = 4f;
    [SerializeField] private float _gameplayScale = 1f;

    [Header("Additional Effects")]
    [SerializeField] private bool _enableFadeEffect = true;
    [SerializeField] private CanvasGroup _buttonsCanvasGroup;
    [SerializeField] private float _fadeDuration = 0.2f;

    [Header("Button References")]
    [SerializeField] private NavigationButton _rollButton;
    [SerializeField] private NavigationButton _historyButton;
    [SerializeField] private NavigationButton _multiplayerButton;
    [SerializeField] private NavigationButton _aboutButton;

    private bool _isMainMenuState = false;
    private Sequence _currentAnimation;
    private Vector3[] _buttonOriginalScales;
    private Vector3[] _buttonTargetScales;

    private List<GameObject> _navigationButtons;

    #region Unity Lifecycle

    private void Awake() {
        // Сохраняем оригинальные масштабы кнопок для анимации
        CacheButtonOriginalScales();

        // Инициализируем CanvasGroup если он есть
        if (_buttonsCanvasGroup == null) {
            _buttonsCanvasGroup = GetComponent<CanvasGroup>();
        }
    }

    private void OnDestroy() {
        // Очищаем анимации при уничтожении
        if (_currentAnimation != null && _currentAnimation.IsActive()) {
            _currentAnimation.Kill();
        }
    }

    #endregion

    #region Public Methods

    public void SetMainMenuState(bool status, bool animate = true) {

        if (_isMainMenuState == status && animate)
            return;

        _isMainMenuState = status;

        if (animate == true) { 
            AnimateToState(status);
            return;
        }

        SetStateImmediate(status);
    }

    #endregion

    #region UIPanel Implementation

    public override void Show() {
        base.Show();
        CreateSubscriptions();

        // Если нужно, включаем кнопки
        if (_buttonsCanvasGroup != null) {
            _buttonsCanvasGroup.alpha = 1f;
            _buttonsCanvasGroup.interactable = true;
            _buttonsCanvasGroup.blocksRaycasts = true;
        }
    }

    public override void Hide() {
        RemoveSubscriptions();

        // Анимация скрытия перед вызовом base.Hide()
        if (_buttonsCanvasGroup != null && _enableFadeEffect) {
            _buttonsCanvasGroup.DOFade(0f, _fadeDuration)
                .OnComplete(base.Hide);
        }
        else {
            base.Hide();
        }
    }

    #endregion

    #region Private Methods

    private void CacheButtonOriginalScales() {
        Button[] buttons = { _rollButton.Button, _historyButton.Button, _multiplayerButton.Button, _aboutButton.Button };
        _buttonOriginalScales = new Vector3[buttons.Length];

        for (int i = 0; i < buttons.Length; i++) {
            if (buttons[i] != null) {
                _buttonOriginalScales[i] = buttons[i].transform.localScale;
            }
        }
    }

    private void AnimateToState(bool isMainMenu) {
        // Останавливаем текущую анимацию если она есть
        if (_currentAnimation != null && _currentAnimation.IsActive()) {
            _currentAnimation.Kill();
        }

        _currentAnimation = DOTween.Sequence();

        RectTransform targetParent = isMainMenu ? _mainMenuParent : _gameplayParent;
        float targetScale = isMainMenu ? _mainMenuScale : _gameplayScale;

        // Анимация кнопок (последовательное появление/исчезновение)
        if (isMainMenu) {
            // При переходе в главное меню: сначала масштабируем, потом двигаем
            _currentAnimation.Append(transform.DOScale(Vector3.one * targetScale, _animationDuration)
                .SetEase(_scaleEase));

            _currentAnimation.Join(transform.DOMove(targetParent.position, _animationDuration)
                .SetEase(_positionEase));

            AnimateButtons(true);

            _aboutButton.gameObject.SetActive(true);
            _historyButton.gameObject.SetActive(false);
        }
        else {
            // При переходе в геймплей: сначала двигаем, потом масштабируем
            _currentAnimation.Append(transform.DOMove(targetParent.position, _animationDuration)
                .SetEase(_positionEase));

            _currentAnimation.Join(transform.DOScale(Vector3.one * targetScale, _animationDuration)
                .SetEase(_scaleEase));

            _historyButton.gameObject.SetActive(true);
            _aboutButton.gameObject.SetActive(false);
        }

        _currentAnimation.OnComplete(() => OnAnimationComplete(isMainMenu));
        _currentAnimation.SetUpdate(true); // Работает даже при Time.timeScale = 0
    }

    private void AnimateButtons(bool isMainMenu) {
        _navigationButtons = new List<GameObject>() {
            _rollButton.gameObject,
            _historyButton.gameObject,
            _multiplayerButton.gameObject,
            _aboutButton.gameObject
        };

        for (int i = 0; i < _navigationButtons.Count; i++) {
            // Последовательная анимация появления
            _navigationButtons[i].transform.localScale = Vector3.zero;
            _navigationButtons[i].transform.DOScale(1f, 0.3f)
                                           .SetDelay(i * 0.1f)
                                           .SetEase(Ease.OutBack);
        }
    }

    private void OnAnimationComplete(bool isMainMenu) {
        // Обновляем интерактивность кнопок если нужно
        if (_buttonsCanvasGroup != null) {
            _buttonsCanvasGroup.interactable = true;
            _buttonsCanvasGroup.blocksRaycasts = true;
        }

        // Логирование или вызов событий при завершении анимации
        Debug.Log($"NavigationPanel animation complete. State: {(isMainMenu ? "Main Menu" : "Gameplay")}");
    }

    private void SetStateImmediate(bool isMainMenu) {
        RectTransform targetParent = isMainMenu ? _mainMenuParent : _gameplayParent;
        float targetScale = isMainMenu ? _mainMenuScale : _gameplayScale;

        transform.position = targetParent.position;
        transform.localScale = Vector3.one * targetScale;

        if (_buttonsCanvasGroup != null) {
            _buttonsCanvasGroup.alpha = isMainMenu ? 1f : 0.8f;
            _buttonsCanvasGroup.interactable = true;
            _buttonsCanvasGroup.blocksRaycasts = true;
        }
    }

    private void CreateSubscriptions() {
        if (_rollButton != null)
            _rollButton.Button.onClick.AddListener(OnRollButtonClicked);

        if (_historyButton != null)
            _historyButton.Button.onClick.AddListener(OnHistoryButtonClicked);

        if (_multiplayerButton != null)
            _multiplayerButton.Button.onClick.AddListener(OnMultiplayerButtonClicked);

        if (_aboutButton != null)
            _aboutButton.Button.onClick.AddListener(OnAboutButtonClicked);
    }

    private void RemoveSubscriptions() {
        if (_rollButton != null)
            _rollButton.Button.onClick.RemoveListener(OnRollButtonClicked);

        if (_historyButton != null)
            _historyButton.Button.onClick.RemoveListener(OnHistoryButtonClicked);

        if (_multiplayerButton != null)
            _multiplayerButton.Button.onClick.RemoveListener(OnMultiplayerButtonClicked);

        if (_aboutButton != null)
            _aboutButton.Button.onClick.RemoveListener(OnAboutButtonClicked);
    }

    #endregion

    #region Button Event Handlers

    private void OnRollButtonClicked() {
        RollButtonClicked?.Invoke();

        // Анимация нажатия кнопки
        if (_rollButton != null) {
            AnimateButtonPress(_rollButton.transform);
        }
    }

    private void OnHistoryButtonClicked() {
        HistoryButtonClicked?.Invoke();

        if (_historyButton != null) {
            AnimateButtonPress(_historyButton.transform);
        }
    }

    private void OnMultiplayerButtonClicked() {
        MultiplayerButtonClicked?.Invoke();

        if (_multiplayerButton != null) {
            AnimateButtonPress(_multiplayerButton.transform);
        }
    }

    private void OnAboutButtonClicked() {
        AboutButtonClicked?.Invoke();

        if (_aboutButton != null) {
            AnimateButtonPress(_aboutButton.transform);
        }
    }

    private void AnimateButtonPress(Transform buttonTransform) {
        Sequence pressSequence = DOTween.Sequence();
        pressSequence.Append(buttonTransform.DOScale(buttonTransform.localScale * 0.9f, 0.1f));
        pressSequence.Append(buttonTransform.DOScale(buttonTransform.localScale, 0.1f));
        pressSequence.SetEase(Ease.OutBack);
    }

    #endregion
}