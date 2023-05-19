
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
    StructuredBuffer<float> _Noise;
    StructuredBuffer<float3> _Positions, _Normals;
#endif

float4 _Config;

void ConfigureProcedural () {
    #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
   // float v = floor(_Config.y * unity_InstanceID);
   // float v = floor(_Config.y * unity_InstanceID + 0.00001);
   // float u = unity_InstanceID - _Config.x * v;
		
    unity_ObjectToWorld = 0.0;
    unity_ObjectToWorld._m03_m13_m23_m33 = float4(
        // _Config.y * (u + 0.5) - 0.5,
        // _Config.z * ((1.0 / 255.0) * (_Hashes[unity_InstanceID] >> 24) - 0.5),
        // _Config.y * (v + 0.5) - 0.5,
        _Positions[unity_InstanceID],
        1.0
    );
    // unity_ObjectToWorld._m13 +=
    // _Config.z * ((1.0 / 255.0) * (_Hashes[unity_InstanceID] >> 24) - 0.5);
    unity_ObjectToWorld._m03_m13_m23 +=
         _Config.z * _Noise[unity_InstanceID] * _Normals[unity_InstanceID];
    unity_ObjectToWorld._m00_m11_m22 = _Config.y;
    #endif
}

float3 GetNoiseColor () {
    #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
    float noise = _Noise[unity_InstanceID];
    return noise < 0.0 ? float3(-noise, 0.0, 0.0) : noise;
    #else
    return 1.0;
    #endif
}

void ShaderGraphFunction_float (float3 In, out float3 Out, out float3 Color) {
    Out = In;
    Color = GetNoiseColor();
}

void ShaderGraphFunction_half (half3 In, out half3 Out, out half3 Color) {
    Out = In;
    Color = GetNoiseColor();
}
