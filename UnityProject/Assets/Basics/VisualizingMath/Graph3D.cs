using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph3D : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;
    
    [SerializeField, Range(10, 100)]
    int resolution = 10;

    public FunctionLibrary3D.FunctionName FunctionName;

    private Transform[] points;
    private float step ;
    // Start is called before the first frame update
    void Start()
    {
        step = 2f / resolution;
        var position = Vector3.zero;
        var scale = Vector3.one * step;
        points = new Transform[resolution*resolution];
        for (int i = 0; i < resolution*resolution; i++) {
            Transform point = Instantiate(pointPrefab);
            point.localScale = scale;
            point.SetParent(transform,false);
            points[i] = point;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.time;
        var f = FunctionLibrary3D.GetFunction(FunctionName);
        float v = 0.5f * step - 1f;
        for (int i = 0,x=0,z=0; i < resolution*resolution; i++,x++) {
            if (x==resolution)
            {
                x = 0;
                z++;
                v = (z+0.5f) * step - 1f;
            }
            Transform point = points[i];
           
            float u = (x + 0.5f) * step - 1f;
            point.GetComponent<Point>().SetUV(u,v);
            point.localPosition =f(u,v,time);
        }
    }
}
