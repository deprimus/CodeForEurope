public static class Extensions
{
    /// <summary>
    /// Maps a value from one range to another.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="fromMin">The minimum of the original range.</param>
    /// <param name="fromMax">The maximum of the original range.</param>
    /// <param name="toMin">The minimum of the target range.</param>
    /// <param name="toMax">The maximum of the target range.</param>
    /// <returns>The mapped value in the target range.</returns>
    public static float Map(this float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }
}
