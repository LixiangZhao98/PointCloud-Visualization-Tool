using LixaingZhao.PointcloudTool;
using UnityEngine;

public class RunTimeControl_DensityEstimation : RunTimeController
{

public  ComputeShader kde_shader;

private int gridNum=64;
    [SerializeField, SetProperty("GRIDNUM")]
    private Enum.GRIDNum gRIDNum;
    public Enum.GRIDNum GRIDNUM
    {
          get {return gRIDNum;}
            set
            {      
                gRIDNum=value;

        if (value == Enum.GRIDNum.grid64)
            gridNum = 64;
        if (value == Enum.GRIDNum.grid100)
            {gridNum = 99;}
        if (value == Enum.GRIDNum.grid200)
            gridNum = 200;
              if(Application.isPlaying)
             SwitchDatasetFromFile(dataset.ToString());
                
            }
    }


    void Start()
    {
           if (GRIDNUM ==Enum. GRIDNum.grid100)
            {gridNum = 100;}
        if (GRIDNUM == Enum.GRIDNum.grid64)
            gridNum = 64;
        if (GRIDNUM == Enum.GRIDNum.grid200)
            gridNum = 200;
            SwitchDatasetFromFile(dataset.ToString());
    }
    public override void SwitchDatasetFromFile(string name)
    {
        DataMemory.StacksInitialize();
      
        if (dataset_generator.Contains(name))
        {
                  DataMemory.LoadDataByVec3s(DataGenerator.Generate(name), name);    
        }
        else if(dataset_ply.Contains(name))
        {        
                  DataMemory.LoadDataByPly(name);    
        }
        else
        {
               DataMemory.LoadDataByByte(name);
        }
                            if (LoadFlagNames.Count!=0&&LoadFlag)
            DataMemory.LoadFlagsToStack(LoadFlagNames);

        DataMemory.CreateDensityField(gridNum);
        GPUKDECsHelper.StartGpuKDE(DataMemory.allParticle, DataMemory.densityField, kde_shader);
        
        RenderDataRunTime_demo.GenerateMesh();
       this.gameObject.transform.parent.GetComponentInChildren<MarchingCubeGPU>().Init();

    }

    }




