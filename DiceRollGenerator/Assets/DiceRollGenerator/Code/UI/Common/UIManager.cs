using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class UIManager : MonoBehaviour {
    [field: SerializeField] public List<UIPanel> Panels { get; private set; } = new List<UIPanel>();

    private Dictionary<Type, UIPanel> _panelDictionary;

    public void Init() {
        _panelDictionary = new Dictionary<Type, UIPanel>();

        foreach (var panel in Panels) {
            
            if (panel != null) {
                var panelType = panel.GetType();

                if (_panelDictionary.ContainsKey(panelType) == false) {
                    _panelDictionary.Add(panelType, panel);
                    return;
                }

                Debug.LogWarning($"Duplicate panel type found: {panelType.Name}. Only the first one will be stored.");
            }
        }

        //HideAllPanels();
        //GetPanel<NavigationPanel>().Show();
    }

    public T GetPanel<T>() where T : UIPanel {
        var type = typeof(T);

        if (_panelDictionary.TryGetValue(type, out UIPanel panel)) {
            return panel as T;
        }

        // Если не нашли в словаре, попробуем найти в списке
        foreach (var p in Panels) {
            if (p is T typedPanel) {
                // Добавляем в словарь для будущих запросов
                _panelDictionary[type] = typedPanel;
                return typedPanel;
            }
        }

        Debug.LogError($"Panel of type {type.Name} not found!");
        return null;
    }

    public T ShowPanel<T>() where T : UIPanel {
        var panel = GetPanel<T>();

        if (panel != null) {
            panel.Show();
            return panel;
        }

        return null;
    }

    public void HidePanel<T>() where T : UIPanel {
        var panel = GetPanel<T>();
        panel?.Hide();
    }

    public void HideAllPanels() {

        for (int i = 1; i < Panels.Count; i++) {
            Panels[i].Hide();
        }
    }
}


