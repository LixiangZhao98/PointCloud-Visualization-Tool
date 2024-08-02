using UnityEngine;
using System.Threading.Tasks;
using System.Diagnostics;


public class GPUKDECsHelper:MonoBehaviour
{
    static ComputeShader kde_Cs;
    static ComputeBuffer slModified;
    static ComputeBuffer parPos;
    static ComputeBuffer nodePos;
    static ComputeBuffer nodeDen;
    static ComputeBuffer parDen;
    static Vector3[] parPos_;
    private static Vector3[] parSL_;
    static Vector3[]  gridPos_;
    static float [] dens;
    const int GROUPSIZE=8;
    const int PARGROUPSIZE=512;
 
    static public void KDEGpu(ParticleGroup pG, DensityField dF, ComputeShader KDE_cs_)
    {

        kde_Cs = KDE_cs_;
        parPos = new ComputeBuffer(pG.GetParticlenum(), 3 * sizeof(float), ComputeBufferType.Default);
        nodePos = new ComputeBuffer(dF.GetNodeNum(), 3 * sizeof(float), ComputeBufferType.Default);
        nodeDen = new ComputeBuffer(dF.GetNodeNum(), sizeof(float));
        parDen = new ComputeBuffer(pG.GetParticlenum(), sizeof(float));
        slModified = new ComputeBuffer(pG.GetParticlenum(), 3 * sizeof(float));
        
        
        int kernel_Pilot = kde_Cs.FindKernel("Pilot");
        int kernel_ParDen = kde_Cs.FindKernel("ParDen");
        int kernel_SL_Modified = kde_Cs.FindKernel("SL_Modified");
        int kernel_FinalDensity = kde_Cs.FindKernel("FinalDensity");

        kde_Cs.SetVector("parMinPos_", new Vector4(pG.XMIN, pG.YMIN, pG.ZMIN, 0f));
        kde_Cs.SetVector("nodeStep_", new Vector4(dF.XSTEP, dF.YSTEP, dF.ZSTEP, 0f));
        kde_Cs.SetVector("nodeNum_", new Vector4(dF.XNUM, dF.YNUM, dF.ZNUM, 0f));
        kde_Cs.SetFloat("parCount_",pG.GetParticlenum() );
        kde_Cs.SetVector("SL_", new Vector4(pG.GetSmoothLength()[0], pG.GetSmoothLength()[1], pG.GetSmoothLength()[2], 0f));


       

        parPos_ = new Vector3[pG.GetParticlenum()];
        parSL_ = new Vector3[pG.GetParticlenum()];
        gridPos_ = new Vector3[dF.GetNodeNum()]; 
        dens = new float[dF.GetNodeNum()];

        for (int i = 0; i < pG.GetParticlenum(); i++)
        {
            parPos_[i] = pG.GetParticleObjectPos(i);
        }
        for (int i = 0; i < dF.GetNodeNum(); i++)
        {
            gridPos_[i] = dF.GetNodedPos(i);
        }
        parPos.SetData(parPos_);
        nodePos.SetData(gridPos_);
        
        kde_Cs.SetBuffer(kernel_Pilot, "partiPos_", parPos);
        kde_Cs.SetBuffer(kernel_Pilot, "nodePos_", nodePos);
        kde_Cs.SetBuffer(kernel_Pilot, "nodeDen_", nodeDen);
        
        kde_Cs.SetBuffer(kernel_ParDen, "partiPos_", parPos);
        kde_Cs.SetBuffer(kernel_ParDen, "nodeDen_", nodeDen);
        kde_Cs.SetBuffer(kernel_ParDen, "parDen_", parDen);

        kde_Cs.SetBuffer(kernel_SL_Modified, "partiPos_", parPos);
        kde_Cs.SetBuffer(kernel_SL_Modified, "nodeDen_", nodeDen);
        kde_Cs.SetBuffer(kernel_SL_Modified, "parDen_", parDen);
        kde_Cs.SetBuffer(kernel_SL_Modified, "SL_ModifiedRW_", slModified);
        
        kde_Cs.SetBuffer(kernel_FinalDensity, "partiPos_", parPos);
        kde_Cs.SetBuffer(kernel_FinalDensity, "nodePos_", nodePos);
        kde_Cs.SetBuffer(kernel_FinalDensity, "nodeDen_", nodeDen);
        kde_Cs.SetBuffer(kernel_FinalDensity, "SL_ModifiedRW_", slModified);
        
        Stopwatch sw = new Stopwatch();
        sw.Start();
  
            kde_Cs.Dispatch(kernel_Pilot, dF.XNUM / GROUPSIZE, dF.YNUM / GROUPSIZE, dF.ZNUM / GROUPSIZE);  
            kde_Cs.Dispatch(kernel_ParDen, pG.GetParticlenum() / PARGROUPSIZE, 1, 1);
            kde_Cs.Dispatch(kernel_SL_Modified, pG.GetParticlenum()  / PARGROUPSIZE, 1, 1);
            kde_Cs.Dispatch(kernel_FinalDensity, dF.XNUM / GROUPSIZE, dF.YNUM / GROUPSIZE, dF.ZNUM / GROUPSIZE);
            nodeDen.GetData(dens);
            slModified.GetData(parSL_);
            
        sw.Stop();
        
        UnityEngine.Debug.Log("Density estimation finish in " + sw.ElapsedMilliseconds+" ms");

        for (int i = 0; i < dF.GetNodeNum(); i++)
        {
            dF.SetNodeDensity(i, dens[i]); //set node density
        }

        float delta = 0.1f * (dF.XSTEP + dF.YSTEP + dF.ZSTEP) / 3;
        Parallel.For(0, dF.GetNodeNum(), i =>
        {
            if (!GridPointIsOnDataBoundary(dF, i))
            {
                Vector3 nodePos = dF.GetNodedPos(i);
                float gradx = ((float)Utility.InterpolateVector(new Vector3(nodePos.x + delta, nodePos.y, nodePos.z), pG, dF) - (float)Utility.InterpolateVector(new Vector3(nodePos.x - delta, nodePos.y, nodePos.z), pG, dF)) / (2 * delta);
                float grady = ((float)Utility.InterpolateVector(new Vector3(nodePos.x, nodePos.y + delta, nodePos.z), pG, dF) - (float)Utility.InterpolateVector(new Vector3(nodePos.x, nodePos.y - delta, nodePos.z), pG, dF)) / (2 * delta);
                float gradz = ((float)Utility.InterpolateVector(new Vector3(nodePos.x, nodePos.y, nodePos.z + delta), pG, dF) - (float)Utility.InterpolateVector(new Vector3(nodePos.x, nodePos.y, nodePos.z - delta), pG, dF)) / (2 * delta);

                dF.SetNodeGradient(i, new Vector3(gradx, grady, gradz));
            }
            else
            {
                dF.SetNodeDensity(i, 0f);
            }
        });

        float minDen=float.MaxValue;float maxDen=float.MinValue;
        float sum = 0f;
        for (int i=0;i<pG.GetParticlenum(); i++)
        {
            float density=(float)Utility.InterpolateVector(pG.GetParticleObjectPos(i), pG, dF);
            sum += density;
            pG.SetParticleDensity(i,density);//set particle density
            if(density>maxDen)
            maxDen=density;
            if(density<minDen)
            minDen=density;
            pG.SetMySmoothLength(parSL_[i].x, parSL_[i].y, parSL_[i].z, i);//set smooth length of all the particles

            // List<Vector3> v = Utility.Emit(pG.GetParticlePosition(i), Vector3.zero, dF, pG);
            // pG.SetFlowEnd(i, (v[v.Count - 1]));
        };

        pG.MAXDEN=maxDen; pG.MINDEN=minDen;
        dF.SetAveNodeDensity(sum / pG.GetParticlenum());
        // Parallel.For(0, pG.GetParticlenum(), i =>
        // {
        //     pG.SetParticleDensity(i, Utility.InterpolateVector(pG.GetParticlePosition(i), pG, dF));//set particle density
        //     pG.SetMySmoothLength(parSL_[i].x, parSL_[i].y, parSL_[i].z, i);//set smooth length of all the particles

        //     List<Vector3> v = Utility.Emit(pG.GetParticlePosition(i), Vector3.zero, dF, pG);
        //     pG.SetFlowEnd(i, (v[v.Count - 1]));
        // });


        AddLUT(pG, dF);

       UnityEngine. Debug.Log("Finish density estimation on GPU; Finish gradient estimation on GPU; Finish particle density and SL estimation on GPU; Finish Adding particles to LUT on GPU");


        slModified.Release();
        parPos.Release();
        nodePos.Release();
        nodeDen.Release();
        parDen.Release();



    }
    static bool GridPointIsOnDataBoundary(DensityField df, int index)
    {
        float _xNum, _yNum, _zNum = 0;
        _xNum = df.GetNodeGridPos(index).x;
        _yNum = df.GetNodeGridPos(index).y;
        _zNum = df.GetNodeGridPos(index).z;
        if (_xNum == 0 || _xNum == df.XNUM - 1 || _xNum == df.XNUM || _yNum == 0 || _yNum == df.YNUM - 1 || _yNum == df.YNUM || _zNum == 0 || _zNum == df.ZNUM - 1 || _zNum == df.ZNUM)
        { return true; }
        else
        {
            return false;
        }
    }
    static public void AddLUT(ParticleGroup pG, DensityField dF)
    {
        dF.LUTInit();

        for (int i = 0; i < pG.GetParticlenum(); i++)
        {
                int x = Mathf.FloorToInt((pG.GetParticleObjectPos(i).x - pG.XMIN) / dF.XSTEP);
                int y = Mathf.FloorToInt((pG.GetParticleObjectPos(i).y - pG.YMIN) / dF.YSTEP);
                int z = Mathf.FloorToInt((pG.GetParticleObjectPos(i).z - pG.ZMIN) / dF.ZSTEP);
                int index = z * dF.XNUM * dF.YNUM + y * dF.XNUM + x;
                dF.AddToLUT(index, i);
        }

    }
    
}





