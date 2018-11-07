Shader "Unlit/Custom/feidi"
{
   
   Properties {
      _Conversion ("Conversion (RGB)", 2D) = "white" {}      
      _MainTex ("Main Texture", 2D) = "white" {}          
     _NormalxValue("Normal X Value", Float) = 1
     _Glossiness ("Smoothness", Range(0,1)) = 0.5  
     _Metallic ("Metallic", Range(0,1)) = 0.0
     _Color ("Color", Color) = (0.5,0.5,0.5,1)
    }
    SubShader {
       Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque"}
       LOD 200
     
       Cull Off
 
       Pass
       {
         ZWrite On
         ColorMask 0
 
         CGPROGRAM
         #pragma vertex vert
         #pragma fragment frag
         #include "UnityCG.cginc"
     
         float3 _PlaneNormal;
 
         uniform float _xValue;
         uniform float _yValue;
         uniform float _zValue;
 
         uniform float _NormalxValue;
         uniform float _NormalyValue;
         uniform float _NormalzValue;
 
         uniform half _Radius;
     
         float3 _Origin;
 
         uniform float _xIValue;
         uniform float _yIValue;
         uniform float _zIValue;
 
         uniform float _signe;
         uniform float _InOut;
 
         struct v2f {
           float4 pos : SV_POSITION;
           float4 worldPos : world ;
         };
 
         v2f vert (appdata_base v)
         {
           v2f o;      
           o.worldPos =  mul(unity_ObjectToWorld, v.vertex);
           o.pos = UnityObjectToClipPos (v.vertex); //o.pos = 0 ==> no Depth
           return o;
         }
         half4 frag (v2f i) : COLOR
         {
           _PlaneNormal = float3(_NormalxValue,_NormalyValue,_NormalzValue);
           _PlaneNormal = normalize(_PlaneNormal);
           half dist = (i.worldPos.x * _PlaneNormal.x) + (i.worldPos.y * _PlaneNormal.y) + (i.worldPos.z * _PlaneNormal.z)
                   - (_xValue * _PlaneNormal.x) - (_yValue * _PlaneNormal.y) - (_zValue * _PlaneNormal.z)
                   / sqrt( pow(_PlaneNormal.x, 2) + pow(_PlaneNormal.y, 2) + pow(_PlaneNormal.z,2));
         
           if(any(dist < 0))
           {
               discard;
			   //return half4 (0,0,0,1);
           }
 
           _Origin = float3( _xIValue,_yIValue, _zIValue);
       
           half distIarea = distance( i.worldPos, _Origin);
 
           if(_InOut == 1)
           {  
               clip (_signe * (distIarea - _Radius));
           }
 
           return half4 (0,0,0,1);
         }
         ENDCG  
       }
       CGPROGRAM
 
         #pragma surface surf Standard alpha
         #pragma target 3.0
       
         struct Input {
             float2 uv_MainTex;    
             float2 uv_BumpMap;      
             float2 uv_Conversion;    
             float3 worldPos;  
         };  
         half _Glossiness;
         half _Metallic;
         fixed4 _Color;
 
         float3 _PlaneNormal;
 
         uniform float _xValue;
         uniform float _yValue;
         uniform float _zValue;
 
         uniform float _NormalxValue;
         uniform float _NormalyValue;
         uniform float _NormalzValue;
 
         uniform half _Radius;
     
         uniform float3 _Origin;
 
         uniform float _xIValue;
         uniform float _yIValue;
         uniform float _zIValue;
 
         uniform float _signe; // 1 ou -1
         uniform float _InOut;  // 0 ou 1
 
         sampler2D _MainTex;
         sampler2D _BumpMap;
         sampler2D _Conversion;
       
         void surf (Input IN, inout SurfaceOutputStandard o) {
 
           _PlaneNormal = float3(_NormalxValue,_NormalyValue,_NormalzValue);
           _PlaneNormal = normalize(_PlaneNormal);
         
           half dist = (IN.worldPos.x * _PlaneNormal.x) + (IN.worldPos.y * _PlaneNormal.y) + (IN.worldPos.z * _PlaneNormal.z)
                       - (_xValue * _PlaneNormal.x) - (_yValue * _PlaneNormal.y) - (_zValue * _PlaneNormal.z)
               / sqrt( pow(_PlaneNormal.x, 2) + pow(_PlaneNormal.y, 2) + pow(_PlaneNormal.z,2));
         
 
 
           if(any(dist < 0))
           {
               discard;
           }
           _Origin = float3( _xIValue,_yIValue, _zIValue);
       
       
           half distIarea = distance( IN.worldPos, _Origin);
 
           {  
           if(_InOut == 1)
               clip (_signe * (distIarea - _Radius));
           }
         
		   fixed4 col = tex2D(_MainTex, IN.uv_MainTex);
		  
           o.Albedo = _Color * col;
           o.Metallic = _Metallic;
           o.Smoothness = _Glossiness;
           o.Alpha = _Color.a;  
 
         }
          ENDCG    
    }
    Fallback Off
  }
 