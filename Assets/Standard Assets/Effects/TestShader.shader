Shader "Unlit/TestShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		
		_Color ("Main Color", Color) = (.5,.5,.5,1)
		_Range ("Range", Range (.002, 100.0)) = 3.0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent+1" }
		LOD 100
		
		Blend SrcAlpha one
		Blend SrcAlpha oneMinusSrcAlpha
		//Blend DstColor Zero
		Cull Back
		ZWrite Off
		ZTest greater
		Offset -1, -1
		ColorMask RGB
		
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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 wPos : TEXCOORD1;
			};
			
			uniform float _Range;
			uniform float4 _Color;
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.wPos = mul(UNITY_MATRIX_MV, v.vertex);
				//o.wPos.w = o.vertex.z;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) *_Color;
				//	col *= 1-( length(i.wPos.xyz) / _Range );
				float dis = length(i.wPos.xyz / i.wPos.w );//
				//if( dis > _Range ) col *= 0;
				float fade = 3;
				col.a *= clamp( _Range-dis, 0,fade )/fade;
				col.a *= clamp( (sin( pow( dis, 0.33 )*300 -  _Time.w*8 )-0.995)*10, 0, 1);
				//col.a *= sin( dis );
				// apply fog
			//	UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
