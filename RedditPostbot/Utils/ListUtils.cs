using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditPostbot.Utils
{
    public static class ListUtils
    {

        public static bool Contains(this List<string> list, string text, StringComparison comparison)
        {
            foreach (var element in list)
            {
                if (element.IndexOf(text, comparison) >= 0)
                    return true;
            }
            return false;
        }
    }
}
