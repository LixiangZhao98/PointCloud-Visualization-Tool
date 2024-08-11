using UnityEngine;


public class DataGenerator
{
    public Vector3[] Random_Sphere()
    {
        float positionScale = 16f;
        Random.InitState(2);
        int numBodies = (int)NumParticles.NUM_16K;
        float scale = positionScale * Mathf.Max(1, numBodies / 65536);
        int i = 0;
        Vector3[] v = new Vector3[numBodies];
        while (i < numBodies)
        {
            Vector3 pos = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));

            if (Vector3.Dot(pos, pos) > 1.0) continue;
            v[i] = pos;
            i++;
        }
        return v;
    }

    public Vector3[] Random_Cube()  // Generate random points in Cubic shape

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
}



