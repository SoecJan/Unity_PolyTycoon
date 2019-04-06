// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/WaterShader"
{
	Properties
	{
		_BaseColor ("Color", Color) = (0,0,0,0)
        _GlowColor ("GlowColor", Color) = (1,1,1,0) 
        _FoamColor("FoamColor", Color) = (0,0.5,1,0)
        _FoamSpeed("FoamSpeed", Float) = 1
        _FoamIntensity("FoamIntensity", Float) = 0.5
        _FoamMaximum("FoamMaximum", Float) = 0.5
        _CellSpeed("CellSpeed", Float) = 1
        _CellDensity("CellDensity", Float) = 6
        _CellIntensity("CellIntensity", Float) = 0.3
	}

	SubShader
	{
		Blend One One
		ZWrite Off
		Cull Off

		Tags
		{
			"RenderType"="Transparent"
			"Queue"="Transparent"
		}

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 screenuv : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float3 objectPos : TEXCOORD3;
				float4 vertex : SV_POSITION;
				float depth : DEPTH;
				float3 normal : NORMAL;
			};

            float _CellSpeed;

            float2 random2(float2 p)
			{
				return frac(sin(float2(dot(p,float2(117.12,341.7)),dot(p,float2(269.5,123.3))))*43458.5453);
			}

			float Voronoi_float(float2 UV, float CellDensity)
			{
				UV *= CellDensity; //Scaling amount (larger number more cells can be seen)
				float2 iuv = floor(UV); //gets integer values no floating point
				float2 fuv = frac(UV); // gets only the fractional part
				float minDist = 1.0;  // minimun distance
				for (int y = -1; y <= 1; y++)
				{
					for (int x = -1; x <= 1; x++)
					{
						// Position of neighbour on the grid
						float2 neighbour = float2(float(x), float(y));
						// Random position from current + neighbour place in the grid
						float2 pointv = random2(iuv + neighbour);
						// Move the point with time
						pointv = 0.5 + 0.5*sin(_Time.y + 6.2236*pointv)*_CellSpeed;//each point moves in a certain way
																		// Vector between the pixel and the point
						float2 diff = neighbour + pointv - fuv;
						// Distance to the point
						float dist = length(diff);
						// Keep the closer distance
						minDist = min(minDist, dist);
					}
				}
				// Draw the min distance (distance field)
				return minDist * minDist; // squared it to to make edges look sharper
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				o.screenuv = ((o.vertex.xy / o.vertex.w) + 1)/2;
				o.screenuv.y = 1 - o.screenuv.y;
				o.depth = -UnityObjectToViewPos(v.vertex).z *_ProjectionParams.w;

				o.objectPos = v.vertex.xyz;		
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)));

				return o;
			}
			
			sampler2D _CameraDepthNormalsTexture;
			fixed4 _BaseColor;
            fixed4 _FoamColor;
            fixed4 _GlowColor;

            float _FoamSpeed;
            float _FoamIntensity;
            float _FoamMaximum;

            float _CellDensity;
            float _CellIntensity;
            

			// float triWave(float t, float offset, float yOffset)
			// {
			// 	return saturate(abs(frac(offset + t) * 2 - 1) + yOffset);
			// }

			// fixed4 texColor(v2f i, float rim)
			// {
			// 	fixed4 mainTex = tex2D(_MainTex, i.uv);
			// 	mainTex.r *= triWave(_Time.x * 5, abs(i.objectPos.y) * 2, -0.7) * 6;
			// 	// I ended up saturaing the rim calculation because negative values caused weird artifacts
			// 	mainTex.g *= saturate(rim) * (sin(_Time.z + mainTex.b * 5) + 1);
			// 	return mainTex.r * Base + mainTex.g * Base;
			// }

			fixed4 frag (v2f i) : SV_Target
			{
				/* #####	Uncomment this to add Foam back.	######


				 float screenDepth = DecodeFloatRG(tex2D(_CameraDepthNormalsTexture, i.screenuv).zw);
				 float diff = screenDepth - i.depth;
				 float intersect = 0;
				 float foamStrength = min(0.1 + abs(sin(_Time.y * _FoamSpeed)) * _FoamIntensity, _FoamMaximum);
                
				 if (diff > 0)
				 	intersect = 1 - smoothstep(0, _ProjectionParams.w * foamStrength, diff);
				*/


				//float rim = 1 - abs(dot(i.normal, normalize(i.viewDir))) * 2;
				// float northPole = (i.objectPos.y - 0.45) * 20;

				//fixed4 glowColor = _GlowColor; //fixed4(lerp(Base.rgb, _GlowColor.rgb, pow(glow, 4)), 1);
				
				// fixed4 hexes = texColor(i, rim);

                fixed4 col = _BaseColor;
				float2 uv = i.uv;

				float _voronoi_multiplied = Voronoi_float(uv, _CellDensity) * _CellIntensity;
				float _smoothstep_out = smoothstep(0, 1, _voronoi_multiplied);
				float4 _foam_color = _FoamColor * (_smoothstep_out.xxxx);

				col += _foam_color; // + _GlowColor * intersect;

				// fixed4 col = Base * Base.a + glowColor * glow; // + hexes;
				return col;
			}
			ENDCG
		}
	}
}
