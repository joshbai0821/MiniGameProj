Shader "Mini/StylizedWater"
{
	Properties {
        _BaseColor("BaseColor", Color) = (0.3970588,0.9002028,1,1)
        _FresnelColor ("FresnelColor", Color) = (0.7205882,0.907505,1,1)
        _FresnelExp ("FresnelExp", Range(0, 10)) = 0.3523709
        _MainFoamColor ("MainFoamColor", Color) = (0.4779412,0.8055779,1,1)
        _MainFoamIntensity ("MainFoamIntensity", Float ) = 1
        _MainFoamScale ("MainFoamScale", Float ) = 1
        _SecondaryFoamColor ("SecondaryFoamColor", Color) = (1,1,1,1)
        _SecondaryFoamIntensity ("SecondaryFoamIntensity", Float ) = 1
        _SecondaryFoamScale ("SecondaryFoamScale", Float ) = 1
        _WaterTexture ("WaterTexture", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "DisableBatching"="True"
        }
        Pass {
            Name "FORWARD"
            Tags {
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal
            #pragma target 2.0

            uniform sampler2D _WaterTexture;
            uniform half _MainFoamIntensity, _MainFoamScale;
            uniform half _SecondaryFoamIntensity, _SecondaryFoamScale;
            uniform float4 _FresnelColor;
            uniform float4 _MainFoamColor;
            uniform float4 _SecondFoamColor;
            uniform half _FresnelExp;
            uniform float4 _BaseColor;

            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);

                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = normalize(i.normalDir);

                float2 waterTexUV1 = i.uv0*_MainFoamScale+_Time.g*float2(0.02,0.02);
                float4 waterTexVar1 = tex2D(_WaterTexture,waterTexUV1);
                float2 waterTexUV2 = i.uv0*_SecondaryFoamScale+_Time.g*float2(-0.01,-0.01);
                float4 waterTexVar2 = tex2D(_WaterTexture,waterTexUV2);

                float3 emissive = lerp(_BaseColor, _SecondFoamColor, _SecondaryFoamIntensity * waterTexVar2.g);
                emissive = lerp(emissive, _MainFoamColor, _MainFoamIntensity * waterTexVar1.r);
                emissive = lerp(emissive, _FresnelColor, pow(1.0 - max(0.0, dot(normalDirection, viewDirection)), _FresnelExp));
                float4 finalRGBA = float4(emissive, 1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;

            }
            ENDCG
        }
    }
    FallBack "Mobile/Diffuse"
}
