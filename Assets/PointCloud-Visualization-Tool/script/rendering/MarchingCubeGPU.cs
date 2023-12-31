﻿using ScalarField;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PavelKouril.MarchingCubesGPU
{
    public class MarchingCubeGPU : MonoBehaviour
    {
        public float MCGPUThreshold;
        public ComputeShader MarchingCubesCS;
        public Material meshMaterial;
        public GameObject map;
        public Texture3D DensityTexture { get; set; }
        public Texture3D PosTexture { get; set; }
        public Texture3D McFlagTexture { get; set; }
        Color[] colors_den;
        Color[] colors_pos;
        Color[] colors_McFlag;
        int kernelMC;
        int ResolutionX;
        int ResolutionY;
        int ResolutionZ;
        ComputeBuffer appendVertexBuffer;
        ComputeBuffer argBuffer;
        int[] args;
        RenderDataRunTime_demo r;
        Bounds bounds;
        //Triangle[] ts;
        //Mesh m;
        //int[] index; Vector3[] norm; Vector3[] vertex;

        public void Init()
        {
            r = transform.parent.GetComponentInChildren<RenderDataRunTime_demo>();
            kernelMC = MarchingCubesCS.FindKernel("MarchingCubes");
            ResolutionX = DataMemory.densityField.XNUM;
            ResolutionY = DataMemory.densityField.YNUM;
            ResolutionZ = DataMemory.densityField.ZNUM;

            SetDensityTexture(DataMemory.densityField);
            SetPosTexture(DataMemory.densityField);
            List<int> a = new List<int>();
            for (int i = 0; i < DataMemory.densityField.GetNodeNum(); i++)
                a.Add(i);
            SetMCFlagTexture(a);

            appendVertexBuffer = new ComputeBuffer((ResolutionX) * (ResolutionY) * (ResolutionZ) * 5, sizeof(float) * 18, ComputeBufferType.Append);
          

            argBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
            meshMaterial.SetPass(0);
            meshMaterial.SetBuffer("triangleRW", appendVertexBuffer);
            MarchingCubesCS.SetBuffer(kernelMC, "triangleRW", appendVertexBuffer);
            MarchingCubesCS.SetInt("_gridSize", ResolutionX);
            SetMCGPUThreshold(0f);
            bounds = new Bounds(Vector3.zero, Vector3.one * 100000);
        }

        private void Update()
        {

            MarchingCubesCS.SetFloat("_isoLevel", MCGPUThreshold);

            appendVertexBuffer.SetCounterValue(0);
     

            MarchingCubesCS.Dispatch(kernelMC, ResolutionX / 8, ResolutionY / 8, ResolutionZ / 8);
           
            args = new int[] { 0, 1, 0, 0 };
            argBuffer.SetData(args);

            ComputeBuffer.CopyCount(appendVertexBuffer, argBuffer, 0);
 

            argBuffer.GetData(args);
           
            
            meshMaterial.SetMatrix("_LocalToWorld", map.transform.localToWorldMatrix);
            meshMaterial.SetMatrix("_WorldToLocal", map. transform.worldToLocalMatrix);
            Graphics.DrawProcedural(meshMaterial, bounds, MeshTopology.Triangles, args[0]*3, 1);
            //m = null;
            //ts = new Triangle[args[0]];
            //index = new int[3 * ts.Length];
            //norm = new Vector3[3 * ts.Length];
            //vertex = new Vector3[3 * ts.Length];
            //m = new Mesh();



            //  appendVertexBuffer.GetData(ts);



            //for (int i = 0; i < args[0]; i++)
            //{
            //    norm[3 * i] = ts[i].v1.Norm;
            //    norm[3 * i + 1] = ts[i].v2.Norm;
            //    norm[3 * i + 2] = ts[i].v3.Norm;
            //    vertex[3 * i] = ts[i].v1.point;
            //    vertex[3 * i + 1] = ts[i].v2.point;
            //    vertex[3 * i + 2] = ts[i].v3.point;

            //}
            //for (int i = 0; i < 3 * args[0]; i++)
            //{
            //    index[i] = i;
            //}
            //m.Clear();
            //m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            //m.vertices = vertex;
            //m.normals = norm;
            //m.SetIndices(index, MeshTopology.Triangles, 0);
            //m.RecalculateNormals();

           

            args[0] *= 3;
            argBuffer.SetData(args);
          
        }

        //private void OnRenderObject ()
        //{
        //  //  DrawMCNow(r.map, r.ratio);
        //}
        //public void  DrawMCNow(GameObject g, float s)
        //{
        //    Matrix4x4 mm = Matrix4x4.TRS(g.transform.position, g.transform.rotation, new Vector3(s,s,s));
        //    Graphics.DrawMesh (m, mm, meshMaterial, 0);
        //}

        struct Vertex_
        {
            public Vector3 point;
            public Vector3 Norm;
        }
        struct Triangle
        {
            public Vertex_ v1;
            public Vertex_ v2;
            public Vertex_ v3;
        };
        private void OnDestroy()
        {
            appendVertexBuffer.Release();
            argBuffer.Release();
            //m=null;
     
        }



        public void GenerateMCMesh(int numOFTri)
        {

        }
        public void SetMCGPUThreshold(float f)
        {
            MCGPUThreshold = f;
        }

        public void SetDensityTexture(DensityField dF)
        {
            DensityTexture = new Texture3D(ResolutionX, ResolutionY, ResolutionZ, TextureFormat.RFloat, false);
            DensityTexture.wrapMode = TextureWrapMode.Clamp;
            colors_den = new Color[ResolutionX * ResolutionY * ResolutionZ];
            for (int i = 0; i < colors_den.Length; i++) colors_den[i] = Color.black;

            var idx = 0;
            for (var z = 0; z < ResolutionZ; z++)
            {
                for (var y = 0; y < ResolutionY; y++)
                {
                    for (var x = 0; x < ResolutionX; x++, idx++)
                    {
                       
                     
                            colors_den[idx].r = (float)dF.GetNodeDensity(idx);
                    }
                }
            }
            DensityTexture.SetPixels(colors_den);
            DensityTexture.Apply();
            MarchingCubesCS.SetTexture(kernelMC, "_densityTexture", DensityTexture);

        }

        public void SetPosTexture(DensityField dF)
        {
            PosTexture = new Texture3D(ResolutionX, ResolutionY, ResolutionZ, TextureFormat.RGBAFloat, false);
            PosTexture.wrapMode = TextureWrapMode.Clamp;
            colors_pos = new Color[ResolutionX * ResolutionY * ResolutionZ];
            for (int i = 0; i < colors_pos.Length; i++) colors_pos[i] = Color.clear;

            var idx = 0;
            for (var z = 0; z < ResolutionZ; z++)
            {
                for (var y = 0; y < ResolutionY; y++)
                {
                    for (var x = 0; x < ResolutionX; x++, idx++)
                    {
                        colors_pos[idx].r = dF.GetNodedPos(idx).x;
                        colors_pos[idx].g = dF.GetNodedPos(idx).y;
                        colors_pos[idx].b = dF.GetNodedPos(idx).z;

                    }
                }
            }
            PosTexture.SetPixels(colors_pos);
            PosTexture.Apply();
            MarchingCubesCS.SetTexture(kernelMC, "_posTexture", PosTexture);

        }
        public void SetMCFlagTexture(List<int> list)
        {
            McFlagTexture = new Texture3D(ResolutionX, ResolutionY, ResolutionZ, TextureFormat.RFloat, false);
            McFlagTexture.wrapMode = TextureWrapMode.Clamp;
            colors_McFlag = new Color[ResolutionX * ResolutionY * ResolutionZ];
            for (int i = 0; i < colors_McFlag.Length; i++) colors_McFlag[i] = Color.black;

            for (var i = 0; i < list.Count; i++)
            {
                colors_McFlag[list[i]].r = 1;
            }
            McFlagTexture.SetPixels(colors_McFlag);
            McFlagTexture.Apply();
            MarchingCubesCS.SetTexture(kernelMC, "_mcFlagTexture", McFlagTexture);
        }

    }
    
    
}