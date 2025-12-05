using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(
    fileName = nameof(AboutConfigs),
    menuName = "Configs/" + nameof(AboutConfigs)
)]
public class AboutConfigs: ScriptableObject {
    [field: SerializeField] public List<AboutConfig> Configs { get; private set; }

    public AboutConfig GetConfigByType(ProjectAuthorTypes type) {
        return Configs.FirstOrDefault(c => c.Type == type);
    }
}

