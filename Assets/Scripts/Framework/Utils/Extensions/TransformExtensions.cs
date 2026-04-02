using UnityEngine;

namespace Framework.Utils.Extensions
{
    /// <summary>
    /// <see cref="Transform"/> 컴포넌트에 대한 확장 메서드 모음입니다.
    /// </summary>
    public static class TransformExtensions
    {
        #region Position

        /// <summary>
        /// World Position의 X축 값만 변경합니다.
        /// </summary>
        /// <param name="transform">대상 Transform</param>
        /// <param name="x">설정할 X 값</param>
        public static void SetPositionX(this Transform transform, float x)
        {
            Vector3 pos = transform.position;
            pos.x = x;
            transform.position = pos;
        }

        /// <summary>
        /// World Position의 Y축 값만 변경합니다.
        /// </summary>
        /// <param name="transform">대상 Transform</param>
        /// <param name="y">설정할 Y 값</param>
        public static void SetPositionY(this Transform transform, float y)
        {
            Vector3 pos = transform.position;
            pos.y = y;
            transform.position = pos;
        }

        /// <summary>
        /// World Position의 Z축 값만 변경합니다.
        /// </summary>
        /// <param name="transform">대상 Transform</param>
        /// <param name="z">설정할 Z 값</param>
        public static void SetPositionZ(this Transform transform, float z)
        {
            Vector3 pos = transform.position;
            pos.z = z;
            transform.position = pos;
        }

        /// <summary>
        /// Local Position의 X축 값만 변경합니다.
        /// </summary>
        /// <param name="transform">대상 Transform</param>
        /// <param name="x">설정할 X 값</param>
        public static void SetLocalPositionX(this Transform transform, float x)
        {
            Vector3 pos = transform.localPosition;
            pos.x = x;
            transform.localPosition = pos;
        }

        /// <summary>
        /// Local Position의 Y축 값만 변경합니다.
        /// </summary>
        /// <param name="transform">대상 Transform</param>
        /// <param name="y">설정할 Y 값</param>
        public static void SetLocalPositionY(this Transform transform, float y)
        {
            Vector3 pos = transform.localPosition;
            pos.y = y;
            transform.localPosition = pos;
        }

        /// <summary>
        /// Local Position의 Z축 값만 변경합니다.
        /// </summary>
        /// <param name="transform">대상 Transform</param>
        /// <param name="z">설정할 Z 값</param>
        public static void SetLocalPositionZ(this Transform transform, float z)
        {
            Vector3 pos = transform.localPosition;
            pos.z = z;
            transform.localPosition = pos;
        }

        #endregion

        #region Reset

        /// <summary>
        /// Local Position, Rotation, Scale을 기본값으로 초기화합니다.
        /// </summary>
        /// <param name="transform">대상 Transform</param>
        public static void ResetLocal(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        #endregion

        #region 2D

        /// <summary>
        /// 2D 환경에서 대상 위치를 바라보도록 Z축으로 회전합니다.
        /// </summary>
        /// <param name="transform">대상 Transform</param>
        /// <param name="target">바라볼 월드 좌표</param>
        public static void LookAt2D(this Transform transform, Vector2 target)
        {
            Vector2 direction = target - (Vector2)transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        #endregion

        #region Hierarchy

        /// <summary>
        /// 모든 자식 오브젝트를 제거합니다.
        /// </summary>
        /// <param name="transform">대상 Transform</param>
        public static void DestroyAllChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(transform.GetChild(i).gameObject);
            }
        }

        #endregion
    }
}
