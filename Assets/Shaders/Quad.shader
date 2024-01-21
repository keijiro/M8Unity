Shader "Unlit/Quad"
{
    Properties
    {
        _MainTex("", 2D) = "white" {}
    }

    CGINCLUDE

#include "UnityCG.cginc"

sampler2D _MainTex;
uint _Code;
float4 _Coords;
float4 _Background;
float4 _Foreground;

void Vertex(float4 position : POSITION,
            float2 uv : TEXCOORD0,
            out float4 outPosition : SV_Position,
            out float2 outUV : TEXCOORD0)
{
    float2 p = (_Coords.xy + _Coords.zw * uv) / _ScreenParams;
    float2 t = float2(_Code & 0xf, (_Code >> 4));
    t = (t + uv) / 16;
    t.y = 1 - t.y;
    outPosition = float4(p * 8 - 1, 0, 1);
    outUV = t;
}

float4 Fragment(float4 position : SV_Position,
                float2 uv : TEXCOORD0) : SV_Target
{
    float s = tex2D(_MainTex, uv).g;
    return lerp(_Background, _Foreground, s);
}

    ENDCG

    SubShader
    {
        Pass
        {
            Cull Off ZTest Always ZWrite Off
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDCG
        }
    }
}
