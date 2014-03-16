Shader "Custom/DarkShaderTransparent" {
	Properties {
      _MainTex ("Texture", 2D) = "white" {}
      //_SpotlightPos ("Spotlight Position", Vector) = (0,0,0,0)
      //_SpotlightRadius ("Spotlight Radius", Float) = 0.2
    }
    SubShader {
      Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
      Cull Off
      Lighting On
      ZWrite Off
      Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
      CGPROGRAM
      #pragma surface surf Lambert
      struct Input {
          float2 uv_MainTex;          
          float3 worldPos;
      };
      
      uniform float4 _SpotlightPos;
      uniform float _SpotlightRadius;
      
      sampler2D _MainTex;
      sampler2D _BumpMap;
     
      void surf (Input IN, inout SurfaceOutput o) {
      	  float Distance = distance(IN.worldPos.xz, _SpotlightPos.xz);
          clip (Distance - _SpotlightRadius);
          float a = clamp((Distance - _SpotlightRadius) * 2.0, 0.0, 1.0);
          
          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
          o.Alpha = tex2D (_MainTex, IN.uv_MainTex).a * a;         
          clip (o.Alpha - 0.2f);
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }
