Shader "Unlit/BallBlured"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Scale("Scale", Float) = 1.0
		_Pos("Pos", Vector) = (0,0,0)
		_BallScale("BallScale", Float) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Blend OneMinusDstColor One
		Pass
		{
			CGPROGRAM
			 #pragma exclude_renderers gles
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
			 StructuredBuffer<float2> _PositionBuffer;
			StructuredBuffer<uint> _ColorBuffer;
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				fixed4 color: COLOR;
			};
			float _Scale;
			float _BallScale;
			float3 _Pos;
			sampler2D _MainTex;
			  inline float4 uintToColor(uint u)
            {
                const uint b=255;
                return  float4(u & b,u>>8 & b, u>>16 & b, u>>24)/255;
            }
             inline float4 ScreenToClip(float2 screen)
            {
                return float4((2 * screen.x / _ScreenParams.x - 1),
                              _ProjectionParams.x * (2 * screen.y / _ScreenParams.y - 1), 0, 1);;
            }
			v2f vert (appdata v, const uint instance_id : SV_InstanceID)
			{
				const float2 pos = _PositionBuffer[instance_id];
                const float2 worldPosition = pos + (v.vertex.xy*_BallScale);
                v2f o;
				o.uv= v.uv;
                o.vertex = ScreenToClip(worldPosition*_Scale+_Pos.xy);
				o.color =uintToColor(_ColorBuffer[instance_id]);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				
                float4 col = tex2D(_MainTex, i.uv) * i.color;
				clip(col.a - 1.0 / 255.0);
                col.rgb *= col.a;
				return  col;
            }
			ENDCG
		}
	}
}