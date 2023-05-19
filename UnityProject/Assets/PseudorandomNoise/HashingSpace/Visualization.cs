using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
public abstract class Visualization : MonoBehaviour
{

    static int
        positionsId = Shader.PropertyToID("_Positions"),
        normalsId = Shader.PropertyToID("_Normals"),
        configId = Shader.PropertyToID("_Config");

    [SerializeField]
    Mesh instanceMesh;

    [SerializeField]
    Material material;

    [SerializeField, Range(1, 256)]
    int resolution = 16;
    
    // [SerializeField, Range(-2f, 2f)]
    // float verticalOffset = 1f;
    
    [SerializeField, Range(-0.5f, 0.5f)]
    float displacement = 0.1f;
    

    ComputeBuffer  positionsBuffer, normalsBuffer;

    MaterialPropertyBlock propertyBlock;
    
    NativeArray<float3x4> positions, normals;
    bool isDirty;
    
    Bounds bounds;

    public enum Shape { Plane, Sphere, Torus,OctahedronSphere }

    
    [SerializeField]
    Shape shape;
    
    static Shapes.ScheduleDelegate[] shapeJobs = {
        Shapes.Job<Shapes.Plane>.ScheduleParallel,
        Shapes.Job<Shapes.Sphere>.ScheduleParallel,
        Shapes.Job<Shapes.Torus>.ScheduleParallel,
        Shapes.Job<Shapes.OctahedronSphere>.ScheduleParallel
    };
    
    [SerializeField, Range(0.1f, 10f)]
    float instanceScale = 2f;
    
    protected abstract void EnableVisualization (
        int dataLength, MaterialPropertyBlock propertyBlock
    );

    protected abstract void DisableVisualization ();

    protected abstract void UpdateVisualization (
        NativeArray<float3x4> positions, int resolution, JobHandle handle
    );
    void OnEnable () {
        isDirty = true;
        int length = resolution * resolution;
        length = length / 4 + (length & 1);
        positions = new NativeArray<float3x4>(length, Allocator.Persistent);
        positionsBuffer = new ComputeBuffer(length* 4, 3 * 4);
        normals = new NativeArray<float3x4>(length, Allocator.Persistent);
        normalsBuffer = new ComputeBuffer(length* 4, 3 * 4);
        
        propertyBlock ??= new MaterialPropertyBlock();
        propertyBlock.SetVector(configId, new Vector4(resolution, instanceScale / resolution,displacement));
        propertyBlock.SetBuffer(positionsId, positionsBuffer);
        propertyBlock.SetBuffer(normalsId, normalsBuffer);
        
        EnableVisualization(length, propertyBlock);
    }
    
    void OnDisable () {
        positions.Dispose();
        normals.Dispose();
        positionsBuffer.Release();
        normalsBuffer.Release();
        positionsBuffer = null;
        normalsBuffer = null;
        DisableVisualization();
    }

    void OnValidate () {
        if (positionsBuffer != null && enabled) {
            OnDisable();
            OnEnable();
        }
    }
    
    void Update () {
        if (isDirty || transform.hasChanged) {
            isDirty = false;
            transform.hasChanged = false;
            bounds = new Bounds(
                transform.position,
                float3(2f * cmax(abs(transform.lossyScale)) + displacement)
            );
            
            UpdateVisualization(
                positions, resolution,
                shapeJobs[(int)shape](
                    positions, normals, resolution, transform.localToWorldMatrix, default
                )
            );

            positionsBuffer.SetData(positions.Reinterpret<float3>(3 * 4 * 4));
            normalsBuffer.SetData(normals.Reinterpret<float3>(3 * 4 * 4));
        }

        Graphics.DrawMeshInstancedProcedural(
            instanceMesh, 0, material, bounds,
            resolution * resolution, propertyBlock
        );
    }
}
