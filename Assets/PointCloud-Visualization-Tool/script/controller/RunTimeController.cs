using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System;
using System.Linq;
public class RunTimeController : MonoBehaviour
{

    #region variables
    public  GameObject visCenter;
    [SerializeField, SetProperty("DATASET")]
    protected Dataset dataset;
    public Dataset DATASET
        {
            get { return dataset; }
            set { 
              dataset=value;
              if(Application.isPlaying)
            LoadData((int)dataset);
              

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
    private GRIDRes gRIDNum;
    public GRIDRes GRIDNUM
    {
        get { return gRIDNum; }
        set
        {
            gRIDNum = value;
            gridNum = (int)(value); 
            if (Application.isPlaying)
                LoadData((int)dataset);
        }
    }
         public UnityEvent eventAfterDensityEstimation;

        #endregion

    private void Start()
    {
        GRIDNUM = GRIDNUM;
    }
    
    public void  LoadData(int index)
    {
        DataStorage.StacksInitialize();

        Load(index); //load data
        
        // if (loadTargetNames.Count!=0&&loadTarget)  //load target points with highlighted color
        //     DataMemory.LoadFlagsToStack(loadTargetNames);
        // RenderDataRunTime.Init(visCenter,unselected_mat,selected_mat,loadTargetNames);  
        
        RenderDataRunTime.Init(visCenter,unselected_mat,selected_mat); // set rendering parameters
        
        if(calDen)   CalDen(); //calculate density
        
        eventAfterDensityEstimation?.Invoke(); //actions after data load
        
    }



    public void Load(int index)
    {
        string dataPath = Application.dataPath+ "/PointCloud-Visualization-Tool/data/data";
        int n = index*2; //exclude .meta file

        try
        {
            string[] files = Directory.GetFiles(dataPath).ToArray();

            if (n >= 0 && n < files.Length)
            {
                string nthFileName = Path.GetFileNameWithoutExtension(files[n]); 
                string nthFileExtention = Path.GetExtension(files[n]);
                if (nthFileExtention == ".bin")
                {
                    DataStorage.LoadByte(nthFileName+nthFileExtention);
                }
                else if (nthFileExtention == ".ply")
                {
                    DataStorage.LoadPly(nthFileName+nthFileExtention);    
                }
                
            }
            else
            {
                Console.WriteLine("exceed index. Total {0} files.", files.Length);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("error: " + e.Message);
        }
    }
    
    public void CalDen()
    {
            DataStorage.CreateField(gridNum);
            GPUKDECsHelper.KDEGpu(DataStorage.particles, DataStorage.densityField, kde_shader);
    }

}



 