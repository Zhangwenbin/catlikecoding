using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunctionName=FunctionLibrary3D.FunctionName;
public class GPUGraph : MonoBehaviour
{

    [SerializeField, Range(10, 1000)]
    int resolution = 10;

    public FunctionName function;
    
    private float step ;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;
    float duration;
    bool transitioning;

    FunctionName transitionFunction;
    public enum TransitionMode { Cycle, Random }

    [SerializeField]
    TransitionMode transitionMode;
    
    

    private ComputeBuffer positionComputeBuffer;
    
    [SerializeField]
    private ComputeShader positionComputeShader;
    
    [SerializeField]
    Material material;

    [SerializeField]
    Mesh mesh;

    static readonly int
        positionsId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time");
    private void OnEnable()
    {
        positionComputeBuffer = new ComputeBuffer(resolution*resolution,3*4);
    }

    private void OnDisable()
    {
        positionComputeBuffer.Release();
        positionComputeBuffer = null;
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

        UpdateFunctionOnGPU();
    }
    
    void PickNextFunction () {
        function = transitionMode == TransitionMode.Cycle ?
            FunctionLibrary3D.GetNextFunctionName(function) :
            FunctionLibrary3D.GetRandomFunctionNameOtherThan(function);
    }

    void UpdateFunctionOnGPU () {
        
        float step = 2f / resolution;
        positionComputeShader.SetInt(resolutionId, resolution);
        positionComputeShader.SetFloat(stepId, step);
        positionComputeShader.SetFloat(timeId, Time.time);
        positionComputeShader.SetBuffer(0, positionsId, positionComputeBuffer);
        int groups = Mathf.CeilToInt(resolution / 8f);
        positionComputeShader.Dispatch(0, groups, groups, 1);
        
        
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        material.SetBuffer(positionsId, positionComputeBuffer);
        material.SetFloat(stepId, step);
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material,bounds,positionComputeBuffer.count);
    }
}
