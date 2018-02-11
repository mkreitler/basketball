#if !defined(NORMAL_MAPPED_FULL_V2F)
#define NORMAL_MAPPED_FULL_V2F
struct v2f
{
#ifdef TEX_INDEX_11
#define TEX_INDEX_12 TEXCOORD12
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_12
#else
#ifdef TEX_INDEX_10
#define TEX_INDEX_11 TEXCOORD11
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_11
#else
#ifdef TEX_INDEX_9
#define TEX_INDEX_10 TEXCOORD10
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_10
#else
#ifdef TEX_INDEX_8
#define TEX_INDEX_9 TEXCOORD9
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_9
#else
#ifdef TEX_INDEX_7
#define TEX_INDEX_8 TEXCOORD8
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_8
#else
#ifdef TEX_INDEX_6
#define TEX_INDEX_7 TEXCOORD7
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_7
#else
#ifdef TEX_INDEX_5
#define TEX_INDEX_6 TEXCOORD6
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_6
#else
#ifdef TEX_INDEX_4
#define TEX_INDEX_5 TEXCOORD5
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_5
#else
#ifdef TEX_INDEX_3
#define TEX_INDEX_4 TEXCOORD4
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_4
#else
#ifdef TEX_INDEX_2
#define TEX_INDEX_3 TEXCOORD3
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_3
#else
#ifdef TEX_INDEX_1
#define TEX_INDEX_2 TEXCOORD2
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_2
#else
#ifdef TEX_INDEX_0
#define TEX_INDEX_1 TEXCOORD1
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_1
#else
#define TEX_INDEX_0 TEXCOORD0
#define CURRENT_TEXCOORD TEX_INDEX_0
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
	float2 uv : TEXCOORD0;
	float4 vertex : SV_POSITION;

#ifdef TEX_INDEX_11
#define TEX_INDEX_12 TEXCOORD12
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_12
#else
#ifdef TEX_INDEX_10
#define TEX_INDEX_11 TEXCOORD11
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_11
#else
#ifdef TEX_INDEX_9
#define TEX_INDEX_10 TEXCOORD10
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_10
#else
#ifdef TEX_INDEX_8
#define TEX_INDEX_9 TEXCOORD9
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_9
#else
#ifdef TEX_INDEX_7
#define TEX_INDEX_8 TEXCOORD8
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_8
#else
#ifdef TEX_INDEX_6
#define TEX_INDEX_7 TEXCOORD7
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_7
#else
#ifdef TEX_INDEX_5
#define TEX_INDEX_6 TEXCOORD6
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_6
#else
#ifdef TEX_INDEX_4
#define TEX_INDEX_5 TEXCOORD5
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_5
#else
#ifdef TEX_INDEX_3
#define TEX_INDEX_4 TEXCOORD4
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_4
#else
#ifdef TEX_INDEX_2
#define TEX_INDEX_3 TEXCOORD3
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_3
#else
#ifdef TEX_INDEX_1
#define TEX_INDEX_2 TEXCOORD2
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_2
#else
#ifdef TEX_INDEX_0
#define TEX_INDEX_1 TEXCOORD1
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_1
#else
#define TEX_INDEX_0 TEXCOORD0
#define CURRENT_TEXCOORD TEX_INDEX_0
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
	//remove colour in edge cases 
	//#if !(defined(_LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX) && !defined(_LIGHT_COUNT_ONE))
	float4 colour:  CURRENT_TEXCOORD;
	//#endif
	//----------------- Screen Space UV Variables Texcoords 3 - 7 ------------------

#if defined(_LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX) || defined(_LIGHT_QUALITY_PIXEL_ONLY)
#if defined(_NORMAL_STYLE_VERTEX)
	#ifdef TEX_INDEX_11
		#define TEX_INDEX_12 TEXCOORD12
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_12
	#else
		#ifdef TEX_INDEX_10
		#define TEX_INDEX_11 TEXCOORD11
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_11
		#else
		#ifdef TEX_INDEX_9
		#define TEX_INDEX_10 TEXCOORD10
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_10
		#else
		#ifdef TEX_INDEX_8
		#define TEX_INDEX_9 TEXCOORD9
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_9
		#else
		#ifdef TEX_INDEX_7
		#define TEX_INDEX_8 TEXCOORD8
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_8
		#else
		#ifdef TEX_INDEX_6
		#define TEX_INDEX_7 TEXCOORD7
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_7
		#else
		#ifdef TEX_INDEX_5
		#define TEX_INDEX_6 TEXCOORD6
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_6
		#else
		#ifdef TEX_INDEX_4
		#define TEX_INDEX_5 TEXCOORD5
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_5
		#else
		#ifdef TEX_INDEX_3
		#define TEX_INDEX_4 TEXCOORD4
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_4
		#else
		#ifdef TEX_INDEX_2
		#define TEX_INDEX_3 TEXCOORD3
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_3
		#else
		#ifdef TEX_INDEX_1
		#define TEX_INDEX_2 TEXCOORD2
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_2
		#else
		#ifdef TEX_INDEX_0
		#define TEX_INDEX_1 TEXCOORD1
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_1
		#else
		#define TEX_INDEX_0 TEXCOORD0
		#define CURRENT_TEXCOORD TEX_INDEX_0
		#endif
		#endif
		#endif
		#endif
		#endif
		#endif
		#endif
		#endif
		#endif
		#endif
		#endif
	#endif
	//normal for normal mapped normals plus optionaly fog
	NORMAL_FOG_PACKED;
#else
	#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
		#ifdef TEX_INDEX_11
		#define TEX_INDEX_12 TEXCOORD12
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_12
		#else
		#ifdef TEX_INDEX_10
		#define TEX_INDEX_11 TEXCOORD11
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_11
		#else
		#ifdef TEX_INDEX_9
		#define TEX_INDEX_10 TEXCOORD10
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_10
		#else
		#ifdef TEX_INDEX_8
		#define TEX_INDEX_9 TEXCOORD9
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_9
		#else
		#ifdef TEX_INDEX_7
		#define TEX_INDEX_8 TEXCOORD8
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_8
		#else
		#ifdef TEX_INDEX_6
		#define TEX_INDEX_7 TEXCOORD7
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_7
		#else
		#ifdef TEX_INDEX_5
		#define TEX_INDEX_6 TEXCOORD6
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_6
		#else
		#ifdef TEX_INDEX_4
		#define TEX_INDEX_5 TEXCOORD5
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_5
		#else
		#ifdef TEX_INDEX_3
		#define TEX_INDEX_4 TEXCOORD4
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_4
		#else
		#ifdef TEX_INDEX_2
		#define TEX_INDEX_3 TEXCOORD3
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_3
		#else
		#ifdef TEX_INDEX_1
		#define TEX_INDEX_2 TEXCOORD2
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_2
		#else
		#ifdef TEX_INDEX_0
		#define TEX_INDEX_1 TEXCOORD1
		#undef CURRENT_TEXCOORD
		#define CURRENT_TEXCOORD TEX_INDEX_1
		#else
		#define TEX_INDEX_0 TEXCOORD0
		#define CURRENT_TEXCOORD TEX_INDEX_0
		#endif
		#endif
		#endif
		#endif
		#endif
		#endif
		#endif
		#endif
		#endif
		#endif
		#endif
		#endif
		//normal for normal mapped normals plus optionaly fog
		NORMAL_FOG_PACKED;
	#endif
#endif
	//precalculate vertex light directions
#ifdef TEX_INDEX_11
#define TEX_INDEX_12 TEXCOORD12
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_12
#else
#ifdef TEX_INDEX_10
#define TEX_INDEX_11 TEXCOORD11
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_11
#else
#ifdef TEX_INDEX_9
#define TEX_INDEX_10 TEXCOORD10
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_10
#else
#ifdef TEX_INDEX_8
#define TEX_INDEX_9 TEXCOORD9
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_9
#else
#ifdef TEX_INDEX_7
#define TEX_INDEX_8 TEXCOORD8
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_8
#else
#ifdef TEX_INDEX_6
#define TEX_INDEX_7 TEXCOORD7
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_7
#else
#ifdef TEX_INDEX_5
#define TEX_INDEX_6 TEXCOORD6
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_6
#else
#ifdef TEX_INDEX_4
#define TEX_INDEX_5 TEXCOORD5
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_5
#else
#ifdef TEX_INDEX_3
#define TEX_INDEX_4 TEXCOORD4
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_4
#else
#ifdef TEX_INDEX_2
#define TEX_INDEX_3 TEXCOORD3
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_3
#else
#ifdef TEX_INDEX_1
#define TEX_INDEX_2 TEXCOORD2
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_2
#else
#ifdef TEX_INDEX_0
#define TEX_INDEX_1 TEXCOORD1
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_1
#else
#define TEX_INDEX_0 TEXCOORD0
#define CURRENT_TEXCOORD TEX_INDEX_0
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
	float4 LightDirectionAndFalloff0 : CURRENT_TEXCOORD;
#if (defined(_LIGHT_COUNT_TWO) || defined(_LIGHT_COUNT_FOUR) ) && !defined(_LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX)
#ifdef TEX_INDEX_11
#define TEX_INDEX_12 TEXCOORD12
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_12
#else
#ifdef TEX_INDEX_10
#define TEX_INDEX_11 TEXCOORD11
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_11
#else
#ifdef TEX_INDEX_9
#define TEX_INDEX_10 TEXCOORD10
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_10
#else
#ifdef TEX_INDEX_8
#define TEX_INDEX_9 TEXCOORD9
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_9
#else
#ifdef TEX_INDEX_7
#define TEX_INDEX_8 TEXCOORD8
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_8
#else
#ifdef TEX_INDEX_6
#define TEX_INDEX_7 TEXCOORD7
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_7
#else
#ifdef TEX_INDEX_5
#define TEX_INDEX_6 TEXCOORD6
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_6
#else
#ifdef TEX_INDEX_4
#define TEX_INDEX_5 TEXCOORD5
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_5
#else
#ifdef TEX_INDEX_3
#define TEX_INDEX_4 TEXCOORD4
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_4
#else
#ifdef TEX_INDEX_2
#define TEX_INDEX_3 TEXCOORD3
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_3
#else
#ifdef TEX_INDEX_1
#define TEX_INDEX_2 TEXCOORD2
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_2
#else
#ifdef TEX_INDEX_0
#define TEX_INDEX_1 TEXCOORD1
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_1
#else
#define TEX_INDEX_0 TEXCOORD0
#define CURRENT_TEXCOORD TEX_INDEX_0
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
	float4 LightDirectionAndFalloff1 : CURRENT_TEXCOORD;
#if defined(_LIGHT_COUNT_FOUR) && !defined(_LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX)
#ifdef TEX_INDEX_11
#define TEX_INDEX_12 TEXCOORD12
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_12
#else
#ifdef TEX_INDEX_10
#define TEX_INDEX_11 TEXCOORD11
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_11
#else
#ifdef TEX_INDEX_9
#define TEX_INDEX_10 TEXCOORD10
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_10
#else
#ifdef TEX_INDEX_8
#define TEX_INDEX_9 TEXCOORD9
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_9
#else
#ifdef TEX_INDEX_7
#define TEX_INDEX_8 TEXCOORD8
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_8
#else
#ifdef TEX_INDEX_6
#define TEX_INDEX_7 TEXCOORD7
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_7
#else
#ifdef TEX_INDEX_5
#define TEX_INDEX_6 TEXCOORD6
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_6
#else
#ifdef TEX_INDEX_4
#define TEX_INDEX_5 TEXCOORD5
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_5
#else
#ifdef TEX_INDEX_3
#define TEX_INDEX_4 TEXCOORD4
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_4
#else
#ifdef TEX_INDEX_2
#define TEX_INDEX_3 TEXCOORD3
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_3
#else
#ifdef TEX_INDEX_1
#define TEX_INDEX_2 TEXCOORD2
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_2
#else
#ifdef TEX_INDEX_0
#define TEX_INDEX_1 TEXCOORD1
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_1
#else
#define TEX_INDEX_0 TEXCOORD0
#define CURRENT_TEXCOORD TEX_INDEX_0
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
	float4 LightDirectionAndFalloff2 : CURRENT_TEXCOORD;

#ifdef TEX_INDEX_11
#define TEX_INDEX_12 TEXCOORD12
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_12
#else
#ifdef TEX_INDEX_10
#define TEX_INDEX_11 TEXCOORD11
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_11
#else
#ifdef TEX_INDEX_9
#define TEX_INDEX_10 TEXCOORD10
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_10
#else
#ifdef TEX_INDEX_8
#define TEX_INDEX_9 TEXCOORD9
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_9
#else
#ifdef TEX_INDEX_7
#define TEX_INDEX_8 TEXCOORD8
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_8
#else
#ifdef TEX_INDEX_6
#define TEX_INDEX_7 TEXCOORD7
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_7
#else
#ifdef TEX_INDEX_5
#define TEX_INDEX_6 TEXCOORD6
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_6
#else
#ifdef TEX_INDEX_4
#define TEX_INDEX_5 TEXCOORD5
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_5
#else
#ifdef TEX_INDEX_3
#define TEX_INDEX_4 TEXCOORD4
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_4
#else
#ifdef TEX_INDEX_2
#define TEX_INDEX_3 TEXCOORD3
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_3
#else
#ifdef TEX_INDEX_1
#define TEX_INDEX_2 TEXCOORD2
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_2
#else
#ifdef TEX_INDEX_0
#define TEX_INDEX_1 TEXCOORD1
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_1
#else
#define TEX_INDEX_0 TEXCOORD0
#define CURRENT_TEXCOORD TEX_INDEX_0
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
	float4 LightDirectionAndFalloff3 : CURRENT_TEXCOORD;

#endif
#endif
#endif

#if defined(_LIGHT_QUALITY_VERTEX_ONLY) || defined(_LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX)
#ifdef TEX_INDEX_11
#define TEX_INDEX_12 TEXCOORD12
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_12
#else
#ifdef TEX_INDEX_10
#define TEX_INDEX_11 TEXCOORD11
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_11
#else
#ifdef TEX_INDEX_9
#define TEX_INDEX_10 TEXCOORD10
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_10
#else
#ifdef TEX_INDEX_8
#define TEX_INDEX_9 TEXCOORD9
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_9
#else
#ifdef TEX_INDEX_7
#define TEX_INDEX_8 TEXCOORD8
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_8
#else
#ifdef TEX_INDEX_6
#define TEX_INDEX_7 TEXCOORD7
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_7
#else
#ifdef TEX_INDEX_5
#define TEX_INDEX_6 TEXCOORD6
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_6
#else
#ifdef TEX_INDEX_4
#define TEX_INDEX_5 TEXCOORD5
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_5
#else
#ifdef TEX_INDEX_3
#define TEX_INDEX_4 TEXCOORD4
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_4
#else
#ifdef TEX_INDEX_2
#define TEX_INDEX_3 TEXCOORD3
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_3
#else
#ifdef TEX_INDEX_1
#define TEX_INDEX_2 TEXCOORD2
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_2
#else
#ifdef TEX_INDEX_0
#define TEX_INDEX_1 TEXCOORD1
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_1
#else
#define TEX_INDEX_0 TEXCOORD0
#define CURRENT_TEXCOORD TEX_INDEX_0
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
	float3 VertexLighting : CURRENT_TEXCOORD; //---------------- Vertex lighting


#endif

#if defined(_LIGHT_QUALITY_VERTEX_ONLY)
	#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2) //----------------- Fog Value for per vertex
	#ifdef TEX_INDEX_11
	#define TEX_INDEX_12 TEXCOORD12
	#undef CURRENT_TEXCOORD
	#define CURRENT_TEXCOORD TEX_INDEX_12
	#else
	#ifdef TEX_INDEX_10
	#define TEX_INDEX_11 TEXCOORD11
	#undef CURRENT_TEXCOORD
	#define CURRENT_TEXCOORD TEX_INDEX_11
	#else
	#ifdef TEX_INDEX_9
	#define TEX_INDEX_10 TEXCOORD10
	#undef CURRENT_TEXCOORD
	#define CURRENT_TEXCOORD TEX_INDEX_10
	#else
	#ifdef TEX_INDEX_8
	#define TEX_INDEX_9 TEXCOORD9
	#undef CURRENT_TEXCOORD
	#define CURRENT_TEXCOORD TEX_INDEX_9
	#else
	#ifdef TEX_INDEX_7
	#define TEX_INDEX_8 TEXCOORD8
	#undef CURRENT_TEXCOORD
	#define CURRENT_TEXCOORD TEX_INDEX_8
	#else
	#ifdef TEX_INDEX_6
	#define TEX_INDEX_7 TEXCOORD7
	#undef CURRENT_TEXCOORD
	#define CURRENT_TEXCOORD TEX_INDEX_7
	#else
	#ifdef TEX_INDEX_5
	#define TEX_INDEX_6 TEXCOORD6
	#undef CURRENT_TEXCOORD
	#define CURRENT_TEXCOORD TEX_INDEX_6
	#else
	#ifdef TEX_INDEX_4
	#define TEX_INDEX_5 TEXCOORD5
	#undef CURRENT_TEXCOORD
	#define CURRENT_TEXCOORD TEX_INDEX_5
	#else
	#ifdef TEX_INDEX_3
	#define TEX_INDEX_4 TEXCOORD4
	#undef CURRENT_TEXCOORD
	#define CURRENT_TEXCOORD TEX_INDEX_4
	#else
	#ifdef TEX_INDEX_2
	#define TEX_INDEX_3 TEXCOORD3
	#undef CURRENT_TEXCOORD
	#define CURRENT_TEXCOORD TEX_INDEX_3
	#else
	#ifdef TEX_INDEX_1
	#define TEX_INDEX_2 TEXCOORD2
	#undef CURRENT_TEXCOORD
	#define CURRENT_TEXCOORD TEX_INDEX_2
	#else
	#ifdef TEX_INDEX_0
	#define TEX_INDEX_1 TEXCOORD1
	#undef CURRENT_TEXCOORD
	#define CURRENT_TEXCOORD TEX_INDEX_1
	#else
	#define TEX_INDEX_0 TEXCOORD0
	#define CURRENT_TEXCOORD TEX_INDEX_0
	#endif
	#endif
	#endif
	#endif
	#endif
	#endif
	#endif
	#endif
	#endif
	#endif
	#endif
	#endif
		NORMAL_FOG_PACKED;
	#endif
#endif

