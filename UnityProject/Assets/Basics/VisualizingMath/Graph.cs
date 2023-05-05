using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;
    
    [SerializeField, Range(10, 100)]
    int resolution = 10;

    private Transform[] points;
    private float step ;
    // Start is called before the first frame update
    void Start()
    {
        step = 2f / resolution;
        var position = Vector3.zero;
        var scale = Vector3.one * step;
        points = new Transform[resolution];
        for (int i = 0; i < resolution; i++) {
            Transform point = Instantiate(pointPrefab);
            position.x = (i + 0.5f) * step - 1f;
            position.y = 0;
            point.localPosition = position;
            point.localScale = scale;
            point.SetParent(transform,false);
            points[i] = point;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.time;
        for (int i = 0; i < resolution; i++) {
            Transform point = points[i];
            var position = point.localPosition;
            position.y = Mathf.Sin(Mathf.PI * (position.x+time));
            point.localPosition = position;
            point.SetParent(transform,false);
            points[i] = point;
        }
    }
}
