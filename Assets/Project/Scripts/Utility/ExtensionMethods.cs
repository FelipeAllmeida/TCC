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

    public static Color ToColor(this PlayerNetwork.Colors p_source)
    {
        switch (p_source)
        {
            case PlayerNetwork.Colors.BLACK:
                return Color.black;
            case PlayerNetwork.Colors.BLUE:
                return Color.blue;
            case PlayerNetwork.Colors.GREEN:
                return Color.green;
            case PlayerNetwork.Colors.ORANGE:
                return new Color(255f/255f, 153f/255f, 0f);
            //case PlayerNetwork.Colors.RED:
            //    return Color.red;
            case PlayerNetwork.Colors.WHITE:
                return Color.white;
            case PlayerNetwork.Colors.YELLOW:
                return Color.yellow;
            default:
                return Color.white;
        }
    }
}
