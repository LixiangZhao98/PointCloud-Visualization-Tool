
using System.Collections.Generic;
using UnityEngine;

 
public class DataStorage : MonoBehaviour
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

            DataStorage.particles.SetTarget(flags[i], true, names.IndexOf(name)); 
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
    static public ParticleGroup particles = new ParticleGroup();
    
    static public void LoadPly(string loadFileName)
    {
        particles.LoadPly(Application.dataPath + "/PointCloud-Visualization-Tool/data/data/",loadFileName);
        Debug.Log("Load success" + " " + loadFileName + " with " + particles.GetParticlenum() + " particles." + " SmoothLength: " + particles.GetSmoothLength());
    }
    
    static public void LoadPly(ParticleGroup pG, string loadFileName)
    {

        pG.LoadPly(Application.dataPath + "/PointCloud-Visualization-Tool/data/data/",loadFileName);
        Debug.Log("Load success" + " " + loadFileName + " with " + pG.GetParticlenum() + " particles." + " SmoothLength: " + pG.GetSmoothLength());

    }
    

    static public void LoadByte(string loadFileName)
    {
        particles.LoadByte(Application.dataPath + "/PointCloud-Visualization-Tool/data/data/" + loadFileName,loadFileName);
        Debug.Log("Load success" + " " + loadFileName + " with " + particles.GetParticlenum() + " particles." + " SmoothLength: " + particles.GetSmoothLength().x + " " + particles.GetSmoothLength().y + " " + particles.GetSmoothLength().z);
    }
    public static void LoadByte(ParticleGroup pG, string loadFileName)
    {

        pG.LoadByte(Application.dataPath + "/PointCloud-Visualization-Tool/data/data/" + loadFileName,loadFileName);
        Debug.Log("Load success" + " " + loadFileName + " with " + pG.GetParticlenum() + " particles." + " SmoothLength: " + pG.GetSmoothLength().x + " " + pG.GetSmoothLength().y + " " + pG.GetSmoothLength().z);
    }
    
    
    static public void LoadCsv(string loadFileName)
    {
        particles.LoadCsv(Application.dataPath + "/PointCloud-Visualization-Tool/data/data/" + loadFileName, loadFileName);
        Debug.Log("Load success" + " " + loadFileName + " with " + particles.GetParticlenum() + " particles." + " SmoothLength: " + particles.GetSmoothLength());
    }
    static public void LoadCsv(ParticleGroup pG, string loadFileName)
    {

        pG.LoadCsv(Application.dataPath + "/PointCloud-Visualization-Tool/data/data/" + loadFileName, loadFileName);
        Debug.Log("Load success" + " " + loadFileName + " with " + pG.GetParticlenum() + " particles." + " SmoothLength: " + pG.GetSmoothLength());
    }
   
    
    
    static public void LoadVec3s( Vector3[] v, string dataname,bool forSimulation = false)
    {
        particles.LoadVec3s(v, dataname, forSimulation);

        Debug.Log("Load success" + " " + dataname + " with " + particles.GetParticlenum() + " particles." + " SmoothLength: " + particles.GetSmoothLength());
    }
    static public void LoadVec3s(ParticleGroup pG, Vector3[] v, string dataname, bool forSimulation = false)
    {


        pG.LoadVec3s(v, dataname, forSimulation);

        Debug.Log("Load success" + " " + dataname + " with " + pG.GetParticlenum() + " particles." + " SmoothLength: " + pG.GetSmoothLength());

    }

   
    public static void ClearParticleMemory()
    {
        particles = new ParticleGroup();
    }

    
    #endregion

    #region DensityFieldInfo
    [SerializeField]
  
    static public DensityField densityField = new DensityField();
    
    static public void CreateField(int gridNum)
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
    [SerializeField]
    static public void CreateField(DensityField dF, ParticleGroup pG, int gridNum)
    {
        float scalingFactor = 0f;
        float step = (pG.XMAX - pG.XMIN) / gridNum;
        pG.XMAX += scalingFactor*step;
        pG.XMIN -= scalingFactor*step;
        pG.YMAX += scalingFactor*step;
        pG.YMIN -= scalingFactor*step;
        pG.ZMAX += scalingFactor*step;
        pG.ZMIN -= scalingFactor*step;
        dF.InitializeDensityFieldByGapDis(pG.name, pG.XMIN, pG.XMAX, gridNum, pG.YMIN, pG.YMAX, gridNum, pG.ZMIN, pG.ZMAX, gridNum);
    }

    #endregion


        public static void StoreFlags(ParticleGroup pG, string ExtendstoreFileName)
    {
        pG.StoreFlags(Application.dataPath + "/PointCloud-Visualization-Tool/data/" + "/Flags/" + pG.name+"_"+ ExtendstoreFileName);

    }

        public static void SaveSelectedAsNewData(ParticleGroup pG, string ExtendstoreFileName)
    {
        pG.SaveSelectedAsNewData(Application.dataPath + "/PointCloud-Visualization-Tool/data/" + "/data/" + pG.name+"_"+ ExtendstoreFileName);

    }

            public static void SaveTargetAsNewData(ParticleGroup pG, string ExtendstoreFileName)
    {
        pG.SaveTargetAsNewData(Application.dataPath + "/PointCloud-Visualization-Tool/data/" + "/data/" + pG.name+"_"+ ExtendstoreFileName);
    }


}