	//---------------- Texture Emmisive ---------------------------------------
	//#if  (defined(_EMISSIVE_SIMPLE) || defined( MAPPED_EMISSIVE))&& ((defined(_LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX) && defined(_LIGHT_COUNT_ONE)) || defined (_LIGHT_QUALITY_PIXEL_ONLY))
	//				float3 EmmisiveLight  : TEXCOORD9;
	//#endif

	//----------------- Soft Particle factor ----------------------------------
#if defined(_SOFT_PARTICLE_HEIGHT)
#ifdef TEX_INDEX_11
#define TEX_INDEX_12 TEXCOORD12
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_12
#else
#ifdef TEX_INDEX_10
#define TEX_INDEX_11 TEXCOORD11
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_11
#else
#ifdef TEX_INDEX_9
#define TEX_INDEX_10 TEXCOORD10
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_10
#else
#ifdef TEX_INDEX_8
#define TEX_INDEX_9 TEXCOORD9
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_9
#else
#ifdef TEX_INDEX_7
#define TEX_INDEX_8 TEXCOORD8
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_8
#else
#ifdef TEX_INDEX_6
#define TEX_INDEX_7 TEXCOORD7
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_7
#else
#ifdef TEX_INDEX_5
#define TEX_INDEX_6 TEXCOORD6
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_6
#else
#ifdef TEX_INDEX_4
#define TEX_INDEX_5 TEXCOORD5
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_5
#else
#ifdef TEX_INDEX_3
#define TEX_INDEX_4 TEXCOORD4
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_4
#else
#ifdef TEX_INDEX_2
#define TEX_INDEX_3 TEXCOORD3
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_3
#else
#ifdef TEX_INDEX_1
#define TEX_INDEX_2 TEXCOORD2
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_2
#else
#ifdef TEX_INDEX_0
#define TEX_INDEX_1 TEXCOORD1
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_1
#else
#define TEX_INDEX_0 TEXCOORD0
#define CURRENT_TEXCOORD TEX_INDEX_0
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
	float SoftParticleFactor : CURRENT_TEXCOORD;
#endif
#if defined(_SOFT_PARTICLE_FULL)
#ifdef TEX_INDEX_11
#define TEX_INDEX_12 TEXCOORD12
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_12
#else
#ifdef TEX_INDEX_10
#define TEX_INDEX_11 TEXCOORD11
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_11
#else
#ifdef TEX_INDEX_9
#define TEX_INDEX_10 TEXCOORD10
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_10
#else
#ifdef TEX_INDEX_8
#define TEX_INDEX_9 TEXCOORD9
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_9
#else
#ifdef TEX_INDEX_7
#define TEX_INDEX_8 TEXCOORD8
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_8
#else
#ifdef TEX_INDEX_6
#define TEX_INDEX_7 TEXCOORD7
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_7
#else
#ifdef TEX_INDEX_5
#define TEX_INDEX_6 TEXCOORD6
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_6
#else
#ifdef TEX_INDEX_4
#define TEX_INDEX_5 TEXCOORD5
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_5
#else
#ifdef TEX_INDEX_3
#define TEX_INDEX_4 TEXCOORD4
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_4
#else
#ifdef TEX_INDEX_2
#define TEX_INDEX_3 TEXCOORD3
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_3
#else
#ifdef TEX_INDEX_1
#define TEX_INDEX_2 TEXCOORD2
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_2
#else
#ifdef TEX_INDEX_0
#define TEX_INDEX_1 TEXCOORD1
#undef CURRENT_TEXCOORD
#define CURRENT_TEXCOORD TEX_INDEX_1
#else
#define TEX_INDEX_0 TEXCOORD0
#define CURRENT_TEXCOORD TEX_INDEX_0
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
	float4 SoftParticleFactor : CURRENT_TEXCOORD;
#endif
};

#endif