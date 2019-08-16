using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using DXFRendering.LOGICAL;

namespace DXFRendering.GRAPHICAL
{
    public abstract class DXFentryForDisplay
    {

        
    }
    public class MyDxfLineForDisplay : DXFentryForDisplay
    {
        public MyDxfLineForDisplay(double in_XStart, double in_YStart, double in_XEnd, double in_YEnd)
        {
            XStart = in_XStart; YStart = in_YStart; XEnd = in_XEnd; YEnd = in_YEnd;
            penStructure = new  Pen( Color.Black);
        }
        public MyDxfLineForDisplay(double in_XStart, double in_YStart, double in_XEnd, double in_YEnd, String colorToUse)
        {
            XStart = in_XStart; YStart = in_YStart; XEnd = in_XEnd; YEnd = in_YEnd;
            penStructure = new  Pen( Color.FromName(colorToUse) );
        }
        public MyDxfLineForDisplay(double in_XStart, double in_YStart, double in_XEnd, double in_YEnd, Pen penToUse)
        {
            XStart = in_XStart; YStart = in_YStart; XEnd = in_XEnd; YEnd = in_YEnd;
            penStructure = penToUse;
        }
        public  Pen penStructure;
        public double XStart; public double YStart;
        public double XEnd; public double YEnd;
    }
    public class MyDxfArcForDisplay : DXFentryForDisplay
    {
        public  Pen penStructure;
        //The x-coordinate of the upper-left corner of the rectangle that defines the underlying circle
        public double XUpper;
        //The y-coordinate of the upper-left corner of the rectangle that defines the underlying circle
        public double YUpper;
        //Width of the rectangle that defines the underlying circle
        public double Width;
        //height of the rectangle that defines the underlying circle
        public double Height;
        // Angle in degrees measured clockwise from the x-axis to the starting point of the arc
        public double startAngle;
        // Angle in degrees measured clockwise from the startAngle parameter to ending point of the arc
        public double sweepAngle;
        public MyDxfArcForDisplay(double in_XCenter, double in_YCenter, double in_Radius, double in_StartAngleCCW, double in_EndAngleCCW)
        {
               startAngle = in_StartAngleCCW;
                if (in_StartAngleCCW < in_EndAngleCCW) {
                    sweepAngle = in_EndAngleCCW - in_StartAngleCCW;
                } else {                    
                    sweepAngle = in_EndAngleCCW+360 - in_StartAngleCCW;
                }
            
                
                XUpper = in_XCenter - in_Radius;
            //hmmm~ sometimes we need to do "+", other times we need to do "-"
            
                YUpper = in_YCenter - in_Radius;
            
            Width = 2 * in_Radius;
            Height = 2 * in_Radius;
            penStructure = new  Pen( Color.Black);
        }
        public MyDxfArcForDisplay(double in_XCenter, double in_YCenter, double in_Radius, double in_StartAngleCCW, double in_EndAngleCCW, Pen in_Pen)
        {
            startAngle = in_StartAngleCCW;
            if (in_StartAngleCCW < in_EndAngleCCW)
            {
                sweepAngle = in_EndAngleCCW - in_StartAngleCCW;
            }
            else
            {
                sweepAngle = in_EndAngleCCW + 360 - in_StartAngleCCW;
            }


            XUpper = in_XCenter - in_Radius;
            //hmmm~ sometimes we need to do "+", other times we need to do "-"

            YUpper = in_YCenter - in_Radius;

            Width = 2 * in_Radius;
            Height = 2 * in_Radius;
            penStructure = in_Pen;
        }

        public MyDxfArcForDisplay(Pen in_Pen, double in_XUpper, double in_YUpper, double in_Width, double in_Height, double in_StartAngleCCW, double in_sweepAngle)
        {
            startAngle = in_StartAngleCCW;
            sweepAngle = in_sweepAngle;
            XUpper = in_XUpper;
            YUpper = in_YUpper;

            Width = in_Width;
            Height = in_Height;
            penStructure = in_Pen;
        }

    }
    public class CompleteDxfDrawingStruct: IEnumerable {
        public double XLowerLeft { get; private set; }
        public double YLowerLeft { get; private set; }
        public double XUpperRight { get; private set; }
        public double YUpperRight { get; private set; }
        private List<DXFentryForDisplay> allEntriesUsingInDisplay = new List<DXFentryForDisplay>();

