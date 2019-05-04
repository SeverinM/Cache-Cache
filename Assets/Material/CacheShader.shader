// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "CustomMy/CacheShader"
{
	Properties
	{
		_ColorNight("Color normal night", Color) = (1,1,1,1)
		_ColorNShadow("Color shadow night", Color) = (1,1,1,1)
		_ColorReveal("Color normal day", Color) = (1,1,1,1)
		_ColorRShadow("Color shadow day", Color) = (1,1,1,1)

		_LightEffect("Light Effect", Range(0,1)) = 1
		_EdgeEffect("Edge Effect", Range(0,0.2)) = 0.2

		_SpecIntensity("Spec Intensity", Range(0,1)) = 1
		_Glowsiness("Spec size",float) = 32
		_SpecularColor("SpecColor", Color) = (1,1,1,1)
		_SpecEdgeEffect("Spec Edge Effect", Range(0.001,1)) = 0.2

		_ShadowEdge("Shadow edge", Range(1,10)) = 1
    }
    SubShader
    {
		Tags
		{
			"LightMode" = "ForwardBase"
			//"PassFlags" = "OnlyDirectional"
		}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_fwdbase

			#include "Lighting.cginc"
            #include "UnityCG.cginc"
			#include "AutoLight.cginc"

            struct appdata
            {
				float3 normal : NORMAL;
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
				float4 pos : SV_POSITION;
				float4 worldPos : TEXCOORD3;
				float3 viewDir : TEXCOORD1;
				float3 worldNormal : NORMAL;

				SHADOW_COORDS(2)
            };

			float4 _ColorNight;
			float4 _ColorNShadow;
			float4 _ColorReveal;
			float4 _ColorRShadow;


            v2f vert (appdata v)
            {
                v2f o; 
				o.viewDir = WorldSpaceViewDir(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

				TRANSFER_SHADOW(o)
                return o;
            }

			half _LightEffect;
			half _EdgeEffect;

			half _SpecIntensity;
			float _Glowsiness;
			float4 _SpecularColor;
			half _SpecEdgeEffect;

			float _ShadowEdge;

            fixed4 frag (v2f i) : SV_Target
            {
				float3 normal = normalize(i.worldNormal);
				float NdotL = dot(_WorldSpaceLightPos0, normal); 

				float shadow = SHADOW_ATTENUATION(i);
				float lightIntensity = smoothstep(0, _EdgeEffect, NdotL * shadow);

				half lightEffect = _LightEffect * NdotL;
				float4 light = lerp(float4(1, 1, 1, 1), _LightColor0, lightEffect);

				//light reveal
				float index = 0;
				float3 lightPosition = float3(unity_4LightPosX0[index],
					unity_4LightPosY0[index],
					unity_4LightPosZ0[index]);

				//half day = (distance(i.worldPos.xyz, lightPosition.xyz) > unity_4LightAtten0[index]) ? 0 : 1;
				float distanceEffect = 1 / unity_4LightAtten0[index];
				float3 dist = i.worldPos.xyz - lightPosition.xyz;
				float distanceSquare = dist.x * dist.x + dist.y * dist.y + dist.z * dist.z;
				half day = clamp((distanceSquare - distanceEffect * distanceEffect)/(distanceEffect * distanceEffect), 0, 1);
				day = pow(day, _ShadowEdge);
				half4 colorNormal = _ColorNight * day + _ColorReveal * (1 - day);;
				half4 colorShadow = _ColorNShadow * day + _ColorRShadow * (1-day);
				//endreveal

				fixed4 col = (lightIntensity * colorNormal) + (1 - lightIntensity) * colorShadow;

				//specular 
				float3 viewDir = normalize(i.viewDir);

				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = dot(normal, halfVector);

				float specularIntensity = pow(NdotH * lightIntensity, _Glowsiness * _Glowsiness);
				specularIntensity = smoothstep(0, _SpecEdgeEffect, specularIntensity);
				float4 specular = specularIntensity * _SpecularColor * _SpecIntensity;

				

				return col * (light + specular); //RIM// +rim;
            }
            ENDCG
        }
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
