using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXFRendering.LOGICAL
{
    public struct MyDxfBoundingBox
    {
        public double XLowerLeft;
        public double YLowerLeft;
        public double XUpperRight;
        public double YUpperRight;
    }
    public abstract class DXFdrawingEntry {
        public abstract MyDxfBoundingBox GetBoundingBox();
        public string layerIdentifier;
    }
    public class MyDxfLine : DXFdrawingEntry
    {
      public override MyDxfBoundingBox GetBoundingBox()   {
            MyDxfBoundingBox valueToReturn = new MyDxfBoundingBox();
            valueToReturn.XLowerLeft = XStart < XEnd ? XStart : XEnd;
            valueToReturn.YLowerLeft = YStart < YEnd ? YStart : YEnd;
            valueToReturn.XUpperRight = XStart < XEnd ? XEnd : XStart;
            valueToReturn.YUpperRight = YStart < YEnd ? YEnd : YStart;
            return valueToReturn;
        }
        public MyDxfLine(double in_XStart, double in_YStart, double in_XEnd, double in_YEnd, string in_LayerID) {
            XStart = in_XStart; YStart = in_YStart; XEnd = in_XEnd; YEnd = in_YEnd; layerIdentifier = in_LayerID;
        }
        public double XStart; public double YStart;
        public double XEnd; public double YEnd;

    }
    public class MyDxfArc : DXFdrawingEntry
    {
        /// <summary>
        /// calculate bound box for arc
        /// </summary>
        /// <returns></returns>
        public override MyDxfBoundingBox GetBoundingBox()
        {
            //convert to ccw
            double alphaPrecomputedDegree = StartAngleDegree % 360;
            double alphaPrecomputedRad = MathHelperForTransformations.ConvertDegreesToRadians( alphaPrecomputedDegree);
            double bethaPrecomputedDegree = EndAngleDegree % 360;
            double bethaPrecomputedRad = MathHelperForTransformations.ConvertDegreesToRadians( bethaPrecomputedDegree );

            if (doneTransformationOfAngle)
            {
                bethaPrecomputedRad += Math.PI*2;
                bethaPrecomputedDegree += 360;
            }
            System.Diagnostics.Debug.Assert(alphaPrecomputedRad < 360);
            //arc endpoints
            double YZeroAlpha = YCenter + Radius * Math.Sin(alphaPrecomputedRad);
            double XZeroAlpha = XCenter + Radius * Math.Cos(alphaPrecomputedRad);
            double YZeroBetha = YCenter + Radius * Math.Sin(bethaPrecomputedRad);
            double XZeroBetha = XCenter + Radius * Math.Cos(bethaPrecomputedRad);
            //next, find all points on circle that divide it by 90 degrees (polar points, name it) between two angles defining the arc
            // https://stackoverflow.com/questions/1336663/2d-bounding-box-of-a-sector
            //maximum 4 points
            List<double> polarPointsXCoordinates = new List<double>();
            List<double> polarPointsYCoordinates = new List<double>();
            double angularCounter = Math.Ceiling(alphaPrecomputedDegree / 90) * 90;
            while (angularCounter<bethaPrecomputedDegree) {
                if ((angularCounter % 360 == 0 )||(angularCounter==0)) {
                    polarPointsXCoordinates.Add(XCenter + Radius);
                    polarPointsYCoordinates.Add(YCenter);
                } else if (angularCounter % 270 ==0)
                {
                    polarPointsXCoordinates.Add(XCenter );
                    polarPointsYCoordinates.Add(YCenter - Radius);
                } else if (angularCounter % 180 ==0)
                {
                    polarPointsXCoordinates.Add(XCenter - Radius);
                    polarPointsYCoordinates.Add(YCenter );
                } else if (angularCounter % 90 == 0) {
                    polarPointsXCoordinates.Add(XCenter);
                    polarPointsYCoordinates.Add(YCenter + Radius);
                }
                angularCounter += 90;
            }
            MyDxfBoundingBox resultBox;
            List<double> allPointsY = new List<double>();
            allPointsY.AddRange(polarPointsYCoordinates);
            allPointsY.Add(YZeroAlpha); allPointsY.Add(YZeroBetha);
            List<double> allPointsX = new List<double>();
            allPointsX.AddRange(polarPointsXCoordinates);
            allPointsX.Add(XZeroAlpha); allPointsX.Add(XZeroBetha);
            resultBox.YUpperRight = allPointsY.Max();
            resultBox.YLowerLeft = allPointsY.Min();
            resultBox.XLowerLeft = allPointsX.Min();
            resultBox.XUpperRight = allPointsX.Max();
            return resultBox;
        }
        public double XCenter;
        public double YCenter;
        public double StartAngleRad;
        /// <summary>
        /// DO NOT THINK ABOUT MODIFYING THIS MANUALLY. it is set up only during constructor works and accessed during rotation
        /// </summary>
        public double StartAngleDegree;
        public double EndAngleRad;
        /// <summary>
        /// DO NOT THINK ABOUT MODIFYING THIS MANUALLY. it is set up only during constructor works and accessed during rotation
        /// </summary>
        public double EndAngleDegree;
        public double Radius;
        /// <summary>
        /// Used to determine that upon creation of arc: EndAngleRad &lt; StartAngleRad ; end angle was adjusted by 2*pi
        /// </summary>
        public bool doneTransformationOfAngle { get; }
        public MyDxfArc(double in_XCenter, double in_YCenter, double in_StartDegreeAngle, double in_EndDegreeAngle, double in_Radius, string in_Layer)  {

            XCenter = in_XCenter;
            YCenter = in_YCenter;
            StartAngleRad = MathHelperForTransformations.ConvertDegreesToRadians(in_StartDegreeAngle);
            StartAngleDegree = in_StartDegreeAngle;
            EndAngleDegree = in_EndDegreeAngle;
            EndAngleRad = MathHelperForTransformations.ConvertDegreesToRadians(in_EndDegreeAngle);
            layerIdentifier = in_Layer;
            doneTransformationOfAngle = false;
            if (EndAngleRad < StartAngleRad)
            {
                EndAngleRad += 2 * Math.PI;
                doneTransformationOfAngle = true;
            }
            Radius = in_Radius;

        }
    }

    public class completeDxfStruct
    {
        private List<DXFdrawingEntry> AllDXFdrawingEntries = new List<DXFdrawingEntry>();
        private MyDxfBoundingBox currentBoundingBox = new MyDxfBoundingBox();
        public void addDxfDrawingEntry(DXFdrawingEntry in_DxfEntry)
        {
            AllDXFdrawingEntries.Add(in_DxfEntry);
            MyDxfBoundingBox obtainedDxfBoundingBox = in_DxfEntry.GetBoundingBox();
            if (AllDXFdrawingEntries.Count > 1)  {
                currentBoundingBox.XLowerLeft = currentBoundingBox.XLowerLeft < obtainedDxfBoundingBox.XLowerLeft ? currentBoundingBox.XLowerLeft : obtainedDxfBoundingBox.XLowerLeft;
                currentBoundingBox.YLowerLeft = currentBoundingBox.YLowerLeft < obtainedDxfBoundingBox.YLowerLeft ? currentBoundingBox.YLowerLeft : obtainedDxfBoundingBox.YLowerLeft;
                currentBoundingBox.XUpperRight = currentBoundingBox.XUpperRight > obtainedDxfBoundingBox.XUpperRight ? currentBoundingBox.XUpperRight : obtainedDxfBoundingBox.XUpperRight;
                currentBoundingBox.YUpperRight = currentBoundingBox.YUpperRight > obtainedDxfBoundingBox.YUpperRight ? currentBoundingBox.YUpperRight : obtainedDxfBoundingBox.YUpperRight;
            } else {
                currentBoundingBox = obtainedDxfBoundingBox;
            }
        }
        public void recalculateBoundingBoxFromScratch()
        {
            if ((AllDXFdrawingEntries == null) || (AllDXFdrawingEntries.Count <= 0))
            {
                return;
            }
            MyDxfBoundingBox obtainedBoundingBox = AllDXFdrawingEntries[0].GetBoundingBox();
            currentBoundingBox.XLowerLeft = obtainedBoundingBox.XLowerLeft;
            currentBoundingBox.XUpperRight = obtainedBoundingBox.XUpperRight;
            currentBoundingBox.YLowerLeft = obtainedBoundingBox.YLowerLeft;
            currentBoundingBox.YUpperRight = obtainedBoundingBox.YUpperRight;
            for (int i = 1; i < AllDXFdrawingEntries.Count; i++)
            {
                obtainedBoundingBox = AllDXFdrawingEntries[i].GetBoundingBox();
                currentBoundingBox.XLowerLeft = currentBoundingBox.XLowerLeft < obtainedBoundingBox.XLowerLeft ? currentBoundingBox.XLowerLeft : obtainedBoundingBox.XLowerLeft;
                currentBoundingBox.YLowerLeft = currentBoundingBox.YLowerLeft < obtainedBoundingBox.YLowerLeft ? currentBoundingBox.YLowerLeft : obtainedBoundingBox.YLowerLeft;
                currentBoundingBox.XUpperRight = currentBoundingBox.XUpperRight > obtainedBoundingBox.XUpperRight ? currentBoundingBox.XUpperRight : obtainedBoundingBox.XUpperRight;
                currentBoundingBox.YUpperRight = currentBoundingBox.YUpperRight > obtainedBoundingBox.YUpperRight ? currentBoundingBox.YUpperRight : obtainedBoundingBox.YUpperRight;
            }

        }

        public MyDxfBoundingBox GetBoundingBox()
        {
            return currentBoundingBox;
        }
        /// <summary>
        /// Return item from internal List of structures
        /// </summary>
        /// <param name="i"></param>
        /// <returns>DXFdrawingEntry</returns>
        public DXFdrawingEntry getItemByIndex(int i)
        {
            return AllDXFdrawingEntries[i];
        }
        /// <summary>
        /// get size of internal List of structures
        /// </summary>
        /// <returns></returns>
        public int getSize()
        {
            return AllDXFdrawingEntries.Count;
        }
    }


}
