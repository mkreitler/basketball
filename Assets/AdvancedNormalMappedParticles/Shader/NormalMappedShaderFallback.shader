// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// KREITLER: Fixed line endings.

// Upgrade NOTE: replaced 'defined FRAG_FUNCTION_CALL' with 'defined (FRAG_FUNCTION_CALL)'

Shader "Hidden / Encap / NormalMappedParticle / NormalMappedShaderFallback"
{
	Properties
	{
		_MainTex ("Albedo", 2D) = "white" {}
		_NormalMap("Normal", 2D) = "white" {}
		_LightWrapAround("Light Bleadthrough", Range(0,1)) = 0.1
		_NormalSharpness("Bumpyness",Float) = 1
		_ShadowEffectMultiplyer("Shadow Effect Multiplyer",Range(0,1)) = 1
		_NormalBend("NormalBend",Float) = 1
		_ParticleColour("Colour", Color) = (1,1,1,1)
		_InvFade("Soft Particles Factor", Range(0.01,10.0)) = 1.0
		_HeightClippedOffset("Fade In Height",Float) = 0
		_HeightClippedFadeRate("Fade In Rate",Range(-10,10)) = 1
		_DistanceFadeStart("Distance Fade Start",Float) = 1
		_DistanceFadeEnd("Distance Fade Start",Float) = 1
		_DistanceFadeRate("Distance Fade Rate",Range(-4,4)) = 1
		_FixedAmbientColour("Fixed Ambient Colour", Color) = (0,0,0,0)
		_ShadowDitherTex("Shadow Dither Patern", 2D) = "white" {}
		_fShadowClip("At what alpha do the shadows start", Float) = 0.5
		_fShadowMultiplyer("Dithered shadow multiplyer", Float) = 0.5
		_UVRowsAndColumbs("UV Rows And Columbs", Vector) = (1,1,0,0)
		_fNonSquareUVFactor("NonSquareUVFactor", Vector) = (1,1,0,0)

		[KeywordEnum(vertex_only, One_Pixel_Many_Vertex, Pixel_Only)]  _Light_Quality("Light quality",Float) = 0
		[KeywordEnum(vertex, Pixel)]  _Normal_Style("Normal Format",Float) = 0
		[KeywordEnum(One, Two, Four)]  _Light_Count("Light Count",Float) = 0
		[KeywordEnum(Fixed, Wrap_Around)]  _Light_Style("Shadow Edge Calculation",Float) = 0
		[KeywordEnum(Fixed,Variable)]  _Shadowing_Effect("Shadow Effect Calculation",Float) = 0
		[KeywordEnum(None, Simple)]  _Emissive("Emissive",Float) = 0
		[KeywordEnum(Hard, Height, Full)]  _Soft_Particle("Soft Particle",Float) = 0
		[KeywordEnum(Off, On)]  _Distance_Fade("Distance Fade",Float) = 0
		[KeywordEnum(Fade, Erode)]  _Alpha_Style("Alpha Fade Style",Float) = 0
		[KeywordEnum(Off, Vertex, Pixel)]  _Spotlight_Quality("Spotlight Quality",Float) = 0
		[KeywordEnum(Off, Fixed )] _Ambient_Light("Ambient Light Quality",Float) = 0
		[KeywordEnum(Off, Stencil , Dithered)] _Cast_Shadows("Shadow Casting Options",Float) = 0
		[KeywordEnum(Off,On)] _Non_Square_Uv("Using Non Square Particle Texture",Float) = 0
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" }


		Pass
		{
			
			Tags{ "LightMode" = "Vertex"  "Queue" = "Transparent" }
			Tags{ "Queue" = "Transparent"   "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
//#define CALC_ATTENUATION_USING_LOOKUP
//#define _NORMAL_STYLE_VERTEX
//#define _LIGHT_QUALITY_VERTEX_ONLY
// Upgrade NOTE: excluded shader from DX11 and Xbox360 because it uses wrong array syntax (type[size] name)
//#pragma exclude_renderers d3d11 xbox360
//---------------------------------------- Shader Compilation Options ----------------------------------------------
						///		#pragma multi_compile _OVERLAY_NONE, _OVERLAY_ADD, _OVERLAY_MULTIPLY
/*-----Light computation*/		#pragma multi_compile _LIGHT_QUALITY_VERTEX_ONLY _LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX _LIGHT_QUALITY_PIXEL_ONLY 
/*----------------Normal*/		#pragma shader_feature _NORMAL_STYLE_VERTEX _NORMAL_STYLE_PIXEL
/*------- -ambient light*/		#pragma shader_feature __ _AMBIENT_LIGHT_FIXED
/*---dynamic light count*/		#pragma multi_compile _LIGHT_COUNT_ONE _LIGHT_COUNT_TWO _LIGHT_COUNT_FOUR 
/*-----Light Attenuation*/		#pragma shader_feature __ _LIGHT_STYLE_WRAP_AROUND
								#pragma shader_feature __ _SHADOWING_EFFECT_VARIABLE
/*--------------emissive*/	 	#pragma shader_feature __ _EMISSIVE_SIMPLE 
/*--------soft particles*/	 	#pragma multi_compile _SOFT_PARTICLE_HARD _SOFT_PARTICLE_HEIGHT _SOFT_PARTICLE_FULL
/*particel distance fade*/		#pragma shader_feature __ _DISTANCE_FADE_ON
/*------alpha fade style*/		#pragma shader_feature __ _ALPHA_STYLE_ERODE
///*---Shadow cast options*/	 	#pragma multi_compile _CAST_SHADOWS_OFF CAST_CUTOUT_SHADOWS _CAST_SHADOWS_DITHERED
/*-----spotlight options*/	 	#pragma multi_compile _SPOTLIGHT_QUALITY_OFF _SPOTLIGHT_QUALITY_VERTEX _SPOTLIGHT_QUALITY_PIXEL
/*-----Non square uv adjuster*/	#pragma shader_feature __ _NON_SQUARE_UV_ON

//if using one light and one pixel light change the definition to pixel only lights
#if defined(_LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX) && defined(_LIGHT_COUNT_ONE)
#undef _LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX
#define _LIGHT_QUALITY_PIXEL_ONLY
#endif





			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#pragma multi_compile_particles

//force height normals only
#ifdef _SOFT_PARTICLE_FULL
#undef _SOFT_PARTICLE_FULL
#define _SOFT_PARTICLE_HARD
#endif

			//force vertex normals only
#if defined(_LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX) || defined ( _LIGHT_QUALITY_PIXEL_ONLY )
#undef _LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX
#undef _LIGHT_QUALITY_PIXEL_ONLY
#define _LIGHT_QUALITY_VERTEX_ONLY
#endif

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "NormalMappedShaderUtilitys.cginc"
			#include "NormalMapFullV2F.cginc"
			#pragma target 2.0


			//upgrade the shader model for more complex shaders


			//----------------------------------- Static Variables ---------------------------------------
			sampler2D _MainTex;
			sampler2D _NormalMap;
			float4 _MainTex_ST;
			float _BrightnessMultiplyer;
			float _NormalSharpness;
			float _ShadowEffectMultiplyer;
			float _NormalBend;
			float _LightWrapAround; //what percent of light is not effected by the normalmapping
			float4 _ParticleColour; //the colour to tint the particles when using emissive 

			float4 _FixedAmbientColour; //fixed ambient light colour

			//light attenuation lookup
#ifdef CALC_ATTENUATION_USING_LOOKUP
			sampler2D _LightTextureB0;
#endif

#if defined(_SOFT_PARTICLE_FULL)
			sampler2D _CameraDepthTexture;
			float _InvFade; //Soft particle factor
#endif 
			float _HeightClippedOffset;
			float _HeightClippedFadeRate;

			float _DistanceFadeStart;
			float _DistanceFadeRate;
			float4 _fNonSquareUVFactor;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
				float4 normal : NORMAL;
			};







//------------------------------------------- Lighting funcitons ---------------------------------------------------------------------------


			//calculate soft particle factor
			inline float CalculateSoftParticleFactor(v2f i)
			{
#if defined(_SOFT_PARTICLE_FULL)
				return saturate(_InvFade * (LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.SoftParticleFactor)))) - i.SoftParticleFactor.z));

