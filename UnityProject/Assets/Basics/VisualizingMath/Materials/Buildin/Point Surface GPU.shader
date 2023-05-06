Shader "Custom/Point Surface GPU"
{
    Properties
    {
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows addshadow
        #pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
        #pragma target 4.5
        
        struct Input
        {
             float3 worldPos;
        };

        half _Glossiness;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

       	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
			StructuredBuffer<float3> _Positions;
		#endif
        	float _Step;
        void ConfigureProcedural ()
        {
            #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
			float3 position = _Positions[unity_InstanceID];
            	unity_ObjectToWorld = 0.0;
				unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0);
				unity_ObjectToWorld._m00_m11_m22 = _Step;
		    #endif
        }
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo.rgb = IN.worldPos.xyz * 0.5 + 0.5;
            o.Smoothness = _Glossiness;
 
        }
        ENDCG
    }
    FallBack "Diffuse"
}
