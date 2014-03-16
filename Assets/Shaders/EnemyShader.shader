Shader "Custom/EnemyShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		
		uniform float _DarknessAmount;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			float2 uv2 = IN.uv_MainTex;
			uv2.x += 0.5;
			half4 c = lerp(tex2D (_MainTex, IN.uv_MainTex), tex2D (_MainTex, uv2), _DarknessAmount);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
