using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Finance.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsMissing(this string s)
        {
            return s.Equals("[MISSING]");
        }
    }
}