using UnityEngine;

namespace SurvivalGame.Camera
{
    /// <summary>
    /// 플레이어를 쿼터뷰 고정 각도로 추적하는 카메라입니다.
    /// 카메라 회전은 Inspector에서 고정하고 위치만 따라갑니다.
    /// </summary>
    public class PlayerCamera : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Transform _target;
        [SerializeField] private Vector3   _offset      = new Vector3(0f, 15f, -10f);
        [SerializeField] private float     _smoothSpeed = 10f;

        #endregion

        #region Unity Lifecycle

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 desired = _target.position + _offset;
            transform.position = Vector3.Lerp(transform.position, desired, _smoothSpeed * Time.deltaTime);
            transform.LookAt(_target);
        }

        #endregion
    }
}
