using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System;
using System.Linq;
using System.Reflection;
public class DataLoader : MonoBehaviour
{
    [HideInInspector] public ParticleGroup particles;
    public UnityEvent eventAfterLoadData;
    private String dataPath;
    
    public void  LoadData(int datasetIndex,int customIndex, bool custom)
    {
        // DataStorage.StacksInitialize();
        
        if (!custom)
            LoadDataset(datasetIndex); //load data file
        else
            LoadCustomGenerator(customIndex); //load data by function
        
        // if (loadTargetNames.Count!=0&&loadTarget)  //load target points with highlighted color
        //     DataMemory.LoadFlagsToStack(loadTargetNames);
        // RenderDataRunTime.Init(visCenter,unselected_mat,selected_mat,loadTargetNames);  
        
        
        eventAfterLoadData?.Invoke(); //actions after data load

    }

    public void LoadDataset(int index)
    {
        dataPath =UnityEngine. Application.dataPath + "/PointCloud-Visualization-Tool/data/data/";
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
                    particles.LoadBin(dataPath,nthFileName);
                }
                else if (nthFileExtention == ".ply")
                {
                    particles.LoadPly(dataPath,nthFileName);   
                }
                else if (nthFileExtention == ".pcd")
                {
                    particles.LoadPcd(dataPath,nthFileName);    
                }
                else if (nthFileExtention == ".txt")
                {
                    particles.LoadTxt(dataPath,nthFileName);    
                }
                else if (nthFileExtention == ".csv")
                {
                    particles.LoadCsv(dataPath,nthFileName);    
                }
                this.transform.parent. gameObject.name=nthFileName; 
                this.transform.parent. GetComponentInChildren<PointRenderer>().Init(); // set rendering parameters
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
    public void LoadCustomGenerator(int index)
    {
        try
        {
            MethodInfo[] mi= typeof(DataGenerator).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            particles.LoadVec3s((Vector3[])mi[index].Invoke(new DataGenerator(),null),mi[index].Name);
            this.transform.parent. gameObject.name="Custom_"+mi[index].Name; 
            this.transform.parent. GetComponentInChildren<PointRenderer>().Init(); // set rendering parameters
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void OnApplicationQuit()
    {
        Destroy(this); 
    }
    
    public void SaveAsBin(string location)
    {
        particles.SaveAsBin(location);
    }
    public void SaveAsPly(string location)
    {
        particles.SaveAsPly(location);
    }
    public void SaveAsTxt(string location)
    {
        particles.SaveAsTxt(location);
    }
    public void SaveAsPcd(string location)
    {
        particles.SaveAsPcd(location);
    }
}
