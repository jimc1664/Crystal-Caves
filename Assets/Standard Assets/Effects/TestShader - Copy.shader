// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/TestShader2"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		
		_Color ("Main Color", Color) = (.5,.5,.5,1)
		_Color2 ("Main Color2", Color) = (.5,.5,.5,1)
		_Range ("Range", Range (.002, 100.0)) = 3.0
		_MaxTime ("_MaxTime", Range (.002, 100.0)) = 3.0
		_SrcPos ("SrcPos", Vector) = (0,0,0,1)
		_Time2 ("_Time2", Vector) = (0,0,0,1)
	}

	CGINCLUDE
	#pragma vertex vert
	#pragma fragment frag
	// make fog work
	//#pragma multi_compile_fog
	
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
	
	uniform float _Range, _MaxTime;
	uniform float4 _Color, _Color2;
	uniform float4 _SrcPos, _Time2;
	
	sampler2D _MainTex;
	float4 _MainTex_ST;
	
	
		
	v2f vert (appdata v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		o.wPos = mul(unity_ObjectToWorld, v.vertex);
		//o.wPos.w = o.vertex.z;
		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}
	
	fixed4 fragSub (v2f i, fixed4 color )
	{
		// sample the texture
		fixed4 col = tex2D(_MainTex, i.uv) *color;
		//	col *= 1-( length(i.wPos.xyz) / _Range );
		
		fixed3 vec = i.wPos.xyz / i.wPos.w - _SrcPos.xyz;
		vec.y *= 2;
		float dis = length(vec );//
		
		float fade = 2;
		
		float t = (_Time2.y -_SrcPos.w)/_MaxTime;
		float r = _Range  *t;
		float p = 1.5;
		col.a *= clamp( (fade + 40*t*t - pow( pow(r,p)- pow(dis,p), 3.5- _MaxTime)), 0,fade )/fade;
		col.a *= min( 1,(1- t)*2);
		//col.a *= clamp( (sin( pow( dis, 0.33 )*10 -  _Time.w*2 )-0.95)*100, 0, 1);
		
		//col.a *=  _Range - dis 
		//col.a *= sin( dis );
		// apply fog
	//	UNITY_APPLY_FOG(i.fogCoord, col);
		return col;
	}
	

	ENDCG
		
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 100
		
		Blend SrcAlpha one
		Blend SrcAlpha oneMinusSrcAlpha
		//Blend DstColor Zero
		Cull Back
		ZWrite Off
		//ZTest off
		Offset -1, -1
		ColorMask RGB
		

		Pass
		{
			CGPROGRAM		
			fixed4 frag(v2f i ) : SV_Target { return fragSub(i, _Color ); }
			ENDCG
		}
		
		ZTest greater
		Pass
		{
			CGPROGRAM		
			fixed4 frag(v2f i ) : SV_Target { return fragSub(i, _Color2 ); }
			ENDCG
		}
	}
}
