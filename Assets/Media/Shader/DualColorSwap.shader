Shader "Custom/DualColorSwap"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        
        [Header(Color 1 Settings)]
        _SourceColor1 ("Source Color 1 (To Replace)", Color) = (0, 0, 1, 1)
        _TargetColor1 ("Target Color 1 (New Color)", Color) = (1, 0, 0, 1)
        _Tolerance1 ("Color 1 Tolerance", Range(0, 1)) = 0.1
        
        [Header(Color 2 Settings)]
        _SourceColor2 ("Source Color 2 (To Replace)", Color) = (0, 1, 0, 1)
        _TargetColor2 ("Target Color 2 (New Color)", Color) = (1, 1, 0, 1)
        _Tolerance2 ("Color 2 Tolerance", Range(0, 1)) = 0.1
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
            
            // Variables for Color 1
            fixed4 _SourceColor1;
            fixed4 _TargetColor1;
            float _Tolerance1;
            
            // Variables for Color 2
            fixed4 _SourceColor2;
            fixed4 _TargetColor2;
            float _Tolerance2;

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
                fixed4 texColor = tex2D(_MainTex, i.texcoord) * i.color;

                // Calculate distance for Color 1
                float dist1 = distance(texColor.rgb, _SourceColor1.rgb);
                
                // Calculate distance for Color 2
                float dist2 = distance(texColor.rgb, _SourceColor2.rgb);

                // Logic: Check Color 1 first, then Color 2
                if (dist1 <= _Tolerance1)
                {
                    texColor.rgb = _TargetColor1.rgb;
                }
                else if (dist2 <= _Tolerance2)
                {
                    texColor.rgb = _TargetColor2.rgb;
                }
                
                return texColor;
            }
            ENDCG
        }
    }
}