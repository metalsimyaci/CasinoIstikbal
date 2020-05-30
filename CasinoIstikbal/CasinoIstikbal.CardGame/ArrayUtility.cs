using System;
using System.Collections.Generic;

namespace CasinoIstikbal.CardGame
{
    public static class ArrayUtility
    {
        public static IEnumerable<T> SliceRow<T>(this T[,] array, int row)
        {
            for (var i = array.GetLowerBound(1); i <= array.GetUpperBound(1); i++)
            {
                yield return array[row, i];
            }
        }
        public static IEnumerable<T> SliceRow<T>(this T[,,] array, int row)
        {
            for (var i = array.GetLowerBound(1); i <= array.GetUpperBound(1); i++)
            {
                for (var j = array.GetLowerBound(2); j <= array.GetUpperBound(2); j++)
                    yield return array[row, i, j];
            }
        }

        public static IEnumerable<T> SliceColumn<T>(this T[,] array, int column)
        {
            for (var i = array.GetLowerBound(0); i <= array.GetUpperBound(0); i++)
            {
                yield return array[i, column];
            }
        }
        public static IEnumerable<T> SliceColumn<T>(this T[,,] array, int column)
        {
            for (var i = array.GetLowerBound(0); i <= array.GetUpperBound(0); i++)
            {
                for (var j = array.GetLowerBound(1); j <= array.GetUpperBound(1); j++)
                    yield return array[i, j, column];
            }
        }
        public static int Push<T>(this T[] source, T value)
        {
            var index = Array.IndexOf(source, default(T));

            if (index != -1)
            {
                source[index] = value;
            }

            return index;
        }
    }
}