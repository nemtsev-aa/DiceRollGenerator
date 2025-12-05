using UnityEngine;
using System;

[Serializable]
public class AboutConfig {
    [field: SerializeField] public ProjectAuthorTypes Type { get; private set; }
    [field: SerializeField] public string ProjectRolleName { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
}

