using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class RunTimeController : MonoBehaviour
{

    #region variables
    public  GameObject visCenter;
    [SerializeField, SetProperty("DATASET")]
    protected EnumVariables.Dataset dataset;
    public EnumVariables.Dataset DATASET
        {
            get { return dataset; }
            set { 
              dataset=value;
              if(Application.isPlaying)
            LoadData(dataset.ToString());
                    
            }
        }
        protected List<string> dataset_generator = new List<string> { "random_sphere" };
        protected List<string> dataset_ply = new List<string> { "dragon_vrip" };

   
    public Material unselected_mat;
    public Material selected_mat;
    
    // public bool loadTarget;
    // public List<FlagNamesCollection> loadTargetNames;


    public bool calDen;
    public ComputeShader kde_shader;
    private int gridNum = 64;
    [SerializeField, SetProperty("GRIDNUM")]
    private EnumVariables.GRIDNum gRIDNum;
    public EnumVariables.GRIDNum GRIDNUM
    {
        get { return gRIDNum; }
        set
        {
            gRIDNum = value;
            gridNum = (int)(value); 
            if (Application.isPlaying)
                LoadData(dataset.ToString());
        }
    }
         public UnityEvent eventAfterDensityEstimation;

        #endregion

    private void Start()
    {
        GRIDNUM = GRIDNUM;
    }
    
    public void  LoadData(string name)
    {
        DataMemory.StacksInitialize();

        Load(name); //load data
        
        // if (loadTargetNames.Count!=0&&loadTarget)  //load target points with highlighted color
        //     DataMemory.LoadFlagsToStack(loadTargetNames);
        // RenderDataRunTime.Init(visCenter,unselected_mat,selected_mat,loadTargetNames);  
        
        RenderDataRunTime.Init(visCenter,unselected_mat,selected_mat); // set rendering parameters
        
        if(calDen)   CalDen(); //calculate density
        
        eventAfterDensityEstimation?.Invoke(); //actions after data load
        
    }



    public void Load(string name)
    {
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
    }
    
    public void CalDen()
    {
            DataMemory.CreateDensityField(gridNum);
            GPUKDECsHelper.KDEGpu(DataMemory.particles, DataMemory.densityField, kde_shader);
    }

}



 