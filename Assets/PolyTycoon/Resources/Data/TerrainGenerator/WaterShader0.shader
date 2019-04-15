/*Please do support www.bitshiftprogrammer.com by joining the facebook page : fb.com/BitshiftProgrammer*/

Shader "Custom/WaterShader0"
{
	Properties
	{
		_BaseColor("BaseColor", Color) = (0,0.7,1,0)
		_FoamColor("FoamColor", Color) = (0,0.5,1,0)
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"

			

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

			float2 random2(float2 p)
			{
				return frac(sin(float2(dot(p,float2(117.12,341.7)),dot(p,float2(269.5,123.3))))*43458.5453);
			}

			float Voronoi_float(float2 UV, float AngleOffset, float CellDensity)
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
						pointv = 0.5 + 0.5*sin(_Time.y + 6.2236*pointv);//each point moves in a certain way
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

			void Multiply_float(float A, float B, out float Out)
			{
				Out = A * B;
			}

			void Add_float(float A, float B, out float Out)
			{
				Out = A + B;
			}

			void Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
			{
				Out = smoothstep(Edge1, Edge2, In);
			}

			float4 _BaseColor;
			float4 _FoamColor;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = _BaseColor;
				float2 uv = i.uv;

				 float _voronoi_multiplied = Voronoi_float(uv, 1, 6.0) * 0.3;
				 float _smoothstep_out = smoothstep(0, 1, _voronoi_multiplied);
				 float4 _foam_color = _FoamColor * (_smoothstep_out.xxxx);

				col += _foam_color;
				
				return col;
			}
		ENDCG
		}
	}
}
