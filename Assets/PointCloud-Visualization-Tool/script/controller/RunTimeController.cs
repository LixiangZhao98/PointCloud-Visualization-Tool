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
            SwitchDatasetFromFile(dataset.ToString());
                    
            }
        }
        protected List<string> dataset_generator = new List<string> { "random_sphere" };
        protected List<string> dataset_ply = new List<string> { "dragon_vrip" };

   
    public Material unselected_mat;
    public Material selected_mat;
    public bool LoadTarget;
    [SerializeField]
    public List<FlagNamesCollection> LoadFlagNames;
    public string StoreFlagName;



    public bool CalculateDensity;
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

            if (value == EnumVariables.GRIDNum.grid64)
                gridNum = 64;
            if (value == EnumVariables.GRIDNum.grid100)
            { gridNum = 99; }
            if (value == EnumVariables.GRIDNum.grid200)
                gridNum = 200;
            if (Application.isPlaying)
                SwitchDatasetFromFile(dataset.ToString());

        }
    }
        public UnityAction myAction;
        public UnityEvent myEvent;

        #endregion

    private void Start()
    {
        SwitchDatasetFromFile(dataset.ToString());

    }

     public void SetGRIDNum(int g)
    {
        gridNum = g;
    }
    public void  SwitchDatasetFromFile(string name)
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
                            
        if (LoadFlagNames.Count!=0&&LoadTarget)
            DataMemory.LoadFlagsToStack(LoadFlagNames);

           if( CalculateDensity)
            {
                DataMemory.CreateDensityField(gridNum);
                GPUKDECsHelper.StartGpuKDE(DataMemory.allParticle, DataMemory.densityField, kde_shader);
                this.transform.parent.GetComponentInChildren<MarchingCubeGPU>().enabled=true;
                this.transform.parent.GetComponentInChildren<MarchingCubeGPU>().Init();
            }
           else
            {
                this.transform.parent.GetComponentInChildren<MarchingCubeGPU>().enabled = false;
            }
            RenderDataRunTime.Init(visCenter,unselected_mat,selected_mat,LoadFlagNames);
            myEvent?.Invoke();

        }

    }



