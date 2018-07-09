Shader "Custom/Curve" {
	Properties{
		_Tint("Tint", Color) = (1, 1, 1, 1)
		_MainTex("Albedo", 2D) = "white" {}

		[NoScaleOffset] _NormalMap("Normals", 2D) = "bump" {}
		_BumpScale("Bump Scale", Float) = 1

		[NoScaleOffset] _MetallicMap("Metallic", 2D) = "white" {}
		[Gamma] _Metallic("Metallic", Range(0, 1)) = 0

		_Smoothness("Smoothness", Range(0, 1)) = 0.1

		[NoScaleOffset] _OcclusionMap("Occlusion", 2D) = "white" {}
		_OcclusionStrength("Occlusion Strength", Range(0, 1)) = 1

		[NoScaleOffset] _EmissionMap("Emission", 2D) = "black" {}
		_Emission("Emission", Color) = (0, 0, 0)
	
		[NoScaleOffset] _DetailMask("Detail Mask", 2D) = "white" {}
		_DetailTex("Detail Albedo", 2D) = "gray" {}
		[NoScaleOffset] _DetailNormalMap("Detail Normals", 2D) = "bump" {}
		_DetailBumpScale("Detail Bump Scale", Float) = 1

		_QOffset("Offset", Vector) = (0,0,0,0)
		_Dist("Distance", Float) = 100.0
	}
	SubShader {
		Tags { "RenderType"="Opaque"}
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Tint;
		sampler2D _MainTex;
		sampler2D _NormalMap;
		float _BumpScale;
		sampler2D __MetallicMap;
		half _Metallic;
		
		half _Glossiness;

		sampler2D _OcclusionMap;
		float _OcclusionStrength;
		sampler2D _EmissionMap;
		fixed4 _Emission;
		sampler2D _DetailMask;
		sampler2D _DetailTex;
		sampler2D _DetailNormalMap;
		sampler2D _DetailBumpScale;

		float4 _QOffset;
		float _Dist;

		void vert(inout appdata_full v) {
			float4 vPos = mul(unity_ObjectToWorld, v.vertex);
			//apply transformations desired
			float zOff = vPos.x / _Dist;
			vPos += _QOffset*zOff*zOff;
			v.vertex = mul(unity_WorldToObject, vPos);
			v.texcoord = v.texcoord;
		}



		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Tint;
			o.Albedo = c.rgb;
			o.Emission = tex2D(_EmissionMap, IN.uv_MainTex) * _Emission;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;

			//added
			o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex)); //basing eveything in uv_Maintext because i know that they are going to have the shame texture coords
		}
		ENDCG
	}
	CustomEditor "CurveShaderGUI"
}
