using PavelKouril.MarchingCubesGPU;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunTimeController : MonoBehaviour
{

        private void Start()
    {   gridNum=0;
        SwitchDatasetFromFile(dataset.ToString());
    }


  private int gridNum;
  [SerializeField, SetProperty("GRIDNUM")]
    private GRIDNum gRIDNum;
    public GRIDNum GRIDNUM
        {
            get { return gRIDNum; }
            set {     if (gRIDNum == GRIDNum.none)
            gridNum = 0;    
                if (gRIDNum == GRIDNum.grid100)
            gridNum = 99;
        if (gRIDNum == GRIDNum.grid64)
            gridNum = 64;
        if (gRIDNum == GRIDNum.grid200)
            gridNum = 200;

       SwitchDatasetFromFile(dataset.ToString());
            }
        }


  [SerializeField, SetProperty("DATASET")]
    private Dataset dataset;
    public Dataset DATASET
        {
            get { return dataset; }
            set { 
            SwitchDatasetFromFile(dataset.ToString());
                    
            }
        }
   [Serializable]
    public class FlagNamesCollection
    {
        
        public string[] FlagNames;
        public Material target_mat;
    }

    public Material unselected_mat;
    public Material selected_mat;
    
    [SerializeField]
    public List<FlagNamesCollection> LoadFlagNames;

    private List<string> dataset_generator = new List<string> { "random_sphere" };
  public bool LoadFlag;

    public string StoreFlagName;
  
    
    public ComputeShader kde_shader;




    [ContextMenu("StoreFlags")]
    public void StoreFlages()
    {
      DataMemory.StoreFlags(StoreFlagName);
    }

    public void SwitchDatasetFromFile(string name)
    {
        DataMemory.StacksInitialize();
      
        if (!dataset_generator.Contains(name))
        {
                   DataMemory.LoadDataByByte(name);
                    if (LoadFlagNames.Count!=0&&LoadFlag)
            DataMemory.LoadFlagsToStack(LoadFlagNames);
           
        }
        else
        {
                    DataMemory.LoadDataByVec3s(DataGenerator.Generate(name), name);
                 if (LoadFlagNames.Count!=0&&LoadFlag)
            DataMemory.LoadFlagsToStack(LoadFlagNames);
        }
        
        if(gRIDNum!=0)
        {
        DataMemory.CreateDensityField(gridNum);
        GPUKDECsHelper.StartGpuKDE(DataMemory.allParticle, DataMemory.densityField, kde_shader);
        }
       

        RenderDataRunTime.GenerateMesh();
   

    }

    }

[Serializable]
public enum Dataset { disk,uniform_Lines, ball_hemisphere, ununiform_Lines, Flocculentcube1, strings, Flocculentcube2, Flocculentcube3, galaxy, nbody1, nbody2, training_torus, random_sphere , three_rings, multiEllipsolds, fiveellipsolds , stringf , stringf1, snap_C02_200_127_animation,complex };
[Serializable]
public enum GRIDNum { none,grid64,grid100,grid200 };


