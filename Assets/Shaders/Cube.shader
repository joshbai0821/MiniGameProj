Shader "Mini/Cube" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MaskTex ("Mask", 2D) = "white" {}
		_Channel ("Mask Channel", Color) = (1,0,0,1)
		_LineColor("Line Color", Color) = (1,1,1,1)
		_EmissColor("Emissive Color", Color) = (0,0,0,1)
		_EmissPower("Emissive Power", Range(0,100)) = 5 
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert noforwardadd nolightmap noshadow novertexlights nodynlightmap nodirlightmap 

		sampler2D _MainTex; 
		sampler2D _MaskTex;
		fixed4 _Channel, _Color, _LineColor, _EmissColor;
		fixed _EmissPower;


		struct Input {
			float2 uv_MainTex;
			float2 uv_MaskTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			fixed4 mask = tex2D(_MaskTex, IN.uv_MaskTex) * _Channel;
			fixed maskChannel = max(max(mask.x, mask.y), mask.z);
			fixed3 finalColor = lerp(_LineColor.rgb, c.rgb, maskChannel);
			finalColor += _EmissPower * _EmissColor  * abs(frac(0.5 * _Time.y)-0.5) * (1 - mask.a);
			o.Albedo = finalColor;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
