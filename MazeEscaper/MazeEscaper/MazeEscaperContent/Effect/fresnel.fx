/*****************************************************************
 * uniform variables
*****************************************************************/
/***		matrices		***/
float4x4 World;
float4x4 View;
float4x4 Projection;

float2 KernelStep[25];

/***		textures		***/
// albed texture
texture AlbedTexture;
sampler AlbedSampler = sampler_state{
	Texture = AlbedTexture;
	MagFilter = Linear;
	MinFilter = Linear;
};
// shadowmap texture
texture Shadowmap;
sampler ShadowmapSampler = sampler_state{
	Texture = Shadowmap;
    AddressU = Clamp;
    AddressV = Clamp;
    MinFilter = POINT;
    MagFilter = POINT;
    MipFilter = POINT;
};

/***		light settings		***/
float3 AmbientLightColor;
// light 0
float3 Light0DiffuseColor;
float3 Light0SpecularColor;
float3 Light0Direction;
bool Light0Enabled;
// light 1
float3 Light1DiffuseColor;
float3 Light1SpecularColor;
float3 Light1Direction;
bool Light1Enabled;
// light 2
float3 Light2DiffuseColor;
float3 Light2SpecularColor;
float3 Light2Direction;
bool Light2Enabled;

/***		material settings		***/
float3 DiffuseColor;
float3 SpecularColor;
float3 EmissiveColor;
float SpecularPower;
float Alpha;

/***		shadowmap		***/
float4x4 ShadowView;
float4x4 ShadowProjection;


/*****************************************************************
 * vertex shader io structures
*****************************************************************/
/**			vertex shader with texture 			**/
struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal : NORMAL;
	float4 TexCoord : TEXCOORD0;
};
struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 vPosition : TEXCOORD0;
	float3 ObjPosition : TEXCOORD1;
	float3 Normal : TEXCOORD2;
	float3 viewNormal : TEXCOORD3;
	float4 TexCoord : TEXCOORD4;
};

/**			vertex shader without texture			**/
struct VertexShaderInputWithoutTexture
{
    float4 Position : POSITION0;
	float3 Normal : NORMAL;
};

struct VertexShaderOutputWithoutTexture
{
    float4 Position : POSITION0;
	float4 vPosition : TEXCOORD0;
	float3 ObjPosition : TEXCOORD1;
	float3 Normal : TEXCOORD2;
	float3 viewNormal : TEXCOORD3;
};

/**			shadowmap vertex shader			**/
struct ShadowmapVertexShaderOutput
{
    float4 Position : POSITION0;
	float Depth : TEXCOORD0;
};

/**********************************************************************
 * functions
**********************************************************************/

/**********************************************************************
 * GetPositionFromLight
**********************************************************************/
float4 GetPositionFromLight(float4 position)
{
	float4x4 WorldViewProjection = mul(mul(World, ShadowView), ShadowProjection);
	return mul(position, WorldViewProjection);
}

/**********************************************************************
 * ComputeDiffuseColor
**********************************************************************/
float3 ComputeDiffuseColor( float3 normal ){
	float dotResult0 = clamp( dot(normal, -Light0Direction), 0.0, 1.0 );
	float dotResult1 = clamp( dot(normal, -Light1Direction), 0.0, 1.0 );
	float dotResult2 = clamp( dot(normal, -Light2Direction), 0.0, 1.0 );

	float3 diffuse0 = dotResult0 * Light0DiffuseColor;
	float3 diffuse1 = dotResult1 * Light1DiffuseColor;
	float3 diffuse2 = dotResult2 * Light2DiffuseColor;

	return diffuse0 + diffuse1 + diffuse2;
}

/**********************************************************************
 * ComputeSpecularColor
**********************************************************************/
float3 ComputeSpecularColor( float3 objPos, float3 normal ){
	float3 halfway = normalize( -Light0Direction + objPos );
	float specular = max( pow( dot( normal, halfway ), SpecularPower ), 0.0 );
	return specular * Light0SpecularColor * SpecularColor;
}

/*********************************************************
 * Fresnel()
 * n : 相対屈折率 viewVec : 視線ベクトル normVec : 法線ベクトル
 * return : フレネル係数
 *  : 間違ってるかも
//normVec = normalize( normal )
// viewVec = -normalize( position )
// n = 2.0
*********************************************************/
float FresnelFactor( float n, float3 viewVec, float3 normVec ){

    
    float LH = dot( viewVec, normVec );
    float g = sqrt( 1.0/(n*n) - 1.0 + LH*LH );

    float gmLH = g - LH;
    float gpLH = g + LH;
    float gmLH1 = LH * ( g - LH ) + 1.0;
    float gpLH1 = LH * ( g + LH ) - 1.0;
    
    float gLH = gmLH / gpLH;
    float gLH1 = gpLH1 / gmLH1;
    
    float f = 0.5 * gLH * gLH * ( gLH1 * gLH1 + 1.0 );
    
    return clamp( f, 0.0, 1.0 );
}

/**********************************************************************
 * inShadow
**********************************************************************/
float inShadow_(float4 vPosition){
	float4 lightingPosition = GetPositionFromLight( vPosition );
	float2 shadowTexCoord = 0.5 * lightingPosition.xy / lightingPosition.w + float2( 0.5, 0.5 );
	shadowTexCoord.y = 1.0 - shadowTexCoord.y;

	float shadowDepth = tex2D( ShadowmapSampler, shadowTexCoord ).r;
	float ourDepth = 1 - (lightingPosition.z / lightingPosition.w);

	if( shadowDepth - 0.003 > ourDepth ){
		return 1.0;
	}
	return 0.0;
}

