using Unity.Mathematics;
using static Unity.Mathematics.math;

public static partial class Noise
{
    public struct LatticeTiling : ILattice
    {
        public  LatticeSpan4 GetLatticeSpan4(float4 coordinates, int frequency)
        {
            coordinates *= frequency;
            float4 points = floor(coordinates);
            LatticeSpan4 span;
            span.p0 = (int4)points;
           // span.p1 = span.p0 + 1;
            span.g0 = coordinates - span.p0;
            span.g1 = span.g0 - 1f;
            
            span.p0 -= (int4)ceil(points / frequency) * frequency;
            span.p0 = select(span.p0, span.p0 + frequency, span.p0 < 0);
            span.p1 = span.p0 + 1;
            span.p1 = select(span.p1, 0, span.p1 == frequency);
            
            span.t = coordinates - points;
            //  span.t = smoothstep(0f, 1f, span.t);//3tt-2ttt
            span.t = span.t * span.t * span.t * (span.t * (span.t * 6f - 15f) + 10f); //6ttttt-15tttt+10ttt
            return span;
        }
        public int4 ValidateSingleStep (int4 points, int frequency) =>
            select(select(points, 0, points == frequency), frequency - 1, points == -1);
    }
}