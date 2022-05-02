Shader "Hidden/Depth"
{
	Properties
	{

	}

	v2f vert
	{
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}

	fixed _Amp;
	sampler2D _MainTex;
	sampler2D _CameraDepthTexture;

	fixed4 frag(v2f i) : SV_Target
	{
		fixed depth = tex2D(_CameraDepthTexture, i.uv).r;
		
		depth *= _Amp;

		fixed4 col = tex2D(_MainTex, i.uv);

		col *= (1 - depth);

		return col;
	}
	ENDCG
}