// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// // public class MyPointCloud : MonoBehaviour
// // {
// //     public Material particleMat;  // the material of the points
// //     public GameObject visCenter; //The visualization will always follow the `Vis center` when starting the game. 
// //     void Start()
// //     {
// //         DataStorage.StacksInitialize();  //Initialize
// //         DataStorage.LoadByte("Flocculentcube2");  //load the data from the the binary file; the input is the name of the binary file
// //         RenderDataRunTime.visSize = 1f;  //the size of the visualization
// //         RenderDataRunTime.Init(visCenter, particleMat);  // Assign materials and center to the RenderDataRunTime.cs`
// //     }
// // }
//
//
//
//
//
//
//
//
//
//
//
//
//
//
// public class MyPointCloud : MonoBehaviour
// {
//     public Material particleMat;  // the material of the points
//     public GameObject visCenter; //The visualization will always follow the `Vis center` when starting the game. 
//     void Start()
//     {
//         Vector3[] v = Generate_Cube();  // Generate random points in Cubic shape
//         DataStorage.StacksInitialize();//Initialize
//         DataStorage.LoadVec3s(v, "cube");  // the first input is Vector[], the second is the name of the data (you can name it as you like)
//         RenderDataRunTime.visSize = 1f;  //the size of the visualization
//         RenderDataRunTime.Init(visCenter,particleMat);  // Assign materials and center to the RenderDataRunTime.cs`
//     }
//     public Vector3[] Generate_Cube()  // Generate random points in Cubic shape
//     {
//         Random.InitState(2);
//         int num = 100000;
//         int i = 0;
//         Vector3[] v = new Vector3[num];
//         while (i < num)
//         {
//             v[i] = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
//             i++;
//         }
//         return v;
//     }
// }
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
// // public class MyPointCloud : MonoBehaviour
// // {
// //     public Material particleMat;  // the material of the points
// //     public GameObject visCenter; //The visualization will always follow the `Vis center` when starting the game. 
// //     void Start()
// //     {
// //         DataStorage.StacksInitialize();  //Initialize
// //         DataStorage.LoadPly("dragon_vrip");  //load the data from the the ply file; the input is the name of the binary file
// //         RenderDataRunTime.visSize = 1f;
// //         RenderDataRunTime.Init(visCenter, particleMat);  // Assign materials and center to the RenderDataRunTime.cs`
// //     }
// // }
//
//
