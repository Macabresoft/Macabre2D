// Default shader based on http://rbwhitaker.wikidot.com/post-processing-effects
// Also a fantastic source of MonoGame/XNA tutorials

texture ScreenTexture;

sampler TextureSampler = sampler_state
{
	Texture = <ScreenTexture>;
};

float4 PixelShaderFunction(float2 TextureCoordinate : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(TextureSampler, TextureCoordinate);
	return color;
}

technique Plain
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}