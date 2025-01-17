
Shader "Custom/MCmesh"{
	Properties{
 _MainTex("Albedo", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        _DistortionStrength("Strength", Float) = 1.0
        _DistortionBlend("Blend", Range(0.0, 1.0)) = 0.5

        _SoftParticlesNearFadeDistance("Soft Particles Near Fade", Float) = 0.0
        _SoftParticlesFarFadeDistance("Soft Particles Far Fade", Float) = 1.0
        _CameraNearFadeDistance("Camera Near Fade", Float) = 1.0
        _CameraFarFadeDistance("Camera Far Fade", Float) = 2.0
        _CustomRadius ("CustomRadius", Range(0, 0.1)) = 0.05
        // Hidden properties
        [HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _ColorMode ("__colormode", Float) = 0.0
        [HideInInspector] _FlipbookMode ("__flipbookmode", Float) = 0.0
        [HideInInspector] _LightingEnabled ("__lightingenabled", Float) = 0.0
        [HideInInspector] _DistortionEnabled ("__distortionenabled", Float) = 0.0
        [HideInInspector] _EmissionEnabled ("__emissionenabled", Float) = 0.0
        [HideInInspector] _BlendOp ("__blendop", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
        [HideInInspector] _Cull ("__cull", Float) = 2.0
        [HideInInspector] _SoftParticlesEnabled ("__softparticlesenabled", Float) = 0.0
        [HideInInspector] _CameraFadingEnabled ("__camerafadingenabled", Float) = 0.0
        [HideInInspector] _SoftParticleFadeParams ("__softparticlefadeparams", Vector) = (0,0,0,0)
        [HideInInspector] _CameraFadeParams ("__camerafadeparams", Vector) = (0,0,0,0)
        [HideInInspector] _ColorAddSubDiff ("__coloraddsubdiff", Vector) = (0,0,0,0)
        [HideInInspector] _DistortionStrengthScaled ("__distortionstrengthscaled", Float) = 0.0
		
	}

	SubShader{
		
            Tags { "RenderType"="Opaque" "IgnoreProjector"="True" "PreviewType"="Plane" "PerformanceChecks"="False" }

            BlendOp [_BlendOp]
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            Cull [_Cull]
            ColorMask RGB

            GrabPass
            {
                Tags { "LightMode" = "Always" }
                "_GrabTexture"
            }


        Pass
        {
            Tags {"LightMode" = "ForwardBase"}
            ZWrite off
            Blend SrcAlpha OneMinusSrcAlpha
   

			CGPROGRAM

			#include "UnityCG.cginc"
			#include "Triangle.cginc"
			#pragma vertex vert
			#pragma fragment frag

			fixed4 _Color;
			 fixed _Cutoff;
	    float4x4 _LocalToWorld;
			//buffers
		  StructuredBuffer<Triangle> triangleRW;
                float4 vert(uint vertex_id: SV_VertexID, uint instance_id: SV_InstanceID) : SV_POSITION{
                    float3 position = triangleRW[vertex_id/3].v[vertex_id%3].vPosition;
		            unity_ObjectToWorld = _LocalToWorld;
					   return mul(UNITY_MATRIX_VP,  mul(unity_ObjectToWorld, float4(position, 1)));
			}
          
			fixed4 frag() : SV_TARGET{
		
				return fixed4(_Color.x,_Color.y,_Color.z,_Color.a*_Cutoff) ;
			}
			
			ENDCG
		}
	}
	Fallback "VertexLit"
}
