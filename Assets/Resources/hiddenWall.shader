   
Shader "Custom/Wall/WallOccluder"
{
 
 Category
 {
       Lighting Off
       Cull Back
       zwrite off
       
       SubShader
	   {
		   Colormask 0
		   
			Stencil
			{
				Ref 1
				Comp always
				Pass replace
			}
		   
		   Tags
		   {
			"Queue"="Geometry"
			"LightMode" = "Always"
			"RenderType"="Opaque"
			"IgnoreProjector"="True"
			}
			
			Pass
			{
				
			}
        }
	}
}