Shader "Mini/DistortionFog"
{
	Properties
	{
		_Brightness ("Brightness", Float ) = 1
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Maintex ("Maintex", 2D) = "white" {}
        _U ("U", Float ) = 0
        _V ("V", Float ) = 0
        _diss_map ("diss_map", 2D) = "white" {}
        _UDiss ("UDiss", Float ) = 1
        _VDiss ("VDiss", Float ) = 1
        _Mask ("Mask", 2D) = "white" {}
        _UStep ("UStep", Float ) = 0
        _VStep ("VStep", Float ) = 0
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
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
            Blend SrcAlpha OneMinusSrcAlpha
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
            uniform fixed4 _Color;
            uniform sampler2D _Maintex; uniform fixed4 _Maintex_ST;
            uniform sampler2D _diss_map; uniform fixed4 _diss_map_ST;
            uniform sampler2D _Mask; uniform fixed4 _Mask_ST;
            uniform half _UDiss;
            uniform half _VDiss;
            uniform half _UStep;
            uniform half _VStep;
            uniform half _Brightness;
            uniform half _U;
            uniform half _V;
            struct VertexInput {
                fixed4 vertex : POSITION;
                fixed2 texcoord0 : TEXCOORD0;
                fixed4 vertexColor : COLOR;
            };
            struct VertexOutput {
                fixed4 pos : SV_POSITION;
                fixed2 uv0 : TEXCOORD0;
                fixed4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            fixed4 frag(VertexOutput i, half facing : VFACE) : COLOR {
                half isFrontFace = ( facing >= 0 ? 1 : 0 );
                half faceSign = ( facing >= 0 ? 1 : -1 );

                fixed2 dissMapUV = i.uv0+fixed2(_UStep*_Time.g,_Time.g*_VStep);
                fixed4 dissMapVar = tex2D(_diss_map,TRANSFORM_TEX(dissMapUV, _diss_map));
                fixed2 mainUV = fixed2(_U*_Time.g,_Time.g*_V)+fixed2(_UDiss*dissMapVar.r,dissMapVar.g*_VDiss)+i.uv0;
                fixed4 mainTexVar = tex2D(_Maintex,TRANSFORM_TEX(mainUV, _Maintex));
                fixed2 maskUV = i.uv0+fixed2(_UStep*_Time.g,_Time.g*_VStep);
                fixed4 maskVar = tex2D(_Mask,TRANSFORM_TEX(maskUV, _Mask));
                fixed3 emissive = _Color.rgb*mainTexVar.rgb*_Brightness*maskVar.r*i.vertexColor.rgb;

                return fixed4(emissive,_Color.a*mainTexVar.a*maskVar.a*i.vertexColor.a);
            }
            ENDCG
		}
	}
}
