Shader "Custom/XRay_Effect"
{
    Properties
    {
        _Color ("XRay Color", Color) = (0,0,1,1)
        _FadeStart ("Fade Start Distance", Float) = 5
        _FadeEnd ("Fade End Distance", Float) = 50
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalRenderPipeline"
            "RenderType"="Transparent"
            "Queue"="Overlay"
        }

        Pass
        {
            Name "XRay"
            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _Color;
            float _FadeStart;
            float _FadeEnd;

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionWS  = TransformObjectToWorld(v.positionOS.xyz);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                // Distancia cámara → fragmento
                float dist = distance(i.positionWS, _WorldSpaceCameraPos);

                // Factor de fade (1 cerca, 0 lejos)
                float fade = saturate(1.0 - (dist - _FadeStart) / (_FadeEnd - _FadeStart));

                half4 col = _Color;
                col.a *= fade;

                return col;
            }
            ENDHLSL
        }
    }
}
