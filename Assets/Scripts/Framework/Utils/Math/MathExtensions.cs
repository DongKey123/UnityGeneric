using UnityEngine;

namespace Framework.Utils.Math
{
    /// <summary>
    /// 게임 개발에서 자주 사용되는 수학 유틸리티 메서드 모음입니다.
    /// </summary>
    public static class MathExtensions
    {
        #region Range

        /// <summary>
        /// 값을 한 범위에서 다른 범위로 재매핑합니다.
        /// </summary>
        /// <param name="value">재매핑할 값</param>
        /// <param name="fromMin">원본 범위 최솟값</param>
        /// <param name="fromMax">원본 범위 최댓값</param>
        /// <param name="toMin">목표 범위 최솟값</param>
        /// <param name="toMax">목표 범위 최댓값</param>
        /// <returns>목표 범위로 재매핑된 값</returns>
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            float t = Mathf.InverseLerp(fromMin, fromMax, value);
            return Mathf.Lerp(toMin, toMax, t);
        }

        /// <summary>
        /// 인덱스를 배열 크기 범위 내로 순환합니다. 음수 인덱스도 올바르게 처리합니다.
        /// </summary>
        /// <param name="index">순환할 인덱스</param>
        /// <param name="length">배열 또는 범위의 크기</param>
        /// <returns>0 이상 length 미만의 순환된 인덱스</returns>
        public static int LoopIndex(int index, int length)
        {
            return ((index % length) + length) % length;
        }

        #endregion

        #region Comparison

        /// <summary>
        /// 두 float 값이 지정한 오차 범위 내에서 같은지 비교합니다.
        /// </summary>
        /// <param name="a">비교할 첫 번째 값</param>
        /// <param name="b">비교할 두 번째 값</param>
        /// <param name="tolerance">허용 오차 (기본값: 0.0001)</param>
        /// <returns>두 값의 차이가 tolerance 이하이면 true</returns>
        public static bool IsApproximately(float a, float b, float tolerance = 0.0001f)
        {
            return Mathf.Abs(a - b) <= tolerance;
        }

        #endregion

        #region Rounding

        /// <summary>
        /// 값을 지정한 소수점 자릿수로 반올림합니다.
        /// </summary>
        /// <param name="value">반올림할 값</param>
        /// <param name="digits">소수점 자릿수</param>
        /// <returns>반올림된 값</returns>
        public static float RoundToDecimal(float value, int digits)
        {
            float multiplier = Mathf.Pow(10f, digits);
            return Mathf.Round(value * multiplier) / multiplier;
        }

        /// <summary>
        /// 값을 지정한 단위로 스냅합니다.
        /// </summary>
        /// <param name="value">스냅할 값</param>
        /// <param name="step">스냅 단위</param>
        /// <returns>가장 가까운 step 배수로 스냅된 값</returns>
        public static float Snap(float value, float step)
        {
            return Mathf.Round(value / step) * step;
        }

        #endregion

        #region Angle

        /// <summary>
        /// 각도를 -180 ~ 180 범위로 정규화합니다.
        /// </summary>
        /// <param name="angle">정규화할 각도</param>
        /// <returns>-180 ~ 180 범위의 각도</returns>
        public static float NormalizeAngle(float angle)
        {
            angle %= 360f;

            if (angle > 180f)
            {
                angle -= 360f;
            }
            else if (angle < -180f)
            {
                angle += 360f;
            }

            return angle;
        }

        #endregion
    }
}
