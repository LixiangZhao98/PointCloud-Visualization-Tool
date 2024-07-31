
using System.Collections.Generic;
using UnityEngine;

public class RenderDataRunTime : MonoBehaviour
{
    private static GameObject visCenter;
    public static float  visSize=64f; //real size in VR of visualization
    public static float ratio;
    private static Material unselected_mat;
    private static Material selected_mat;
    private static Material[] target_mat;
    [SerializeField]
    [HideInInspector] public static Mesh unselected_mesh;
    [SerializeField]
    [HideInInspector] private static Mesh selected_mesh;
    [SerializeField]
    [HideInInspector] private static Mesh[] target_mesh;
  
    Matrix4x4 m;

    public static void Init(GameObject visCenter_, Material unselected_mat_, Material selected_mat_=null, List<FlagNamesCollection> flagNamesCollections=null)
   {
        visCenter=visCenter_;
        unselected_mat = unselected_mat_;
        selected_mat =selected_mat_;
        if(flagNamesCollections==null)
        {
            target_mat = new Material[0];
        }
        else
        {
            target_mat = new Material[flagNamesCollections.Count];
            for (int i = 0; i < flagNamesCollections.Count; i++)
                target_mat[i] = flagNamesCollections[i].target_mat;
        }

        GenerateMesh();
   }
    public static void GenerateMesh(bool fromStarck=true)
    {
        if(unselected_mesh!=null)
            DestroyImmediate(unselected_mesh, true);
        if (selected_mesh != null)
            DestroyImmediate(selected_mesh, true);
        if (target_mesh != null )
        {
            foreach (var m in target_mesh)
                DestroyImmediate(m, true);
        }

          
        unselected_mesh = new Mesh();
        selected_mesh= new Mesh();
        if (target_mesh != null)
        {
            target_mesh = new Mesh[target_mat.Length];
            for (int i = 0; i < target_mesh.Length; i++)
                target_mesh[i] = new Mesh();
        }
        else
        {
            target_mesh = new Mesh[0];
        }
        DisplayParticles.GenerateMeshFromPg( unselected_mesh, selected_mesh, target_mesh, DataMemory.particles, fromStarck);
        ratio = 1f / (DataMemory.particles.XMAX - DataMemory.particles.XMIN) * visSize;
    }


   public static void SetUnselectedUV1(Vector3[] lp)
   {
       unselected_mesh.SetUVs(1,lp);
   }
       private void Update()
    {
       
         m = Matrix4x4.TRS(visCenter.transform.position, visCenter.transform.rotation, new Vector3(ratio, ratio, ratio));
          visCenter.transform.localScale = new Vector3(ratio, ratio, ratio);
       
    }
       void LateUpdate()
    {
     
 
            Graphics.DrawMesh(unselected_mesh, m, unselected_mat, 1);
            if (selected_mat != null)
            Graphics.DrawMesh(selected_mesh, m, selected_mat, 1);
            if (target_mesh.Length != 0)
            {
            for (int i = 0; i < target_mesh.Length; i++)
                Graphics.DrawMesh(target_mesh[i], m, target_mat[i], 1);
            }
    }
}


