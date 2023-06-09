using ProceduralMeshes.Streams;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
namespace ProceduralMeshes.Generators
{
    public interface IMeshGenerator {

        void Execute<S> (int i, S streams) where S : struct, IMeshStreams;
        
        int VertexCount { get; }
		
        int IndexCount { get; }
        
        int JobLength { get; }
        
        Bounds Bounds { get; }
        
        int Resolution { get; set; }
    }
}