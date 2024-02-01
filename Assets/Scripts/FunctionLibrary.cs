using System;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Mathf;

public static class FunctionLibrary
{
    // 1
    public delegate Vector3 Function(float u, float v, float t);  // not static!
    // 2
    public enum FunctonName { Wave, MultiWave, Ripple, Sphere, Cylinder, TweakSphere, Torus }
    static Function[] functions = { Wave, MultiWave, Ripple, Sphere, Cylinder, TweakSphere, Torus };
    // 3
    public static Function GetFunction(FunctonName name)
    {
        return functions[(int)name];
    }

    public static Vector3 Wave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y=Sin(PI * (u + v + t));
        p.z = v;
        return p;
    }

    public static Vector3 MultiWave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        float y= Sin(PI * (u + 0.5f*t));
        y += 0.5f * Sin(2f * PI * (v + t));
        y += Sin(PI * (u + v + 0.25f * t));
        // Do Not write as (2/3), the result will be 0(an integer divided by an integer is an integer).
        p.y=y * (1/2.5f);
        p.z = v; 
        return p;
    }

    public static Vector3 Ripple(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        float d = Sqrt(u * u + v * v);
        // Why divide +1? 
        // Because d could be 0, we want to also avoid to divide a very small number.
        // Thus we want the divided number at least be 1. Thus +1.
        p.y=Sin(PI*(4.0f * d -t))/(1f+10*d);
        p.z=v;
        return p;
    }

    public static Vector3 Sphere(float u, float v, float t)
    {
        Vector3 p;
        float r = Cos(0.5f * PI * v);

        p.x = r*Sin(PI * u);
        p.y = Sin(PI * 0.5f * v);
        p.z = r*Cos(PI * u);
        return p;
    }

    public static Vector3 Cylinder(float u, float v, float t)
    {
        Vector3 p;
        p.x = Sin(PI * u);
        p.y = v;
        p.z = Cos(PI * u);
        return p;
    }

    public static Vector3 TweakSphere(float u, float v, float t)
    {
        Vector3 p;
        float r = 0.9f + 0.1f * Sin(PI * (6f * u + 4f * v + t));
        float s = r * Cos(0.5f * PI * v);
        
        p.x = s * Sin(PI * u);
        p.y = r * Sin(0.5f * PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }

    public static Vector3 Torus(float u, float v, float t)
    {
        Vector3 p;
        float r1 = 0.7f + 0.1f * Sin(PI * (6f * u + 0.5f * t));
        float r2 = 0.15f + 0.05f * Sin(PI * (8f * u + 4f * v + 2f * t));
        float s = r1 + r2 * Cos(PI * v);
        
        p.x = s * Sin(PI * u);
        p.y = r2 * Sin(PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
}
