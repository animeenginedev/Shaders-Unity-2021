Shader "Kadz/Outline/PostProcess/DepthBased"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}        
        _Intensity("Intensity", Range(0, 1)) = 1
        _OutlineSize("Outline Size", Range(1, 10)) = 1
        _InvertStrength("Invert Strength", Range(0, 1)) = 0
        _OutlineThreshold("Outline Threshold", Range(0, 1)) = 0.02
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
            sampler2D _CameraDepthTexture;

            float _Intensity;
            float _OutlineSize;
            float4 _OutlineColour;
            float _OutlineThreshold;
            float _InvertStrength;

            float4 sampleSorbel(sampler2D sampleFrom, float2 position) {
                float2 positionT = position * _ScreenParams.xy;

                float4 sample0 = tex2D(sampleFrom, (positionT + float2(-_OutlineSize, _OutlineSize)) / _ScreenParams.xy);
                float4 sample1 = tex2D(sampleFrom, (positionT + float2(0, _OutlineSize)) / _ScreenParams.xy);
                float4 sample2 = tex2D(sampleFrom, (positionT + float2(_OutlineSize, _OutlineSize)) / _ScreenParams.xy);
                float4 sample3 = tex2D(sampleFrom, (positionT + float2(-_OutlineSize, 0)) / _ScreenParams.xy);
                float4 sample4 = tex2D(sampleFrom, (positionT + float2(_OutlineSize, 0)) / _ScreenParams.xy);
                float4 sample5 = tex2D(sampleFrom, (positionT + float2(-_OutlineSize, -_OutlineSize)) / _ScreenParams.xy);
                float4 sample6 = tex2D(sampleFrom, (positionT + float2(0, -_OutlineSize)) / _ScreenParams.xy);
                float4 sample7 = tex2D(sampleFrom, (positionT + float2(_OutlineSize, -_OutlineSize)) / _ScreenParams.xy);

                sample0 = lerp(sample0, Linear01Depth(sample0), (sample0));
                sample1 = lerp(sample1, Linear01Depth(sample1), (sample1));
                sample2 = lerp(sample2, Linear01Depth(sample2), (sample2));
                sample3 = lerp(sample3, Linear01Depth(sample3), (sample3));
                sample4 = lerp(sample4, Linear01Depth(sample4), (sample4));
                sample5 = lerp(sample5, Linear01Depth(sample5), (sample5));
                sample6 = lerp(sample6, Linear01Depth(sample6), (sample6));
                sample7 = lerp(sample7, Linear01Depth(sample7), (sample7));

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
                float depth = tex2D(_CameraDepthTexture, i.uv).r;
                float linear_depth = Linear01Depth(depth);
                fixed4 depth_sample = sampleSorbel(_CameraDepthTexture, i.uv);

                return col + (clamp(0, _Intensity, step(_OutlineThreshold, depth_sample.a) * depth_sample) * _OutlineColour);
            }
            ENDCG
        }
    }
}
