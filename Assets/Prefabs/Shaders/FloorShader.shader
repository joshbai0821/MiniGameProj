Shader "Unlit/FloorShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Channel ("Mask Channel", Color) = (1,0,0,1)
		_LineColor("Line Color", Color) = (1,1,1,1)
		_MainColor("Background Color", Color) = (1,1,1,1)
		_EmissColor("Emissive Color", Color) = (1,0,0,1)
		[HideInInspector]
		_EmissPower("Emissive Power", Range(0,100)) = 0 
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Tags{"LightMode" = "ForwardBase"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 pos : SV_POSITION;
				SHADOW_COORDS(2)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Channel, _LineColor, _EmissColor, _MainColor;
			fixed _EmissPower;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.pos);
				TRANSFER_SHADOW(o);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 mask = tex2D(_MainTex, i.uv) * _Channel;
				fixed maskChannel = max(max(mask.x, mask.y), mask.z);
				fixed4 col = lerp(_LineColor, _MainColor, maskChannel);
				// emiss 可以加个开关 开的时候一直闪烁
				col += _EmissColor * _EmissPower * mask.y;
				// fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				// shadow
				fixed shadow = SHADOW_ATTENUATION(i);

				return col * shadow;
			}
			ENDCG
		}
	}
	FallBack "Specular"
}
