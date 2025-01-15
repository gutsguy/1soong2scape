Shader "Custom/TimeBarShader"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,0,0,1) // 기본 색상
        _Progress ("Progress", Range(0, 1)) = 0 // 진행률
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;
            float _Progress;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (i.uv.x < _Progress) // UV 좌표 기준으로 진행률 결정
                    return fixed4(0, 1, 0, 1); // 초록색
                return _Color; // 기본 색상 (빨간색)
            }
            ENDCG
        }
    }
}
