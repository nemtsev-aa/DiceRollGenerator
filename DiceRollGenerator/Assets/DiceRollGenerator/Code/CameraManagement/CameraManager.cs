using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CameraManagmentService {
    public sealed class CameraManager {
        private readonly CameraManagerConfig _config;
        private readonly Camera _camera;

        private CameraState _currentState;

        public CameraManager(CameraManagerConfig config, Camera camera) {
            _config = config;
            _camera = camera;
        }

        public async UniTask SwitchState(CameraPositionsTypes type) {

            if (_config.TryGetState(type, out CameraState state) == true) {
                _currentState = state;
                await UpdatePosition();
            }
        }

        private async UniTask UpdatePosition() {

            Vector3 startPosition = _camera.transform.position;
            Quaternion startRotation = _camera.transform.rotation;

            Vector3 targetPosition = _currentState.Parent.position;
            Quaternion targetRotation = _currentState.Parent.rotation;

            float duration = _config.MoveAnimationDuration;
            float elapsedTime = 0f;

            while (elapsedTime < duration) {
                float t = elapsedTime / duration;
                float smoothT = Mathf.SmoothStep(0f, 1f, t);

                _camera.transform.position = Vector3.Lerp(startPosition, targetPosition, smoothT);
                _camera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, smoothT);

                elapsedTime += Time.deltaTime;

                await UniTask.WaitForEndOfFrame();
            }

            _camera.transform.position = targetPosition;
            _camera.transform.rotation = targetRotation;
        }
    }
}

