using System;

namespace Ceresta
{
    public class Common
    {
        public static string FormatAmount(int value)
        {
            if (value >= 1000000)
            {
                return (value / 1000000m).ToString("0.000") + "M";
            }
            else if (value >= 1000)
            {
                return (value / 1000m).ToString("0.00") + "K";
            }
            else
            {
                return value.ToString();
            }
        }
    }
}