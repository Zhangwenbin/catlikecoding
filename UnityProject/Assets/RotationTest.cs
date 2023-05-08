using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTest : MonoBehaviour
{
    public Transform rootTransform;


    [ContextMenu("FitTransform")]
    public void FitTransform()
    {
        var parent = rootTransform;
        Quaternion rotation=parent.localRotation;
        Vector3 position = parent.localPosition;
        while (parent.name!="whale")
        {
            parent = parent.GetChild(0);
            rotation = rotation*parent.localRotation;
            position = position +  parent.localRotation*parent.localPosition;
        }
        transform.localRotation = rotation;
        transform.localPosition = position;
    }
    
    [ContextMenu("FitTransformByMatrix")]
    public void FitTransformByMatrix()
    {
        var parent = rootTransform;
        Quaternion rotation=parent.localRotation;
        Vector3 position = parent.localPosition;
        while (parent.name!="whale")
        {
            parent = parent.GetChild(0);
            rotation = rotation*parent.localRotation;
            position = position +  parent.localRotation*parent.localPosition;
        }
        transform.localEulerAngles = parent.parent.localToWorldMatrix.MultiplyVector(parent.localEulerAngles);
        transform.localPosition = position;
    }
}
