//
// Shader: "FX/Force Field"
// Version: v1.0
// Written by: Thomas Phillips
//
// Anyone is free to use this shader for non-commercial or commercial projects.
//

 
Shader "FX/Force Field" {
   
Properties {
   _Color ("Color1", Color) = (1,1,1,1)
   _Color2 ("Color2", Color) = (1,1,1,1)
   _Alpha("Alpha", Range (0.0, 1.0))=0.15
   _Rate ("Oscillation Rate", Range (5, 200)) = 50
   _Scale ("Scale", Range (0.02, 0.5)) = 0.25
   _Distortion ("Distortion", Range (0.1, 20)) = 1
}
 
SubShader {
   
   ZWrite Off
   Tags { "Queue" = "Transparent" }
   Blend One One
   //Blend SrcAlpha OneMinusSrcAlph
   
 
   Pass {
 
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
 
float4 _Color;
float4 _Color2;
float _Alpha;
float _Rate;
float _Scale;
float _Distortion;
 
struct v2f {
   float4 pos : SV_POSITION;
   float3 uv : TEXCOORD0;
   float3 vert : TEXCOORD1;
};
 
v2f vert (appdata_base v)
{
    v2f o;
    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
   
    float s = 1 / _Scale;
    float t = _Time[0]*_Rate*_Scale;
    o.vert = sin((v.vertex.xyz + t) * s);  
    o.uv = sin((v.texcoord.xyz + t) * s) * _Distortion;
   
    return o;
}
 
half4 frag (v2f i) : COLOR
{
    float3 vert = i.vert;
    float3 uv = i.uv;
    float mix = 1 + sin((vert.x - uv.x) + (vert.y - uv.y) + (vert.z - uv.z));
    float mix2 = 1 + sin((vert.x + uv.x) - (vert.y + uv.y) - (vert.z + uv.z)) / _Distortion;
   
    return half4(((_Color.rgb * mix * 0.3) + (_Color2.rgb * mix2 * 0.3)) * _Alpha, 1.0);
}
ENDCG
 
    }
}
Fallback "Transparent/Diffuse"
}
 
