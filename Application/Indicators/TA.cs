namespace Application.Indicators;
public static class TA
{
    public static double[] Ema(IReadOnlyList<decimal> src, int period)
    {
        if (period <= 1) throw new ArgumentOutOfRangeException(nameof(period));
        if (src.Count == 0) return Array.Empty<double>();
        var outArr = new double[src.Count];
        var k = 2.0 / (period + 1);
        outArr[0] = (double)src[0];
        for (int i = 1; i < src.Count; i++)
            outArr[i] = (double)src[i] * k + outArr[i - 1] * (1 - k);
        return outArr;
    }

    public static double[] Momentum(IReadOnlyList<decimal> src, int lookback)
    {
        var n = src.Count;
        var outArr = new double[n];
        for (int i = 0; i < n; i++)
        {
            var j = i - lookback;
            outArr[i] = j >= 0 ? (double)(src[i] - src[j]) / (double)src[j] : 0.0;
        }
        return outArr;
    }
}