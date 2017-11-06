using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods 
{
    public static Color ToColor(this float[] p_source)
    {
        return new Color(p_source[0], p_source[1], p_source[2], p_source[3]);
    }

    public static float[] ToArray(this Color p_source)
    {
        return new float[] { p_source.r, p_source.g, p_source.b, p_source.a };
    }
}
