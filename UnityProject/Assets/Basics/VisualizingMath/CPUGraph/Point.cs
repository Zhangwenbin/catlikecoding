using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    public Vector3 uvt;

    public void SetUV(float u, float v)
    {
        uvt.x = u;
        uvt.z = v;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(uvt,transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(uvt,transform.localScale);
    }
}
