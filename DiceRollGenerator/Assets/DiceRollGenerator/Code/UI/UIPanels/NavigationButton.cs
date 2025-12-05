using UnityEngine;
using UnityEngine.UI;

public sealed class NavigationButton : MonoBehaviour {
    public Button Button => _button;

    [SerializeField] private Button _button;

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() { 
        gameObject.SetActive(false);
    }
}
