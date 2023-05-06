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
        Rotatingtwistedsphere,
        Torus,
        Twistingtorus
    }
    public delegate Vector3 Function (float u, float v, float t);

    private static Function[] _functions = {Wave,MultiWave,Ripple ,Sphere,Spherewithverticalbands,Spherewithhorizantalbands,Rotatingtwistedsphere,Torus,Twistingtorus};

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
    
    public static Vector3 Rotatingtwistedsphere (float u, float v, float t) {
        float r = 0.9f + 0.1f * Sin( PI * (6*u+4*v+t));
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(0.5f * PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
    
    public static Vector3 Torus (float u, float v, float t) {
        float r1 = 0.75f;
        float r2 = 0.25f;
        float s = r1 + r2 * Cos(PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r2 * Sin(PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
    
    public static Vector3 Twistingtorus (float u, float v, float t) {
        float r1 = 0.7f + 0.1f * Sin(PI * (6f * u + 0.5f * t));
        float r2 = 0.15f + 0.05f * Sin(PI * (8f * u + 4f * v + 2f * t));
        float s = r1 + r2 * Cos(PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r2 * Sin(PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
}
