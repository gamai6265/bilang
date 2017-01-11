Shader "Bilang/ClothShader" {
	Properties
	{
		_MainTint("Global Tint", Color) = (1,1,1,1)
		_BumpMap("Normal Map", 2D) = "bump" {}
		_DetailBump("Detail Normal Map", 2D) = "bump" {}
		_MainTex("Fabric Weave", 2D) = "white" {}
		_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
		_FresnelPower("Fresnel Power", Range(0, 12)) = 3
		_RimPower("Rim FallOff", Range(0, 12)) = 3
		_SpecIntesity("Specular Intensiity", Range(0, 1)) = 0.2
		_SpecWidth("Specular Width", Range(0, 1)) = 0.2
		_Rotation("Rotation", float) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Velvet
		//#pragma target 3.0

		sampler2D _BumpMap;
		sampler2D _DetailBump;
		sampler2D _MainTex;
		float4 _MainTint;
		float4 _FresnelColor;
		float _FresnelPower;
		float _RimPower;
		float _SpecIntesity;
		float _SpecWidth;
		float _Rotation;

	struct Input
	{
		float2 uv_BumpMap;
		float2 uv_DetailBump;
		float2 uv_MainTex;
		//float3 worldNormal;
		//INTERNAL_DATA
	};

	inline fixed4 LightingVelvet(SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
	{
		//Create lighting vectors here
		viewDir = normalize(viewDir);
		lightDir = normalize(lightDir);
		half3 halfVec = normalize(lightDir + viewDir);
		fixed NdotL = max(0, dot(s.Normal, lightDir)); 

		//Create Specular 
		float NdotH = max(0, dot(s.Normal, halfVec));
		float spec = pow(NdotH, s.Specular*128.0) * s.Gloss;

		//Create Fresnel
		float HdotV = pow(1 - max(0, dot(halfVec, viewDir)), _FresnelPower);
		float NdotE = pow(1 - max(0, dot(s.Normal, viewDir)), _RimPower);
		float finalSpecMask = NdotE * HdotV;

		//Output the final color
		fixed4 c;
		c.rgb = (s.Albedo * NdotL * _LightColor0.rgb)
			+ (spec * (finalSpecMask * _FresnelColor)) * (atten * 2);
		c.a = 1.0;
		return c;
	}

	void surf(Input IN, inout SurfaceOutput o)
	{
		float cosf = cos(_Rotation);
		float sinf = sin(_Rotation);
		float4 rotation = float4(cosf, -sinf, sinf, cosf);
		float2 rotationFlare = mul(IN.uv_MainTex, float2x2(rotation));

		half4 c = tex2D(_MainTex, rotationFlare);//IN.uv_MainTex);
		fixed3 normals = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)).rgb;
		fixed3 detailNormals = UnpackNormal(tex2D(_DetailBump, IN.uv_DetailBump)).rgb;
		fixed3 finalNormals = float3(normals.x + detailNormals.x,
			normals.y + detailNormals.y,
			normals.z + detailNormals.z);

		o.Normal = normalize(finalNormals);
		o.Specular = _SpecWidth;
		o.Gloss = _SpecIntesity;
		o.Albedo = c.rgb * _MainTint;
		o.Alpha = c.a;
	}
	ENDCG
	}
			/*
		SubShader
		{
		Pass
		{
			CGPROGRAM
#pragma vertex vert  
#pragma fragment frag  
//#pragma fragmentoption ARB_precision_hint_fastest  
//#pragma glsl
//#pragma target 3.0  
			//包含辅助CG头文件  
#include "UnityCG.cginc"
		 
		uniform sampler2D _MainTex;
		uniform sampler2D _BumpMap;
		uniform sampler2D _DetailBump;
		uniform float2 _MainTex_TexelSize;

		float _SpecWidth;
		float _SpecIntesity;
		float4 _MainTint;
		float _FresnelPower;
		float _RimPower;
		float4 _FresnelColor;
		struct vertexInput{
			float4 vertex : POSITION;
			float4 color : COLOR;
			float2 uv_MainTex : TEXCOORD0;
			float2 uv_BumpMap : TEXCOORD1;
			float2 uv_DetailBump :  TEXCOORD2;
			float3 normal : NORMAL;
		};

		struct vertexOutput
		{
			//half2 uv : TEXCOORD0;
			float4 pos : SV_POSITION;
			//fixed4 color : COLOR;
			float3 normal : NORMAL;
			float4 Specular : TEXCOORD1;
			float4 Gloss : TEXCOORD2;
			float3 Albedo: TEXCOORD3;
			float4 Alpha : TEXCOORD4;
		};

		vertexOutput vert(vertexInput v) 
		{
			vertexOutput o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

			half4 c = tex2Dlod(_MainTex, float4(v.uv_MainTex.xy, 0, 0));
			fixed3 normals = UnpackNormal(tex2Dlod(_BumpMap, float4(v.uv_BumpMap.xy, 0, 0))).rgb;
			fixed3 detailNormals = UnpackNormal(tex2Dlod(_DetailBump, float4(v.uv_DetailBump.xy,0,0))).rgb;
			fixed3 finalNormals = float3(normals.x + detailNormals.x,
				normals.y + detailNormals.y,
				normals.z + detailNormals.z);
			o.normal = normalize(finalNormals);
			o.Specular = _SpecWidth;
			o.Gloss = _SpecIntesity;
			o.Albedo = c.rgb * _MainTint;
			o.Alpha = c.a;

			return o;
		}

		fixed4 frag(vertexOutput v) : COLOR
		{
			float3 viewDir = UnityWorldSpaceViewDir(v.pos);//normalize(viewDir);
			float3 lightDir = UnityWorldSpaceLightDir(v.pos);//normalize(lightDir);
			half3 halfVec = normalize(lightDir + viewDir);
			fixed NdotL = max(0, dot(v.normal, lightDir));

			//Create Specular 
			float NdotH = max(0, dot(v.normal, halfVec));
			float spec = pow(NdotH, v.Specular*128.0) * v.Gloss;

			//Create Fresnel
			float HdotV = pow(1 - max(0, dot(halfVec, viewDir)), _FresnelPower);
			float NdotE = pow(1 - max(0, dot(v.normal, viewDir)), _RimPower);
			float finalSpecMask = NdotE * HdotV;

			float3 viewpos = mul(UNITY_MATRIX_MV, v.pos).xyz;
			float3 toLight = unity_LightPosition[0].xyz - viewpos.xyz * unity_LightPosition[0].w;
			float lengthSq = dot(toLight, toLight);
			float atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[0].z);
			//Output the final color
			fixed4 c;
			c.rgb = (v.Albedo * NdotL *  UNITY_LIGHTMODEL_AMBIENT.xyz)
				+ (spec * (finalSpecMask * _FresnelColor)) * (atten * 2);
			c.a = 1.0;
			return c;
		}
		ENDCG
		}
		}
		SubShader{
			LOD 200
			Pass{
				Cull Back
				Blend Off
				Lighting On
				AlphaTest Off
				ZWrite On
				ZTest LEqual
				Fog {Mode Off}
				Tags{ "RenderType" = "Opaque"}
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				sampler2D _BumpMap;
				sampler2D _DetailBump;
				float2 _MainTex_TexelSize;

				float _SpecWidth;
				float _SpecIntesity;
				float4 _MainTint;
				float _FresnelPower;
				float _RimPower;
				float4 _FresnelColor;
				struct vertexInput {
					float4 vertex : POSITION;
					float4 color : COLOR;
					float2 uv_MainTex : TEXCOORD0;
					float2 uv_BumpMap : TEXCOORD1;
					float2 uv_DetailBump :  TEXCOORD2;
					float3 normal : NORMAL;
				};

				struct vertOUT {
					float4 pos : SV_POSITION;
					fixed2 uv_MainTex : TEXCOORD0;
					fixed2 uv_BumpMap : TEXCOORD1;
					fixed2 uv_DetailBump : TEXCOORD2;
					half3 nDir : NORMAL;
					fixed3 vDir : TEXCOORD3;
					fixed3 lDir : TEXCOORD4;
					fixed3 rDir : TEXCOORD5;
					float atten : TEXCOORD6;
				};

				vertOUT vert(vertexInput v) {
					vertOUT o;
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv_MainTex = v.uv_MainTex;
					o.uv_BumpMap = v.uv_BumpMap;
					o.uv_DetailBump = v.uv_DetailBump;
					o.nDir = normalize(mul(_World2Object, half4(v.normal, 0)).xyz);
					o.vDir = WorldSpaceViewDir(v.vertex);
					o.lDir = WorldSpaceLightDir(v.vertex);
					o.rDir = normalize(reflect(-o.vDir, o.nDir));

					float3 viewpos = mul(UNITY_MATRIX_MV, v.vertex).xyz;
					float3 toLight = unity_LightPosition[0].xyz - viewpos.xyz * unity_LightPosition[0].w;
					float lengthSq = dot(toLight, toLight);
					o.atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[0].z);
					return o;
				}
				fixed4 frag(vertOUT v) : COLOR
				{
					fixed3 normals = UnpackNormal(tex2D(_BumpMap, v.uv_BumpMap)).rgb;
					fixed3 detailNormals = UnpackNormal(tex2D(_DetailBump, v.uv_DetailBump)).rgb;
					fixed3 finalNormals = float3(normals.x + detailNormals.x,
					normals.y + detailNormals.y,
					normals.z + detailNormals.z);
					fixed3 normal = normalize(finalNormals);
					normal = normalize(mul(_World2Object, normal));

					half4 xc = tex2D(_MainTex, v.uv_MainTex);
					float3 Albedo = xc.rgb * _MainTint;
					half3 halfVec = normalize(v.lDir+ v.vDir);
					fixed NdotL = max(0, dot(normal, v.vDir));//v.nDir

					//Create Specular 
					float NdotH = max(0, dot( normal, halfVec));//v.nDir
					float spec = pow(NdotH, _SpecWidth*128.0) * _SpecIntesity;

					//Create Fresnel
					float HdotV = pow(1 - max(0, dot(halfVec, v.vDir)), _FresnelPower);
					float NdotE = pow(1 - max(0, dot(normal, v.vDir)), _RimPower);//v.nDir
					float finalSpecMask = NdotE * HdotV;

					//Output the final color
					fixed4 c;
					c.rgb = (Albedo * NdotL *  UNITY_LIGHTMODEL_AMBIENT.xyz)
						+ (spec * (finalSpecMask * _FresnelColor)) * (v.atten * 2);
					c.a = 1.0;
					return c;
				}
				ENDCG
			}
		}*/
	
	FallBack off
}