//
//
// //
// //  GPUKDECsHelper.cs
// //  MeTACAST
// //
// //  Copyright (c) 2022, 2023 Lixiang Zhao. All rights reserved.
// //
// using System.Collections.Generic;
// using UnityEngine;
// using System.Threading.Tasks;
// using System.Diagnostics;
//
//
// public class GPUKDECsHelper
// {
//     static ComputeShader KDE_Cs;
//     static ComputeBuffer SL_modified;
//     static ComputeBuffer parPos;
//     static ComputeBuffer gridPos;
//     static ComputeBuffer nodeDen;
//     static ComputeBuffer parDen;
//     static Vector3[] parPos_;
//     static Vector3[]  gridPos_;
//     static Vector3[] parSL_;
//     static float [] dens;
//     static float [] parDens;
//
//     static public void KDEGpu(ParticleGroup pG, DensityField dF, ComputeShader KDE_cs_)
//     {
//
//         KDE_Cs = KDE_cs_;
//         parPos = new ComputeBuffer(pG.GetParticlenum(), 3 * sizeof(float), ComputeBufferType.Default);
//         gridPos = new ComputeBuffer(dF.GetNodeNum(), 3 * sizeof(float), ComputeBufferType.Default);
//         nodeDen = new ComputeBuffer(dF.GetNodeNum(), sizeof(float));
//         parDen = new ComputeBuffer(pG.GetParticlenum(), sizeof(float));
//         SL_modified = new ComputeBuffer(pG.GetParticlenum(), 3 * sizeof(float));
//         
//         
//         int kernel_Pilot = KDE_Cs.FindKernel("Pilot");
//         int kernel_SL_Modified = KDE_Cs.FindKernel("SL_Modified");
//         int kernel_FinalDensity = KDE_Cs.FindKernel("FinalDensity");
//
//         KDE_Cs.SetVector("parMinPos", new Vector4(pG.XMIN, pG.YMIN, pG.ZMIN, 0f));
//         KDE_Cs.SetVector("gridStep", new Vector4(dF.XSTEP, dF.YSTEP, dF.ZSTEP, 0f));
//         KDE_Cs.SetVector("gridNum", new Vector4(dF.XNUM, dF.YNUM, dF.ZNUM, 0f));
//         KDE_Cs.SetFloat("parCount",pG.GetParticlenum() );
//         KDE_Cs.SetVector("SL", new Vector4(pG.GetSmoothLength()[0], pG.GetSmoothLength()[1], pG.GetSmoothLength()[2], 0f));
//
//         parPos_ = new Vector3[pG.GetParticlenum()];
//         gridPos_ = new Vector3[dF.GetNodeNum()]; 
//         parSL_ = new Vector3[pG.GetParticlenum()];
//         dens = new float[dF.GetNodeNum()];
//         parDens = new float[pG.GetParticlenum()];
//
//         for (int i = 0; i < pG.GetParticlenum(); i++)
//         {
//             parPos_[i] = pG.GetParticlePosition(i);
//         }
//         for (int i = 0; i < dF.GetNodeNum(); i++)
//         {
//             gridPos_[i] = dF.GetNodedPos(i);
//         }
//         KDE_Cs.SetBuffer(kernel_Pilot, "partiPos", parPos);
//         KDE_Cs.SetBuffer(kernel_Pilot, "gridPos", gridPos);
//         KDE_Cs.SetBuffer(kernel_Pilot, "Den", nodeDen);
//         
//         KDE_Cs.SetBuffer(kernel_SL_Modified, "partiPos", parPos);
//         KDE_Cs.SetBuffer(kernel_SL_Modified, "gridPos", gridPos);
//         KDE_Cs.SetBuffer(kernel_SL_Modified, "parDen", parDen);
//         KDE_Cs.SetBuffer(kernel_SL_Modified, "SL_ModifiedRW", SL_modified);
//         KDE_Cs.SetBuffer(kernel_SL_Modified, "Den", nodeDen);
//         
//         KDE_Cs.SetBuffer(kernel_FinalDensity, "partiPos", parPos);
//         KDE_Cs.SetBuffer(kernel_FinalDensity, "gridPos", gridPos);
//         KDE_Cs.SetBuffer(kernel_FinalDensity, "Den", nodeDen);
//         KDE_Cs.SetBuffer(kernel_FinalDensity, "SL_ModifiedRW", SL_modified);
//         
//         parPos.SetData(parPos_);
//         gridPos.SetData(gridPos_);
//         Stopwatch sw = new Stopwatch();
//         sw.Start();
//  
//         //-------------------------------------------------------------------------------------------------kernel 0
//         KDE_Cs.Dispatch(kernel_Pilot, dF.XNUM / 8, dF.YNUM / 8, dF.ZNUM / 8);  //pilot density
//         //-------------------------------------------------------------------------------------------------kernel 1
//         KDE_Cs.Dispatch(kernel_SL_Modified, pG.GetParticlenum()/256,1,1);//new SL
//         //-------------------------------------------------------------------------------------------------kernel 2
//         KDE_Cs.Dispatch(kernel_FinalDensity, dF.XNUM / 8, dF.YNUM / 8, dF.ZNUM / 8);  // Final density
//         //-------------------------------------------------------------------------------------------------end
//         
//         nodeDen.GetData(dens);
//         SL_modified.GetData(parSL_);
//         sw.Stop();
//         UnityEngine.Debug.Log("Density estimation finish in " + sw.ElapsedMilliseconds);
//
//         for (int i = 0; i < dF.GetNodeNum(); i++)
//         {
//             dF.SetNodeDensity(i, dens[i]); //set node density
//         }
//
//         float delta = 0.1f * (dF.XSTEP + dF.YSTEP + dF.ZSTEP) / 3;
//         Parallel.For(0, dF.GetNodeNum(), i =>
//         {
//             if (!GridPointIsOnDataBoundary(dF, i))
//             {
//                 Vector3 nodePos = dF.GetNodedPos(i);
//                 float gradx = ((float)Utility.InterpolateVector(new Vector3(nodePos.x + delta, nodePos.y, nodePos.z), pG, dF) - (float)Utility.InterpolateVector(new Vector3(nodePos.x - delta, nodePos.y, nodePos.z), pG, dF)) / (2 * delta);
//                 float grady = ((float)Utility.InterpolateVector(new Vector3(nodePos.x, nodePos.y + delta, nodePos.z), pG, dF) - (float)Utility.InterpolateVector(new Vector3(nodePos.x, nodePos.y - delta, nodePos.z), pG, dF)) / (2 * delta);
//                 float gradz = ((float)Utility.InterpolateVector(new Vector3(nodePos.x, nodePos.y, nodePos.z + delta), pG, dF) - (float)Utility.InterpolateVector(new Vector3(nodePos.x, nodePos.y, nodePos.z - delta), pG, dF)) / (2 * delta);
//
//                 dF.SetNodeGradient(i, new Vector3(gradx, grady, gradz));
//             }
//             else
//             {
//                 dF.SetNodeDensity(i, 0f);
//             }
//         });
//
//         float minDen=float.MaxValue;float maxDen=float.MinValue;
//         float sum = 0f;
//         for (int i=0;i<pG.GetParticlenum(); i++)
//         {
//             float density=(float)Utility.InterpolateVector(pG.GetParticlePosition(i), pG, dF);
//             sum += density;
//             pG.SetParticleDensity(i,density);//set particle density
//             if(density>maxDen)
//             maxDen=density;
//             if(density<minDen)
//             minDen=density;
//             // pG.SetMySmoothLength(parSL_[i].x, parSL_[i].y, parSL_[i].z, i);//set smooth length of all the particles
//
//             // List<Vector3> v = Utility.Emit(pG.GetParticlePosition(i), Vector3.zero, dF, pG);
//             // pG.SetFlowEnd(i, (v[v.Count - 1]));
//         };
//
//         pG.MAXDEN=maxDen; pG.MINDEN=minDen;
//         dF.SetAveNodeDensity(sum / pG.GetParticlenum());
//         // Parallel.For(0, pG.GetParticlenum(), i =>
//         // {
//         //     pG.SetParticleDensity(i, Utility.InterpolateVector(pG.GetParticlePosition(i), pG, dF));//set particle density
//         //     pG.SetMySmoothLength(parSL_[i].x, parSL_[i].y, parSL_[i].z, i);//set smooth length of all the particles
//
//         //     List<Vector3> v = Utility.Emit(pG.GetParticlePosition(i), Vector3.zero, dF, pG);
//         //     pG.SetFlowEnd(i, (v[v.Count - 1]));
//         // });
//
//
//         AddLUT(pG, dF);
//
//        UnityEngine. Debug.Log("Finish density estimation on GPU; Finish gradient estimation on GPU; Finish particle density and SL estimation on GPU; Finish Adding particles to LUT on GPU");
//
//
//         SL_modified.Release();
//         parPos.Release();
//         gridPos.Release();
//         nodeDen.Release();
//         parDen.Release();
//
//
//
//     }
//     static bool GridPointIsOnDataBoundary(DensityField df, int index)
//     {
//         float _xNum, _yNum, _zNum = 0;
//         _xNum = df.GetNodeGridPos(index).x;
//         _yNum = df.GetNodeGridPos(index).y;
//         _zNum = df.GetNodeGridPos(index).z;
//         if (_xNum == 0 || _xNum == df.XNUM - 1 || _xNum == df.XNUM || _yNum == 0 || _yNum == df.YNUM - 1 || _yNum == df.YNUM || _zNum == 0 || _zNum == df.ZNUM - 1 || _zNum == df.ZNUM)
//         { return true; }
//         else
//         {
//             return false;
//         }
//     }
//     static public void AddLUT(ParticleGroup pG, DensityField dF)
//     {
//         dF.LUTInit();
//
//         for (int i = 0; i < pG.GetParticlenum(); i++)
//         {
//             if (dF.VectorToBoxIndex(pG.GetParticlePosition(i), pG) != -1)
//             {
//                 int x = Mathf.FloorToInt((pG.GetParticlePosition(i).x - pG.XMIN) / dF.XSTEP);
//                 int y = Mathf.FloorToInt((pG.GetParticlePosition(i).y - pG.YMIN) / dF.YSTEP);
//                 int z = Mathf.FloorToInt((pG.GetParticlePosition(i).z - pG.ZMIN) / dF.ZSTEP);
//                 int index = z * dF.XNUM * dF.YNUM + y * dF.XNUM + x;
//                 dF.AddToLUT(index, i);
//             }
//         }
//
//     }
//
//
// }
//


