float inShadow(float4 vPosition){

	if( vPosition.y < -5.0 ){
		return 0.0;
	}

	float4 lightingPosition = GetPositionFromLight( vPosition );
	float2 shadowTexCoord = 0.5 * lightingPosition.xy / lightingPosition.w + float2( 0.5, 0.5 );
	shadowTexCoord.y = 1.0 - shadowTexCoord.y;

	float ourDepth = 1 - (lightingPosition.z / lightingPosition.w);

	float shadowPer = 0.0;
	for( int i=0; i<25; i++ ){
		float shadowDepth = tex2D( ShadowmapSampler, shadowTexCoord + KernelStep[i] );
		if( shadowDepth - 0.003 > ourDepth ){
			shadowPer += 0.9 / 25.0;		// shadowPer += 1.0 / 25.0;
		} 
	}

	return shadowPer;
}

/**********************************************************************
 * vertex shader
**********************************************************************/
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.vPosition = input.Position;
	output.ObjPosition = viewPosition;

	output.Normal = mul(input.Normal, World);
	output.viewNormal = mul(output.Normal, View);
	output.TexCoord = input.TexCoord;

    return output;
}

/**********************************************************************
 * pixel shader
**********************************************************************/
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float shadow = 1 - inShadow( input.vPosition );

	// normalize vector
	float3 normal = normalize(input.Normal);
	float3 viewNormal = normalize(input.viewNormal);
	float3 position = normalize(input.ObjPosition);

	float3 albed = tex2D( AlbedSampler, input.TexCoord);

//	float rim = FresnelFactor( 2.0, -position, viewNormal ) * 0.2;
	float rim = pow( clamp( dot(viewNormal, position) + 1.0, 0.0, 1.0 ), 3.0 ) * 0.3;

	// compute lighting colors
	float3 diffuse = (ComputeDiffuseColor( normal ) * DiffuseColor * shadow + rim * DiffuseColor ) * albed;
	float3 specular = shadow > 0.5 ? ComputeSpecularColor( position, normal ) : 0.0;
	float3 ambient = AmbientLightColor * albed;
	float3 emissive = EmissiveColor;
	
	return float4( diffuse + specular + ambient + emissive, Alpha );
}

/**********************************************************************
 * vertex shader without texture
**********************************************************************/
VertexShaderOutputWithoutTexture VertexShaderWithoutTexture(VertexShaderInputWithoutTexture input)
{
    VertexShaderOutputWithoutTexture output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.vPosition = input.Position;
	output.ObjPosition = viewPosition;

	output.Normal = mul(input.Normal, World);
	output.viewNormal = mul(output.Normal, View);

    return output;
}

/**********************************************************************
 * pixel shader without texture
**********************************************************************/
float4 PixelShaderWithoutTexture(VertexShaderOutputWithoutTexture input) : COLOR0
{
	float shadow = 1.0 - inShadow( input.vPosition );

	// normalize vector
	float3 normal = normalize(input.Normal);
	float3 viewNormal = normalize(input.viewNormal);
	float3 position = normalize(input.ObjPosition);

	float rim = FresnelFactor( 4.0, -position, viewNormal ) * 0.4;
//	float rim = pow( clamp( dot(viewNormal, position) + 1.0, 0.0, 1.0 ), 3.0 ) * 0.3;

	// compute lighting colors
	float3 diffuse = ComputeDiffuseColor( normal ) * DiffuseColor * shadow + rim * DiffuseColor;
	float3 specular = shadow > 0.5 ? ComputeSpecularColor( position, normal ) : 0.0;
	float3 ambient = AmbientLightColor;
	float3 emissive = EmissiveColor;

	return float4( diffuse + specular + ambient + emissive, Alpha );
}

/**********************************************************************
 * shadowmap vertex shader
**********************************************************************/
ShadowmapVertexShaderOutput ShadowmapVertexShader(float4 position : POSITION0)
{
	ShadowmapVertexShaderOutput output;

	output.Position = GetPositionFromLight( position );
	output.Depth.x = 1 - (output.Position.z / output.Position.w);
	return output;
}

/**********************************************************************
 * shadowmap pixel shader
**********************************************************************/
float4 ShadowmapPixelShader(ShadowmapVertexShaderOutput input) : COLOR0
{
	return float4( input.Depth.x, 0, 0, 1 );
}


/**********************************************************************
 * Fresnel technique
**********************************************************************/
technique Fresnel
{
    pass Pass1
    {
		CullMode = CCW;
		ZEnable = TRUE;
		ZWriteEnable = TRUE;
		AlphaBlendEnable = TRUE;

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

/**********************************************************************
 * Fresnel technique without Texture
**********************************************************************/
technique FresnelWithoutTexture
{
    pass Pass1
    {
		CullMode = CCW;
		ZEnable = TRUE;
		ZWriteEnable = TRUE;
		AlphaBlendEnable = TRUE;

        VertexShader = compile vs_3_0 VertexShaderWithoutTexture();
        PixelShader = compile ps_3_0 PixelShaderWithoutTexture();
    }
}

/**********************************************************************
 * create shadowmap technique
**********************************************************************/
technique CreateShadowmap
{
	pass Pass1
	{
		CullMode = CCW;
		ZEnable = TRUE;
		ZWriteEnable = TRUE;
		AlphaBlendEnable = FALSE;
		
		VertexShader = compile vs_3_0 ShadowmapVertexShader();
		PixelShader = compile ps_3_0 ShadowmapPixelShader();
	}
}