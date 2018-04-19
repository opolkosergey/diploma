using System;

namespace Diploma.Extensions
{
    public static class RandomExtensions
    {
        public static bool NextBool(this Random random) => random.Next(1) == 1 ? true : false;
    }
}
