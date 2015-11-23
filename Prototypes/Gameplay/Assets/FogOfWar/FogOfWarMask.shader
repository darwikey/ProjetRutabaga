Shader "Custom/FogOfWarMask" {
	Properties {
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BlurPower("BlurPower", float) = 0.002
	}
	
	SubShader {
      Tags { "Queue" = "Transparent" } 
         // draw after all opaque geometry has been drawn
      Pass {
         ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects

         Blend SrcAlpha OneMinusSrcAlpha // use alpha blending

         CGPROGRAM 
 
         #pragma vertex vert 
         #pragma fragment frag
         
         fixed4 _Color;
	     sampler2D _MainTex;
		 float _BlurPower;
		 
		 // vertex input: position, UV
         struct appdata {
            float4 vertex : POSITION;
            float4 texcoord : TEXCOORD0;
         };
		 
		 struct v2f {
            float4 pos : SV_POSITION;
            float4 uv : TEXCOORD0;
         };

 
         v2f vert(appdata v)
         {
            v2f o;
            o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
            o.uv = float4( v.texcoord.xy, 0, 0 );
            return o;
         }
 
         half4 frag(v2f i) : SV_Target 
         {
         	half baseColor1 = tex2D(_MainTex, i.uv + float2(-_BlurPower, 0)).r;
			half baseColor2 = tex2D(_MainTex, i.uv + float2(0, -_BlurPower)).r;
			half baseColor3 = tex2D(_MainTex, i.uv + float2(_BlurPower, 0)).r;
			half baseColor4 = tex2D(_MainTex, i.uv + float2(0, _BlurPower)).r;
			half baseColor = 0.25 * (baseColor1 + baseColor2 + baseColor3 + baseColor4);
            return half4(_Color.rgb, 1.0 - baseColor); 
         }
 
         ENDCG  
      }
   }
}
