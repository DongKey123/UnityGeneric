using UnityEngine;

namespace Framework.Utils.Extensions
{
    /// <summary>
    /// <see cref="Vector2"/> 및 <see cref="Vector3"/>에 대한 확장 메서드 모음입니다.
    /// </summary>
    public static class VectorExtensions
    {
        #region Vector3 With

        /// <summary>
        /// X 값만 교체한 새 <see cref="Vector3"/>를 반환합니다.
        /// </summary>
        /// <param name="v">원본 벡터</param>
        /// <param name="x">교체할 X 값</param>
        /// <returns>X가 교체된 새 Vector3</returns>
        public static Vector3 WithX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);

        /// <summary>
        /// Y 값만 교체한 새 <see cref="Vector3"/>를 반환합니다.
        /// </summary>
        /// <param name="v">원본 벡터</param>
        /// <param name="y">교체할 Y 값</param>
        /// <returns>Y가 교체된 새 Vector3</returns>
        public static Vector3 WithY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);

        /// <summary>
        /// Z 값만 교체한 새 <see cref="Vector3"/>를 반환합니다.
        /// </summary>
        /// <param name="v">원본 벡터</param>
        /// <param name="z">교체할 Z 값</param>
        /// <returns>Z가 교체된 새 Vector3</returns>
        public static Vector3 WithZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);

        #endregion

        #region Vector2 With

        /// <summary>
        /// X 값만 교체한 새 <see cref="Vector2"/>를 반환합니다.
        /// </summary>
        /// <param name="v">원본 벡터</param>
        /// <param name="x">교체할 X 값</param>
        /// <returns>X가 교체된 새 Vector2</returns>
        public static Vector2 WithX(this Vector2 v, float x) => new Vector2(x, v.y);

        /// <summary>
        /// Y 값만 교체한 새 <see cref="Vector2"/>를 반환합니다.
        /// </summary>
        /// <param name="v">원본 벡터</param>
        /// <param name="y">교체할 Y 값</param>
        /// <returns>Y가 교체된 새 Vector2</returns>
        public static Vector2 WithY(this Vector2 v, float y) => new Vector2(v.x, y);

        #endregion

        #region Conversion

        /// <summary>
        /// <see cref="Vector3"/>를 XY 성분만 사용하는 <see cref="Vector2"/>로 변환합니다.
        /// </summary>
        /// <param name="v">원본 벡터</param>
        /// <returns>변환된 Vector2</returns>
        public static Vector2 ToVector2(this Vector3 v) => new Vector2(v.x, v.y);

        /// <summary>
        /// <see cref="Vector2"/>를 Z = 0인 <see cref="Vector3"/>로 변환합니다.
        /// </summary>
        /// <param name="v">원본 벡터</param>
        /// <param name="z">설정할 Z 값 (기본값: 0)</param>
        /// <returns>변환된 Vector3</returns>
        public static Vector3 ToVector3(this Vector2 v, float z = 0f) => new Vector3(v.x, v.y, z);

        #endregion

        #region Utility

        /// <summary>
        /// 두 <see cref="Vector3"/> 사이의 랜덤 값을 반환합니다.
        /// </summary>
        /// <param name="min">최솟값 벡터</param>
        /// <param name="max">최댓값 벡터</param>
        /// <returns>각 축이 min~max 범위인 랜덤 Vector3</returns>
        public static Vector3 RandomRange(Vector3 min, Vector3 max)
        {
            return new Vector3(
                Random.Range(min.x, max.x),
                Random.Range(min.y, max.y),
                Random.Range(min.z, max.z)
            );
        }

        #endregion
    }
}
