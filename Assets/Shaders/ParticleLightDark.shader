Shader "Custom/ParticleLightDark" {
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
      Blend One One // use alpha blending
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
          if (Distance - _SpotlightRadius < 0.0f)
          {
          	o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;          	
          	o.Albedo.b = 0.0f;
          }
          else
          {
          	o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
          	o.Albedo.g = 0.0f;          	
          }	
          o.Alpha = tex2D (_MainTex, IN.uv_MainTex).a / 32.0f;          
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }
