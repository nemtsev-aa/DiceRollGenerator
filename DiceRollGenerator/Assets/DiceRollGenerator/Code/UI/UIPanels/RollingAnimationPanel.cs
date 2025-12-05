using DG.Tweening;
using TMPro;
using UnityEngine;

public sealed class RollingAnimationPanel : UIPanel {
    [SerializeField] private TMP_Text _rollingText;
    [SerializeField] private Transform _spinningDiceIcon;

    public override void Show() {
        base.Show();

        // Анимация текста
        _rollingText.DOFade(0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);

        // Вращение иконки
        _spinningDiceIcon.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    public override void Hide() {
        base.Hide();

        DOTween.Kill(_rollingText);
        DOTween.Kill(_spinningDiceIcon);
    }
}
