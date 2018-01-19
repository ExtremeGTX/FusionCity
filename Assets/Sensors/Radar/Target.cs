using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
public class Target  {
	public struct PointData
	{
		public Vector3 point;
		public float distance;
		public float angle;
		public Vector3 RelativeVelocity;
	}
	public string name;
	//public List<Vector3> Points = new List<Vector3>();
	//public float minAngle=0.0f;
	//public float maxAngle=0.0f;
	public List<PointData> PointsData = new List<PointData>();
	public Vector2 RelativePosition;
	public float RelativeVelocity;
	public float yawRate=0.0f;
	public int SensorCycleNumber=0;

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
#if EXPORT_ALLDATA
		foreach (PointData p in PointsData)
		{
			sb.Append(p.point + ";");
			sb.Append(p.distance+ ";");
			sb.Append(p.angle);
			sb.Append("\n");
		}
#endif
		if (!name.Contains("Car"))
			return name+"\n";
		
		sb.Append(name);
		sb.Append(" Vrel:"+RelativeVelocity.ToString("00.0000"));
		sb.Append(" pts:"+PointsData.Count+"\n"); //Number of Points hitted the target from the sensor
		sb.Append("   Hit Point  |Dist |Angle| VRel(p)\n");
		for (int i=0;i< (PointsData.Count > 10 ? 10:PointsData.Count) ;i++)
		{
			
			//sb.Append("({0},{1})".Format(PointsData[i].point.x.ToString(),PointsData[i].point.z.ToString()));
			sb.Append("("+PointsData[i].point.x.ToString("000.00")+","+PointsData[i].point.z.ToString("000.00") + ");");
			sb.Append(PointsData[i].distance.ToString("##0.00")+ ";");
			sb.Append(PointsData[i].angle.ToString("##0.00")+ ";");
			sb.Append(PointsData[i].RelativeVelocity.magnitude.ToString("00.0000") + ";");
			sb.Append("\n");
		}
		//sb.Append(";"+yawRate);
		return sb.ToString();
	}
}
