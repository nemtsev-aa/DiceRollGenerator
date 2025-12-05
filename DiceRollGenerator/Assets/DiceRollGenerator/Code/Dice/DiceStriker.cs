using UnityEngine;

public sealed class DiceStriker : MonoBehaviour {
    [Header("Raycast Settings")]
    [SerializeField] private LayerMask _diceLayer;
    [SerializeField] private float _raycastDistance = 100f;

    [Header("Selection Settings")]
    [SerializeField] private bool _allowMultipleSelection = false;
    [SerializeField] private Color _selectionOutlineColor = Color.green;

    private Camera _mainCamera;
    private DicePhysics _currentHoveredDice;

    private void Start() {
        _mainCamera = Camera.main;
    }

    private void Update() {
        
        
    }

    private void OnDrawGizmos() {

        if (Input.GetMouseButton(0) == true) {
            Gizmos.color = Color.red;
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            Gizmos.DrawRay(ray.origin, ray.direction * _raycastDistance);

            if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _diceLayer) == true) {

                if (hit.rigidbody.TryGetComponent(out DicePhysics dicePhysics) == true) {
                    hit.rigidbody.AddForce(ray.direction * 1f, ForceMode.Impulse);
                    Debug.Log($"Add Force to Dice: {hit.rigidbody.gameObject.name}");
                }
            }
        }
    }
}
