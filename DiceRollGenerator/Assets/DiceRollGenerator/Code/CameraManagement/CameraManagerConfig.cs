using System.Collections.Generic;
using UnityEngine;
using System;

namespace CameraManagmentService {

    [Serializable]
    public sealed class CameraManagerConfig {
        [field: SerializeField] public List<CameraState> CameraStates { get; private set; }
        [field: SerializeField] public float MoveAnimationDuration { get; private set; } = 0.3f;
        public bool TryGetState(CameraPositionsTypes type, out CameraState state) {

            for (int i = 0; i < CameraStates.Count; i++) {
                var iState = CameraStates[i];

                if (iState.Type == type) {
                    state = iState;
                    return true;
                }
            }

            state = default;
            return false;
        }
    }
}

