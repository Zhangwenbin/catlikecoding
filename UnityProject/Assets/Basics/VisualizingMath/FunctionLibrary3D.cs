using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;
public class FunctionLibrary3D
{
    public enum FunctionName
    {
        Wave,
        MultiWave,
        Ripple,
        Sphere,
        Spherewithverticalbands,
        Spherewithhorizantalbands,
    }
    public delegate Vector3 Function (float u, float v, float t);

    private static Function[] _functions = {Wave,MultiWave,Ripple ,Sphere,Spherewithverticalbands,Spherewithhorizantalbands};

    public static Function GetFunction(FunctionName name)
    {
        return _functions[(int)name];
    }
    
    public static Vector3 Wave (float u,float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.z = v;
        p.y=Mathf.Sin(Mathf.PI * (u+v + t));
        return p;
    }
    
    public static Vector3 MultiWave (float u,float v, float t) {
        Vector3 p;
        p.x = u;
        p.z = v;
        float y = Sin(PI * (u + 0.5f * t));
        y += 0.5f * Sin(2f * PI * (v + t));
        y += Sin(PI * (u + v + 0.25f * t));
        p.y = y* (1f / 2.5f);
        return p;
    }
    
    public static Vector3 Ripple (float u,float v, float t) {
        Vector3 p;
        p.x = u;
        p.z = v;
        float d = Sqrt(u * u + v * v);
        float y = Sin(PI * (4f * d - t));
        p.y = y/ (1f + 10f * d);
        return p;
    }
    
    public static Vector3 Sphere (float u, float v, float t) {
        float r = 0.5f + 0.5f * Sin(PI * t);
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(0.5f * PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
    public static Vector3 Spherewithverticalbands (float u, float v, float t) {
        float r = 0.9f + 0.1f * Sin(8f * PI * u);
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(0.5f * PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
    public static Vector3 Spherewithhorizantalbands (float u, float v, float t) {
        float r = 0.9f + 0.1f * Sin(8f * PI * v);
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(0.5f * PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
}
