using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRVisualizer : MonoBehaviour
{
    public enum DisplayModes{Full, HitsOnly};

    private ISensor sensor;
    private List<LineRenderer> LineList = new List<LineRenderer>();
	
    public bool DisplayBeams = true;
    public DisplayModes DisplayMode = DisplayModes.HitsOnly;

    void Start()
    {
        sensor = GetComponentInParent<ISensor>();
        //sensor.OnSensorInitDone += InitLRList;
        sensor.OnSensorBeamUpdate += DrawLines;
    	sensor.OnSensorUpdate += BuildBeamsList;

    }

    public void BuildBeamsList()
    {
        if (!DisplayBeams)
        {
            return;
        }

        int count = sensor.getNoOfBeams();
        while (count > LineList.Count)
        {
            AppendBeam();
        }

        while (LineList.Count > count && LineList.Count > 0)
        {
            RemoveBeam();
        }

    }

    void AppendBeam()
    {
        GameObject g = new GameObject();
        g.transform.parent = this.gameObject.transform;
        LineRenderer lr = g.AddComponent<LineRenderer>();
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;

        lr.material = new Material(Shader.Find("Unlit/Color"));
        lr.material.enableInstancing = true;
        lr.material.color = Color.green;
        lr.startColor = Color.green; //Color need a basic material
        lr.endColor = Color.green;
        lr.receiveShadows = false;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        LineList.Add(lr);
    }
	void RemoveBeam()
	{
		GameObject g = LineList[LineList.Count - 1].transform.gameObject;
		Destroy(g);
		LineList.RemoveAt(LineList.Count - 1);
	}

	void RemoveAllBeams()
	{
		while (LineList.Count > 0)
		{
			RemoveBeam();
		}
	}
    // Use this for initialization
   
    public void DrawLines(int index, Vector3 Start, Vector3 end, bool alert)
    {
        if (!DisplayBeams)
        {
            RemoveAllBeams();
			return;
        }
        LineRenderer lr = LineList[index];
        lr.name = "LRV_" + index.ToString();//(angle * 0.5f + (-1 * index * angularResolution)).ToString();
        lr.SetPosition(0, Start);
        lr.SetPosition(1, end);

        switch(DisplayMode)
        {
            case DisplayModes.Full:
                if(alert) {lr.material.color = Color.red;} else {lr.material.color = Color.green;};
            break;

            case DisplayModes.HitsOnly:
                if(alert) {lr.enabled = true;} else {lr.enabled = false;};
            break;
        }
    }

    public void EnableBeams(bool enable)
    {
        DisplayBeams = enable;
        BuildBeamsList();
    }
}
