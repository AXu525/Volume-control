using System;

namespace AudioVolumeControl
{
    /// <summary>
    /// 音量计算器
    /// 用于音量值的转换和计算
    /// </summary>
    public static class VolumeCalculator
    {
        /// <summary>
        /// 将线性音量值（0-100）转换为分贝值
        /// </summary>
        /// <param name="linear">线性音量值（0-100）</param>
        /// <returns>分贝值</returns>
        public static float LinearToDecibel(int linear)
        {
            if (linear <= 0)
                return -100f; // 完全静音

            if (linear >= 100)
                return 0f; // 最大音量

            // 使用对数曲线转换
            // 0-100 的线性值映射到 -100 到 0 dB
            return (float)(20 * Math.Log10(linear / 100.0));
        }

        /// <summary>
        /// 将分贝值转换为线性音量值（0-100）
        /// </summary>
        /// <param name="decibel">分贝值</param>
        /// <returns>线性音量值（0-100）</returns>
        public static int DecibelToLinear(float decibel)
        {
            if (decibel <= -100f)
                return 0; // 完全静音

            if (decibel >= 0f)
                return 100; // 最大音量

            // 使用指数曲线转换
            // -100 到 0 dB 映射到 0-100 的线性值
            return (int)Math.Round(Math.Pow(10, decibel / 20.0) * 100);
        }

        /// <summary>
        /// 限制音量值在有效范围内
        /// </summary>
        /// <param name="volume">音量值</param>
        /// <returns>限制后的音量值（0-100）</returns>
        public static int ClampVolume(int volume)
        {
            return Math.Clamp(volume, 0, 100);
        }

        /// <summary>
        /// 平滑过渡音量值
        /// </summary>
        /// <param name="current">当前音量</param>
        /// <param name="target">目标音量</param>
        /// <param name="step">步进值（每次调整的量）</param>
        /// <returns>下一步音量值</returns>
        public static int SmoothTransition(int current, int target, int step = 1)
        {
            if (current == target)
                return current;

            if (current < target)
            {
                return Math.Min(current + step, target);
            }
            else
            {
                return Math.Max(current - step, target);
            }
        }

        /// <summary>
        /// 计算音量百分比
        /// </summary>
        /// <param name="current">当前音量</param>
        /// <param name="max">最大音量</param>
        /// <returns>百分比（0-100）</returns>
        public static int CalculatePercentage(int current, int max)
        {
            if (max <= 0)
                return 0;

            return (int)Math.Round((double)current / max * 100);
        }
    }
}
