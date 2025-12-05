using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProjectAuthorView : MonoBehaviour {
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _nameLabel;
    [SerializeField] private TextMeshProUGUI _typeLabel;

    private AboutConfig _config;

    public void Init(AboutConfig config) {
        _config = config;
        UpdateContent();
        Activate(true);
    } 
    
    public void Activate(bool status) => gameObject.SetActive(status);
    
    public void UpdateContent() {
        _nameLabel.text = _config.Name;
        _icon.sprite = _config.Icon;
        _typeLabel.text = _config.ProjectRolleName;
    }
}
