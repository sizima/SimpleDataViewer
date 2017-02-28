Shader "Custom/visualizationTestShader_simple" {
	SubShader
	{
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha	// Alpha blending

		Pass
		{
			CGPROGRAM

			// The shader model specifies 5.0.
			#pragma target 5.0

			// Specifies which functions will be used for the vertex, geometry and fragment computations.
			#pragma vertex _vert
			#pragma geometry _geom
			#pragma fragment _frag
			#include "UnityCG.cginc"	//contains a number of really useful functions to use in the shaders

			// Texture
			sampler2D _MainTex;

			// structure of data point
			struct Atom
			{
				float3 _pos;
				float _rad;
				float4 _col;
			};

			// This structure buffer has the same structure as the compute buffer prepared in C# script.
			StructuredBuffer<Atom> _atoms;

			// use this structure to pass data from the vertex shader to the next shader
			struct vertexOutput {
				float4 _pos : SV_POSITION;
				float2 _tex : TEXCOORD0;
				float  _rad : TEXCOORD1;
				float4 _col : COLOR;
			};


			// Vertex shader
			vertexOutput _vert(uint id : SV_VertexID)
			{
				// Vertex shaders get values of each data point from compute buffer.
				vertexOutput _output;
				_output._pos = float4(_atoms[id]._pos, 1);
				_output._tex = float2(0, 0);
				_output._rad = _atoms[id]._rad;
				_output._col = _atoms[id]._col;
				return _output;
			}

			// Geometry shader
			[maxvertexcount(4)]
			void _geom(point vertexOutput input[1], inout TriangleStream<vertexOutput> outStream)
			{
				// Geometry Shader makes billboards at each coordinate.
				vertexOutput _output;

				float4 _pos = input[0]._pos;
				float4 _col = input[0]._col;
				float _rad = input[0]._rad;
				// Generate the vertices of billboard
				for (int x = 0; x < 2; x++)
				{
					for (int y = 0; y < 2; y++)
					{
						float4x4 billboardMatrix = UNITY_MATRIX_V;
						billboardMatrix._m03 =
							billboardMatrix._m13 =
							billboardMatrix._m23 =
							billboardMatrix._m33 = 0;

						// texture coordinates
						float2 _tex = float2(x, y);
						_output._tex = _tex;

						// Calculate the coordinates of the vertices of billboard
						_output._pos = _pos + mul(float4((_tex * 2 - float2(1, 1)) * _rad, 0, 1), billboardMatrix);
						_output._pos = mul(UNITY_MATRIX_VP, _output._pos);

						// radius
						_output._rad = _rad;

						// color
						_output._col = _col;

						// append the vertex values to stream
						outStream.Append(_output);
					}
				}
				// Ends the current triangle strip
				outStream.RestartStrip();
			}

			// Fragment shader
			fixed4 _frag(vertexOutput _i) : COLOR
			{
				// Fragment Shader draws the screen from the texture attached to the billboard and the color value of the data point.
				float4 _col = tex2D(_MainTex, _i._tex) * _i._col;

				// discard if alpha is less than a certain value
				if (_col.a < 0.9) discard;

				// Return the final color
				return _col;
			}
			ENDCG
		}
	}
}