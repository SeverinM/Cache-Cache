// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
Shader "Custom/Shader4Color"
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

		_Debug("Debug", float) = 1//specular
	}
		SubShader{


			CGPROGRAM
					#pragma surface surf SimpleLambert fullforwardshadows



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


			void surf(Input IN, inout SurfaceOutput o) {
				o.Albedo = _ColorNShadow;
				o.Emission = 0;
				o.Specular = _Debug;
				o.Gloss = 0;
				o.Alpha = 0;
			}
			ENDCG
		

		}

		FallBack "Diffuse"
}

//Save :
/*
				half NdotL = dot(s.Normal, lightDir);
				half4 c;
				c.rgb = _LightColor0.rgb * (NdotL * atten);

				half ambientLightIntensity = _LightColor0.b * atten;

				ambientLightIntensity = clamp(ambientLightIntensity, 0, 1);

				half3 nightVal = lerp(_ColorNShadow, _ColorNight, ambientLightIntensity).rgb;
				half3 dayVal = lerp(_ColorRShadow, _ColorReveal, ambientLightIntensity).rgb;

				half pointLightIntensity = clamp(_LightColor0.r * NdotL, 0, 1);

				//pointLightIntensity = smoothstep(_Debug, 0.5f, pointLightIntensity);
				half isThisADirectionalLight = (_LightColor0.r);

				//c.rgb = lerp(nightVal, dayVal, pointLightIntensity * atten) * isThisAPointLight;
				c.rgb = nightVal * isThisADirectionalLight;
				//c.rgb = _LightColor0;

				c.a = s.Alpha;
				return c;
*/