using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
public class HashSpaceVisualization : MonoBehaviour
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    struct HashJob : IJobFor {

        [ReadOnly]
        public NativeArray<float3x4> positions;
        
        [WriteOnly]
        public NativeArray<uint4> hashes;
        
        // public int resolution;
        //
        // public float invResolution;
        
        public SmallXXHash4 hash;
        
        public float3x4 domainTRS;
        
        float4x3 TransformPositions (float3x4 trs, float4x3 p) => float4x3(
            trs.c0.x * p.c0 + trs.c1.x * p.c1 + trs.c2.x * p.c2 + trs.c3.x,
            trs.c0.y * p.c0 + trs.c1.y * p.c1 + trs.c2.y * p.c2 + trs.c3.y,
            trs.c0.z * p.c0 + trs.c1.z * p.c1 + trs.c2.z * p.c2 + trs.c3.z
        );
        public void Execute(int i) {
            //hashes[i] = (uint)i;
            
           // hashes[i] = (uint)(frac(i * 0.381f) * 256f);
           
         //  float v = floor( i/resolution + 0.00001f);
         
            // float v = floor(invResolution * i + 0.00001f);
            // float u = i - resolution * v;
            // hashes[i] = (uint)(frac(u * v * 0.381f) * 255f);
            
            // int v = (int)floor(invResolution * i + 0.00001f);
            // int u = i - resolution * v;
            
            // float vf = floor(invResolution * i + 0.00001f);
            // float uf = invResolution * (i - resolution * vf + 0.5f) - 0.5f;
            // vf = invResolution * (vf + 0.5f) - 0.5f;

           // float3 p = mul(domainTRS, float4(positions[i], 1f));
           float4x3 p = TransformPositions(domainTRS, transpose(positions[i]));

            int4 u = (int4)floor(p.c0);
            int4 v = (int4)floor(p.c1);
            int4 w = (int4)floor(p.c2);

            hashes[i] = hash.Eat(u).Eat(v).Eat(w);
        }
    }
    
    static int
        hashesId = Shader.PropertyToID("_Hashes"),
        positionsId = Shader.PropertyToID("_Positions"),
        normalsId = Shader.PropertyToID("_Normals"),
        configId = Shader.PropertyToID("_Config");

    [SerializeField]
    Mesh instanceMesh;

    [SerializeField]
    Material material;

    [SerializeField, Range(1, 512)]
    int resolution = 16;
    
    [SerializeField]
    int seed;
    
    // [SerializeField, Range(-2f, 2f)]
    // float verticalOffset = 1f;
    
    [SerializeField, Range(-0.5f, 0.5f)]
    float displacement = 0.1f;
    
    [SerializeField]
    SpaceTRS domain = new SpaceTRS {
        scale = 8f
    };

    NativeArray<uint4> hashes;

    ComputeBuffer hashesBuffer, positionsBuffer, normalsBuffer;

    MaterialPropertyBlock propertyBlock;
    
    NativeArray<float3x4> positions, normals;
    bool isDirty;
    
    Bounds bounds;
    void OnEnable () {
        isDirty = true;
        int length = resolution * resolution;
        length = length / 4 + (length & 1);
        hashes = new NativeArray<uint4>(length, Allocator.Persistent);
        positions = new NativeArray<float3x4>(length, Allocator.Persistent);
        hashesBuffer = new ComputeBuffer(length* 4, 4);
        positionsBuffer = new ComputeBuffer(length* 4, 3 * 4);
        normals = new NativeArray<float3x4>(length, Allocator.Persistent);
        normalsBuffer = new ComputeBuffer(length* 4, 3 * 4);
        
        propertyBlock ??= new MaterialPropertyBlock();
        propertyBlock.SetBuffer(hashesId, hashesBuffer);
        propertyBlock.SetVector(configId, new Vector4(resolution, 1f / resolution,displacement));
        propertyBlock.SetBuffer(positionsId, positionsBuffer);
        propertyBlock.SetBuffer(normalsId, normalsBuffer);
    }
    
    void OnDisable () {
        hashes.Dispose();
        positions.Dispose();
        normals.Dispose();
        hashesBuffer.Release();
        positionsBuffer.Release();
        normalsBuffer.Release();
        hashesBuffer = null;
        positionsBuffer = null;
        normalsBuffer = null;
    }

    void OnValidate () {
        if (hashesBuffer != null && enabled) {
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
            JobHandle handle = Shapes.Job.ScheduleParallel(
                positions,normals, resolution, transform.localToWorldMatrix, default
            );

            new HashJob {
                positions = positions,
                hashes = hashes,
                hash = SmallXXHash.Seed(seed),
                domainTRS = domain.Matrix
            }.ScheduleParallel(hashes.Length, resolution, handle).Complete();

            hashesBuffer.SetData(hashes.Reinterpret<uint>(4 * 4));
            positionsBuffer.SetData(positions.Reinterpret<float3>(3 * 4 * 4));
            normalsBuffer.SetData(normals.Reinterpret<float3>(3 * 4 * 4));
        }

        Graphics.DrawMeshInstancedProcedural(
            instanceMesh, 0, material, bounds,
            resolution * resolution, propertyBlock
        );
    }
}
