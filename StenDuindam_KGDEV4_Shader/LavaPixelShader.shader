Shader "Custom/VertexShader" {
	Properties {
		//Create variables
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_WaveZ ("wave intensity Z",float) = 0.5
		_WaveY("wave intensity Y",float) = 0.5
		_OffsetSpeed("offset speed",float) = 0.5
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert 

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		//Declare our variables again
		float _WaveZ;
		float _WaveY;
		float _OffsetSpeed;
		fixed4 _Color;

		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v) {
			//apply wave motion to the z axis of the texture
			v.vertex.z += sin(_Time.z + v.vertex.x) * _WaveZ;

			//depending on the texture color declare the intensity to apply the sin wave along the Y axis.
			fixed4 _lavaColor = tex2Dlod(_MainTex, v.texcoord);
			fixed4 _VertexY = (v.vertex.y + (v.vertex.z * ((_lavaColor.r + _lavaColor.g + _lavaColor.b) / 3))*0.5);

			//Apply wave to the Y axis.
			v.vertex.y += abs(sin(_Time.y + _VertexY)) * _WaveY;

		}

		void surf (Input IN, inout SurfaceOutputStandard o) {

			//Stretch the texture and downsize it to create the pixel effect. Also give the possibility for an offset with _Offsetspeed.
			fixed4 c = tex2D(_MainTex, (ceil(IN.uv_MainTex*100)/100) + (_Time.x * _OffsetSpeed)) * _Color; 

			//Apply to the texture
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
