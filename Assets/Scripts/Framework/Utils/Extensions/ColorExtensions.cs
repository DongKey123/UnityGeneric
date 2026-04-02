using UnityEngine;

namespace Framework.Utils.Extensions
{
    /// <summary>
    /// <see cref="Color"/>에 대한 확장 메서드 모음입니다.
    /// </summary>
    public static class ColorExtensions
    {
        #region With

        /// <summary>
        /// Alpha 값만 교체한 새 <see cref="Color"/>를 반환합니다.
        /// </summary>
        /// <param name="color">원본 색상</param>
        /// <param name="alpha">교체할 Alpha 값 (0~1)</param>
        /// <returns>Alpha가 교체된 새 Color</returns>
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        /// <summary>
        /// R 값만 교체한 새 <see cref="Color"/>를 반환합니다.
        /// </summary>
        /// <param name="color">원본 색상</param>
        /// <param name="r">교체할 R 값 (0~1)</param>
        /// <returns>R이 교체된 새 Color</returns>
        public static Color WithR(this Color color, float r)
        {
            return new Color(r, color.g, color.b, color.a);
        }

        /// <summary>
        /// G 값만 교체한 새 <see cref="Color"/>를 반환합니다.
        /// </summary>
        /// <param name="color">원본 색상</param>
        /// <param name="g">교체할 G 값 (0~1)</param>
        /// <returns>G가 교체된 새 Color</returns>
        public static Color WithG(this Color color, float g)
        {
            return new Color(color.r, g, color.b, color.a);
        }

        /// <summary>
        /// B 값만 교체한 새 <see cref="Color"/>를 반환합니다.
        /// </summary>
        /// <param name="color">원본 색상</param>
        /// <param name="b">교체할 B 값 (0~1)</param>
        /// <returns>B가 교체된 새 Color</returns>
        public static Color WithB(this Color color, float b)
        {
            return new Color(color.r, color.g, b, color.a);
        }

        #endregion

        #region Conversion

        /// <summary>
        /// <see cref="Color"/>를 "#RRGGBB" 형식의 Hex 문자열로 변환합니다.
        /// </summary>
        /// <param name="color">변환할 색상</param>
        /// <returns>"#RRGGBB" 형식의 문자열</returns>
        public static string ToHex(this Color color)
        {
            Color32 c = color;
            return $"#{c.r:X2}{c.g:X2}{c.b:X2}";
        }

        /// <summary>
        /// "#RRGGBB" 또는 "#RRGGBBAA" 형식의 Hex 문자열을 <see cref="Color"/>로 변환합니다.
        /// 파싱에 실패하면 <see cref="Color.white"/>를 반환합니다.
        /// </summary>
        /// <param name="hex">"#RRGGBB" 또는 "#RRGGBBAA" 형식의 문자열</param>
        /// <returns>변환된 Color. 실패 시 Color.white</returns>
        public static Color FromHex(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out Color color))
            {
                return color;
            }

            Debug.LogWarning($"[ColorExtensions] FromHex: 잘못된 Hex 형식 → {hex}");
            return Color.white;
        }

        #endregion
    }
}
