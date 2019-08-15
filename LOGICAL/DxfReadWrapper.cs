using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IxMilia.Dxf;
using IxMilia.Dxf.Entities;
using IxMilia.Dxf.Blocks;

namespace DXFRendering.LOGICAL
{
    public class singleDXFListBoxItem {
        public singleDXFListBoxItem(string in_path, string in_renderPath)
        {
            fullPath = in_path; renderPath = in_renderPath;
        }
        public string fullPath;
        public string renderPath;
        public override String ToString()
        {
            return renderPath;
        }
    }
    public class DxfReadWrapper
    {
        public static List<singleDXFListBoxItem> listAllDxfFilesInFolder(String in_Folder)  {
            try  {
                List<singleDXFListBoxItem> allFiles = new List<singleDXFListBoxItem>();
                string[] meow = System.IO.Directory.GetFiles(in_Folder, "*.dxf");
                foreach (var item in meow)
                {
                    allFiles.Add(new singleDXFListBoxItem(item, new System.IO.FileInfo(item).Name));
                }

                return allFiles;
            }  catch (Exception)  {
                throw;
            }
                
            
            
        }
        public static completeDxfStruct processDxfFile(string in_obtainedFileName)
        {
            completeDxfStruct valueToReturn = new completeDxfStruct();
            DxfFile dxfFile;
            using (System.IO.FileStream fs = new System.IO.FileStream(in_obtainedFileName, System.IO.FileMode.Open))
            {
                dxfFile = DxfFile.Load(fs);
                IList<DxfBlock> allBlocks = dxfFile.Blocks;
                IList<DxfEntity> usedEntities;
                if ((allBlocks.Count == 0) || (allBlocks[0].Name.ToUpper().Contains("MODEL") == false))
                {
                    usedEntities = dxfFile.Entities;
                }
                else
                {
                    usedEntities = allBlocks[0].Entities;
                }

                foreach (DxfEntity entity in dxfFile.Entities)
                {
                    switch (entity.EntityType)
                    {
                        case DxfEntityType.Line:
                            {
                                DxfLine line = (DxfLine)entity;
                                MyDxfLine TransfLine = new MyDxfLine(line.P1.X, line.P1.Y, line.P2.X, line.P2.Y, line.Layer);
                                valueToReturn.addDxfDrawingEntry(TransfLine);
                                break;
                            }
                        case DxfEntityType.Arc:
                            {
                                DxfArc arc = (DxfArc)entity;
                                MyDxfArc TransfArc = new MyDxfArc(arc.Center.X, arc.Center.Y, arc.StartAngle, arc.EndAngle, arc.Radius, arc.Layer);
                                valueToReturn.addDxfDrawingEntry(TransfArc);
                                break;
                            }
                        case DxfEntityType.LwPolyline:
                            { //polyline. It has vertices. 
                                DxfLwPolyline polylineS = entity as DxfLwPolyline;
                                int totalnumberOfVertices = polylineS.Vertices.Count;
                                for (int i = 0; i < totalnumberOfVertices - 1; i++)
                                { //iterate through vertices, taking them by 2. A figure is between these two
                                    DxfLwPolylineVertex point1 = polylineS.Vertices[i];
                                    DxfLwPolylineVertex point2 = polylineS.Vertices[i + 1];
                                    if (point1.Bulge == 0)  {
                                        MyDxfLine TransfLine = new MyDxfLine(point1.X, point1.Y, point2.X, point2.Y,polylineS.Layer);
                                        valueToReturn.addDxfDrawingEntry(TransfLine);
                                    } else { //it is arc
                                        // The bulge is the tangent of one fourth the included angle for an arc segment, 
                                        // made negative if the arc goes clockwise from the start point to the endpoint. 
                                        // A bulge of 0 indicates a straight segment, and a bulge of 1 is a semicircle
                                        double angleOfArcRad = System.Math.Atan(Math.Abs(point1.Bulge)) * 4;
                                        // http://mymathforum.com/algebra/21368-find-equation-circle-given-two-points-arc-angle.html
                                        // tides of Internet have almost washed this post in forum
                                        double xA; double xB; double yA; double yB;
                                        if (point1.Bulge < 0) {
                                            xA = point2.X; yA = point2.Y;
                                            xB = point1.X; yB = point1.Y;
                                        } else {
                                             xA = point1.X;  yA = point1.Y;
                                             xB = point2.X;  yB = point2.Y;
                                        }
                                        double d_square = (xA - xB) * (xA - xB) + (yA - yB) * (yA - yB);
                                        double r_square = (d_square) / (2.0 * (1.0 - Math.Cos(angleOfArcRad)));
                                        double m = (xA - xB) / (yB - yA);
                                        double a = Math.Sqrt(r_square - d_square / 4.0);
                                        double xM = (xA + xB) / 2.0; double yM = (yA + yB) / 2.0;
                                        double xC_plus = xM + a / Math.Sqrt(m * m + 1);
                                        double xC_minus = xM- a / Math.Sqrt(m * m + 1);
                                        double yC_plus = yM + m*a / Math.Sqrt(m * m + 1);
                                        double yC_minus = yM - m*a / Math.Sqrt(m * m + 1);
                                        // https://en.wikipedia.org/wiki/Linear_equation#Point%E2%80%93slope_form
                                        double usedXCenter; double usedYCenter; double usedAngle1; double usedAngle2;
                                        // https://stackoverflow.com/questions/1311049/how-to-map-atan2-to-degrees-0-360
                                        double angle1_candidate1 = (Math.Atan2(yA - yC_plus, xA - xC_plus) * 180 / Math.PI+360)%360;
                                        double angle2_candidate1 = (Math.Atan2(yB - yC_plus, xB - xC_plus) * 180 / Math.PI+360)%360;
                                        double angle1_candidate2 = (Math.Atan2(yA - yC_minus, xA - xC_minus) * 180 / Math.PI+360)%360;
                                        double angle2_candidate2 = (Math.Atan2(yB - yC_minus, xB - xC_minus) * 180 / Math.PI+360)%360;
                                        //mydxfarc expects angles counterclockwise
                                        if (point1.Bulge>0)
                                        {
                                            if (angle1_candidate1 < angle2_candidate1) {
                                                usedAngle1 = angle1_candidate1;
                                                usedAngle2 = angle2_candidate1;
                                                usedXCenter = xC_plus; usedYCenter = yC_plus;
                                            } else
                                            {
                                                usedAngle1 = angle1_candidate2;
                                                usedAngle2 = angle2_candidate2;
                                                usedXCenter = xC_minus; usedYCenter = yC_minus;
                                            }
                                        } else {
                                            if (angle1_candidate1 > angle2_candidate1)  {
                                                usedAngle1 = angle1_candidate2;
                                                usedAngle2 = angle2_candidate2;
                                                usedXCenter = xC_minus; usedYCenter = yC_minus;
                                            } else {
                                                usedAngle1 = angle1_candidate1;
                                                usedAngle2 = angle2_candidate1;
                                                usedXCenter = xC_plus; usedYCenter = yC_plus;
                                            }
                                        }
                                        MyDxfArc TransfArc = new MyDxfArc(usedXCenter, usedYCenter,
                                            usedAngle1,
                                            usedAngle2,
                                            Math.Sqrt(r_square),
                                            polylineS.Layer);
                                        valueToReturn.addDxfDrawingEntry(TransfArc);
                                    }
                                }
                                if (polylineS.IsClosed)  {
                                    DxfLwPolylineVertex point1 = polylineS.Vertices[totalnumberOfVertices-1];
                                    DxfLwPolylineVertex point2 = polylineS.Vertices[0];
                                    if (point1.Bulge == 0)  {
                                        MyDxfLine TransfLine = new MyDxfLine(point1.X, point1.Y, point2.X, point2.Y, polylineS.Layer);
                                        valueToReturn.addDxfDrawingEntry(TransfLine);
                                    } else { // arc
                                             // I so like the code above, so I cannot resist to copypaste it

                                        double angleOfArcRad = System.Math.Atan(Math.Abs(point1.Bulge)) * 4;
                                        double xA; double xB; double yA; double yB;
                                        if (point1.Bulge < 0)     {
                                            xA = point2.X; yA = point2.Y;   xB = point1.X; yB = point1.Y;
                                        } else {
                                            xA = point1.X; yA = point1.Y;   xB = point2.X; yB = point2.Y;
                                        }
                                        double d_square = (xA - xB) * (xA - xB) + (yA - yB) * (yA - yB);
                                        double r_square = (d_square) / (2.0 * (1.0 - Math.Cos(angleOfArcRad)));
                                        double m = (xA - xB) / (yB - yA);
                                        double a = Math.Sqrt(r_square - d_square / 4.0);
                                        double xM = (xA + xB) / 2.0; double yM = (yA + yB) / 2.0;
                                        double xC_plus = xM + a / Math.Sqrt(m * m + 1);
                                        double xC_minus = xM - a / Math.Sqrt(m * m + 1);
                                        double yC_plus = yM + m * a / Math.Sqrt(m * m + 1);
                                        double yC_minus = yM - m * a / Math.Sqrt(m * m + 1);
                                        // https://en.wikipedia.org/wiki/Linear_equation#Point%E2%80%93slope_form
                                        double usedXCenter; double usedYCenter; double usedAngle1; double usedAngle2;
                                        // https://stackoverflow.com/questions/1311049/how-to-map-atan2-to-degrees-0-360
                                        double angle1_candidate1 = (Math.Atan2(yA - yC_plus, xA - xC_plus) * 180 / Math.PI + 360) % 360;
                                        double angle2_candidate1 = (Math.Atan2(yB - yC_plus, xB - xC_plus) * 180 / Math.PI + 360) % 360;
                                        double angle1_candidate2 = (Math.Atan2(yA - yC_minus, xA - xC_minus) * 180 / Math.PI + 360) % 360;
                                        double angle2_candidate2 = (Math.Atan2(yB - yC_minus, xB - xC_minus) * 180 / Math.PI + 360) % 360;
                                        //mydxfarc expects angles counterclockwise
                                        if (point1.Bulge > 0) {
                                            if (angle1_candidate1 < angle2_candidate1) {
                                                usedAngle1 = angle1_candidate1; usedAngle2 = angle2_candidate1;
                                                usedXCenter = xC_plus; usedYCenter = yC_plus;
                                            } else {
                                                usedAngle1 = angle1_candidate2; usedAngle2 = angle2_candidate2;
                                                usedXCenter = xC_minus; usedYCenter = yC_minus;
                                            }
                                        } else {
                                            if (angle1_candidate1 > angle2_candidate1) {
                                                usedAngle1 = angle1_candidate2; usedAngle2 = angle2_candidate2;
                                                usedXCenter = xC_minus; usedYCenter = yC_minus;
                                            } else {
                                                usedAngle1 = angle1_candidate1; usedAngle2 = angle2_candidate1;
                                                usedXCenter = xC_plus; usedYCenter = yC_plus;
                                            }
                                        }
                                        MyDxfArc TransfArc = new MyDxfArc(usedXCenter, usedYCenter,
                                            usedAngle1, usedAngle2, Math.Sqrt(r_square), polylineS.Layer);
                                        valueToReturn.addDxfDrawingEntry(TransfArc);

                                    }
                                }

                                break;
                            }
                        case DxfEntityType.Polyline:
                            {
                                //this is a spawn of autocad. Either copypaste code from LwPolyline section. Or just save the file in QCad. 
                                //QCad replaces all polylines by lwPolylines
                                //https://github.com/IxMilia/Dxf/issues/90
                                DxfPolyline polyline = entity as DxfPolyline;

                                break;
                            }
                    }
                }
            }
            return valueToReturn;
        }

    }
}

