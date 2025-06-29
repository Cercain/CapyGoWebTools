namespace CapyGoDMG.Data
{
    public static class Extensions
    {
        public static string ToKMB(this decimal num)
        {
            if (num > 999999999 || num < -999999999)
            {
                return num.ToString("0,,,.###B");
            }
            else
            if (num > 999999 || num < -999999)
            {
                return num.ToString("0,,.##M");
            }
            else
            if (num > 999 || num < -999)
            {
                return num.ToString("0,.#K");
            }
            else
            {
                return num.ToString();
            }
        }

        public static string ToKMB(this long num)
        {
            if (num > 999999999 || num < -999999999)
            {
                return num.ToString("0,,,.###B");
            }
            else
            if (num > 999999 || num < -999999)
            {
                return num.ToString("0,,.##M");
            }
            else
            if (num > 999 || num < -999)
            {
                return num.ToString("0,.#K");
            }
            else
            {
                return num.ToString();
            }
        }

        public static void Combine(this Dictionary<Enums.StatsEnum, float> pairs, Enums.StatsEnum key, float val)
        {
            if(pairs.ContainsKey(key))
            {
                pairs[key] += val;
            }
            else
            {
                pairs[key] = val;
            }
        }
    }
}
