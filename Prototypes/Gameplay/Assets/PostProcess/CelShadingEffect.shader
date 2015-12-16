Shader "Post/CelShadingEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 depthUv = i.uv;
#if UNITY_UV_STARTS_AT_TOP
				depthUv.y = 1 - depthUv.y;
#endif

				float2 RADIUS = float2(0.0008, 0.00142);

				// gauche, droite, bas, haut
				float depthX0 = tex2D(_CameraDepthTexture, depthUv + float2(-RADIUS.x, 0.0)).r;
				float depthX1 = tex2D(_CameraDepthTexture, depthUv + float2(RADIUS.x, 0.0)).r;
				float depthY0 = tex2D(_CameraDepthTexture, depthUv + float2(0.0, -RADIUS.y)).r;
				float depthY1 = tex2D(_CameraDepthTexture, depthUv + float2(0.0, RADIUS.y)).r;

				// coins
				float depthX0Y0 = tex2D(_CameraDepthTexture, depthUv + float2(-RADIUS.x, -RADIUS.y)).r;
				float depthX1Y0 = tex2D(_CameraDepthTexture, depthUv + float2(RADIUS.x, -RADIUS.y)).r;
				float depthX0Y1 = tex2D(_CameraDepthTexture, depthUv + float2(-RADIUS.x, RADIUS.y)).r;
				float depthX1Y1 = tex2D(_CameraDepthTexture, depthUv + float2(RADIUS.x, RADIUS.y)).r;

				float sumX = abs(depthX0Y0 + 2.0 * depthX0 + depthX0Y1 - (depthX1Y0 + 2.0 * depthX1 + depthX1Y1));
				float sumY = abs(depthX0Y0 + 2.0 * depthY0 + depthX1Y0 - (depthX0Y1 + 2.0 * depthY1 + depthX1Y1));

				//if (sumX + sumY > 0.001)
				
				float grey = 0.03;	
				
				fixed3 color = lerp(tex2D(_MainTex, i.uv).rgb, fixed3(grey, grey, grey), (sumX + sumY) * 500.0);
				return fixed4(color, 1.0);
				
			}
			ENDCG
		}
	}
}
