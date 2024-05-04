Shader "Custom/PaniniShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CameraCube ("CameraCube", Cube) = "" {}
        _HudRT ("HudRT", 2D) = "white" {}
        
        _Facing ("Facing", Vector) = (0.0, 0.0, 0.0, 0.0)
        _FOV ("FOV", Float) = 110.0
        _Warp ("Warp", Float) = 0.5
    }
    SubShader
    {
        // No culling or depth
        Cull Off
        ZWrite On
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment sampling

            #include "UnityCG.cginc"

            #define RADIANS 0.01745329251
            #define UP float3(0.0, 1.0, 0.0)

            float Pow2(float x) {return x*x;}

            float3 PaniniProjection(float2 tc, float fov, float d)
            {
                float d2 = d*d;

                {
                    float fo = UNITY_HALF_PI - fov * 0.5;

                    float f = cos(fo)/sin(fo) * 2.0;
                    float f2 = f*f;

                    float b = (sqrt(max(0.0, Pow2(d+d2)*(f2+f2*f2))) - (d*f+f)) / (d2+d2*f2-1.0);

                    tc *= b;
                }
                
                /* http://tksharpless.net/vedutismo/Pannini/panini.pdf */
                float h = tc.x;
                float v = tc.y;
                
                float h2 = h*h;
                
                float k = h2/Pow2(d+1.0);
                float k2 = k*k;
                
                float discr = max(0.0, k2*d2 - (k+1.0)*(k*d2-1.0));
                
                float cosPhi = (-k*d+sqrt(discr))/(k+1.0);
                float S = (d+1.0)/(d+cosPhi);
                float tanTheta = v/S;
                
                float sinPhi = sqrt(max(0.0, 1.0-Pow2(cosPhi)));
                if(tc.x < 0.0) sinPhi *= -1.0;

                rsqrt(1.0+Pow2(tanTheta));
                
                float s = rsqrt(1.0+Pow2(tanTheta));
                
                return float3(sinPhi, tanTheta, cosPhi) * s;
            }

            float3 RotateAxisAngle(float3 v, float3 axis, float angle)
            {
                float cos_a = cos(angle);
                float sin_a = sin(angle);
                
                return v * cos_a + cross(axis, v) * sin_a + axis * dot(axis, v) * (1 - cos_a);
            }


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            samplerCUBE _CameraCube;
            sampler2D _HudRT;

            float4 _Facing;
            float _FOV;
            float _Warp;

            fixed4 sampling (v2f iTexCoord) : SV_Target
            {
                fixed4 hudColor = tex2D(_HudRT, iTexCoord.uv);
                if (hudColor.w >= 1.0) return hudColor;
                
                float ratio = _ScreenParams.x / _ScreenParams.y;
                float2 uv = iTexCoord.uv * 2 - 1.0;
                uv.y /= ratio;
                
                float3 rd = PaniniProjection(uv, _FOV * RADIANS, _Warp);

                float4 facing_rad = _Facing * RADIANS;

                float3 forward = RotateAxisAngle(float3(1.0, 0.0, 0.0), UP, facing_rad.y);
                float3 right = cross(forward, UP);
                
                rd = RotateAxisAngle(rd, float3(0.0, 1.0, 0.0), facing_rad.y);
                rd = RotateAxisAngle(rd, forward, facing_rad.x);
                rd = RotateAxisAngle(rd, right, facing_rad.z);

                float blend_alpha = hudColor.w;
                fixed4 texColor = texCUBE(_CameraCube, rd);
                return float4(texColor.xyz * (1.0 - blend_alpha) + hudColor.xyz * blend_alpha, 1.0);
            }
            ENDCG
        }
    }
}
