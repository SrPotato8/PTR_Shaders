Shader "Custom/SurfaceRadar"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.03, 0.06, 0.08, 1)
        _GridColor ("Grid Color", Color) = (0.05, 0.25, 0.30, 1)
        _GridScale ("Grid Scale", Float) = 1.5

        _RadarOrigin ("Radar Origin (world)", Vector) = (0,0,0,0)
        _RadarRadius ("Radar Radius", Float) = 25
        _SweepWidth ("Sweep Width", Float) = 1.0
        _SweepSharpness ("Sweep Sharpness", Float) = 4.0
        _SweepColor ("Sweep Color", Color) = (0.2, 1.0, 0.8, 1)

        _TimeScale ("Time Scale", Float) = 1.0
        _SweepSpeed ("Sweep Speed (units/sec)", Float) = 6.0

        _NoiseTex ("Noise (R)", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 0.15
        _NoiseStrength ("Noise Strength", Range(0,1)) = 0.25
        _NoiseSpeed ("Noise Speed", Float) = 0.8

        _TrailLength ("Trail Length", Float) = 3.0
        _TrailStrength ("Trail Strength", Range(0,3)) = 1.0

        _RimDarken ("Rim Darken", Range(0,1)) = 0.25
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _GridColor;
                float  _GridScale;

                float4 _RadarOrigin;
                float  _RadarRadius;
                float  _SweepWidth;
                float  _SweepSharpness;
                float4 _SweepColor;

                float  _TimeScale;
                float  _SweepSpeed;

                float4 _NoiseTex_ST;
                float  _NoiseScale;
                float  _NoiseStrength;
                float  _NoiseSpeed;

                float  _TrailLength;
                float  _TrailStrength;

                float  _RimDarken;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vpi = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vpi.positionCS;
                OUT.positionWS  = vpi.positionWS;
                return OUT;
            }

            float grid(float2 p, float scale)
            {
                float2 uv = p * scale;
                float2 fw = fwidth(uv);

                float2 cell = abs(frac(uv) - 0.5) / fw;

                float m = cell.x;
                if (cell.y < m)
                    m = cell.y;

                return 1.0 - saturate(m);
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float2 xz = IN.positionWS.xz;

                float g = grid(xz, _GridScale);
                float3 col = lerp(_BaseColor.rgb, _GridColor.rgb, g * 0.6);

                // Distance from origin
                float2 o = _RadarOrigin.xz;
                float dist = distance(xz, o);

                float radius = max(_RadarRadius, 0.0001);

                // Time + sweep
                float t = _Time.y * _TimeScale;
                float sweep = fmod(t * _SweepSpeed, radius);

                // Animated noise
                float2 nUV = xz * _NoiseScale + float2(t * _NoiseSpeed, -t * _NoiseSpeed * 0.7);
                float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, nUV).r;
                noise = (noise - 0.5) * 2.0; 

                // Perturb the band slightly
                float distNoisy = dist + noise * (_NoiseStrength * _SweepWidth);

                // Main band
                float width = max(_SweepWidth, 0.0001);
                float band = 1.0 - saturate(abs(distNoisy - sweep) / width);
                band = pow(band, _SweepSharpness);

                // Trail behind sweep
                float behind = sweep - distNoisy;
                float trail = 0.0;
                if (behind > 0.0)
                {
                    trail = 1.0 - saturate(behind / max(_TrailLength, 0.0001));
                    trail = trail * trail; 
                }

                // Radial fade and rim darken
                float radial01 = saturate(dist / radius);
                float radialFade = 1.0 - radial01;             // brighter near center
                float rim = smoothstep(0.7, 1.0, radial01);     
                col *= (1.0 - rim * _RimDarken);

                // Combine sweep and trail
                float3 sweepCol = _SweepColor.rgb * (band + trail * _TrailStrength);

                col += sweepCol * radialFade;

                return half4(col, 1);
            }
            ENDHLSL
        }
    }
}
