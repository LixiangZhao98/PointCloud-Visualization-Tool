using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System;
using System.Linq;
public class DataLoader : MonoBehaviour
{
    [HideInInspector] public ParticleGroup particles;
    public UnityEvent eventAfterLoadData;
    private String dataPath = Application.dataPath + "/PointCloud-Visualization-Tool/data/data/";
    
    public void  LoadData(int index)
    {
        // DataStorage.StacksInitialize();
        
        Load(index); //load data
        
        // if (loadTargetNames.Count!=0&&loadTarget)  //load target points with highlighted color
        //     DataMemory.LoadFlagsToStack(loadTargetNames);
        // RenderDataRunTime.Init(visCenter,unselected_mat,selected_mat,loadTargetNames);  
        
        
        eventAfterLoadData?.Invoke(); //actions after data load

    }

    public void Load(int index)
    {
        int n = index*2; //exclude .meta file
        particles = new ParticleGroup();
        try
        {
            string[] files = Directory.GetFiles(dataPath).ToArray();

            if (n >= 0 && n < files.Length)
            {
                string nthFileName = Path.GetFileNameWithoutExtension(files[n]); 
                string nthFileExtention = Path.GetExtension(files[n]);
                if (nthFileExtention == ".bin")
                {
                    particles.LoadByte(dataPath,nthFileName);
                }
                else if (nthFileExtention == ".ply")
                {
                    particles.LoadPly(dataPath,nthFileName);   
                }
                else if (nthFileExtention == ".csv")
                {
                    particles.LoadCsv(dataPath,nthFileName);    
                }
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
}
