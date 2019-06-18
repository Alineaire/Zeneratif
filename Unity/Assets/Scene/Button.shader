Shader "Unlit/Button"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_Radius("Radius", Range(0., .5)) = .4
		_Width("Width", Range(0., .5)) = .1
		_Smoothness("Smoothness", Range(0., .1)) = .05
		_Inside("Inside", Range(0., 1.)) = 0.
	}

		SubShader
	{
		Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float4 _Color;
			float _Radius;
			float _Width;
			float _Smoothness;
			float _Inside;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float r = length(i.uv - 0.5);
				fixed4 col = _Color;
				col.a = lerp(_Inside, 1., smoothstep(0, _Smoothness, r - _Radius + _Width)) * smoothstep(_Smoothness, 0., r - _Radius - _Width);
				return col;
			}

			ENDCG
		}
	}
}
