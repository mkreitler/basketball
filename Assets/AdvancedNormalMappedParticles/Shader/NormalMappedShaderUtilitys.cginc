// Upgrade NOTE: replaced 'defined defined' with 'defined (defined)'

// Upgrade NOTE: replaced 'defined #if' with 'defined (#if)'

#if !defined(NORMALMAPPED_SHADER_UTILITYS)
#define NORMALMAPPED_SHADER_UTILITYS

#include "UnityCG.cginc"

//--------------------- Fog Compute Defines --------------------
#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
	#define CALCULATE_PARTICLE_FOG_FACTOR(Distance,Destination)  UNITY_CALC_FOG_FACTOR(Distance); Destination = unityFogFactor
#else
	//define null value to avoid calculations when not needed
	#define CALCULATE_PARTICLE_FOG_FACTOR(Distance,Destination)
#endif

#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
	//#define APPLY_PARTICLE_FOG(FogFactor, Destination)   UNITY_FOG_LERP_COLOR(Destination, unity_FogColor, FogFactor)
	#define APPLY_PARTICLE_FOG(FogFactor, Destination) Destination.rgb = lerp((Destination).rgb, (Destination).rgb, float(saturate(FogFactor)))
#else
	//define null value to avoid calculations when not needed
	#define APPLY_PARTICLE_FOG(FogFactor,Destination)
#endif

#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
	#define CALC_AND_APPLY_PARTICLE_FOG(Distance, Destination) UNITY_CALC_FOG_FACTOR(Distance); UNITY_FOG_LERP_COLOR(Destination, unity_FogColor, unityFogFactor)
#else
	//define null value to avoid calculations when not needed
	#define CALC_AND_APPLY_PARTICLE_FOG(Distance,Destination)
#endif


//---------------- these tools reduce the number of needed tex cords ------------------------------------

//---------------- Texcoord Packing tool



//---------------- Get The Particles Colour

//if the shader has no emissive 
#if !defined(_EMISSIVE_SIMPLE) 
	#define PARTICLE_COLOUR(i) i.colour

	#define PARTICLE_RGB(i) i.colour.rgb

	#define PARTICLE_A(i) i.colour.a
#endif
#if defined(_EMISSIVE_SIMPLE)
	#define PARTICLE_COLOUR(i) float4(_ParticleColour.rgb,i.colour.a)
	#define PARTICLE_RGB(i)  _ParticleColour.rgb 
	#define PARTICLE_A(i)  i.colour.a 
#endif




#if  defined(_LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX) || defined(_LIGHT_QUALITY_PIXEL_ONLY)
	#if defined(_NORMAL_STYLE_VERTEX) 	
		#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
			#define NORMAL_FOG_PACKED float4 PackedNormalAndFog : CURRENT_TEXCOORD
			#define ACCESS_NORMAL(i)i.PackedNormalAndFog.xy
			#define ACCESS_FOG(i) i.PackedNormalAndFog.z
			//#define ACCESS_ALPHA(i) i.PackedNormalAndFog.w
		#else
			#define NORMAL_FOG_PACKED float2 PackedNormal:CURRENT_TEXCOORD
			#define ACCESS_NORMAL(i) i.PackedNormal.xy
			#define ACCESS_FOG(i) 0
			//#define ACCESS_ALPHA(i) i.PackedNormalAndFog.z
		#endif
	#else
		#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
			#define NORMAL_FOG_PACKED  float1 Fog  : CURRENT_TEXCOORD
			#define ACCESS_NORMAL(i) 0
			#define ACCESS_FOG(i) i.Fog
			//#define ACCESS_ALPHA(i) i.colour.a
		#else
			#define NORMAL_FOG_PACKED
			#define ACCESS_NORMAL(i) 0
			#define ACCESS_FOG(i) 0 
			//#define ACCESS_ALPHA(i) i.colour.a
		#endif
	#endif
#else
	#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
	#define NORMAL_FOG_PACKED  float1 Fog  : CURRENT_TEXCOORD
	#define ACCESS_NORMAL(i) 0
	#define ACCESS_FOG(i) i.Fog
	//#define ACCESS_ALPHA(i) i.colour.a
	#else
	#define NORMAL_FOG_PACKED
	#define ACCESS_NORMAL(i) 0
	#define ACCESS_FOG(i) 0 
	//#define ACCESS_ALPHA(i) i.colour.a
	#endif
#endif
#endif