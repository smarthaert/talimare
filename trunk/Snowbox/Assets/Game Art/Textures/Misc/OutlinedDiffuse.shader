Shader "Outlined Diffuse" 
{ 
	Properties 
	{ 
		_Color ("Main Color", Color) = (.5,.5,.5,1) 
		_OutlineColor ("Outline Color", Color) = (0,1,0,1) 
		_Outline ("Outline width", Range (0.002, 0.03)) = 0.01 
		_MainTex ("Base (RGBA)", 2D) = "white" { }
	} 

	SubShader 
	{ 
		Tags {"Queue"="Transparent" "RenderType"="Transparent"} 

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float4 _Color;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG

		Pass
		{ 
			Name "OUTLINE"
			Tags { "LightMode" = "Always" }

			Cull Front
			ZWrite On
			ColorMask RGBA
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma exclude_renderers d3d11
			#pragma exclude_renderers xbox360
			#pragma exclude_renderers gles
			#pragma exclude_renderers flash
			#pragma vertex vert
			#pragma fragment frag

			struct appdata { 
				float4 vertex; 
				float3 normal; 
			};

			struct v2f { 
				float4 pos : POSITION; 
				float4 color : COLOR; 
				float fog : FOGC; 
			};
			
			uniform float _Outline; 
			uniform float4 _OutlineColor; 

			v2f vert(appdata v) { 
				v2f o; 
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex); 
				float3 norm = mul ((float3x3)UNITY_MATRIX_MV, v.normal); 
				norm.x *= UNITY_MATRIX_P[0][0]; 
				norm.y *= UNITY_MATRIX_P[1][1]; 
				o.pos.xy += norm.xy * o.pos.z * _Outline; 

				o.fog = o.pos.z; 
				o.color = _OutlineColor; 
				return o; 
			}

			half4 frag(v2f i) : COLOR {
			    return i.color;
			}
			ENDCG
		}
	} 

	Fallback "Diffuse" 
}
