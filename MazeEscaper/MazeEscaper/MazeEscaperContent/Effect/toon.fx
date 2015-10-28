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
// tone texture
texture ToneTexture;
sampler ToneSampler = sampler_state{
	Texture = ToneTexture;
	AddressU = Clamp;
	AddressV = Clamp;
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
bool EnableDepthShadow;
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
	float3 Normal : TEXCOORD1;
	float3 ObjPosition : TEXCOORD2;
	float4 TexCoord : TEXCOORD3;
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
	float3 Normal : TEXCOORD1;
	float3 ObjPosition : TEXCOORD2;
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
	float dotResult0 = clamp( 0.0, 1.0, dot(normal, -Light0Direction) );
	float dotResult1 = clamp( 0.0, 1.0, dot(normal, -Light1Direction) );
	float dotResult2 = clamp( 0.0, 1.0, dot(normal, -Light2Direction) );

	float accum = dotResult0 + dotResult1 + dotResult2;
	float3 diffuse = tex2D( ToneSampler, float2( accum, 0.5 ) ) * 
							( Light0DiffuseColor + Light1DiffuseColor + Light2DiffuseColor  );

	float3 diffuse0 = tex2D( ToneSampler, float2( dotResult0, 0.5 ) ) * Light0DiffuseColor;
	float3 diffuse1 = tex2D( ToneSampler, float2( dotResult1, 0.5 ) ) * Light1DiffuseColor;
	float3 diffuse2 = tex2D( ToneSampler, float2( dotResult2, 0.5 ) ) * Light2DiffuseColor;

	return diffuse;
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

	if( !EnableDepthShadow || vPosition.y < -5.0 ){
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
			shadowPer += (0.5 / 25.0);		// shadowPer += 1.0 .
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

	output.Normal = mul(input.Normal, World);
	output.ObjPosition = viewPosition;
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
	float3 position = normalize(input.ObjPosition);

	float3 albed = tex2D( AlbedSampler, input.TexCoord);

	// compute lighting colors
	float3 diffuse = ComputeDiffuseColor( normal ) * DiffuseColor * albed * shadow;
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

	output.Normal = mul(input.Normal, World);
	output.ObjPosition = viewPosition;

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
	float3 position = normalize(input.ObjPosition);

	// compute lighting colors
	float3 diffuse = ComputeDiffuseColor( normal ) * DiffuseColor * shadow;
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
 * Toon technique
**********************************************************************/
technique Toon
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
 * Toon technique without Texture
**********************************************************************/
technique ToonWithoutTexture
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