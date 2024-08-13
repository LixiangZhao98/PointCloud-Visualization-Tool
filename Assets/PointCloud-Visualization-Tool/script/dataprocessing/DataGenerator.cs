using System.Collections.Generic;
using UnityEngine;


public class DataGenerator
{
    public Vector3[] Uniform_Sphere()
    {
        int numberOfPoints =  (int)NumParticles.NUM_16K;
        float radius = 1.0f; 
        Vector3[] points = new Vector3[numberOfPoints];

        for (int i = 0; i < numberOfPoints; i++)
        {
            float r = radius * (float)Mathf.Pow(Random.Range(0f, 1.0f), (float)(1f/3f)); 

            float theta = 2 * (float)Mathf.PI * Random.Range(0f, 1.0f); 
            float phi = (float)Mathf.Acos(2 * Random.Range(0f, 1.0f) - 1); 

            float x = r * (float)(Mathf.Sin(phi) * Mathf.Cos(theta));
            float y = r * (float)(Mathf.Sin(phi) * Mathf.Sin(theta));
            float z = r * (float)(Mathf.Cos(phi));

            points[i]=new Vector3(x, y, z);
        }

        return points;
    }

    public Vector3[] Uniform_Cube()  // Generate random points in Cubic shape

    {

        Random.InitState(2);

        int num = 100000;

        int i = 0;

        Vector3[] v = new Vector3[num];

        while (i < num)

        {

            v[i] = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));

            i++;

        }

        return v;

    }
    
    public Vector3[] Three_Sphere()
    {
        int numberOfPoints =  (int)NumParticles.NUM_16K  *10;
        float radius = 1.0f; 
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < numberOfPoints; i++)
        { 
            float r = radius * (float)Mathf.Pow(Random.Range(0f, 1.0f), (float)(1f/3f)); 

            float theta = 2 * (float)Mathf.PI * Random.Range(0f, 1.0f); 
            float phi = (float)Mathf.Acos(2 * Random.Range(0f, 1.0f) - 1); 

            float x = r * (float)(Mathf.Sin(phi) * Mathf.Cos(theta));
            float y = r * (float)(Mathf.Sin(phi) * Mathf.Sin(theta));
            float z = r * (float)(Mathf.Cos(phi));

            points.Add(new Vector3(x, y, z));
        }

        
         numberOfPoints = (int)NumParticles.NUM_16K*8  *4;
         radius = 2f; 

        for (int i = 0; i < numberOfPoints; i++)
        {
            float r = radius * (float)Mathf.Pow(Random.Range(0f, 1.0f), (float)(1f/3f)); 

            float theta = 2 * (float)Mathf.PI * Random.Range(0f, 1.0f); 
            float phi = (float)Mathf.Acos(2 * Random.Range(0f, 1.0f) - 1); 

            float x = r * (float)(Mathf.Sin(phi) * Mathf.Cos(theta));
            float y = r * (float)(Mathf.Sin(phi) * Mathf.Sin(theta));
            float z = r * (float)(Mathf.Cos(phi));

            points.Add(new Vector3(x, y, z+6f));
        }
        
        
        numberOfPoints = (int)((float)NumParticles.NUM_16K*3.375f);
        radius = 1.5f; 

        for (int i = 0; i < numberOfPoints; i++)
        {
            float r = radius * (float)Mathf.Pow(Random.Range(0f, 1.0f), (float)(1f/3f)); 

            float theta = 2 * (float)Mathf.PI * Random.Range(0f, 1.0f); 
            float phi = (float)Mathf.Acos(2 * Random.Range(0f, 1.0f) - 1); 

            float x = r * (float)(Mathf.Sin(phi) * Mathf.Cos(theta));
            float y = r * (float)(Mathf.Sin(phi) * Mathf.Sin(theta));
            float z = r * (float)(Mathf.Cos(phi));

            points.Add(new Vector3(x+6f, y, z+7f));
        }
        
        
        int num = 10000;

        int ii = 0;
        while (ii < num)

        {
            points.Add (new Vector3(Random.Range(-4.0f, 8.0f), Random.Range(-6f, 6f),Random.Range(-2.0f, 10.0f)));
            ii++;
        }

        
        return points.ToArray();
    }
    
    public Vector3[] Three_Sphere_sameSize()
    {
        int numberOfPoints =  (int)NumParticles.NUM_16K  *10;
        float radius = 1.0f; 
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < numberOfPoints; i++)
        { 
            float r = radius * (float)Mathf.Pow(Random.Range(0f, 1.0f), (float)(1f/3f)); 

            float theta = 2 * (float)Mathf.PI * Random.Range(0f, 1.0f); 
            float phi = (float)Mathf.Acos(2 * Random.Range(0f, 1.0f) - 1); 

            float x = r * (float)(Mathf.Sin(phi) * Mathf.Cos(theta));
            float y = r * (float)(Mathf.Sin(phi) * Mathf.Sin(theta));
            float z = r * (float)(Mathf.Cos(phi));

            points.Add(new Vector3(x, y, z));
        }

        
         numberOfPoints = (int)NumParticles.NUM_16K  *4;
         radius = 1f; 

        for (int i = 0; i < numberOfPoints; i++)
        {
            float r = radius * (float)Mathf.Pow(Random.Range(0f, 1.0f), (float)(1f/3f)); 

            float theta = 2 * (float)Mathf.PI * Random.Range(0f, 1.0f); 
            float phi = (float)Mathf.Acos(2 * Random.Range(0f, 1.0f) - 1); 

            float x = r * (float)(Mathf.Sin(phi) * Mathf.Cos(theta));
            float y = r * (float)(Mathf.Sin(phi) * Mathf.Sin(theta));
            float z = r * (float)(Mathf.Cos(phi));

            points.Add(new Vector3(x, y, z+6f));
        }
        
        
        numberOfPoints = (int)((float)NumParticles.NUM_16K);
        radius = 1f; 

        for (int i = 0; i < numberOfPoints; i++)
        {
            float r = radius * (float)Mathf.Pow(Random.Range(0f, 1.0f), (float)(1f/3f)); 

            float theta = 2 * (float)Mathf.PI * Random.Range(0f, 1.0f); 
            float phi = (float)Mathf.Acos(2 * Random.Range(0f, 1.0f) - 1); 

            float x = r * (float)(Mathf.Sin(phi) * Mathf.Cos(theta));
            float y = r * (float)(Mathf.Sin(phi) * Mathf.Sin(theta));
            float z = r * (float)(Mathf.Cos(phi));

            points.Add(new Vector3(x+6f, y, z+7f));
        }
        
        
        int num = 10000;

        int ii = 0;
        while (ii < num)

        {
            points.Add (new Vector3(Random.Range(-3.0f, 8.0f), Random.Range(-6f, 5f),Random.Range(-2.0f, 9.0f)));
            ii++;
        }

        
        return points.ToArray();
    }
}



