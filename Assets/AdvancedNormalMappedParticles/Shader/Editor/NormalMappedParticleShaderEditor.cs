using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class NormalMappedParticleShaderEditor : MaterialEditor
{
    //   string[] strLightQualityOptions = { "VERTEX_ONLY_LIGHTS", "ONE_PIXEL_MANY_VERTEX_LIGHTS", "PIXEL_ONLY_LIGHTS" };
    //   string[] strLightCount = { "SINGLE_LIGHT", "TWO_LIGHTS", "FOUR_LIGHTS" };
    //   string[] strSpotlightOptions = { "SPOTLIGHTS_OFF", "VERTEX_ONLY_SPOTLIGHTS", "SPOTLIGHTS_ON" };
    //   string[] strNormalOptions = { "VERTEX_NORMAL", "FIXED_PER_PIXEL_NORMAL", "VARIABLE_PER_PIXEL_NORMAL", "VARIABLE_BENT_PER_PIXEL_NORMAL" };
    //   string[] strLightAttenuationCalculationOptions = { "LIGHT_WRAP_AROUND" };
    //   string[] strLightAttenuationEffectOptions = { "VARIABLE_SHADOWING_EFFECT" };
    //   string[] strAlphaFadeOptions = { "ALPHA_ERODE" };
    //   string[] strEmissiveOptions = { "NO_EMMISIVE", "SIMPLE_EMMISIVE", "MAPPED_EMISSIVE" };
    //   string[] strSoftParticleOptions = { "HARD_PARTICLES", "HEIGHT_CLIPPED", "SOFT_PARTICLE" };
    //   string[] strDistanceFadeOptions = { "DISTANCE_FADE" };

   //protected float _fHeaderHeight = 40;
   //protected float _fSmallHeaderSize = 55;
   //protected float _fBigHeaderSize = 200;
   //protected float _fHeaderCapSize = 31;

    protected float _fHeaderHeight = 80;
    protected float _fSmallHeaderSize = 110;
    protected float _fBigHeaderSize = 399;
    protected float _fHeaderCapSize = 62;

   // protected string _strTestBanner = "Assets\\Textures\\EditorResources\\SmallBannerTest5.png";
    protected string _strLeftCapAddressBasic = "Assets\\AdvancedNormalMappedParticles\\EditorResources\\LeftCapBasic.png";
    protected string _strLeftCapAddressPro = "Assets\\AdvancedNormalMappedParticles\\EditorResources\\LeftCapPro.png";
    protected string _strLeftCapConnectorAddress = "Assets\\AdvancedNormalMappedParticles\\EditorResources\\CapConnector.png";
    protected string _strBigHeaderAddress = "Assets\\AdvancedNormalMappedParticles\\EditorResources\\BigHeader.png";
    protected string _strSmallHeaderAddress = "Assets\\AdvancedNormalMappedParticles\\EditorResources\\SmallHeader.png";
    protected string _strRightCapConnectorAddress = "Assets\\AdvancedNormalMappedParticles\\EditorResources\\CapConnector.png";
    protected string _strRightCapAddressBasic = "Assets\\AdvancedNormalMappedParticles\\EditorResources\\RightCapBasic.png";
    protected string _strRightCapAddressPro = "Assets\\AdvancedNormalMappedParticles\\EditorResources\\RightCapPro.png";


    protected Texture2D _texLeftCap;
    protected Texture2D _texRightCap;
    protected Texture2D _texLeftCapConnector;
    protected Texture2D _texSmallCenterHeader;
    protected Texture2D _texBigCenterHeader;
    protected Texture2D _texRighCapConnector;

    public override void OnInspectorGUI()
    {

        // if we are not visible... return
        if (!isVisible)
        {
            return;
        }

        if(AllResourcesFound() == false)
        {
            GetResources();
        }

        if (AllResourcesFound() == true)
        {
            //create the header with the optional help button
            Rect recHeaderBounds = EditorGUILayout.GetControlRect(GUILayout.Height(_fHeaderHeight));

            //if the width is greate enough for for full header
            if (recHeaderBounds.width > (_fBigHeaderSize + _fHeaderCapSize + _fHeaderCapSize))
            {
                //draw full header
                DrawFullHeader(recHeaderBounds);
            }
            else
            {
                //draw small header
                DrawSmallHeader(recHeaderBounds);
            }
        }
        //Texture2D texWhiteTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(_strTestBanner);
        //Texture2D texWhiteTexture = EditorGUIUtility.whiteTexture;

        //EditorGUI.DrawPreviewTexture(recHeaderBounds, texWhiteTexture);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //check if bool exists
        if(EditorPrefs.HasKey("NormalShaderHelpHints") == false)
        {
            EditorPrefs.SetBool("NormalShaderHelpHints", true);
        }

        //check if the user has help hints turned on
        bool bHelpHints = EditorPrefs.GetBool("NormalShaderHelpHints");
      
        //check if the user wants tu turn on help
        bHelpHints = EditorGUILayout.Toggle( "Turn on Help Hints", bHelpHints);


       
        //apply the help hints settings
        EditorPrefs.SetBool("NormalShaderHelpHints", bHelpHints);

        //render primary texture field
        MaterialProperty mprMainTex = GetMaterialProperty(targets, "_MainTex");

        //draw primary texture
        TextureProperty(mprMainTex, "Albedo");
        EditorGUILayout.Space();


        //get keyword property
        MaterialProperty mprLightQuality = GetMaterialProperty(targets, "_Light_Quality");
        MaterialProperty mprNormalFormat = GetMaterialProperty(targets, "_Normal_Style");


        //hints for performance
        if (bHelpHints)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Vertex Only = all lighting calculations are done per vertex, this is fast but is not as accurate as per pixel lighting, One Pixel Many Vertex = lighting is calculated per pixel for the strongest light and per vertex for the remaining lights, this provides a good compromise for accuracy and speed, when in this mode both the particle normals and the texture normals are used, Pixel Only = Lighting is calculated per pixel for all lights, this is the slowest but  most accurate.", MessageType.Info);
        }
        //draw light computation options
        ShaderProperty(mprLightQuality, "Light Quality");

        //check if normal format is anything other than vertex
        if (mprLightQuality.floatValue > 0 || mprLightQuality.hasMixedValue)
        {
            if (bHelpHints)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Vertex =  normals created by the Shuriken particle system and can be tweaked in the particle effect renderer, Pixel  =  normal map texture stores the normals in the Unity normal format. Vertex normal mapping will be slightly faster but produces a less detailed image", MessageType.Info);
            }
            ShaderProperty(mprNormalFormat, "Normal Format");
        }

        //check if material needs normal map
        if((mprLightQuality.floatValue > 0 || mprLightQuality.hasMixedValue) && (mprNormalFormat.floatValue > 0 || mprNormalFormat.hasMixedValue))
        {
            EditorGUILayout.Space();
            if (bHelpHints)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("This shader uses Unity's normal format so make sure your normal map texture is marked as NORMAL MAP in the asset import settings instead of TEXTURE", MessageType.Warning);
            }
            MaterialProperty mprNormalMap = GetMaterialProperty(targets, "_NormalMap");
          
            ShaderProperty(mprNormalMap, "Normal Map");

            EditorGUILayout.Space();

        }

        //check if material uses emissive
        MaterialProperty mprEmissiveFormat = GetMaterialProperty(targets, "_Emissive");

        if (bHelpHints)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Self Lighting = how bright the particle is separate from external light sources, a particle like a spark or a flame would have a bright self lighting value while a particle like a leaf or a cloud would have no self lighting. When self lighting is enabled the RGB colour/tint values set in the Shuriken particle system becomes the self lighting value and the particle tint value is migrated here to the shader", MessageType.Info);
        }
        ShaderProperty(mprEmissiveFormat, "Self Lighting");
        if (mprEmissiveFormat.floatValue > 0 || mprEmissiveFormat.hasMixedValue)
        {
            if (bHelpHints)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Base Colour = colour value used to tint the particles separate from shuriken when self lighting is activated", MessageType.Info);
            }

            MaterialProperty mprBaseColour = GetMaterialProperty(targets, "_ParticleColour");
            ShaderProperty(mprBaseColour, "Base Colour");
            EditorGUILayout.Space();
        }

        //check if material uses Ambient Lighting
        if (bHelpHints)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
         //   EditorGUILayout.HelpBox("Ambient Light is the indeirect light coming from light sources like the sky or bouncing off walls without it shadows can seem excessivly dark. Off disables all ambient light, Fixed applys a singe preset Ambinet light to the entire effect at a very low cost, Probe samples a light probe near the center of the particle effect and calculates an ambient light value at a moderate cost and Volume samples a light probe volume to get an ambient light value that more acuratly matches the particles current location but at a higher cost than the other options", MessageType.Info);
            EditorGUILayout.HelpBox("Ambient Light is the indirect light coming from light sources like the sky or bouncing off walls without it shadows can seem excessively dark. Off = disable all ambient light, Fixed = apply a single preset Ambient light to the entire effect at a very low cost", MessageType.Info);
        }
        MaterialProperty mprAmbientLightStyle = GetMaterialProperty(targets, "_Ambient_Light");
        ShaderProperty(mprAmbientLightStyle, "Ambient Light");
        if(mprAmbientLightStyle.floatValue == 1 || mprAmbientLightStyle.hasMixedValue)
        {
            if (bHelpHints)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("The colour of the ambient light hitting the particle", MessageType.Info);
            }
            MaterialProperty mprAmbientLightColour = GetMaterialProperty(targets, "_FixedAmbientColour");
            ShaderProperty(mprAmbientLightColour, "Ambient Light Colour");
        }

        if (bHelpHints)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Light Count = the number of lights processed per pixel, High light counts increase performance costs especially when the particle effect is set to Pixel lights only", MessageType.Info);
        }
        MaterialProperty mprLightCount = GetMaterialProperty(targets, "_Light_Count");
        ShaderProperty(mprLightCount, "Light Count");

        if (bHelpHints)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Spotlights add a cost to every light rendered even if the light isn’t a spotlight,Off = Spotlights behave the same as point lights,Vertex = spotlight calculations are only done per vertex to save performance at the cost of quality especially on large particles, Pixel = spotlight calculations are done for every pixel and are more accurate but also more costly", MessageType.Info);
        }
        MaterialProperty mprSpotlight = GetMaterialProperty(targets, "_Spotlight_Quality");
        ShaderProperty(mprSpotlight, "Spotlight Quality");


        if (bHelpHints)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Wrap around lighting controls how much light wraps around the surface of the object and makes it to the pixels facing away from the light source. Use wrap around lighting to add additional lighting detail to the sides of your particle facing away from the light. wrap around lighting adds a small additional cost to the shader, Fixed = no light wrap around , Wrap around = light wrap around value adjustable in material editor at a small additional cost", MessageType.Info);
        }
        MaterialProperty mprLightStyle = GetMaterialProperty(targets, "_Light_Style");
        ShaderProperty(mprLightStyle, "Shadow Calculation Style");

        if (mprLightStyle.floatValue > 0 || mprLightStyle.hasMixedValue)
        {
            MaterialProperty mprLightWrapAroundValue = GetMaterialProperty(targets, "_LightWrapAround");
            ShaderProperty(mprLightWrapAroundValue, "Shadow Wrap Around");
            EditorGUILayout.Space();

        }

        if (bHelpHints)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Variable shadow effect allows you to increase or decrease the amount of shadowing caused by the normal maps.  Use variable shadow effect to make particles look softer or more defined.. Fixed  = maximum effect of shadows applied, Variable = amount of shadowing adjustable in material editor at very small additional cost.", MessageType.Info);
        }
        MaterialProperty mprShadowEffectStyle = GetMaterialProperty(targets, "_Shadowing_Effect");
        ShaderProperty(mprShadowEffectStyle, "Shadow Effect");

        if (mprShadowEffectStyle.floatValue > 0 || mprShadowEffectStyle.hasMixedValue)
        {
            MaterialProperty mprLightWrapAroundValue = GetMaterialProperty(targets, "_ShadowEffectMultiplyer");
            ShaderProperty(mprLightWrapAroundValue, "Shadow Effect Amount");
            EditorGUILayout.Space();

        }

        if (bHelpHints)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Alpha Style changes the way particles fade out, Fade = same as the unity particle shader (texture alpha is multiplied by object alpha), Erode = low alpha pixels disappear first and works best on particles with a lot of different alpha values (1 - object alpha is subtracted from texture alpha )", MessageType.Info);
        }
        MaterialProperty mprAlphaFadeStyle = GetMaterialProperty(targets, "_Alpha_Style");
        ShaderProperty(mprAlphaFadeStyle, "Alpha Style");

        if (bHelpHints)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Soft particles changes the way particles look when they intersect with world geometry. Hard = particles have a hard edge where it intersects the world, Height = fade the particles out as they go below or above a set height and has very good performance especially on mobile, use when you have a flat world, Full  = uses a depth buffer to check when the particles are getting near other geometry in the world and fades it out. Full soft particles had a large performance cost when used in Forward render mode or on mobile devices", MessageType.Warning);
        }
        MaterialProperty mprSoftParticleOptions = GetMaterialProperty(targets, "_Soft_Particle");
        ShaderProperty(mprSoftParticleOptions, "Soft particle Options");

        if (mprSoftParticleOptions.floatValue == 1 || mprSoftParticleOptions.hasMixedValue)
        {
            MaterialProperty mprHeightClippedStart = GetMaterialProperty(targets, "_HeightClippedOffset");
            MaterialProperty mprHeightClippedRate = GetMaterialProperty(targets, "_HeightClippedFadeRate");
            if (bHelpHints)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Fadeout Height = height that the particle alpha reaches 0", MessageType.Info);
            }
            ShaderProperty(mprHeightClippedStart, "Fadeout Height");

            if (bHelpHints)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Fadeout Rate = the rate that the alpha of the particle transitions from 0 to 1, use a negative value if you want the particle to fade out as they get higher", MessageType.Info);
            }
            ShaderProperty(mprHeightClippedRate, "Fadeout Rate");
            EditorGUILayout.Space();
        }

        if(mprSoftParticleOptions.floatValue ==2 || mprSoftParticleOptions.hasMixedValue)
        {
            MaterialProperty mprSoftParticleFadeRate = GetMaterialProperty(targets, "_InvFade");

            if (bHelpHints)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Thickness = how close a world object gets behind a particle before it starts fading out", MessageType.Info);
            }
            ShaderProperty(mprSoftParticleFadeRate, "thickness");
            EditorGUILayout.Space();
        }

        if (bHelpHints)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Warning! Does not work in Orthographic Camera mode. Distance fade =  fades out particles as they get closer or further away from the camera, Use this to hide effects that look good at a distance but do not work at close ranges like smoke from a distant volcano or raindrops", MessageType.Info);
        }

        //------------------------------ Distance Fade Options --------------------
        MaterialProperty mprDistanceFadeOptions = GetMaterialProperty(targets, "_Distance_Fade");
        ShaderProperty(mprDistanceFadeOptions, "Distance Fade");

        if (mprDistanceFadeOptions.floatValue > 0 || mprDistanceFadeOptions.hasMixedValue)
        {
            MaterialProperty mprDistanceFadeStart = GetMaterialProperty(targets, "_DistanceFadeStart");
            MaterialProperty mprDistanceFadeEnd = GetMaterialProperty(targets, "_DistanceFadeEnd");
            MaterialProperty mprDistanceFadeRate = GetMaterialProperty(targets, "_DistanceFadeRate");

            if (bHelpHints)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Fadeout distance = distance that the particle alpha is 0", MessageType.Info);
            }
            ShaderProperty(mprDistanceFadeStart, "Fade Out Distance");

            if (bHelpHints)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Fade Start Distance = distance that the particle starts to fade out", MessageType.Info);
            }
            ShaderProperty(mprDistanceFadeEnd, "Fade Start Distance");
            if ((mprDistanceFadeStart.floatValue - mprDistanceFadeEnd.floatValue) != 0)
            {
                mprDistanceFadeRate.floatValue = 1 / (mprDistanceFadeEnd.floatValue - mprDistanceFadeStart.floatValue );
            }
            else
            {
                mprDistanceFadeRate.floatValue = float.MaxValue;
            }
       
            EditorGUILayout.Space();
        }

        //------------------------------------------------------------------- Shadow Casting -----------------------------------

        //       if (bHelpHints)
        //       {
        //           EditorGUILayout.Space();
        //           EditorGUILayout.Space();
        //           EditorGUILayout.HelpBox("Shadow casting allows the particles to create shadows on world geometry, the shadows are only an approximation", MessageType.Info);
        //       }
        //       MaterialProperty mprShadowCastOptions = GetMaterialProperty(targets, "_Cast_Shadows");
        //       ShaderProperty(mprShadowCastOptions, "Shadow Casting Options");
        //
        //       if(mprShadowCastOptions.floatValue == 1)
        //       {
        //           MaterialProperty mprShadowStencilClip = GetMaterialProperty(targets, "_fShadowClip");
        //           if (bHelpHints)
        //           {
        //               EditorGUILayout.Space();
        //               EditorGUILayout.Space();
        //               EditorGUILayout.HelpBox("Stencil shadows cerate a solid shadow that blocks all light, The alpha clip value defines what alpha that solid shadow starts", MessageType.Info);
        //           }
        //           ShaderProperty(mprShadowStencilClip, "Alpha Clip");
        //       }
        //
        //       if (mprShadowCastOptions.floatValue == 2)
        //       {
        //           MaterialProperty mprShadowDitherTexture = GetMaterialProperty(targets, "_ShadowDitherTex");
        //           MaterialProperty mprShadowMultiplyer = GetMaterialProperty(targets, "_fShadowMultiplyer");
        //           if (bHelpHints)
        //           {
        //               EditorGUILayout.Space();
        //               EditorGUILayout.Space();
        //               EditorGUILayout.HelpBox("Dithered shadows create a shadow of varying darkness by creating many small Stencil shadows and allowing unitys soft shadows to blur them togeather to create a mixed darkness shadow. If Hard Shadows are used (for example on a moblie platform) Dithered shadows wont be blurred togeather and will create nasty shadow artifacts. The Dither texture defines the patten that the small stencil shadows are cast", MessageType.Info);
        //           }
        //           ShaderProperty(mprShadowDitherTexture, "Dither Texture");
        //
        //           if (bHelpHints)
        //           {
        //               EditorGUILayout.Space();
        //               EditorGUILayout.Space();
        //               EditorGUILayout.HelpBox("Shadow Multiplyer increases the darkness of the dithered shadow", MessageType.Info);
        //           }
        //           ShaderProperty(mprShadowMultiplyer, "Shadow Darkness Multiplyer");
        //       }



        //------------------------------------ Render Queue -------------------------------------------------

        //MaterialProperty mprRenderQueue = GetMaterialProperty(targets, "renderQueue");
        // ShaderProperty(mprRenderQueue, "Render Queue");


        //------------------------------------ Non Square UVs -----------------------------------------------

        if (bHelpHints)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Advanced Normal Mapped Particles needs to know the width vs height layout of the UVs, If you are not using sprite sheets or your sprite sheets are square (same number of columns as rows) then this isn’t an issue however if you are you must indicate the number of columns and rows in the non square uv option", MessageType.Info);
        }
        MaterialProperty mprNonSquareUVs = GetMaterialProperty(targets, "_Non_Square_Uv");
        ShaderProperty(mprNonSquareUVs, "Using Non Square Texture UVs");

        if (mprNonSquareUVs.floatValue > 0 || mprNonSquareUVs.hasMixedValue)
        {
            //get uv value
            MaterialProperty mprNonSquareUVFix = GetMaterialProperty(targets, "_fNonSquareUVFactor");
            MaterialProperty mprUVRowColumb = GetMaterialProperty(targets, "_UVRowsAndColumbs");

            float fRows = mprUVRowColumb.vectorValue.x;
            float fColumbs = mprUVRowColumb.vectorValue.y;

            fRows =(float) EditorGUILayout.IntField("Sprite Sheet Rows",(int)fRows);
            fColumbs = (float)EditorGUILayout.FloatField("Sprite Sheet Columbs",(int)fColumbs);

            //protect agains devide by 0
            if(fRows == 0)
            {
                fRows = 1;
            }

            if (fColumbs == 0)
            {
                fColumbs = 1;
            }

            mprUVRowColumb.vectorValue = new Vector4(fRows, fColumbs, 0, 0);
            //calculate uv scalign factor
            mprNonSquareUVFix.vectorValue = new Vector4(fColumbs / fRows, 1,0,0);
        }


        //help link
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //GUI.color = Color.blue;
        if (GUILayout.Button("For Extra Help Documentation Click Here"))
        {
           
            Application.OpenURL("https://drive.google.com/open?id=1SOYQRm2PUlI5LSNdtE75nk15XkYstZYv2WLePmw8t9Q");
           
        }

       // GUI.color = Color.white;

    }

    public void ChangeShaderFeatureKeyword(Material matTarget, string strLabel, string[] strOptions,string strDisabledText)
    {
        List<string> keyWords = new List<string>(matTarget.shaderKeywords);

        //loop through all the options
        string[] strDisplayOptions = new string[strOptions.Length + 1];
        strDisplayOptions[0] = strDisabledText;
        for (int i = 0; i < strOptions.Length; i++)
        {
            strDisplayOptions[i + 1] = strOptions[i];
        }

        //get the currently selected light mode
        int iSelectedLightMode = 0;

        for (int i = 0; i < strOptions.Length; i++)
        {
            if (keyWords.Contains(strOptions[i]))
            {
                iSelectedLightMode = i+1;
                break;
            }
        }

       

        EditorGUI.BeginChangeCheck();
        iSelectedLightMode = EditorGUILayout.Popup(strLabel, iSelectedLightMode, strDisplayOptions);
        if (EditorGUI.EndChangeCheck())
        {
            //remove other options
            for (int i = 0; i < strOptions.Length; i++)
            {
                keyWords.Remove(strOptions[i]);
            }

            if (iSelectedLightMode != 0)
            {
                //add in selected option
                keyWords.Add(strDisplayOptions[iSelectedLightMode]);
            }

            matTarget.shaderKeywords = keyWords.ToArray();

            EditorUtility.SetDirty(matTarget);
        }
    }

    public void ChangeMulticompileKeyword(Material matTarget,string strLabel,string[] strOptions)
    {
        List<string> keyWords = new List<string>(matTarget.shaderKeywords);

        //get the currently selected light mode
        int iSelectedLightMode = 0;

        for (int i = 0; i < strOptions.Length; i++)
        {
            if (keyWords.Contains(strOptions[i]))
            {
                iSelectedLightMode = i;
                break;
            }
        }

        EditorGUI.BeginChangeCheck();
        iSelectedLightMode = EditorGUILayout.Popup(strLabel, iSelectedLightMode, strOptions);
        if (EditorGUI.EndChangeCheck())
        {
            //remove other options
            for (int i = 0; i < strOptions.Length; i++)
            {
                keyWords.Remove(strOptions[i]);
            }

            //add in selected option
            keyWords.Add(strOptions[iSelectedLightMode]);

            matTarget.shaderKeywords = keyWords.ToArray();

            EditorUtility.SetDirty(matTarget);
        }
    }

    public bool CheckIfKeyWordSet(Material matTarget, string _strKeyWord)
    {
        List<string> keyWords = new List<string>(matTarget.shaderKeywords);

        if(keyWords.Contains(_strKeyWord))
        {
            return true;
        }

        return false;
    }

    public void DrawFullHeader(Rect recHeaderSize)
    {
        //caclulate center 
        float fCenter = recHeaderSize.center.x;

        //buld center rect
        Rect recCenterRect = new Rect(fCenter - (0.5f * _fBigHeaderSize) , recHeaderSize.y, _fBigHeaderSize, _fHeaderHeight);

        //build cap rects
        Rect recRightCap = new Rect(recHeaderSize.x + (recHeaderSize.width - _fHeaderCapSize), recHeaderSize.y, _fHeaderCapSize, _fHeaderHeight);
        Rect recLeftCap = new Rect(recHeaderSize.x, recHeaderSize.y, _fHeaderCapSize, _fHeaderHeight);

        //build cap links
        Rect recLeftCapLink = new Rect(recLeftCap.xMax, recLeftCap.y, recCenterRect.x - recLeftCap.xMax, _fHeaderHeight);
        Rect recRightCapLink = new Rect(recCenterRect.xMax, recCenterRect.y, recRightCap.x - recCenterRect.xMax , _fHeaderHeight);


        //draw all the header graphics
        EditorGUI.DrawPreviewTexture(recLeftCap, _texLeftCap);
        EditorGUI.DrawPreviewTexture(recLeftCapLink, _texLeftCapConnector);
        EditorGUI.DrawPreviewTexture(recCenterRect, _texBigCenterHeader);
        EditorGUI.DrawPreviewTexture(recRightCapLink, _texRighCapConnector);
        EditorGUI.DrawPreviewTexture(recRightCap, _texRightCap);
    }

    public void DrawSmallHeader(Rect recHeaderSize)
    {
        //caclulate center 
        float fCenter = recHeaderSize.center.x;

        //buld center rect
        Rect recCenterRect = new Rect(fCenter - (0.5f * _fSmallHeaderSize), recHeaderSize.y, _fSmallHeaderSize, _fHeaderHeight);

        //build cap rects
        Rect recRightCap = new Rect(recHeaderSize.x + (recHeaderSize.width - _fHeaderCapSize), recHeaderSize.y, _fHeaderCapSize, _fHeaderHeight);
        Rect recLeftCap = new Rect(recHeaderSize.x, recHeaderSize.y, _fHeaderCapSize, _fHeaderHeight);

        //build cap links
        Rect recLeftCapLink = new Rect(recLeftCap.xMax, recLeftCap.y, recCenterRect.x - recLeftCap.xMax, _fHeaderHeight);
        Rect recRightCapLink = new Rect(recCenterRect.xMax, recCenterRect.y, recRightCap.x - recCenterRect.xMax, _fHeaderHeight);



        //draw all the header graphics
        EditorGUI.DrawPreviewTexture(recLeftCap, _texLeftCap);
        EditorGUI.DrawPreviewTexture(recLeftCapLink, _texLeftCapConnector);
        EditorGUI.DrawPreviewTexture(recCenterRect, _texSmallCenterHeader);
        EditorGUI.DrawPreviewTexture(recRightCapLink, _texRighCapConnector);
        EditorGUI.DrawPreviewTexture(recRightCap, _texRightCap);
    }

    public void GetResources()
    {
        //check what version of the editor we are running 
#if UNITY_PRO_LICENSE
        _texLeftCap = AssetDatabase.LoadAssetAtPath<Texture2D>(_strLeftCapAddressPro);
        _texRightCap = AssetDatabase.LoadAssetAtPath<Texture2D>(_strRightCapAddressPro);
#else
        _texLeftCap = AssetDatabase.LoadAssetAtPath<Texture2D>(_strLeftCapAddressBasic);
        _texRightCap = AssetDatabase.LoadAssetAtPath<Texture2D>(_strRightCapAddressBasic);
#endif
        _texLeftCapConnector = AssetDatabase.LoadAssetAtPath<Texture2D>(_strLeftCapConnectorAddress);
        _texSmallCenterHeader = AssetDatabase.LoadAssetAtPath<Texture2D>(_strSmallHeaderAddress);
        _texBigCenterHeader = AssetDatabase.LoadAssetAtPath<Texture2D>(_strBigHeaderAddress);
        _texRighCapConnector = AssetDatabase.LoadAssetAtPath<Texture2D>(_strRightCapConnectorAddress);
    }

    public bool AllResourcesFound()
    {
        if(_texLeftCap == null)
        {
            return false;
        }

        if (_texRightCap == null)
        {
            return false;
        }

        if (_texLeftCapConnector == null)
        {
            return false;
        }

        if (_texBigCenterHeader == null)
        {
            return false;
        }

        if (_texRighCapConnector == null)
        {
            return false;
        }

        return true;
    }
}
