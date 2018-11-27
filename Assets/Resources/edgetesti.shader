Shader "Hidden/EdgeDetectionShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Threshold("Threshold", float) = 0.20
        _EdgeColor("Edge color", Color) = (0,0,0,1)
		_Dimmer("Dimmer", float) = 0.40
		_Edgesize("_Edgesize", float) = 100
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
             
            #include "UnityCG.cginc"
 
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
 
            sampler2D _CameraDepthNormalsTexture;
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
             
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Threshold;
			float _Dimmer;
			float _Edgesize;
            fixed4 _EdgeColor;
 
            float4 getNormalsAndDepth(in float2 uv)
			{
                half3 normal;
                float depth;
                DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, uv), depth, normal);
                
				
				//depth = 1.0 / depth;
				
				return fixed4(normal, 1/(depth));
				
				//pelkkä syvyys, unohdetaan normaalit
				//return fixed4(depth, depth, depth, depth);
				
            }
 
			//käytännössä shader alkaa tästä.
			//v2f i viittaa rendattuun tekstuuriin jota käsitellään 
			//ja josta returnilla data viedään eteenpäin (forward rendering)
            fixed4 frag (v2f i) : SV_Target
            {
				//otetaan nykyinen pikseli talteen
                fixed4 col = tex2D(_MainTex, i.uv);
				
				//nykyisen pikselin kohdalta normal- ja depth-arvot ylös
                fixed4 orValue = getNormalsAndDepth(i.uv);
				float aB = orValue.a/_Edgesize;
				//float aB = 1.0 - (_Edgesize/orValue.a);
				
				float2 offsets[8] =
				{
                    float2(-0.5*aB, -0.5*aB),
                    float2(-1.0*aB,  0.0*aB),
                    float2(-0.5*aB,  0.5*aB),
                    float2( 0.0*aB, -1.0*aB),
                    float2( 0.0*aB,  1.0*aB),
                    float2( 0.5*aB, -0.5*aB),
                    float2( 1.0*aB,  0.0*aB),
                    float2( 0.5*aB,  0.5*aB)
                };
				
                fixed4 sampledValue = fixed4(0,0,0,0);
                for(int j = 0; j < 8; j++) {
                    sampledValue += getNormalsAndDepth(i.uv + offsets[j] * _MainTex_TexelSize.xy);
                }
                
				sampledValue /= 8;
				
                
				fixed4 dd = fixed4(_Dimmer, _Dimmer, _Dimmer, _Dimmer);
				fixed4 dom = (col * dd) * _EdgeColor;
				
				//palautetaan lineaarinen interpolointi jossa tulos lasketaan nykyisen etäisyyden ja viereisten etäisyyksien mukaan.
                return lerp(col, dom, step(_Threshold, length(orValue - sampledValue)));
            }
            ENDCG
        }
    }
}