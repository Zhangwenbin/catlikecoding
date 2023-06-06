using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunctionName=FunctionLibrary3D.FunctionName;
public class Graph3D : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;
    
    [SerializeField, Range(10, 200)]
    int resolution = 10;

    public FunctionName function;

    private Transform[] points;
    private float step ;
    
    
    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;
    float duration;
    bool transitioning;

    FunctionName transitionFunction;
    public enum TransitionMode { Cycle, Random }

    [SerializeField]
    TransitionMode transitionMode;
    // Start is called before the first frame update
    void Start()
    {
        step = 2f / resolution;
        var scale = Vector3.one * step;
        points = new Transform[resolution*resolution];
        for (int i = 0; i < resolution*resolution; i++) {
            Transform point = Instantiate(pointPrefab);
            point.localScale = scale;
            point.SetParent(transform,false);
            points[i] = point;
        }
    }

    // Update is called once per frame
    void Update()
    {
        duration += Time.deltaTime;
        if (transitioning)
        {
            if (duration >= transitionDuration) {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration) {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            PickNextFunction();
        }
        if (transitioning) {
            UpdateFunctionTransition();
        }
        else {
            UpdateFunction();
        }
    }
    
    void PickNextFunction () {
        function = transitionMode == TransitionMode.Cycle ?
            FunctionLibrary3D.GetNextFunctionName(function) :
            FunctionLibrary3D.GetRandomFunctionNameOtherThan(function);
    }
    void UpdateFunctionTransition () {
        FunctionLibrary3D.Function
            from = FunctionLibrary3D.GetFunction(transitionFunction),
            to = FunctionLibrary3D.GetFunction(function);
        float progress = duration / transitionDuration;
        float time = Time.time;
        var f = FunctionLibrary3D.GetFunction(function);
        float v = 0.5f * step - 1f;
        for (int i = 0,x=0,z=0; i < resolution*resolution; i++,x++) {
            if (x==resolution)
            {
                x = 0;
                z++;
                v = (z+0.5f) * step - 1f;
            }
            Transform point = points[i];
           
            float u = (x + 0.5f) * step - 1f;
            point.GetComponent<Point>().SetUV(u,v);
            point.localPosition = FunctionLibrary3D.Morph(u, v, time, from, to, progress);
        }
    }
    void UpdateFunction()
    {
        float time = Time.time;
        var f = FunctionLibrary3D.GetFunction(function);
        float v = 0.5f * step - 1f;
        for (int i = 0,x=0,z=0; i < resolution*resolution; i++,x++) {
            if (x==resolution)
            {
                x = 0;
                z++;
                v = (z+0.5f) * step - 1f;
            }
            Transform point = points[i];
           
            float u = (x + 0.5f) * step - 1f;
            point.GetComponent<Point>().SetUV(u,v);
            point.localPosition =f(u,v,time);
        }
    }
    
   
}
