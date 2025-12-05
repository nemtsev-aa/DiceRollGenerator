using UnityEngine;
using Zenject;

namespace DiceManagementService {
    public sealed class DiceManagerInstaller : MonoInstaller {
        [SerializeField] private DicePrefabProvider _dicePrefabProvider;
        [SerializeField] private DiceSpawnerConfig _spawnerConfig;

        public override void InstallBindings() {

            Container.BindInstance(_dicePrefabProvider)
                     .AsSingle()
                     .NonLazy();

            Container.BindInstance(_spawnerConfig)
                     .AsSingle()
                     .NonLazy();

            Container.Bind<DiceSpawner>()
                     .AsSingle()
                     .NonLazy();

            Container.Bind<DiceManager>()
                    .AsSingle()
                    .NonLazy();
        }
    }
}
