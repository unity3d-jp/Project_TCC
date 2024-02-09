Shader "Custom/Blur"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Blur("Blur", Float) = 10
    }
    SubShader
    {

        Tags{ "Queue" = "Transparent" }

        GrabPass
        {   
        }

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
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
                float4 vertColor : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                o.vertColor = v.color;
                return o;
            }

            sampler2D _GrabTexture;
            fixed4 _GrabTexture_TexelSize;

            float _Blur;

            half4 frag(v2f i) : SV_Target
            {
                float blur = _Blur;
                blur = max(1, blur);

                fixed4 col = (0, 0, 0, 0);
                float weight_total = 0;

                [loop]
                for (float x = -blur; x <= blur; x += 1)
                {
                    float distance_normalized = abs(x / blur);
                    float weight = exp(-0.5 * pow(distance_normalized, 2) * 5.0);
                    weight_total += weight;
                    col += tex2Dproj(_GrabTexture, i.grabPos + float4(x * _GrabTexture_TexelSize.x, 0, 0, 0)) * weight;
                }

                col /= weight_total;
                return col;
            }
            ENDCG
        }
        GrabPass
        {   
        }

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
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
                float4 vertColor : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                o.vertColor = v.color;
                return o;
            }

            sampler2D _GrabTexture;
            fixed4 _GrabTexture_TexelSize;

            float _Blur;

            half4 frag(v2f i) : SV_Target
            {
                float blur = _Blur;
                blur = max(1, blur);

                fixed4 col = (0, 0, 0, 0);
                float weight_total = 0;

                [loop]
                for (float y = -blur; y <= blur; y += 1)
                {
                    float distance_normalized = abs(y / blur);
                    float weight = exp(-0.5 * pow(distance_normalized, 2) * 5.0);
                    weight_total += weight;
                    col += tex2Dproj(_GrabTexture, i.grabPos + float4(0, y * _GrabTexture_TexelSize.y, 0, 0)) * weight;
                }

                col /= weight_total;
                return col;
            }
            ENDCG
        }

    }
}