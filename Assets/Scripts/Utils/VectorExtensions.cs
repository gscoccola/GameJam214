using System;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 Flatten(this Vector3 input)
    {
        return new Vector2(input.x, input.y);
    }
}