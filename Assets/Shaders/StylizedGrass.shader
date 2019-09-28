Shader "Mini/StylizedGrass"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _TimeScale("TimeScale",Float) = 1
		_WaveParams("Wave Params", Vector) = (0.05, 5, 0.8, 1)
		_fogColor("高度雾-颜色", Color) = (1,1,1,0)
		_FogYStartPos("高度雾-起始位置",Float) = 0
		_fogHeight("高度雾-高度",Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "DisableBatching"="True" }
		LOD 100
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_fog
			#pragma multi_compile_instancing
			
			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				half4 wpos: TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _WaveParams;
			float4 _fogColor;
			float _FogYStartPos;
			float _fogHeight;
			
			float2 WaveGrass (inout float4 vertex)
			{
				float4 _waveXSize = float4(0.012, 0.02, 0.06, 0.024) * _WaveParams.y;
				float4 _waveZSize = float4 (0.006, .02, 0.02, 0.05) * _WaveParams.y;
				float4 waveSpeed = float4 (0.3, .5, .4, 1.2) * 4;

				float4 _waveXmove = float4(0.012, 0.02, -0.06, 0.048) * 2;
				float4 _waveZmove = float4 (0.006, .02, -0.02, 0.1);

				float4 waves;
				waves = vertex.x * _waveXSize;
				waves += vertex.z * _waveZSize;

				// Add in time to model them over time
				waves += _WaveParams.x * _Time.y * waveSpeed;

				float4 s, c;
				waves = frac (waves);
				FastSinCos (waves, s,c);

				s = s * s;
				s = s * s;
				s = s * _WaveParams.w;

				float3 waveMove = float3 (0,0,0);
				waveMove.x = dot (s, _waveXmove);
				waveMove.z = dot (s, _waveZmove);

				return waveMove.xz * _WaveParams.z;
			}

			v2f vert (appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f,o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				//v.vertex.xz += UNITY_ACCESS_INSTANCED_PROP(_Skew).xz * v.vertex.y;
				//v.vertex.y *= UNITY_ACCESS_INSTANCED_PROP(_Skew).y;

				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				worldPos.xz -= WaveGrass(worldPos) * v.vertex.y;

				o.vertex = mul(UNITY_MATRIX_VP, worldPos);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.wpos = mul( unity_ObjectToWorld, half4(v.vertex.xyz,1) );
				UNITY_TRANSFER_FOG(o,o.vertex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				fixed4 col = tex2D(_MainTex, i.uv);
				half fogDensity = clamp((i.wpos.y - _FogYStartPos)/_fogHeight,0,1);
				col.xyz = lerp ( _fogColor, col.xyz, fogDensity);
				UNITY_APPLY_FOG(i.fogCoord,col); 
				return col;
			}
			ENDCG
		}
	}
}
