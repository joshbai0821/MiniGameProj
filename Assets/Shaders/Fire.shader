Shader "Mini/Fire"
{
	Properties
	{
		_Brightness ("Brightness", Float ) = 1
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _U_speed ("U_speed", Float ) = 0
        _V_speed ("V_speed", Float ) = 0
	}
	SubShader
	{
		Tags {  "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent" }

		Pass
		{
			Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            Cull Off
            ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define UNITY_PASS_FORWARDBASE
			#include "UnityCG.cginc"

			#pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x
            #pragma target 3.0

            uniform float4 _Color;
            uniform float _Brightness;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _U_speed;
            uniform float _V_speed;

			struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
			
			VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );

                float4 node_1603 = _Time;
                float2 node_6484 = (i.uv0+float2((_U_speed*node_1603.g),(node_1603.g*_V_speed)));
                float4 _Main_tex_var = tex2D(_MainTex,TRANSFORM_TEX(node_6484, _MainTex));
                float3 emissive = (_Brightness*_Color.rgb*i.vertexColor.rgb*_Main_tex_var.rgb*(_Main_tex_var.a*i.vertexColor.a*_Color.a));
                
				return fixed4(emissive,1);
            }
			ENDCG
		}
	}
}
