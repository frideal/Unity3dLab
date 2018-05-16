Shader "FTP_Shaders/Inside/Inside-Unlit-AlphaMask"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_AlphaMask ("Alpha Mask", 2D) = "white" {}
		_AlphaCutOff("Cut Off", Range(0,1)) = 0.1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		// Cull Front
		Cull Front

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

			sampler2D _MainTex;
			sampler2D _AlphaMask;
			float4 _MainTex_ST;
			fixed _AlphaCutOff;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, fixed2(1-i.uv.x, i.uv.y));
				fixed4 alphaMaskCol = tex2D(_AlphaMask, fixed2(1 - i.uv.x, i.uv.y));
				//if (alphaMaskCol.a > 0.1)
				//	discard;
				clip(alphaMaskCol.a - _AlphaCutOff);
				return col;
			}
			ENDCG
		}
	}
}
