/*****************************************************************
 * uniform variables
*****************************************************************/
/***		matrices		***/
float4x4 World;
float4x4 View;
float4x4 Projection;

float EndTime;
float Time;

/***		textures		***/
texture ExplosionTexture;
sampler ExplosionSampler = sampler_state{
	Texture = ExplosionTexture;
	MagFilter = Linear;
	MinFilter = Linear;
};

/*****************************************************************
 * vertex shader io structures
*****************************************************************/
struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float StartTime : TEXCOORD1;
};


/**********************************************************************
 * functions
**********************************************************************/

/**********************************************************************
 * vertex shader
**********************************************************************/
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	float3 deltaPos = input.Normal * Time;

	output.StartTime = input.Position.z;
	input.Position.z = 0.0f;
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View) + float4( deltaPos, 0.0f );
    output.Position = mul(viewPosition, Projection);

	output.TexCoord = input.TexCoord;	
    return output;
}

/**********************************************************************
 * pixel shader
**********************************************************************/
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float alpha = clamp( 0.0f, 1.0f, 1.0f - Time + input.StartTime );
	float4 color = tex2D( ExplosionSampler, input.TexCoord );
	return color * alpha;
}

/**********************************************************************
 * techniques
**********************************************************************/
technique Technique1
{
    pass Pass1
	{
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
