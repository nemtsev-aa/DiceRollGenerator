using System;
using UnityEngine;

namespace CameraManagmentService {

    [Serializable]
    public struct CameraState {
        public CameraPositionsTypes Type;
        public Transform Parent;
    }
}

