using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class AboutPanel : UIPanel {
    public event Action Closed;

    [SerializeField] private AboutConfigs _authorConfigs;
    [SerializeField] private ProjectAuthorView _projectAuthorViewPrefab;
    [SerializeField] private RectTransform _viewParent;
    [SerializeField] private Button _closeButton;

    private List<ProjectAuthorView> _views;

    public override void Show() {
        base.Show();

        if (_views == null)
            CreateViews();

        _closeButton.onClick.AddListener(() => Hide());
    }

    public override void Hide() {
        base.Hide();

        RemoveViews();
        _closeButton.onClick.RemoveListener(() => Hide());
        Closed?.Invoke();
    }

    private void CreateViews() {
        _views = new List<ProjectAuthorView>();

        foreach (AboutConfig iConfig in _authorConfigs.Configs) {
            var view = Instantiate(_projectAuthorViewPrefab, _viewParent);
            view.Init(iConfig);
            _views.Add(view);
        }

        for (int i = 0; i < _views.Count; i++) {
            _views[i].transform.localScale = Vector3.zero;
            _views[i].transform.DOScale(1f, 0.3f)
                               .SetDelay(i * 0.1f)
                               .SetEase(Ease.OutBack);
        }

    }

    private void RemoveViews() {

        if (_views == null || _views.Count == 0)
            return;

        for (int i = 0; i < _views.Count; i++) {
            Destroy(_views[i].gameObject);
        }

        _views.Clear();
        _views = null;
    }
}
