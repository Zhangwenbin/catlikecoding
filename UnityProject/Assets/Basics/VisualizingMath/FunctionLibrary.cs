using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;
public class FunctionLibrary
{
    public enum FunctionName
    {
        Wave,
        MultiWave,
        Ripple
    }
    public delegate float Function(float x,float t);

    private static Function[] _functions = new[] {(Function)Wave,(Function)MultiWave,(Function)Ripple };

    public static Function GetFunction(FunctionName name)
    {
        return _functions[(int)name];
    }
    
    public static float Wave (float x, float t) {
        return Mathf.Sin(Mathf.PI * (x + t));
    }
    
    public static float MultiWave (float x, float t) {
        float y= Mathf.Sin(Mathf.PI * (x + t));
        y+= Mathf.Sin(2*Mathf.PI * (x + t))*0.5f;
        return y/3*2;
    }
    
    public static float Ripple (float x, float t) {
        float d = Abs(x);
        float y = Sin(4f * PI * d-t);
        return y / (1f + 10f * d);
    }
}
