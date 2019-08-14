using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;

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
        
    }
}
