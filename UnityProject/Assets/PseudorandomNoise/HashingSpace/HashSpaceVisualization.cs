using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
public class HashSpaceVisualization : Visualization 
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
           float4x3 p = domainTRS.TransformVectors(transpose(positions[i]));

            int4 u = (int4)floor(p.c0);
            int4 v = (int4)floor(p.c1);
            int4 w = (int4)floor(p.c2);

            hashes[i] = hash.Eat(u).Eat(v).Eat(w);
        }
    }

    private static int
        hashesId = Shader.PropertyToID("_Hashes");
    
    
    [SerializeField]
    int seed;
    
    
    [SerializeField]
    SpaceTRS domain = new SpaceTRS {
        scale = 8f
    };

    NativeArray<uint4> hashes;

    ComputeBuffer hashesBuffer;
    

    protected override  void EnableVisualization (int dataLength, MaterialPropertyBlock propertyBlock) {
        //…
        hashes = new NativeArray<uint4>(dataLength, Allocator.Persistent);
        //positions = new NativeArray<float3x4>(length, Allocator.Persistent);
        //normals = new NativeArray<float3x4>(length, Allocator.Persistent);
        hashesBuffer = new ComputeBuffer(dataLength * 4,4);
        //positionsBuffer = new ComputeBuffer(length * 4, 3 * 4);
        //normalsBuffer = new ComputeBuffer(length * 4, 3 * 4);

        //propertyBlock ??= new MaterialPropertyBlock();
        propertyBlock.SetBuffer(hashesId, hashesBuffer);
        //…
    }
    
    protected override void DisableVisualization () {
        hashes.Dispose();
        //positions.Dispose();
        //normals.Dispose();
        hashesBuffer.Release();
        //positionsBuffer.Release();
        //normalsBuffer.Release();
        hashesBuffer = null;
        //positionsBuffer = null;
        //normalsBuffer = null;
    }
    
    protected override void UpdateVisualization (
        NativeArray<float3x4> positions, int resolution, JobHandle handle
    ) {
        //…
        new HashJob {
            positions = positions,
            hashes = hashes,
            hash = SmallXXHash.Seed(seed),
            domainTRS = domain.Matrix
        }.ScheduleParallel(hashes.Length, resolution, handle).Complete();

        hashesBuffer.SetData(hashes.Reinterpret<uint>(4 * 4));
        //…
    }
}
