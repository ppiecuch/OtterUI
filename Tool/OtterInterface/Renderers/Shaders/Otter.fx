texture 	DIFFUSE_TEXTURE;

sampler MyTextureSampler = sampler_state
{
    Texture = <DIFFUSE_TEXTURE>;
    
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;

    AddressU = Clamp;
    AddressV = Clamp;
};

float4x4	MATRIX_MODEL;

float4x4	MATRIX_VP;
float4x4	MATRIX_VP_INV;

int		BOUNDS_WIDTH;
int		BOUNDS_HEIGHT;
float4		BOUNDS_COLOR;

struct VS_INPUT
{
    float3 vPosition    : POSITION;
    float2 vTexCoords   : TEXCOORD0;
    float4 vColor		: COLOR0;
    
};

struct VS_OUTPUT
{
    float4 vPosition    : POSITION;
    float2 vTexCoords   : TEXCOORD0;
    float4 vOutPos	: TEXCOORD1;
    float4 vColor	: COLOR0;
};

VS_OUTPUT VS(const VS_INPUT input)
{
    VS_OUTPUT Out = (VS_OUTPUT)0;
		
    Out.vPosition = float4(input.vPosition, 1);
    Out.vPosition = mul(Out.vPosition, MATRIX_MODEL);
    Out.vPosition = mul(Out.vPosition, MATRIX_VP);
    
    Out.vTexCoords  = input.vTexCoords;
    Out.vOutPos = Out.vPosition;
    Out.vColor = input.vColor;    
    
    return Out;
}


float4 PS_WithTexture(VS_OUTPUT IN) : COLOR 
{
	float4 outcol = tex2D(MyTextureSampler, IN.vTexCoords) * IN.vColor;

	float4 modelPos = mul(IN.vOutPos, MATRIX_VP_INV);
	if(modelPos.x < 0.0f || modelPos.x > BOUNDS_WIDTH || modelPos.y < 0.0f || modelPos.y > BOUNDS_HEIGHT)
		outcol = outcol * BOUNDS_COLOR;

	return outcol;
}

float4 PS_WithoutTexture(VS_OUTPUT IN) : COLOR 
{
	float4 outcol = IN.vColor;

	float4 modelPos = mul(IN.vOutPos, MATRIX_VP_INV);
	if(modelPos.x < 0.0f || modelPos.x > BOUNDS_WIDTH || modelPos.y < 0.0f || modelPos.y > BOUNDS_HEIGHT)
		outcol = outcol * BOUNDS_COLOR;

	return outcol;
}

technique WithTexture
{
    pass Pass0
    {
        AlphaBlendEnable = True;
        AlphaTestEnable = True;
        AlphaFunc = NotEqual;
        AlphaRef = 0;
        
        SrcBlend  = SrcAlpha;
        DestBlend = InvSrcAlpha;
        BlendOp = Add;
        
        Lighting       = FALSE;
        SpecularEnable = FALSE;
        
        ZWriteEnable = FALSE;
        ZEnable = FALSE;
        CullMode = None;
        
        AddressU[0] = Clamp;
		AddressV[0] = Clamp;

        // shaders
        VertexShader = compile vs_3_0 VS();
        PixelShader  = compile ps_3_0 PS_WithTexture();
    }
}

technique WithoutTexture
{
    pass Pass0
    {
        AlphaBlendEnable = True;
        AlphaTestEnable = True;
        AlphaFunc = NotEqual;
        AlphaRef = 0;
        
        SrcBlend  = SrcAlpha;
        DestBlend = InvSrcAlpha;
        BlendOp = Add;
        
        Lighting       = FALSE;
        SpecularEnable = FALSE;
        
        ZWriteEnable = FALSE;
        ZEnable = FALSE;
        CullMode = None;
        
        AddressU[0] = Clamp;
		AddressV[0] = Clamp;

        // shaders
        VertexShader = compile vs_3_0 VS();
        PixelShader  = compile ps_3_0 PS_WithoutTexture();

        MipFilter[0] = POINT;
        MinFilter[0] = POINT;
        MagFilter[0] = POINT;
    }
}