/*
GaussianBlur_Mobile_ScreenSpaceCamera.shader
uses properties and the global textures to generate a blurred effect on the screen.
should be used if you decide to have the Canvas in "ScreenSpace-Camera" mode

*/


Shader "Custom/GaussianBlur_Mobile_ScreenSpaceCamera"
{
	Properties
	{
		[PerRendererData] _MainTex ("_MainTex", 2D) = "white" {}
		_Lightness ("_Lightness", Range(0,2)) = 1

        _Saturation ("_Saturation", Range(-10,10)) = 1

		_TintColor ("_TintColor",Color) = (1.0,1.0,1.0,0.0)

	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"PreviewType" = "Plane"
			"DisableBatching" = "True"
		}

		Pass
		{
			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				half4 color : COLOR;
				half4 screenpos : TEXCOORD2;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 screenuv : TEXCOORD1;
				half4 color : COLOR;
				float2 screenpos : TEXCOORD2;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.screenuv = ((o.vertex.xy / o.vertex.w) + 1) * 0.5;
				o.color = v.color;
				o.screenpos = ComputeScreenPos(o.vertex);
				return o;
			}

			float2 safemul(float4x4 M, float4 v)
			{
				float2 r;

				r.x = dot(M._m00_m01_m02, v);
				r.y = dot(M._m10_m11_m12, v);

				return r;
			}

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			// x contains 1.0/width
			// y contains 1.0/height
			// z contains width
			// w contains height

			uniform float _Lightness;
            uniform float _Saturation;
			uniform fixed4 _TintColor;

            uniform sampler2D _MobileBlur;
			float4 _MobileBlur_ST;


			float4 frag(v2f i) : SV_Target
			{
				float4 m = tex2D(_MainTex, i.uv);

				float2 uvWH = float2(_MainTex_TexelSize.z  / _ScreenParams.x,_MainTex_TexelSize.w / _ScreenParams.y);
				uvWH.x *= _MainTex_TexelSize.x;
				uvWH.y *= _MainTex_TexelSize.y;

				
				//Adjust pending of GraphicCard/OS
				float2 thisUV = float2(1,1);
				#if UNITY_UV_STARTS_AT_TOP
					thisUV = float2(i.screenuv.x,1-i.screenuv.y);
				#else
					thisUV = float2(i.screenuv.x,i.screenuv.y);
				#endif

				
				float2 buv = TRANSFORM_TEX(thisUV,_MobileBlur);


                float4 blurColor = float4(0,0,0,0);
                blurColor = tex2D(_MobileBlur,buv);
                //blurColor = tex2D(_MobileBlur,buv);


                blurColor.a *= m.a;
                            
				float4 finalColor = blurColor * i.color;
				finalColor.a = i.color.a * m.a * blurColor.a;


                finalColor.rgb *= _Lightness;
				finalColor.rgb *= _TintColor;

                float3 intensity = dot(finalColor.rgb, float3(0.299,0.587,0.114));
                finalColor.rgb = lerp(intensity, finalColor.rgb  , _Saturation);
                            
				return finalColor;
			}
			ENDCG
		}
	}

	Fallback "Sprites/Default"
}