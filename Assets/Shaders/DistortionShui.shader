Shader "Mini/DistortionShui"
{
	Properties
	{
		_Brightness ("Brightness", Float ) = 1
        _Maintex ("Maintex", 2D) = "white" {}
        _Uspeed ("Uspeed", Float ) = 0
        _Vspeed ("Vspeed", Float ) = 0
        _Mask ("Mask", 2D) = "white" {}
        _MUspeed ("MUspeed", Float ) = 0
        _MVspeed ("MVspeed", Float ) = 0
        _Alpha ("Alpha", Float ) = 1
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
            uniform sampler2D _Maintex; uniform fixed4 _Maintex_ST;
            uniform sampler2D _Mask; uniform fixed4 _Mask_ST;
            uniform half _Vspeed;
            uniform half _Uspeed;
            uniform half _MVspeed;
            uniform half _MUspeed;
            uniform half _Brightness;
            uniform half _Alpha;

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
                fixed2 maskUV = fixed2(_Time.g*_MUspeed,_Time.g*_MVspeed)+i.uv0;
                fixed4 MaskVar = tex2D(_Mask,TRANSFORM_TEX(maskUV, _Mask));
                clip(step(MaskVar.r,i.vertexColor.a) - 0.5);

                fixed2 mainUV = fixed2(_Time.g*_Uspeed,_Time.g*_Vspeed)+i.uv0;
                fixed4 mainTexVar = tex2D(_Maintex,TRANSFORM_TEX(mainUV, _Maintex));
                fixed3 emissive = i.vertexColor.rgb*mainTexVar.rgb*_Brightness;

                return fixed4(emissive, mainTexVar.a*_Alpha);
            }
            ENDCG
		}
		
	}
	Fallback "Diffuse"
}
