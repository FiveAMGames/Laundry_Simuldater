// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader"Custom/SpriteCustomMask"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Geometry-100"
            "IgnoreProjector"="True"
            "RenderType"="Opaque"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

Cull Off

Lighting Off

ZWrite Off

Blend One OneMinusSrcAlpha

Stencil
{
    Ref 1

    Comp always
    Pass replace
}

ColorMask 0

        Pass
        {
  CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile _ PIXELSNAP_ON
#include "UnityCG.cginc"


sampler2D _MainTex;
struct appdata_t
{
    float4 vertex : POSITION;
    float4 color : COLOR; //this is from the spriterenderer, it turns out
    float2 texcoord : TEXCOORD0;
};

struct v2f
{
    float4 vertex : SV_POSITION;
    fixed4 color : COLOR;
    float2 texcoord : TEXCOORD0;
};


v2f vert(appdata_t IN)
{
    v2f OUT;
    OUT.vertex = UnityObjectToClipPos(IN.vertex);
    OUT.texcoord = IN.texcoord;
    OUT.color = IN.color;
    
#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

    return OUT;
}




fixed4 frag(v2f IN) : SV_Target
{
    fixed4 c = fixed4(1, 1, 1, 0);//tex2D(_MainTex, IN.texcoord);
    return c;
    
}

            ENDCG
        }
    }
}