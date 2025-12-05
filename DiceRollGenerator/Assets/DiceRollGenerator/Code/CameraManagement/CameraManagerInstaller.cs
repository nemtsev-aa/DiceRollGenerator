using UnityEngine;
using Zenject;

namespace CameraManagmentService {
    public sealed class CameraManagerInstaller : MonoInstaller {
        [SerializeField] private CameraManagerConfig _config;
        [SerializeField] private Camera _camera;

        public override void InstallBindings() {

            Container.Bind<CameraManager>()
                     .AsSingle()
                     .WithArguments(_config, _camera)
                     .NonLazy();
        }
    }
}