#endif
#if defined(_SOFT_PARTICLE_HEIGHT)
				return saturate(i.SoftParticleFactor);
#endif
#if !defined(_SOFT_PARTICLE_HEIGHT) && !defined(_SOFT_PARTICLE_HEIGHT)
				return 0;
#endif
			}

			inline float CalculateNormalAttenuation(float3 LightDirectionm, float3 Normal)
			{

#ifdef _LIGHT_STYLE_WRAP_AROUND
				//calculate normal lighting value
#ifdef _SHADOWING_EFFECT_VARIABLE
				return  (saturate((dot(LightDirectionm, Normal)* _LightWrapAround) + 1 - _LightWrapAround) * _ShadowEffectMultiplyer) + (1 - _ShadowEffectMultiplyer);
#else
				return  saturate((dot(LightDirectionm, Normal)* _LightWrapAround) + 1 - _LightWrapAround);
#endif
#else
#ifdef _SHADOWING_EFFECT_VARIABLE
				return  (saturate(dot(LightDirectionm, Normal)) * _ShadowEffectMultiplyer) + (1 - _ShadowEffectMultiplyer);
#else
				return  saturate(dot(LightDirectionm, Normal)) ;
#endif
#endif

			}

			//calculate spotlight shadow function
			inline float CalculateSpotlightAttenuation(float3 LightDirection, float3 SpotlightDirection, float2 SpotlightAngleValues)
			{
				return saturate((saturate(dot(LightDirection, SpotlightDirection)) - SpotlightAngleValues.x)  * SpotlightAngleValues.y);
			}

			inline float CalculateDistanceAttenuation(float LengthSquared, float4 Attenuation)
			{

#ifdef CALC_ATTENUATION_USING_LOOKUP
	#if defined (FRAG_FUNCTION_CALL)
				//return tex1D(_LightTextureB0, (LengthSquared * Attenuation.z) / Attenuation.w);
				return tex2D(_LightTextureB0,float2( (LengthSquared ) / Attenuation.w),0).UNITY_ATTEN_CHANNEL;
	#else
				//return tex1Dlod(_LightTextureB0, float4((LengthSquared * Attenuation.z) / Attenuation.w,0,0,0));
				//return tex2Dlod(_LightTextureB0, float4((LengthSquared ) / Attenuation.w, 0, 0, 0));
				return saturate(1.0 / (1.0 + (LengthSquared * Attenuation.z)) *  (1 - (LengthSquared / Attenuation.w)));
	#endif
#else
				//return 1.0 / (1.0 + (LengthSquared * Attenuation.z));
				return saturate(1.0 / (1.0 + (LengthSquared * Attenuation.z)) *  (1 - (LengthSquared / Attenuation.w)));

#endif
			}

			//calculate lighting for a single
			 inline float CalculateVertexLight(float3 ViewSpaceNormal, float3 ViewSpacePos, float4 ViewSpaceLightPos, float4 ViewSpaceSpotlightDirection, float4 Attenuation )
			{
				
				//calc direction to light
				float3 DirectionToLight = ViewSpaceLightPos.xyz - ViewSpacePos.xyz * ViewSpaceLightPos.w;
				float lengthSquared = dot(DirectionToLight, DirectionToLight);
				DirectionToLight *= rsqrt(lengthSquared); //normalise direction to light

				

				//amount of light attenuation
				float LightAttenuation = CalculateDistanceAttenuation(lengthSquared  * ViewSpaceLightPos.w, Attenuation);
				
				//return 1;
#if defined(_SPOTLIGHT_QUALITY_PIXEL) || defined(_SPOTLIGHT_QUALITY_VERTEX)
				//calculate spotlight attenuation
				LightAttenuation *= CalculateSpotlightAttenuation(DirectionToLight, ViewSpaceSpotlightDirection.xyz, Attenuation.xy);

#endif

				LightAttenuation *= CalculateNormalAttenuation(DirectionToLight, ViewSpaceNormal);

				//return result
				return LightAttenuation;
			}

			//calculate light falloff to pass to the pixel shader light multiplyer
			inline float4 CalculateLightDirectionAndFalloff( float3 ViewSpacePos, float4 ViewSpaceLightPos, float4 ViewSpaceSpotlightDirection, float4 Attenuation)
			{

				//calc direction to light
				float3 DirectionToLight = ViewSpaceLightPos.xyz - ViewSpacePos.xyz * ViewSpaceLightPos.w;
				float lengthSquared = dot(DirectionToLight, DirectionToLight);
				DirectionToLight *= rsqrt(lengthSquared);

#if defined(_SPOTLIGHT_QUALITY_VERTEX)
				//calculate spotlight attenuation per vertex
				return float4(DirectionToLight, CalculateDistanceAttenuation(lengthSquared * ViewSpaceLightPos.w, Attenuation) * CalculateSpotlightAttenuation(DirectionToLight, ViewSpaceSpotlightDirection.xyz, Attenuation.xy));
#else
				//amount of light attenuation
				return float4(DirectionToLight, CalculateDistanceAttenuation(lengthSquared * ViewSpaceLightPos.w, Attenuation));
#endif

			}

			//----------------------------------------- Vert To Frag ---------------------------------------------------------------------			
			v2f vert (appdata v)
			{
#undef FRAG_FUNCTION_CALL //indicate that the shader is runing in the vertex calculator

				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				//calculate soft particle values 
#if defined(_SOFT_PARTICLE_HEIGHT)
				o.SoftParticleFactor = (v.vertex.y - _HeightClippedOffset) * _HeightClippedFadeRate;
#endif
#if defined(_SOFT_PARTICLE_FULL) 
				o.SoftParticleFactor = ComputeScreenPos(o.vertex);
				COMPUTE_EYEDEPTH(o.SoftParticleFactor.z);
#endif

				//calculate viewspace normal
				float3 ViewSpaceNormal =  mul(UNITY_MATRIX_MV, v.normal.xyz);

				//calculate viewspace posittion
				float3 ViewspacePos = mul(UNITY_MATRIX_MV, v.vertex).xyz;

				//store view space normal if it is going to be used in the fragment shader
#if (defined(VARIABLE_BENT_PER_PIXEL_NORMAL) || defined(_NORMAL_STYLE_VERTEX)) && (defined(_LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX) || defined(_LIGHT_QUALITY_PIXEL_ONLY))
				ACCESS_NORMAL(o) = ViewSpaceNormal.xy;
#endif
//----------------------------------------- Vertex Lighting ---------------------------------------------------------------------

#if  defined(_LIGHT_QUALITY_VERTEX_ONLY) || defined(_LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX)
#if defined (_LIGHT_COUNT_ONE) ||  defined(_LIGHT_COUNT_TWO) || defined( _LIGHT_COUNT_FOUR) 

				//skip first light if it is being handled by the per pixel shader 
#if !defined(_LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX)
				//calculate light contribution
				float3 LightingColour = unity_LightColor[0].rgb * CalculateVertexLight(ViewSpaceNormal, ViewspacePos, unity_LightPosition[0], unity_SpotDirection[0], unity_LightAtten[0]);
				
#endif
#if defined( _LIGHT_COUNT_TWO) || defined( _LIGHT_COUNT_FOUR)
#if defined(_LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX)
				float3 LightingColour = unity_LightColor[1].rgb * CalculateVertexLight(ViewSpaceNormal, ViewspacePos, unity_LightPosition[1], unity_SpotDirection[1], unity_LightAtten[1]);
#else
				LightingColour += unity_LightColor[1].rgb * CalculateVertexLight(ViewSpaceNormal, ViewspacePos, unity_LightPosition[1], unity_SpotDirection[1], unity_LightAtten[1]);
#endif
#ifdef _LIGHT_COUNT_FOUR
				LightingColour += unity_LightColor[2].rgb * CalculateVertexLight(ViewSpaceNormal, ViewspacePos, unity_LightPosition[2], unity_SpotDirection[2], unity_LightAtten[2]);
				LightingColour += unity_LightColor[3].rgb * CalculateVertexLight(ViewSpaceNormal, ViewspacePos, unity_LightPosition[3], unity_SpotDirection[3], unity_LightAtten[3]);


#endif
#endif
#endif

				//-=------------------------ Add Ambent Light --------------------------------
#if defined(_AMBIENT_LIGHT_FIXED)
				LightingColour += _FixedAmbientColour;
#endif
				//store ombined vertex lighting 
				o.VertexLighting = LightingColour;

#endif
				//-------------------------------------- Pass data for per pixel normal mapping ------------------------------------

				//if single normal mapped particel
#if defined(_LIGHT_QUALITY_PIXEL_ONLY) || defined(_LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX)
#if defined (_LIGHT_COUNT_ONE) ||  defined(_LIGHT_COUNT_TWO) || defined( _LIGHT_COUNT_FOUR) 
				o.LightDirectionAndFalloff0 = CalculateLightDirectionAndFalloff(ViewspacePos, unity_LightPosition[0], unity_SpotDirection[0], unity_LightAtten[0]);
#if (defined(_LIGHT_COUNT_TWO) || defined( _LIGHT_COUNT_FOUR)) && defined(_LIGHT_QUALITY_PIXEL_ONLY) 
				o.LightDirectionAndFalloff1= CalculateLightDirectionAndFalloff(ViewspacePos, unity_LightPosition[1], unity_SpotDirection[1], unity_LightAtten[1]);
#if  defined( _LIGHT_COUNT_FOUR) && defined(_LIGHT_QUALITY_PIXEL_ONLY) 
				o.LightDirectionAndFalloff2 = CalculateLightDirectionAndFalloff(ViewspacePos, unity_LightPosition[2], unity_SpotDirection[2], unity_LightAtten[2]);
				o.LightDirectionAndFalloff3 = CalculateLightDirectionAndFalloff(ViewspacePos, unity_LightPosition[3], unity_SpotDirection[3], unity_LightAtten[3]);
#endif
#endif
#endif
#endif



				//------------------------------------- Pass emmisive data ---------------------------------------------------------

#if defined(_EMISSIVE_SIMPLE) || defined( MAPPED_EMISSIVE)
#if (defined(_LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX) && defined(_LIGHT_COUNT_ONE)) || defined (_LIGHT_QUALITY_PIXEL_ONLY)
				o.colour = v.color;
#else
				o.colour = v.color;
				o.VertexLighting += v.color.rgb;
#endif
#else
				o.colour = v.color;
#endif

				//------------------------------------ Apply distance fade and hide -----------------------------------------------
#if defined(_DISTANCE_FADE_ON)
				PARTICLE_A(o) *= saturate((o.vertex.z - _DistanceFadeStart) * _DistanceFadeRate);
				//float test = ACCESS_ALPHA(o) = saturate((o.vertex.z - _DistanceFadeStart) * _DistanceFadeRate);

				//hide particle when range limit reached
				o.vertex = lerp(float4(0, 0, -100, 0), o.vertex, saturate( o.colour.a * 10000000 ));

#endif

				CALCULATE_PARTICLE_FOG_FACTOR(v.vertex.z, ACCESS_FOG(o));

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
#define FRAG_FUNCTION_CALL //indicate that the shader is runing in the fragment calculator

#if defined(_LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX)  || defined(_LIGHT_QUALITY_PIXEL_ONLY)

#if defined(_NORMAL_STYLE_VERTEX)
				float3 NormalMap = float3(ACCESS_NORMAL(i) , sqrt(1 - dot(ACCESS_NORMAL(i) ,ACCESS_NORMAL(i))));
#endif 

#if defined(_NORMAL_STYLE_PIXEL)

				

				//get normal texture
				float3 NormalMap = UnpackNormal(tex2D(_NormalMap, i.uv));

				//factor in non square uv
#if defined(_NON_SQUARE_UV_ON)
				//calc rotation values 
				float2 DeltaUV = normalize(ddx(i.uv *_fNonSquareUVFactor.xy));
#else
				//calc rotation values 
				float2 DeltaUV = normalize(ddx(i.uv));
#endif

				//rotate normal map to screen space  
				NormalMap = float3((NormalMap.x * DeltaUV.x) + (NormalMap.y * DeltaUV.y), (NormalMap.x * -DeltaUV.y) + (NormalMap.y *  DeltaUV.x), NormalMap.z);

#endif
#ifdef VARIABLE_PER_PIXEL_NORMAL
				NormalMap = lerp(float3(0,0,1),NormalMap, _NormalSharpness);
#endif

				// sample the texture
				float4 Albedo = tex2D(_MainTex,i.uv);

				//-----------------------calculate lighting for a single per pixel ---------------------------------------
#if defined(_LIGHT_QUALITY_ONE_PIXEL_MANY_VERTEX)

				//calculate normal lighting value
				float lightValue = CalculateNormalAttenuation(i.LightDirectionAndFalloff0.xyz,NormalMap) * i.LightDirectionAndFalloff0.w;

				//calculate spotlight effect
#if defined(_SPOTLIGHT_QUALITY_PIXEL)
				
				lightValue *= CalculateSpotlightAttenuation(i.LightDirectionAndFalloff0.xyz,unity_SpotDirection[0].xyz,unity_LightAtten[0].xy);
#endif

#if defined(_SOFT_PARTICLE_HEIGHT) || defined( _SOFT_PARTICLE_FULL)
				//calculate soft particle factor
				PARTICLE_A(i) *= CalculateSoftParticleFactor(i);
#endif

#if defined(_ALPHA_STYLE_ERODE)
				Albedo = float4(Albedo.rgb * ((unity_LightColor[0]  * lightValue) + i.VertexLighting) * PARTICLE_RGB(i), saturate( Albedo.a - (1 - PARTICLE_A(i)))) ;
#else
				Albedo = float4(Albedo.rgb * ((unity_LightColor[0] * lightValue) + i.VertexLighting), Albedo.a) * PARTICLE_COLOUR(i);
#endif

				// apply fog
				APPLY_PARTICLE_FOG(ACCESS_FOG(i), Albedo.rgb);

				return Albedo;
#endif


//---------------------------------------------- Pixel only lighting ----------------------------------------------
#if defined(_LIGHT_QUALITY_PIXEL_ONLY) 
				//calculate per pixel lighting for the first 2 lights

				
//---------------- Calculate first pixle light value --------------------
#if defined(_SPOTLIGHT_QUALITY_PIXEL)//calculate spotlight effect
				float3 LightingColour = unity_LightColor[0] * (CalculateNormalAttenuation(i.LightDirectionAndFalloff0.xyz, NormalMap) * i.LightDirectionAndFalloff0.w * CalculateSpotlightAttenuation(i.LightDirectionAndFalloff0.xyz, unity_SpotDirection[0].xyz, unity_LightAtten[0].xy));
#else
				float3 LightingColour = unity_LightColor[0] * (CalculateNormalAttenuation(i.LightDirectionAndFalloff0.xyz, NormalMap) * i.LightDirectionAndFalloff0.w);
#endif

				//---------------- Calculate Second pixle light value --------------------
#if defined(_LIGHT_COUNT_TWO) || defined(_LIGHT_COUNT_FOUR)
#if defined(_SPOTLIGHT_QUALITY_PIXEL)//calculate spotlight effect
				LightingColour += unity_LightColor[1] * (CalculateNormalAttenuation(i.LightDirectionAndFalloff1.xyz, NormalMap) * i.LightDirectionAndFalloff1.w * CalculateSpotlightAttenuation(i.LightDirectionAndFalloff1.xyz, unity_SpotDirection[1].xyz, unity_LightAtten[1].xy));
#else
				LightingColour += unity_LightColor[1] * (CalculateNormalAttenuation(i.LightDirectionAndFalloff1.xyz, NormalMap) * i.LightDirectionAndFalloff1.w);
#endif
#endif

#if defined(_LIGHT_COUNT_FOUR)
				//---------------- Calculate Third pixle light value --------------------
#if defined(_SPOTLIGHT_QUALITY_PIXEL)//calculate spotlight effect
				LightingColour += unity_LightColor[2] * (CalculateNormalAttenuation(i.LightDirectionAndFalloff2.xyz, NormalMap) * i.LightDirectionAndFalloff2.w * CalculateSpotlightAttenuation(i.LightDirectionAndFalloff2.xyz, unity_SpotDirection[2].xyz, unity_LightAtten[2].xy));
#else
				LightingColour += unity_LightColor[2] * (CalculateNormalAttenuation(i.LightDirectionAndFalloff2.xyz, NormalMap) * i.LightDirectionAndFalloff2.w);
#endif

				//---------------- Calculate forth pixle light value --------------------
#if defined(_SPOTLIGHT_QUALITY_PIXEL)//calculate spotlight effect
				LightingColour += unity_LightColor[3] * (CalculateNormalAttenuation(i.LightDirectionAndFalloff3.xyz, NormalMap) * i.LightDirectionAndFalloff3.w * CalculateSpotlightAttenuation(i.LightDirectionAndFalloff3.xyz, unity_SpotDirection[3].xyz, unity_LightAtten[3].xy));
#else
				LightingColour += unity_LightColor[3] * (CalculateNormalAttenuation(i.LightDirectionAndFalloff3.xyz, NormalMap) * i.LightDirectionAndFalloff3.w);
#endif

#endif
				//check if emmisive needs to be added
#if defined(_EMISSIVE_SIMPLE) || defined( MAPPED_EMISSIVE)
				LightingColour += i.colour.rgb;
				i.colour.rgb = _ParticleColour.rgb;
#endif

				//check if ambient light needs to be added
#if defined(_AMBIENT_LIGHT_FIXED)
				LightingColour += _FixedAmbientColour;
#endif

#if defined(_SOFT_PARTICLE_HEIGHT) || defined( _SOFT_PARTICLE_FULL)
				//calculate soft particle factor
				PARTICLE_A(i) *= CalculateSoftParticleFactor(i);
#endif

#if defined(_ALPHA_STYLE_ERODE)
				//combine light colour with world vlaues
				Albedo = float4(Albedo.rgb * LightingColour * PARTICLE_RGB(i), saturate(Albedo.a - (1 - PARTICLE_A(i))));
#else
				//combine light colour with world vlaues
				Albedo = float4(Albedo.rgb * LightingColour, Albedo.a)* PARTICLE_COLOUR(i);
#endif
				
				// apply fog
				APPLY_PARTICLE_FOG(ACCESS_FOG(i), Albedo.rgb);
				//return float4(lightValue, lightValue, lightValue, 1);
				return Albedo;
#endif
#endif

				//----------------------------------------- Vertex Only -------------------------------------------
#if defined(_LIGHT_QUALITY_VERTEX_ONLY)
				// sample the texture
				float4 Albedo = tex2D(_MainTex, i.uv);

				//calculate soft particles
#if defined(_SOFT_PARTICLE_HEIGHT) || defined( _SOFT_PARTICLE_FULL)
				//calculate soft particle factor
				PARTICLE_A(i) *= CalculateSoftParticleFactor(i);
#endif

				//apply the calculated lighting to the texture
#if defined(_ALPHA_STYLE_ERODE)
				Albedo = float4(Albedo.rgb *  i.VertexLighting * PARTICLE_RGB(i), saturate(Albedo.a - (1 - PARTICLE_A(i)))) ;
#else
				Albedo = float4(Albedo.rgb *  i.VertexLighting, Albedo.a) *  PARTICLE_COLOUR(i);
#endif
				
				// apply fog
				APPLY_PARTICLE_FOG(ACCESS_FOG(i), Albedo.rgb);
				
				return Albedo;

#endif

				return float4(1,0,1,1);
				
			}
			
		ENDCG
		}

