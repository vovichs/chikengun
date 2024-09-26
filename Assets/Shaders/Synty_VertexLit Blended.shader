Shader "Synty/VertexLit Blended"
{
  Properties
  {
    _EmisColor ("Emissive Color", Color) = (0.2,0.2,0.2,0)
    _MainTex ("Particle Texture", 2D) = "white" {}
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "LIGHTMODE" = "Vertex"
      "PreviewType" = "Plane"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "LIGHTMODE" = "Vertex"
        "PreviewType" = "Plane"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      ZWrite Off
      Fog
      { 
        Mode  Off
      } 
      Blend SrcAlpha OneMinusSrcAlpha
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 unity_LightColor[8];
      //uniform float4 unity_LightPosition[8];
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4 glstate_lightmodel_ambient;
      //uniform float4x4 unity_MatrixInvV;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _EmisColor;
      uniform float4 _MainTex_ST;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_COLOR0 :COLOR0;
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_COLOR0 :COLOR0;
          float2 xlv_TEXCOORD0 :TEXCOORD0;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float4x4 m_1;
          m_1 = mul(unity_WorldToObject, unity_MatrixInvV);
          float4 tmpvar_2;
          float4 tmpvar_3;
          float4 tmpvar_4;
          tmpvar_2.x = conv_mxt4x4_0(m_1).x;
          tmpvar_2.y = conv_mxt4x4_1(m_1).x;
          tmpvar_2.z = conv_mxt4x4_2(m_1).x;
          tmpvar_2.w = conv_mxt4x4_3(m_1).x;
          tmpvar_3.x = conv_mxt4x4_0(m_1).y;
          tmpvar_3.y = conv_mxt4x4_1(m_1).y;
          tmpvar_3.z = conv_mxt4x4_2(m_1).y;
          tmpvar_3.w = conv_mxt4x4_3(m_1).y;
          tmpvar_4.x = conv_mxt4x4_0(m_1).z;
          tmpvar_4.y = conv_mxt4x4_1(m_1).z;
          tmpvar_4.z = conv_mxt4x4_2(m_1).z;
          tmpvar_4.w = conv_mxt4x4_3(m_1).z;
          float3 lcolor_5;
          float3 eyeNormal_6;
          float4 color_7;
          color_7.w = in_v.color.w;
          float3x3 tmpvar_8;
          tmpvar_8[0] = tmpvar_2.xyz;
          tmpvar_8[1] = tmpvar_3.xyz;
          tmpvar_8[2] = tmpvar_4.xyz;
          float3 tmpvar_9;
          tmpvar_9 = normalize(mul(tmpvar_8, in_v.normal));
          eyeNormal_6 = tmpvar_9;
          lcolor_5 = (_EmisColor.xyz + (in_v.color.xyz * glstate_lightmodel_ambient.xyz));
          float3 tmpvar_10;
          tmpvar_10 = unity_LightPosition[0].xyz;
          float3 dirToLight_11;
          dirToLight_11 = tmpvar_10;
          lcolor_5 = (lcolor_5 + min((((max(dot(eyeNormal_6, dirToLight_11), 0) * in_v.color.xyz) * unity_LightColor[0].xyz) * 0.5), float3(1, 1, 1)));
          float3 tmpvar_12;
          tmpvar_12 = unity_LightPosition[1].xyz;
          float3 dirToLight_13;
          dirToLight_13 = tmpvar_12;
          lcolor_5 = (lcolor_5 + min((((max(dot(eyeNormal_6, dirToLight_13), 0) * in_v.color.xyz) * unity_LightColor[1].xyz) * 0.5), float3(1, 1, 1)));
          float3 tmpvar_14;
          tmpvar_14 = unity_LightPosition[2].xyz;
          float3 dirToLight_15;
          dirToLight_15 = tmpvar_14;
          lcolor_5 = (lcolor_5 + min((((max(dot(eyeNormal_6, dirToLight_15), 0) * in_v.color.xyz) * unity_LightColor[2].xyz) * 0.5), float3(1, 1, 1)));
          float3 tmpvar_16;
          tmpvar_16 = unity_LightPosition[3].xyz;
          float3 dirToLight_17;
          dirToLight_17 = tmpvar_16;
          lcolor_5 = (lcolor_5 + min((((max(dot(eyeNormal_6, dirToLight_17), 0) * in_v.color.xyz) * unity_LightColor[3].xyz) * 0.5), float3(1, 1, 1)));
          float3 tmpvar_18;
          tmpvar_18 = unity_LightPosition[4].xyz;
          float3 dirToLight_19;
          dirToLight_19 = tmpvar_18;
          lcolor_5 = (lcolor_5 + min((((max(dot(eyeNormal_6, dirToLight_19), 0) * in_v.color.xyz) * unity_LightColor[4].xyz) * 0.5), float3(1, 1, 1)));
          float3 tmpvar_20;
          tmpvar_20 = unity_LightPosition[5].xyz;
          float3 dirToLight_21;
          dirToLight_21 = tmpvar_20;
          lcolor_5 = (lcolor_5 + min((((max(dot(eyeNormal_6, dirToLight_21), 0) * in_v.color.xyz) * unity_LightColor[5].xyz) * 0.5), float3(1, 1, 1)));
          float3 tmpvar_22;
          tmpvar_22 = unity_LightPosition[6].xyz;
          float3 dirToLight_23;
          dirToLight_23 = tmpvar_22;
          lcolor_5 = (lcolor_5 + min((((max(dot(eyeNormal_6, dirToLight_23), 0) * in_v.color.xyz) * unity_LightColor[6].xyz) * 0.5), float3(1, 1, 1)));
          float3 tmpvar_24;
          tmpvar_24 = unity_LightPosition[7].xyz;
          float3 dirToLight_25;
          dirToLight_25 = tmpvar_24;
          lcolor_5 = (lcolor_5 + min((((max(dot(eyeNormal_6, dirToLight_25), 0) * in_v.color.xyz) * unity_LightColor[7].xyz) * 0.5), float3(1, 1, 1)));
          color_7.xyz = float3(lcolor_5);
          color_7.w = color_7.w;
          float4 tmpvar_26;
          float4 tmpvar_27;
          tmpvar_27 = clamp(color_7, 0, 1);
          tmpvar_26 = tmpvar_27;
          float4 tmpvar_28;
          tmpvar_28.w = 1;
          tmpvar_28.xyz = in_v.vertex.xyz;
          out_v.xlv_COLOR0 = tmpvar_26;
          out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_28));
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          tmpvar_1 = (in_f.xlv_COLOR0 * tex2D(_MainTex, in_f.xlv_TEXCOORD0));
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
