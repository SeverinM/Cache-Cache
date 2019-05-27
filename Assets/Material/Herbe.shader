// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Herbe"
{
	Properties
	{
		_ColorNight("Color normal night", Color) = (1,1,1,1)
		_ColorNShadow("Color shadow night", Color) = (1,1,1,1)
		_ColorReveal("Color normal day", Color) = (1,1,1,1)
		_ColorRShadow("Color shadow day", Color) = (1,1,1,1)

		_LightEffect("Point light intensity",float) = 1

		_Edge1Effect("SmoothForShadow 1", float) = 0.2
		_Edge2Effect("SmoothForShadow 2", float) = 0.2

		_ShadowEdge1("Spotlight smooth 1", Range(0,10)) = 1
		_ShadowEdge2("Spotlight smooth 2", Range(0,10)) = 1

		_SpecIntensity("Spec Intensity", Range(0,1)) = 1
		_Glowsiness("Spec size",float) = 32
		_SpecularColor("SpecColor", Color) = (1,1,1,1)
		_SpecEdgeEffect("Spec Edge Effect", Range(0.001,1)) = 0.2


		_RotationSpeed("Rotation value", float) = 1

		_HeightMax("Hauteur Max", float) = 1
		_HeighEffect("Influence hauteur", float) = 1
		_SourceCenter("Position of the center", Vector) = (0,0,0,0)



		_Debug("Debug", float) = 1

		
	}


		SubShader
	{


					CGPROGRAM
					#pragma surface surf SimpleLambert vertex:vert fullforwardshadows



					//users variables
					float3 _ColorNight;
					float3 _ColorNShadow;
					float3 _ColorReveal;
					float3 _ColorRShadow;

					float _LightEffect;

					float _Edge1Effect;
					float _Edge2Effect;
					float _ShadowEdge1;
					float _ShadowEdge2;

					half _SpecIntensity;
					float _Glowsiness;
					float4 _SpecularColor;
					half _SpecEdgeEffect;

					float _Debug;

					//custom lighting
					half4 LightingSimpleLambert(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
						//TO DO :	NdotL 
						//			smoothStep to have nicer shadow 
						//			add more extreme
						half NdotL = dot(s.Normal, lightDir);

						half ambientLight = _LightColor0.b * atten * NdotL;
						ambientLight = smoothstep(_Edge1Effect, _Edge2Effect, ambientLight);

						half3 nitColor = lerp(_ColorNShadow, _ColorNight , ambientLight);
						half3 dayColor = lerp(_ColorRShadow, _ColorReveal, ambientLight);

						half pointLightDist = atten * NdotL;
						pointLightDist = smoothstep(_ShadowEdge1, _ShadowEdge2, pointLightDist);

						half3 pointLightColor = lerp(half3(0, 0, 0), dayColor, pointLightDist);

						half pointLightIntensity = _LightColor0.r;




						float shadow = atten;
						float lightIntensity = smoothstep(0, 0.1, NdotL * shadow);

						//Final choice 
						half4 col;
						col.rgb = lerp(nitColor, pointLightColor * _LightEffect, pointLightIntensity);
						col.a = s.Alpha;

						//Specular
						viewDir = normalize(viewDir);

						float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
						float NdotH = dot(s.Normal, halfVector);

						float specularIntensity = pow(NdotH * lightIntensity, _Glowsiness * _Glowsiness);
						specularIntensity = smoothstep(0, _SpecEdgeEffect, specularIntensity);
						float4 specular = specularIntensity * _SpecularColor * _SpecIntensity;
						//End specular


						//float specIntensity = pow(NdotL, _SpecSize);


						return col + specular;
					}

					//init input
					struct Input {
						float4 _Color;
					};

					//For Vertex 
					struct appdata {
						float4 vertex : POSITION;
						float3 normal : NORMAL;
					};

					struct v2f
					{
						float4 vertex : POSITION;
					};

					float _RotationSpeed;
					float _HeightMax;
					float _HeighEffect;

					float4 _SourceCenter;



					v2f vert(inout appdata v)
					{
						v2f o;

						////Preparation
						float4 objOriginalPos = v.vertex;
						float4 worldOriginalPos = mul(unity_ObjectToWorld, objOriginalPos);

						float4 worldPos = worldOriginalPos;

						////Calcul of the offset total value
						worldPos -= _SourceCenter;
						//rotation
						half heigthValue = clamp(objOriginalPos.y / _HeightMax, 0, 1);
						float val = _RotationSpeed * heigthValue * heigthValue;
						worldPos.x = worldPos.x * cos(val) - worldPos.z * sin(val);
						worldPos.z = worldPos.x * sin(val) + worldPos.z * cos(val);

						worldPos += _SourceCenter;

						worldPos = lerp(worldOriginalPos, worldPos, _HeighEffect);

						////Basic curve, no link to rotation
						//half heigthValue = clamp(objOriginalPos.y / _HeightMax, 0, 1);
						//								//give a curve effect
						//worldPos.x += _RotationSpeed * heigthValue * heigthValue * _HeighEffect;
						


						////put the value in the right input/output
						float4 objPos = mul(unity_WorldToObject, worldPos);
						o.vertex = UnityObjectToClipPos(objPos);
						v.vertex = objPos;
						return o;
					}
					//End vertex


					void surf(Input IN, inout SurfaceOutput o) {
						o.Albedo = _ColorNShadow;
						o.Emission = 0;
						o.Specular = _Debug;
						o.Gloss = 0;
						o.Alpha = 0;
					}
					ENDCG


	}

		FallBack "Mobile/Diffuse"
}