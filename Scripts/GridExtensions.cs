using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridExtensions
{
    public enum State
    {
        Alive,
        Dead,
    }

    public enum Axis
    {
        Up = 0,
        Right,
        Down,
        Left,
        RightUp,
        LeftUp,
        RightDown,
        LeftDown,
        None,

    }

    public static class GridExtensions
    {
        public static void Jitter<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                var tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }
    }
}