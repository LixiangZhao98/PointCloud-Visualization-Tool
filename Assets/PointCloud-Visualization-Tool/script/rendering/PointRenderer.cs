using System.Collections.Generic;
using UnityEngine;

public class PointRenderer : MonoBehaviour
{
    private Transform visCenter;
    [HideInInspector] public  float xRatio;
    [HideInInspector] public  float yRatio;
    [HideInInspector] public  float zRatio;

    private  Mesh unselected_mesh;
    private  Mesh selected_mesh;
    private  Mesh[] target_mesh;
    public Material unselected_mat;
    public Material selected_mat;
    public  Material[] target_mat;

    Matrix4x4 m;
    private ParticleGroup pG;
    
    public  void Init(List<FlagNamesCollection> flagNamesCollections=null)
   {
        pG = transform.parent.GetComponentInChildren<DataLoader>().particles;

        visCenter=this.transform.parent;
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
    private  void GenerateMesh(bool fromStarck=true)
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
        DisplayParticles.GenerateMeshFromPg( unselected_mesh, selected_mesh, target_mesh, pG, fromStarck);
        xRatio = 1f / (pG.XMAX -pG.XMIN) * this.transform.parent.localScale.x;
        yRatio = 1f / (pG.YMAX -pG.YMIN) * this.transform.parent.localScale.y;
        zRatio = 1f / (pG.ZMAX -pG.ZMIN) * this.transform.parent.localScale.z;
    }


   public  void SetUnselectedUV1(Vector3[] lp)
   {
       unselected_mesh.SetUVs(1,lp);
   }
       private void Update()
    {
        xRatio = 1f / (pG.XMAX -pG.XMIN) * this.transform.parent.localScale.x;
        yRatio = 1f / (pG.YMAX -pG.YMIN) * this.transform.parent.localScale.y;
        zRatio = 1f / (pG.ZMAX -pG.ZMIN) * this.transform.parent.localScale.z;
        m = Matrix4x4.TRS(visCenter.transform.position, visCenter.transform.rotation, new Vector3(xRatio, yRatio, zRatio));
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


