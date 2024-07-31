
using System.Collections.Generic;
using UnityEngine;

 
public class DataMemory : MonoBehaviour
{

    #region Operation Stack and DisplayInfo

    private static Stack<List<int>> pStack;
    private static Stack<List<int>> pOperateStack;



    public static void StacksInitialize()
    {
        pStack = new Stack<List<int>>();
        pOperateStack=new Stack<List<int>>();

    }
    public static void LoadFlagsToStack(List<FlagNamesCollection> names)
    {
        foreach(var name in names)
        {
          for(int n=0;n<name.FlagNames.Length;n++)
        {
        int[] flags = LoadDataBybyte.StartLoadFlags(Application.dataPath + "/PointCloud-Visualization-Tool/data/flags/" + particles.name+"_"+ name.FlagNames[n]);
        for (int i = 0; i < flags.Length; i++)
        {

            DataMemory.particles.SetTarget(flags[i], true, names.IndexOf(name)); 
        }
        }
        }
  
        }
      

    public static void AddParticles(List<int> l)  // previous+new
    {
        if (pStack.Count == 0)
            pStack.Push(l);
        else
        {
            List<int> newl = new List<int>();
            newl.AddRange( pStack.Peek());
            newl.AddRange(l);
            pStack.Push(newl);
        }
    }

    public static void AddParticlesDirectly(List<int> l)  //only add new, previous is not considered
    {
        pStack.Push(l);
    }


    public static List<int> GetpStack()
    {
        if(pStack.Count>0)
        return pStack.Peek();
        else
            return new List<int>();
    }

    public static void Return() 
    {
        if (pStack.Count == 0)
            return;
          pOperateStack.Push(  pStack.Pop());
    }
    public static void Forward()
    {
        if (pOperateStack.Count == 0)
            return;
       pStack.Push(pOperateStack.Pop());
    }
    public static void ReleaseOperatorStack() 
    {

        pOperateStack = new Stack<List<int>>();
    }




    public static void DisplayAllParticle(bool loadFlag, List<FlagNamesCollection>  LoadFlagNames)
    {
        StacksInitialize();
            LoadFlagsToStack(LoadFlagNames);
        DisplayParticles.DisplayMesh(GameObject.Find("PointCloudMesh"), particles);
    }
    #endregion

    #region ParticleInfo
    [SerializeField]
    [HideInInspector] public List<Vector3> particleflow_dest;
    [SerializeField]
    static public ParticleGroup particles = new ParticleGroup();
    static public void LoadDataByPly(string loadFileName)
    {

        particles.LoadDatasetsByPly(Application.dataPath + "/PointCloud-Visualization-Tool/data/data/",loadFileName);

        Debug.Log("Load success" + " " + loadFileName + " with " + particles.GetParticlenum() + " particles." + " SmoothLength: " + particles.GetSmoothLength());

    }
    public static void LoadDataByByte(string loadFileName)
    {

        particles.LoadDatasetByByte(Application.dataPath + "/PointCloud-Visualization-Tool/data/data/" + loadFileName,loadFileName);
        Debug.Log("Load success" + " " + loadFileName + " with " + particles.GetParticlenum() + " particles." + " SmoothLength: " + particles.GetSmoothLength().x + " " + particles.GetSmoothLength().y + " " + particles.GetSmoothLength().z);
    }
    static public void LoadDataByCsv(string loadFileName)
    {

        particles.LoadDatasetByCsv(Application.dataPath + "/PointCloud-Visualization-Tool/data/data/" + loadFileName, loadFileName);
        Debug.Log("Load success" + " " + loadFileName + " with " + particles.GetParticlenum() + " particles." + " SmoothLength: " + particles.GetSmoothLength());
    }
   
    static public void LoadDataByVec3s(Vector3[] v, string dataname, bool forSimulation = false)
    {


        particles.LoadDatasetByVec3s(v, dataname, forSimulation);

        Debug.Log("Load success" + " " + dataname + " with " + particles.GetParticlenum() + " particles." + " SmoothLength: " + particles.GetSmoothLength());

    }

   
    public static void ClearParticleMemory()
    {
        particles = new ParticleGroup();
    }

    
    #endregion

    #region DensityFieldInfo
    [SerializeField]
  
    static public DensityField densityField = new DensityField();
    [SerializeField]
    static public void CreateDensityField(int gridNum)
    {
        float scalingFactor = 0f;
        float step = (particles.XMAX - particles.XMIN) / gridNum;
        particles.XMAX += scalingFactor*step;
        particles.XMIN -= scalingFactor*step;
        particles.YMAX += scalingFactor*step;
        particles.YMIN -= scalingFactor*step;
        particles.ZMAX += scalingFactor*step;
        particles.ZMIN -= scalingFactor*step;
        densityField.InitializeDensityFieldByGapDis(particles.name, particles.XMIN, particles.XMAX, gridNum, particles.YMIN, particles.YMAX, gridNum, particles.ZMIN, particles.ZMAX, gridNum);
    }

    #endregion


        public static void StoreFlags(string ExtendstoreFileName)
    {
        particles.StoreFlags(Application.dataPath + "/PointCloud-Visualization-Tool/data/" + "/Flags/" + particles.name+"_"+ ExtendstoreFileName);

    }

        public static void SaveSelectedAsNewData(string ExtendstoreFileName)
    {
        particles.SaveSelectedAsNewData(Application.dataPath + "/PointCloud-Visualization-Tool/data/" + "/data/" + particles.name+"_"+ ExtendstoreFileName);

    }

            public static void SaveTargetAsNewData(string ExtendstoreFileName)
    {
        particles.SaveTargetAsNewData(Application.dataPath + "/PointCloud-Visualization-Tool/data/" + "/data/" + particles.name+"_"+ ExtendstoreFileName);

    }


}
