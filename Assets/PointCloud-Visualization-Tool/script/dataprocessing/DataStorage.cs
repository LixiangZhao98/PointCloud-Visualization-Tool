
using System.Collections.Generic;
using UnityEngine;

 
public class DataStorage : MonoBehaviour
{    [HideInInspector] static public ParticleGroup particles;


    #region Operation Stack and DisplayInfo

    private static Stack<List<int>> pStack;
    private static Stack<List<int>> pOperateStack;



    public static void StacksInitialize()
    {
        pStack = new Stack<List<int>>();
        pOperateStack=new Stack<List<int>>();

    }
    // public static void LoadFlagsToStack(List<FlagNamesCollection> names)
    // {
    //     foreach(var name in names)
    //     {
    //       for(int n=0;n<name.FlagNames.Length;n++)
    //     {
    //     int[] flags = LoadDataBybyte.StartLoadFlags(Application.dataPath + "/PointCloud-Visualization-Tool/data/flags/" + particles.name+"_"+ name.FlagNames[n]);
    //     for (int i = 0; i < flags.Length; i++)
    //     {
    //
    //         DataStorage.particles.SetTarget(flags[i], true, names.IndexOf(name)); 
    //     }
    //     }
    //     }
    //
    //     }
      

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




    // public static void DisplayAllParticle(bool loadFlag, List<FlagNamesCollection>  LoadFlagNames)
    // {
    //     StacksInitialize();
    //         LoadFlagsToStack(LoadFlagNames);
    //     DisplayParticles.DisplayMesh(GameObject.Find("PointCloudMesh"), particles);
    // }
    #endregion

    #region ParticleInfo
    // [SerializeField]
    // static public ParticleGroup particles = new ParticleGroup();
    //
    //
    //
    // static public void LoadVec3s( Vector3[] v, string dataname,bool forSimulation = false)
    // {
    //     particles.LoadVec3s(v, dataname, forSimulation);
    //
    // }


   
    // public static void ClearParticleMemory()
    // {
    //     particles = new ParticleGroup();
    // }

    
    
    
    #endregion


    //
    //     public static void StoreFlags(ParticleGroup pG, string ExtendstoreFileName)
    // {
    //     pG.StoreFlags(Application.dataPath + "/PointCloud-Visualization-Tool/data/" + "/Flags/" + pG.name+"_"+ ExtendstoreFileName);
    //
    // }
    //
    //     public static void SaveSelectedAsNewData(ParticleGroup pG, string ExtendstoreFileName)
    // {
    //     pG.SaveSelectedAsNewData(Application.dataPath + "/PointCloud-Visualization-Tool/data/" + "/data/" + pG.name+"_"+ ExtendstoreFileName);
    //
    // }
    //
    //         public static void SaveTargetAsNewData(ParticleGroup pG, string ExtendstoreFileName)
    // {
    //     pG.SaveTargetAsNewData(Application.dataPath + "/PointCloud-Visualization-Tool/data/" + "/data/" + pG.name+"_"+ ExtendstoreFileName);
    // }


}
