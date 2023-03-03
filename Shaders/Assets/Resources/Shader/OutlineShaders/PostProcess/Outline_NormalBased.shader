Shader "Kadz/Outline/PostProcess/NormalBased"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}        
        _Intensity("Intensity", Range(0, 1)) = 1 
        _OutlineSize("Outline Size", Range(1, 10)) = 1
        _InvertStrength("Invert Strength", Range(0, 1)) = 0
        _OutlineThreshold("Outline Threshold", Range(-1, 5)) = 0.8
        _OutlineColour("Outline Colour", Color) = (1,0,0,1)
    }
    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _CameraDepthNormalsTexture;

            float _Intensity;
            float _OutlineSize;
            float4 _OutlineColour;
            float _OutlineThreshold;
            float _InvertStrength;

            float4 disregardDepth(float4 input) {
                float3 normal;
                float depth;
                DecodeDepthNormal(input, depth, normal);
                return float4(normal, depth);
            }

            float4 sampleSorbel(sampler2D sampleFrom, float2 position) {
                float2 positionT = position * _ScreenParams.xy;

                float4 sample0 = disregardDepth(tex2D(sampleFrom, (positionT + float2(-_OutlineSize, _OutlineSize)) / _ScreenParams.xy));
                float4 sample1 = disregardDepth(tex2D(sampleFrom, (positionT + float2(0, _OutlineSize)) / _ScreenParams.xy));
                float4 sample2 = disregardDepth(tex2D(sampleFrom, (positionT + float2(_OutlineSize, _OutlineSize)) / _ScreenParams.xy));
                float4 sample3 = disregardDepth(tex2D(sampleFrom, (positionT + float2(-_OutlineSize, 0)) / _ScreenParams.xy));
                float4 sample4 = disregardDepth(tex2D(sampleFrom, (positionT + float2(_OutlineSize, 0)) / _ScreenParams.xy));
                float4 sample5 = disregardDepth(tex2D(sampleFrom, (positionT + float2(-_OutlineSize, -_OutlineSize)) / _ScreenParams.xy));
                float4 sample6 = disregardDepth(tex2D(sampleFrom, (positionT + float2(0, -_OutlineSize)) / _ScreenParams.xy));
                float4 sample7 = disregardDepth(tex2D(sampleFrom, (positionT + float2(_OutlineSize, -_OutlineSize)) / _ScreenParams.xy));

                float4 horizontal = float4(0, 0, 0, 0);
                float4 vertical = float4(0, 0, 0, 0);

                horizontal += sample0; // top left (factor +1)
                horizontal += sample2 * -1; // top right (factor -1)
                horizontal += sample3 * 2; // center left (factor +2)
                horizontal += sample4 * -2; // center right (factor -2)
                horizontal += sample5; // bottom left (factor +1)
                horizontal += sample7 * -1; // bottom right (factor -1)

                vertical += sample0; // top left (factor +1)
                vertical += sample1 * 2; // top center (factor +2)
                vertical += sample2; // top right (factor +1)
                vertical += sample5 * -1; // bottom left (factor -1)
                vertical += sample6 * -2; // bottom center (factor -2)
                vertical += sample7 * -1; // bottom right (factor -1)

                float iStrength = (1 + -(2 * _InvertStrength));

                return sqrt(dot(horizontal, horizontal) + dot(vertical, vertical)) * float4(iStrength, iStrength, iStrength, 1); 
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float4 depthnormal = tex2D(_CameraDepthNormalsTexture, i.uv);
                float3 normal;
                float depth;
                DecodeDepthNormal(depthnormal, depth, normal);
                //float linear_depth = Linear01Depth(depth);
                fixed4 depthnormal_sample = sampleSorbel(_CameraDepthNormalsTexture, i.uv);

                return col + (clamp(0, _Intensity, step(_OutlineThreshold, depthnormal_sample.a) * depthnormal_sample) * _OutlineColour);

                //return col + (clamp(0, _Intensity, step(_OutlineThreshold, depth_sample.a) * depth_sample) * _OutlineColour);
            }
            ENDCG
        }
    }
}
