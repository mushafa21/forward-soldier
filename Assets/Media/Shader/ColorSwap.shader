// Simple Unlit shader for 2D sprites that swaps a source color
// with a target color within a given tolerance.
Shader "Custom/ColorSwap"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _SourceColor ("Source Color (Blue)", Color) = (0, 0, 1, 1)
        _TargetColor ("Target Color (Red)", Color) = (1, 0, 0, 1)
        _Tolerance ("Color Tolerance", Range(0, 1)) = 0.1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
        }

        LOD 100

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

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color    : COLOR;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color    : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            fixed4 _SourceColor;
            fixed4 _TargetColor;
            float _Tolerance;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 texColor = tex2D(_MainTex, i.texcoord) * i.color;

                // Calculate the distance (difference) between the
                // texture color and the source color
                float colorDistance = distance(texColor.rgb, _SourceColor.rgb);

                // If the distance is within our tolerance...
                if (colorDistance <= _Tolerance)
                {
                    // ...replace it with the target color.
                    // We keep the original alpha to maintain transparency.
                    texColor.rgb = _TargetColor.rgb;
                }
                
                // Return the (potentially modified) color
                return texColor;
            }
            ENDCG
        }
    }
}