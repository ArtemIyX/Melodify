using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melodify
{
    public static class YoutubeHelper
    {
        public static string FormatViewCount(long views)
        {
            double absViews = Math.Abs(views);
            string sign = views < 0 ? "-" : "";

            if (absViews >= 1_000_000_000)
            {
                double billions = absViews / 1_000_000_000.0;
                return $"{sign}{billions:0.##}B";
            }
            else if (absViews >= 1_000_000)
            {
                double millions = absViews / 1_000_000.0;
                return $"{sign}{millions:0.##}M";
            }
            else if (absViews >= 1_000)
            {
                double thousands = absViews / 1_000.0;
                return $"{sign}{thousands:0.##}K";
            }
            else
            {
                return $"{sign}{views:N0}";
            }
        }
    }
}
