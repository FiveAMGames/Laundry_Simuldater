Shader "Custom/ScratchStain"
{
    Properties
    {
        [PerRendererData] _MainTex (
            "Sprite Texture",
            2D
        ) = "white" {}

        _Color (
            "Tint",
            Color
        ) = (1,1,1,1)

        _ScratchMask (
            "Scratch Mask",
            2D
        ) = "black" {}
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _ScratchMask;

            float4 _Color;


            v2f vert(appdata v)
            {
                v2f o;

                o.vertex =
                    UnityObjectToClipPos(
                        v.vertex
                    );

                o.uv =
                    v.uv;

                o.color =
                    v.color * _Color;

                o.screenPos =
                    ComputeScreenPos(
                        o.vertex
                    );

                return o;
            }


            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col =
                    tex2D(
                        _MainTex,
                        i.uv
                    );

                col *= i.color;

                float2 screenUV =
                    i.screenPos.xy /
                    i.screenPos.w;

                fixed scratch =
                    tex2D(
                        _ScratchMask,
                        screenUV
                    ).r;

                col.a *=
                    1.0 - scratch;

                return col;
            }

            ENDCG
        }
    }
}