//		// Pass to render object as a shadow caster
//		Pass
//		{
//			Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
//			Name "Caster"
//			Tags{ "LightMode" = "ShadowCaster" }
//			Offset 1, 1
//			Fog{ Mode Off }
//			ZWrite On ZTest LEqual Cull Off
//			CGPROGRAM
//
//
//
//	#include "UnityCG.cginc"
//	#pragma glsl
//	#pragma vertex ShadowVert
//	#pragma fragment ShadowFrag
//	#pragma multi_compile_shadowcaster
//	#pragma multi_compile _CAST_SHADOWS_OFF _CAST_SHADOWS_STENCIL _CAST_SHADOWS_DITHERED
//	#include "UnityCG.cginc"
//	
//	#pragma target 3.0
//
//			float4 _MainTex_ST; //texture scaling and translation value
//			sampler2D _MainTex; //main colour texture		
//			sampler2D  _ShadowDitherTex;//shadow dithering texture
//			float _fShadowClip;
//			float _fShadowMultiplyer;
//			float4 _ParticleColour; //the colour to tint the particles when using emissive
//
//			struct VertexInput
//			{
//				float4 vertex	: POSITION;
//				float2 uv0		: TEXCOORD0;
//				fixed4 color : COLOR;
//			};
//
//			struct ShadowV2F
//			{
//				//V2F_SHADOW_CASTER_NOPOS
//				V2F_SHADOW_CASTER;
//					float2 uv0		: TEXCOORD1;
//				float4 Colour : TEXCOORD3;
//			};
//
//
//
//			ShadowV2F ShadowVert(VertexInput v)
//			{
//				ShadowV2F o;
//
//				//TRANSFER_SHADOW_CASTER_NOPOS(Output, opos)
//				TRANSFER_SHADOW_CASTER(o)
//					o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);
//
//#if defined(_CAST_SHADOWS_DITHERED)
//				o.Colour =  v.color.a * _ParticleColour.a * _fShadowMultiplyer;
//#else
//				o.Colour = v.color.a * _ParticleColour.a ;
//#endif
//				return o;
//			}
//
//			float4 ShadowFrag(ShadowV2F i) : COLOR
//			{
//	#ifdef _CAST_SHADOWS_DITHERED
//
//				//get textur Alpha
//				float Alpha = saturate(tex2D(_MainTex, i.uv0).a  * i.Colour.a  );
//				
//				//Clip the target pixel if alpha is not high enough
//				//clip(-(tex2D(_ShadowDitherTex, vpos.xy * 0.03125f).r - Alpha)) ;
//			clip(-(tex2D(_ShadowDitherTex, i.pos.xy * 0.25f).r - Alpha));
//
//	#endif
//	#ifdef _CAST_SHADOWS_STENCIL
//				clip((tex2D(_MainTex, i.uv0).a  * i.Colour.a)  - _fShadowClip);
//	#endif
//			//return float4(1, 1, 1, 1);
//
//			//return float4(0, 0, 0, 0);
//	#ifdef _CAST_SHADOWS_OFF
//				clip(-1);
//	#endif
//				SHADOW_CASTER_FRAGMENT(i)
//			}
//
//			ENDCG
//
//		}
		
	}
	Fallback "Legacy Shaders/Transparent/VertexLit"
	CustomEditor "NormalMappedParticleShaderEditor"
}
