using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MarchingCubeGPUCSHelper : MonoBehaviour
    {
        public float McThreshold;
        public ComputeShader marchingCubesCS;
        private ComputeShader marchingCubesCSInstance;
        private Transform origin;
        private Texture3D DensityTexture { get; set; }
        private Texture3D PosTexture { get; set; }
        private Texture3D McFlagTexture { get; set; }
        private Material meshMaterial;
        private PointRenderer pR;
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
        Bounds bounds;


        public void MarchingCubeGpuCsHelperInit()
        {
            origin = transform.parent;
            origin.gameObject.name += "_MarchingCube";
            DensityField dF = transform.parent.GetComponentInChildren<GPUKDECsHelper>().densityField;
            pR = transform.parent.GetComponentInChildren<PointRenderer>();
            marchingCubesCSInstance =  Instantiate(marchingCubesCS);
            
            meshMaterial = new Material(Shader.Find("Custom/MCmesh"));
            kernelMC = marchingCubesCSInstance.FindKernel("MarchingCubes");
            ResolutionX = dF.XNUM;
            ResolutionY = dF.YNUM;
            ResolutionZ = dF.ZNUM;

            SetDensityTexture(dF);
            SetPosTexture(dF);
            List<int> denList = new List<int>();
            for (int i = 0; i < dF.GetNodeNum(); i++)
                denList.Add(i);
            SetMCFlagTexture(denList);

            appendVertexBuffer = new ComputeBuffer((ResolutionX) * (ResolutionY) * (ResolutionZ) * 5, sizeof(float) * 18, ComputeBufferType.Append);
          

            argBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
            
            meshMaterial.SetPass(0);
            meshMaterial.SetBuffer("triangleRW", appendVertexBuffer);
            
            marchingCubesCSInstance.SetBuffer(kernelMC, "triangleRW", appendVertexBuffer);
            marchingCubesCSInstance.SetInt("_gridSize", ResolutionX);
            bounds = new Bounds(Vector3.zero, Vector3.one * 100000);
        }

        private void Update()
        {

            marchingCubesCSInstance.SetFloat("_isoLevel", McThreshold);

            appendVertexBuffer.SetCounterValue(0);
     

            marchingCubesCSInstance.Dispatch(kernelMC, ResolutionX / 8, ResolutionY / 8, ResolutionZ / 8);
           
            args = new int[] { 0, 1, 0, 0 };
            argBuffer.SetData(args);

            ComputeBuffer.CopyCount(appendVertexBuffer, argBuffer, 0);
 

            argBuffer.GetData(args);
           
            meshMaterial.SetMatrix("_LocalToWorld", Matrix4x4.TRS(origin.transform.position, origin.transform.rotation, new Vector3(pR.xRatio,pR.yRatio,pR.zRatio)));
            Graphics.DrawProcedural(meshMaterial, bounds, MeshTopology.Triangles, args[0]*3, 1);
           

            args[0] *= 3;
            argBuffer.SetData(args);
          
        }

    
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
        if(!this.enabled)
            return;
            appendVertexBuffer.Release();
            argBuffer.Release();
     
        }



        public void SetMCGPUThreshold(float f)
        {
            McThreshold = f;
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
            marchingCubesCSInstance.SetTexture(kernelMC, "_densityTexture", DensityTexture);

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
            marchingCubesCSInstance.SetTexture(kernelMC, "_posTexture", PosTexture);

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
            marchingCubesCSInstance.SetTexture(kernelMC, "_mcFlagTexture", McFlagTexture);
        }

    }
    
    
