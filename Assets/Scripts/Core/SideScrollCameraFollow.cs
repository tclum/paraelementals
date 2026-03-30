using UnityEngine;

public class SideScrollCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _followSpeed = 8f;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 1f, -10f);

    private void LateUpdate()
    {
        if (_target == null)
            return;

        Vector3 desired = new Vector3(_target.position.x, _target.position.y, 0f) + _offset;
        transform.position = Vector3.Lerp(transform.position, desired, _followSpeed * Time.deltaTime);
    }
}
