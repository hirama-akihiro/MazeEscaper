xof 0302txt 0064
template Header {
 <3D82AB43-62DA-11cf-AB39-0020AF71E433>
 WORD major;
 WORD minor;
 DWORD flags;
}

template Vector {
 <3D82AB5E-62DA-11cf-AB39-0020AF71E433>
 FLOAT x;
 FLOAT y;
 FLOAT z;
}

template Coords2d {
 <F6F23F44-7686-11cf-8F52-0040333594A3>
 FLOAT u;
 FLOAT v;
}

template Matrix4x4 {
 <F6F23F45-7686-11cf-8F52-0040333594A3>
 array FLOAT matrix[16];
}

template ColorRGBA {
 <35FF44E0-6C7C-11cf-8F52-0040333594A3>
 FLOAT red;
 FLOAT green;
 FLOAT blue;
 FLOAT alpha;
}

template ColorRGB {
 <D3E16E81-7835-11cf-8F52-0040333594A3>
 FLOAT red;
 FLOAT green;
 FLOAT blue;
}

template IndexedColor {
 <1630B820-7842-11cf-8F52-0040333594A3>
 DWORD index;
 ColorRGBA indexColor;
}

template Boolean {
 <4885AE61-78E8-11cf-8F52-0040333594A3>
 WORD truefalse;
}

template Boolean2d {
 <4885AE63-78E8-11cf-8F52-0040333594A3>
 Boolean u;
 Boolean v;
}

template MaterialWrap {
 <4885AE60-78E8-11cf-8F52-0040333594A3>
 Boolean u;
 Boolean v;
}

template TextureFilename {
 <A42790E1-7810-11cf-8F52-0040333594A3>
 STRING filename;
}

template Material {
 <3D82AB4D-62DA-11cf-AB39-0020AF71E433>
 ColorRGBA faceColor;
 FLOAT power;
 ColorRGB specularColor;
 ColorRGB emissiveColor;
 [...]
}

template MeshFace {
 <3D82AB5F-62DA-11cf-AB39-0020AF71E433>
 DWORD nFaceVertexIndices;
 array DWORD faceVertexIndices[nFaceVertexIndices];
}

template MeshFaceWraps {
 <4885AE62-78E8-11cf-8F52-0040333594A3>
 DWORD nFaceWrapValues;
 Boolean2d faceWrapValues;
}

template MeshTextureCoords {
 <F6F23F40-7686-11cf-8F52-0040333594A3>
 DWORD nTextureCoords;
 array Coords2d textureCoords[nTextureCoords];
}

template MeshMaterialList {
 <F6F23F42-7686-11cf-8F52-0040333594A3>
 DWORD nMaterials;
 DWORD nFaceIndexes;
 array DWORD faceIndexes[nFaceIndexes];
 [Material]
}

template MeshNormals {
 <F6F23F43-7686-11cf-8F52-0040333594A3>
 DWORD nNormals;
 array Vector normals[nNormals];
 DWORD nFaceNormals;
 array MeshFace faceNormals[nFaceNormals];
}

template MeshVertexColors {
 <1630B821-7842-11cf-8F52-0040333594A3>
 DWORD nVertexColors;
 array IndexedColor vertexColors[nVertexColors];
}

template Mesh {
 <3D82AB44-62DA-11cf-AB39-0020AF71E433>
 DWORD nVertices;
 array Vector vertices[nVertices];
 DWORD nFaces;
 array MeshFace faces[nFaces];
 [...]
}

Header{
1;
0;
1;
}

Mesh {
 26;
 0.31990;-0.01096;-0.00000;,
 0.00000;0.47783;0.00000;,
 0.00000;-0.01096;0.31990;,
 -0.31990;-0.01096;0.00000;,
 0.00000;-0.01096;-0.31990;,
 0.00000;-0.49975;-0.00000;,
 0.00000;0.35531;0.09688;,
 0.09688;0.35531;0.00000;,
 0.00000;0.50334;0.00000;,
 -0.09688;0.35531;0.00000;,
 0.00000;0.35531;-0.09688;,
 0.00000;0.35531;0.08220;,
 0.00000;0.47162;0.00000;,
 0.08220;0.35531;0.00000;,
 -0.08220;0.35531;0.00000;,
 0.00000;0.35531;-0.08220;,
 0.00000;-0.35531;0.09688;,
 -0.09688;-0.35531;0.00000;,
 0.00000;-0.50334;-0.00000;,
 0.09688;-0.35531;-0.00000;,
 0.00000;-0.35531;-0.09688;,
 0.00000;-0.35531;0.08220;,
 0.00000;-0.47162;-0.00000;,
 -0.08220;-0.35531;0.00000;,
 0.08220;-0.35531;-0.00000;,
 0.00000;-0.35531;-0.08220;;
 
 40;
 3;0,1,2;,
 3;2,1,3;,
 3;3,1,4;,
 3;4,1,0;,
 3;0,5,4;,
 3;2,5,0;,
 3;3,5,2;,
 3;4,5,3;,
 3;6,7,8;,
 3;9,6,8;,
 3;10,9,8;,
 3;7,10,8;,
 3;11,12,13;,
 3;14,12,11;,
 3;15,12,14;,
 3;13,12,15;,
 4;7,6,11,13;,
 4;6,9,14,11;,
 4;9,10,15,14;,
 4;10,7,13,15;,
 3;16,17,18;,
 3;19,16,18;,
 3;20,19,18;,
 3;17,20,18;,
 3;21,22,23;,
 3;24,22,21;,
 3;25,22,24;,
 3;23,22,25;,
 4;17,16,21,23;,
 4;16,19,24,21;,
 4;19,20,25,24;,
 4;20,17,23,25;,
 3;2,1,0;,
 3;3,1,2;,
 3;4,1,3;,
 3;0,1,4;,
 3;4,5,0;,
 3;0,5,2;,
 3;2,5,3;,
 3;3,5,4;;
 
 MeshMaterialList {
  2;
  40;
  1,
  1,
  1,
  1,
  1,
  1,
  1,
  1,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  1,
  1,
  1,
  1,
  1,
  1,
  1,
  1;;
  Material {
   0.661600;0.649600;0.047200;1.000000;;
   12.000000;
   0.830000;0.830000;0.830000;;
   0.165400;0.162400;0.011800;;
  }
  Material {
   0.039900;0.000000;0.300000;0.700000;;
   29.000000;
   0.800000;0.800000;0.800000;;
   0.039900;0.000000;0.300000;;
  }
 }
}