        //This routine is used by rendering control. 4 parameters here are the bounding box of Entry
        public void addSingleEntry(DXFentryForDisplay in_Entry, double in_XLowerLeft, double in_YLowerLeft, double in_XUpperRight, double in_YUpperRight)
        {

            allEntriesUsingInDisplay.Add(in_Entry);
            if (allEntriesUsingInDisplay.Count > 1)
            {
                 XLowerLeft =  XLowerLeft < in_XLowerLeft ?  XLowerLeft : in_XLowerLeft;
                 YLowerLeft =  YLowerLeft < in_YLowerLeft ?  YLowerLeft : in_YLowerLeft;
                 XUpperRight =  XUpperRight > in_XUpperRight ?  XUpperRight : in_XUpperRight;
                 YUpperRight =  YUpperRight > in_YUpperRight ?  YUpperRight : in_YUpperRight;
            }
            else
            {
                XLowerLeft = in_XLowerLeft; YLowerLeft = in_YLowerLeft;
                XUpperRight = in_XUpperRight; YUpperRight = in_YUpperRight;
            }

        }

        public IEnumerator GetEnumerator()
        {
            foreach (DXFentryForDisplay theAnimal in allEntriesUsingInDisplay)
            {
                yield return theAnimal;
            }
        }

        public CompleteDxfDrawingStruct (CompleteDxfDrawingStruct structToCopy) {
            // https://stackoverflow.com/questions/14520698/i-can-access-private-members-in-a-copy-ctor-in-c-sharp
            if (structToCopy != null)  {
                XLowerLeft = structToCopy.XLowerLeft;
                YLowerLeft = structToCopy.YLowerLeft;
                XUpperRight = structToCopy.XUpperRight;
                YUpperRight = structToCopy.YUpperRight;
                foreach (var item in structToCopy.allEntriesUsingInDisplay)     {
                    allEntriesUsingInDisplay.Add(item);
                }
            }
        }

        public CompleteDxfDrawingStruct(CompleteDxfDrawingStruct structToCopy, double in_initialRotationAngleRad) 
        {
            if (structToCopy != null)  {
                XLowerLeft = structToCopy.XLowerLeft;
                YLowerLeft = structToCopy.YLowerLeft;
                XUpperRight = structToCopy.XUpperRight;
                YUpperRight = structToCopy.YUpperRight;

                //obtain center of rotation
                double horizontalMidPoint = (structToCopy.XLowerLeft + structToCopy.XUpperRight) / 2.0;
                double verticalMidPoint = (structToCopy.YLowerLeft + structToCopy.YUpperRight) / 2.0;
                //obtain rotation matrix
                double[,] currentRotationMatrix = MathHelperForTransformations.getRotationMatrixAroundPoint(horizontalMidPoint, verticalMidPoint, in_initialRotationAngleRad);
                //iterate over all entries in dxf structure altering them
                int allCount = structToCopy.allEntriesUsingInDisplay.Count;
                for (int iiii = 0; iiii < allCount; iiii++ )
                {
                    DXFentryForDisplay item = structToCopy.allEntriesUsingInDisplay[iiii];
                    //get bound box and rotate it
                    
                    if (item is MyDxfLineForDisplay)
                    {

                        Tuple<double, double> coord1 = MathHelperForTransformations.rotateImageUsingPrecalculatedTransformationMatrix(currentRotationMatrix, (item as MyDxfLineForDisplay).XStart, (item as MyDxfLineForDisplay).YStart);
                        double item2XStart = coord1.Item1;
                        double item2YStart = coord1.Item2;
                        Tuple<double, double> coord2 = MathHelperForTransformations.rotateImageUsingPrecalculatedTransformationMatrix(currentRotationMatrix, (item as MyDxfLineForDisplay).XEnd, (item as MyDxfLineForDisplay).YEnd);
                        double item2XEnd = coord2.Item1;
                        double item2YEnd = coord2.Item2;
                        MyDxfLineForDisplay item2 = new MyDxfLineForDisplay(item2XStart, item2YStart, item2XEnd, item2YEnd, (item as MyDxfLineForDisplay).penStructure);
                        
                    }
                    else if (item is MyDxfArcForDisplay)
                    {
                        Tuple<double, double> coordUpper = MathHelperForTransformations.rotateImageUsingPrecalculatedTransformationMatrix(currentRotationMatrix, (item as MyDxfArcForDisplay).XUpper, (item as MyDxfArcForDisplay).YUpper);
                        double startAngleDegree = (item as MyDxfArcForDisplay).startAngle + MathHelperForTransformations.ConvertRadiansToDegrees(in_initialRotationAngleRad);
                        MyDxfArcForDisplay item2 = new MyDxfArcForDisplay((item as MyDxfArcForDisplay).penStructure, coordUpper.Item1, coordUpper.Item2, (item as MyDxfArcForDisplay).Width, (item as MyDxfArcForDisplay).Height, startAngleDegree, (item as MyDxfArcForDisplay).sweepAngle);

                    }
                }
            }
        }
    }
}
