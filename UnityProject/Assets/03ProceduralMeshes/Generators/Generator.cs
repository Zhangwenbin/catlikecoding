﻿using ProceduralMeshes.Streams;
using Unity.Mathematics;
using static Unity.Mathematics.math;
namespace ProceduralMeshes.Generators
{
    public interface IMeshGenerator {

        void Execute<S> (int i, S streams) where S : struct, IMeshStreams;
        
        int VertexCount { get; }
		
        int IndexCount { get; }
        
        int JobLength { get; }
    }

    public struct SquareGrid : IMeshGenerator
    {
        public int VertexCount => 4;

        public int IndexCount => 6;

        public int JobLength => 1;

        public void Execute<S>(int z, S streams) where S : struct, IMeshStreams
        {
            var vertex = new Vertex();
            vertex.normal.z = -1f;
            vertex.tangent.xw = float2(1f, -1f);
            streams.SetVertex(0, vertex);
            
            vertex.position = right();
            vertex.texCoord0 = float2(1f, 0f);
            streams.SetVertex(1, vertex);

            vertex.position = up();
            vertex.texCoord0 = float2(0f, 1f);
            streams.SetVertex(2, vertex);

            vertex.position = float3(1f, 1f, 0f);
            vertex.texCoord0 = 1f;
            streams.SetVertex(3, vertex);
            
            streams.SetTriangle(0, int3(0, 2, 1));
            streams.SetTriangle(1, int3(1, 2, 3));
        }
    }
}