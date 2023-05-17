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
        public NativeArray<float3> positions;
        
        [WriteOnly]
        public NativeArray<uint> hashes;
        
        // public int resolution;
        //
        // public float invResolution;
        
        public SmallXXHash hash;
        
        public float3x4 domainTRS;
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

            float3 p = mul(domainTRS, float4(positions[i], 1f));

            int u = (int)floor(p.x);
            int v = (int)floor(p.y);
            int w = (int)floor(p.z);

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

    NativeArray<uint> hashes;

    ComputeBuffer hashesBuffer, positionsBuffer, normalsBuffer;

    MaterialPropertyBlock propertyBlock;
    
    NativeArray<float3> positions, normals;
    bool isDirty;
    
    Bounds bounds;
    void OnEnable () {
        isDirty = true;
        int length = resolution * resolution;
        hashes = new NativeArray<uint>(length, Allocator.Persistent);
        positions = new NativeArray<float3>(length, Allocator.Persistent);
        hashesBuffer = new ComputeBuffer(length, 4);
        positionsBuffer = new ComputeBuffer(length, 3 * 4);
        normals = new NativeArray<float3>(length, Allocator.Persistent);
        normalsBuffer = new ComputeBuffer(length, 3 * 4);
        
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

            hashesBuffer.SetData(hashes);
            positionsBuffer.SetData(positions);
            normalsBuffer.SetData(normals);
        }

        Graphics.DrawMeshInstancedProcedural(
            instanceMesh, 0, material, bounds,
            hashes.Length, propertyBlock
        );
    }
}
