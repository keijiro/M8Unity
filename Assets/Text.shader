Shader "Unlit/Text"
{
    Properties
    {
        _MainTex("", 2D) = "white" {}
    }

    CGINCLUDE

#include "UnityCG.cginc"

sampler2D _MainTex;
uint _Character;
float2 _Position;
float4 _Background;
float4 _Foreground;

void Vertex(float4 position : POSITION,
            float2 uv : TEXCOORD0,
            out float4 outPosition : SV_Position,
            out float2 outUV : TEXCOORD0)
{
    float2 p = (_Position + 8.0 * uv) / _ScreenParams;
    float2 t = float2(_Character & 0xf, (_Character >> 4));
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
        Cull Off
        ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDCG
        }
    }
}
