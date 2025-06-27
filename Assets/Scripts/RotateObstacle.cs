using UnityEngine;

public class RotateObstacle : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _rotationSpeed = 90f;

    private void FixedUpdate()
    {
        Quaternion deltaRotation = Quaternion.AngleAxis(_rotationSpeed * Time.fixedDeltaTime, Vector3.up);
        _rb.MoveRotation(deltaRotation * _rb.rotation);
    }
}
