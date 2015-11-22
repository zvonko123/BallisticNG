Shader "Retro3D/No Affine Transparent"
{
    Properties
    {
        _MainTex("Base", 2D) = "white" {}
		_Illum("Illumination", 2D) = "black" {}
        _Color("Color", Color) = (0.5, 0.5, 0.5, 1)
        _GeoRes("Geometric Resolution", Float) = 40
    }
		Category{
			Cull Off
			SubShader
			{
				Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
				Blend SrcAlpha OneMinusSrcAlpha
				Pass
				{
					CGPROGRAM
					#include "UnityCG.cginc"

					#pragma vertex vert
					#pragma fragment frag

					struct v2f
					{
						float4 position : SV_POSITION;
						half2 texcoord : TEXCOORD;
						fixed4 color : COLOR;
					};

					sampler2D _MainTex;
					float4 _MainTex_ST;
					sampler2D _Illum;
					float4 _Color;
					float _GeoRes;
					fixed _Cutoff;

					v2f vert(appdata_full v)
					{
						v2f o;
						float4 wp = mul(UNITY_MATRIX_MV, v.vertex);
						wp.xyz = floor(wp.xyz * _GeoRes) / _GeoRes;

						float4 sp = mul(UNITY_MATRIX_P, wp);
						o.position = sp;
						o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
						fixed4 newC = v.color;
						o.color = newC * _Color;
						return o;
					}

					fixed4 frag(v2f i) : SV_Target
					{
						fixed4 col = (tex2D(_MainTex, i.texcoord) * i.color) + tex2D(_Illum, i.texcoord);
						col.a = tex2D(_MainTex, i.texcoord).a;
						return col;
					}

					ENDCG
				}
			}
		}
}
