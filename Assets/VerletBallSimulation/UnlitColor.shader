Shader "Unlit/UnlitColor"
{
//	Properties
//	{
//		_PointSize("Point Size", Range(1,10)) =1
//	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			float _PointSize;
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : Color;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 color : Color;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color= v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : Color
			{
				return i.color;
			}
			ENDCG
		}
	}
}