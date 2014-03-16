Shader "Custom/LightShader" {
	Properties {
      _MainTex ("Texture", 2D) = "white" {}
      //_SpotlightPos ("Spotlight Position", Vector) = (0,0,0,0)
      //_SpotlightRadius ("Spotlight Radius", Float) = 0.2
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      Cull Off
      Lighting On
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
          clip (-(Distance - _SpotlightRadius));   
          
          float3 purple = float3(0.25, 0.1, 0.4);
          float3 green = float3(0.25, 0.5, 0.1);
          float a = clamp((_SpotlightRadius - Distance) * 2.0, 0.0, 1.0);
          o.Albedo = lerp(lerp(purple, green, lerp(0.5,1.0,a)), tex2D (_MainTex, IN.uv_MainTex).rgb, a);  
               
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }
