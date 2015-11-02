 Shader "Custom/Overdraw" {
 Properties {
     _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
 }
 
 SubShader {
     Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
     ZWrite Off
     Blend SrcAlpha OneMinusSrcAlpha 
     
     // Include functions common to both passes
     CGINCLUDE
         struct v2f {
             float4 vertex : SV_POSITION;
             half2 texcoord : TEXCOORD0;
         };
             
         #pragma vertex vert
         #pragma fragment frag
 
         #include "UnityCG.cginc"
         sampler2D _MainTex;
         float4 _MainTex_ST;
             
         v2f vert (appdata_base v)
         {
             v2f o;
             o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
             o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
             return o;
         }
     ENDCG
 
     // Pass for fully visible parts of the object
     Pass {  
         ZTest LEqual
         CGPROGRAM
             fixed4 frag (v2f i) : COLOR
             {
                 fixed4 col = tex2D(_MainTex, i.texcoord);
                 return col;
             }
         ENDCG
     }
     
     //Pass for obscured parts of the object
     Pass {  
         ZTest Greater
         CGPROGRAM
             fixed4 frag (v2f i) : COLOR
             {
                 fixed4 col = tex2D(_MainTex, i.texcoord);
                 col.a *= 0.5;
                 return col;
             }
         ENDCG
     }
 }
 }