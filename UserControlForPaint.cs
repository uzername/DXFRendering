using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DXFRendering.LOGICAL;
using DXFRendering.GRAPHICAL;

namespace DXFRendering
{
    public delegate void internalScaleFactorChangedInternallyCallbackDelegate(double in_valueOfScaleFactor);
    /// <summary>
    /// Class responsible for showing a figure from dxf file on field
    /// </summary>
    public partial class UserControlForPaint : UserControl
    {
        private completeDxfStruct savedStructureOfDxf = null;
        private CompleteDxfDrawingStruct primalDrawingStructure = null;
        private CompleteDxfDrawingStruct actualDrawingStructure = null;
        private double offsetOfDxfHorizontal = 0;
        private double offsetOfDxfVertical = 0;
        /// <summary>
        /// contains descriptions of layers found in dxf files and how to draw them. 
        /// String key is the layer name, tupple: byte for draw order, pen for a drawing Pen
        /// </summary>
        public Dictionary<String, Tuple<byte, Pen>> collectionOfLayerDefinitions = null;
        public void setupCollectionOfLayerDefinitions()
        {
            collectionOfLayerDefinitions = new Dictionary<string, Tuple<byte, Pen>>();
            Pen BluePenAlu = new Pen(new SolidBrush(Color.Blue), 3.0f);
            Pen YellowPenExtProf = new Pen(new SolidBrush(Color.GreenYellow), 1.0f);
            YellowPenExtProf.DashPattern = new float[] { 3.0f, 5.0f, 3.0f, 7.0f, 3.0f };
            Pen RedPenPvc = new Pen(new SolidBrush(Color.Red), 1.0f);
            Pen OrangePenThermalBridge = new Pen(new SolidBrush(Color.OrangeRed), 1.0f);
            Pen genericPen = new Pen(Color.Black);
            collectionOfLayerDefinitions.Add("EXT prof", new Tuple<byte, Pen>(1, YellowPenExtProf));
            collectionOfLayerDefinitions.Add("ALU", new Tuple<byte, Pen>(2, BluePenAlu));
            collectionOfLayerDefinitions.Add("PVC", new Tuple<byte, Pen>(3, RedPenPvc));
            collectionOfLayerDefinitions.Add("Thermal bridge", new Tuple<byte, Pen>(4, OrangePenThermalBridge));
            collectionOfLayerDefinitions.Add("0", new Tuple<byte, Pen>(5, genericPen));
            collectionOfLayerDefinitions.Add("AM_0", new Tuple<byte, Pen>(6, genericPen));
            collectionOfLayerDefinitions.Add("AM_1", new Tuple<byte, Pen>(7, genericPen));
        }
        public double internalScaleFactor { get; set; }
        public event internalScaleFactorChangedInternallyCallbackDelegate internalScaleFactorChangedInternally;
        public Control realPaintingCanvas = new Control();
        private const int marginForDrawControl = 5;
        public UserControlForPaint()
        {
            InitializeComponent();
            this.SuspendLayout();
            realPaintingCanvas.Left = 0+marginForDrawControl;
            realPaintingCanvas.Top = 0+marginForDrawControl;
            realPaintingCanvas.Parent = this;
            realPaintingCanvas.Width = this.Width-marginForDrawControl;
            realPaintingCanvas.Height = this.Height-marginForDrawControl;
            realPaintingCanvas.Padding = new Padding( marginForDrawControl);
            realPaintingCanvas.BackColor = Color.LightCyan;
            realPaintingCanvas.Paint += RealPaintingCanvas_Paint;
            this.ResumeLayout();
        }

        internal void performRescaling(double numericValue)
        {
            this.SuspendLayout();
            this.VerticalScroll.Value = 0;
            this.HorizontalScroll.Value = 0;
            internalScaleFactor = numericValue;
            this.realPaintingCanvas.Height = (int)(internalScaleFactor * Math.Abs(actualDrawingStructure.YLowerLeft - actualDrawingStructure.YUpperRight)) + 3;
            this.realPaintingCanvas.Width = (int)(internalScaleFactor * Math.Abs(actualDrawingStructure.XLowerLeft - actualDrawingStructure.XUpperRight)) + 3;
            //this.realPaintingCanvas.Left = this.Width / 2 - realPaintingCanvas.Width / 2 + 1;
            //this.realPaintingCanvas.Top = this.Height / 2 - realPaintingCanvas.Height / 2 + 1;
            this.realPaintingCanvas.Left = 1;
            this.realPaintingCanvas.Top = 1;
            this.realPaintingCanvas.Refresh();
            this.ResumeLayout();
        }

        private void RealPaintingCanvas_Paint(object sender, PaintEventArgs e)   {
            System.Drawing.Drawing2D.GraphicsContainer containerState = e.Graphics.BeginContainer();
            e.Graphics.ScaleTransform(1.0F, -1.0F);
            e.Graphics.TranslateTransform(0.0F, -(float)this.realPaintingCanvas.Height + 1.0f);
            if (actualDrawingStructure!=null)  {
                foreach (DXFentryForDisplay item in actualDrawingStructure)  {
                    if (item is MyDxfLineForDisplay)  {
                        MyDxfLineForDisplay item2 = item as MyDxfLineForDisplay;
                        e.Graphics.DrawLine(item2.penStructure, 
                            (float)(item2.XStart * internalScaleFactor+1), 
                            (float)(item2.YStart * internalScaleFactor+1), 
                            (float)(item2.XEnd *internalScaleFactor+1 ), 
                            (float)(item2.YEnd*internalScaleFactor+1 ) );
                    } else if (item is MyDxfArcForDisplay)  {
                        MyDxfArcForDisplay item2 = item as MyDxfArcForDisplay;
                        e.Graphics.DrawArc(item2.penStructure, 
                            (float)(item2.XUpper*internalScaleFactor+1), 
                            (float)(item2.YUpper*internalScaleFactor+1), 
                            (float)(item2.Width*internalScaleFactor), (float)(item2.Height*internalScaleFactor), 
                            (float)item2.startAngle, (float)item2.sweepAngle);
                    }
                }
            }
            e.Graphics.EndContainer(containerState);
        }

        /// <summary>
        /// Setup only INITIAL logical and graphical datastructures. 
        /// </summary>
        /// <param name="in_savedStructureOfDxf">Logical structure of DXF as read from file</param>
        public void setupLogicalAndGraphicalDXFstructures(LOGICAL.completeDxfStruct  in_savedStructureOfDxf)
        {
            savedStructureOfDxf = in_savedStructureOfDxf;
            //generate primal drawing structure
            MyDxfBoundingBox obtainedBox2 = savedStructureOfDxf.GetBoundingBox();
            if (obtainedBox2.XLowerLeft != 0) { offsetOfDxfHorizontal = 0 - obtainedBox2.XLowerLeft; }
            if (obtainedBox2.YLowerLeft != 0) { offsetOfDxfVertical = 0 - obtainedBox2.YLowerLeft; }
            primalDrawingStructure = new CompleteDxfDrawingStruct(null);
            int currentSizeOfDxfStruct = savedStructureOfDxf.getSize();
            for (int i=0; i<currentSizeOfDxfStruct; i++)
            {
                DXFdrawingEntry someEntry = savedStructureOfDxf.getItemByIndex(i);
                MyDxfBoundingBox obtainedBox = someEntry.GetBoundingBox();
                //offsets to centralize the drawing in the box
                Pen usedPen = null;
                if ((collectionOfLayerDefinitions != null) && (collectionOfLayerDefinitions.ContainsKey(someEntry.layerIdentifier)))
                {
                    usedPen = collectionOfLayerDefinitions[someEntry.layerIdentifier].Item2;
                }
                else
                {
                    usedPen = new Pen(Color.Black);
                }

                if (someEntry is MyDxfLine) {
                    MyDxfLine castLine = someEntry as MyDxfLine;
                    
                    DXFentryForDisplay theLineForDisplay = new MyDxfLineForDisplay(castLine.XStart+offsetOfDxfHorizontal, castLine.YStart+offsetOfDxfVertical, castLine.XEnd+offsetOfDxfHorizontal, castLine.YEnd+offsetOfDxfVertical,usedPen);
                    primalDrawingStructure.addSingleEntry(theLineForDisplay, obtainedBox.XLowerLeft + offsetOfDxfHorizontal, obtainedBox.YLowerLeft + offsetOfDxfVertical, obtainedBox.XUpperRight + offsetOfDxfHorizontal, obtainedBox.YUpperRight + offsetOfDxfVertical);
                } else if (someEntry is MyDxfArc) {
                    MyDxfArc castArc = someEntry as MyDxfArc;
                    DXFentryForDisplay theArcForDisplay = new MyDxfArcForDisplay(castArc.XCenter + offsetOfDxfHorizontal, castArc.YCenter + offsetOfDxfVertical, castArc.Radius, castArc.StartAngleDegree, castArc.EndAngleDegree,usedPen);
                    primalDrawingStructure.addSingleEntry(theArcForDisplay, obtainedBox.XLowerLeft + offsetOfDxfHorizontal, obtainedBox.YLowerLeft + offsetOfDxfVertical, obtainedBox.XUpperRight + offsetOfDxfHorizontal, obtainedBox.YUpperRight + offsetOfDxfVertical);
                }
            }
            //performing flip on draw structure is done by means of graphical container
        }

        public void prepareActualGraphicalDXFStructure()  {
            if (primalDrawingStructure != null) {
                this.SuspendLayout();
                actualDrawingStructure = new CompleteDxfDrawingStruct(primalDrawingStructure);

                //expand the size of drawing control
                // A TOURNAMENT FOR SELECTION OF INITIAL SCALE FACTOR STARTS NOW!
                // RULES: get two scale factors (horizontal and vertical) and use that one which is smaller
                double internalScaleFactorHorizontalChallenger = (this.Size.Width) / (Math.Abs(actualDrawingStructure.XLowerLeft - actualDrawingStructure.XUpperRight) + 2);
                double internalScaleFactorVerticalChallenger = (this.Size.Height) / (Math.Abs(actualDrawingStructure.YLowerLeft - actualDrawingStructure.YUpperRight) + 2);
                if (internalScaleFactorHorizontalChallenger < internalScaleFactorVerticalChallenger)  {
                    internalScaleFactor = internalScaleFactorHorizontalChallenger;
                } else {
                    internalScaleFactor = internalScaleFactorVerticalChallenger;
                }
                this.realPaintingCanvas.Height = (int)(internalScaleFactor * Math.Abs(actualDrawingStructure.YLowerLeft - actualDrawingStructure.YUpperRight))+3;
                this.realPaintingCanvas.Width = (int)(internalScaleFactor * Math.Abs(actualDrawingStructure.XLowerLeft - actualDrawingStructure.XUpperRight))+3;
                this.realPaintingCanvas.Left = this.Width / 2 - realPaintingCanvas.Width / 2 + 1;
                this.realPaintingCanvas.Top = this.Height / 2 - realPaintingCanvas.Height / 2 + 1;
                internalScaleFactorChangedInternally?.Invoke(internalScaleFactor);
                /*
                if ( Math.Abs(actualDrawingStructure.XLowerLeft - actualDrawingStructure.XUpperRight) > Math.Abs(actualDrawingStructure.YLowerLeft - actualDrawingStructure.YUpperRight) )
                { //horizontal dimension is more than vertical. Take horizontal scale factor
                    internalScaleFactor = (this.Size.Width) / (Math.Abs(actualDrawingStructure.XLowerLeft - actualDrawingStructure.XUpperRight) + 2);
                    double internalScaleFactorSpecial = (this.Size.Width) / (Math.Abs(actualDrawingStructure.XLowerLeft - actualDrawingStructure.XUpperRight) );
                    this.realPaintingCanvas.Width = this.Width - 2;
                    this.realPaintingCanvas.Left = 1;
                    this.realPaintingCanvas.Height = (int)(internalScaleFactor * Math.Abs(actualDrawingStructure.YLowerLeft - actualDrawingStructure.YUpperRight));
                    this.realPaintingCanvas.Top = this.Height / 2 - realPaintingCanvas.Height / 2 + 1;
                } else {
                    internalScaleFactor = (this.Size.Height) / (Math.Abs(actualDrawingStructure.YLowerLeft - actualDrawingStructure.YUpperRight) + 2);
                    this.realPaintingCanvas.Height = this.Height - 2;
                    this.realPaintingCanvas.Top = 1;
                    double internalScaleFactorSpecial = (this.Size.Height) / (Math.Abs(actualDrawingStructure.YLowerLeft - actualDrawingStructure.YUpperRight));
                    this.realPaintingCanvas.Width = (int)(internalScaleFactor * Math.Abs(actualDrawingStructure.XLowerLeft - actualDrawingStructure.XUpperRight));
                    this.realPaintingCanvas.Left = this.Width / 2 - realPaintingCanvas.Width / 2 + 1;
                }
                */
                this.ResumeLayout(true);
            } else {
                throw new NullReferenceException("primalDrawingStructure was null. Either DXF file was not read or setupLogicalAndGraphicalDXFstructures was not called");
            }
        }

        public void UserControlForPaint_Resize(object sender, EventArgs e)
        {
            // https://stackoverflow.com/questions/19782208/refresh-scroll-bars-on-winform
            // hmmmm~ When scrollbarr are appearing on some dimension, 
            // it means that when user scrolls then Top or Left position may change and become <0 whether we want this or no
            bool relayoutNeeded = false;
            if (!(realPaintingCanvas.Left < 0)) {
                int realPaintingCanvasLeft = this.Width / 2 - realPaintingCanvas.Width / 2;
                if (realPaintingCanvasLeft < 0) { realPaintingCanvas.Left = 0; relayoutNeeded = true; }
                else { realPaintingCanvas.Left = realPaintingCanvasLeft; }
            }
            if (!(realPaintingCanvas.Top < 0))
            {
                int realPaintingCanvasTop = this.Height / 2 - realPaintingCanvas.Height / 2;
                if (realPaintingCanvasTop < 0) { realPaintingCanvas.Top = 0; relayoutNeeded = true; }
                else { realPaintingCanvas.Top = realPaintingCanvasTop; }
            }
            if (relayoutNeeded) { this.PerformLayout(); }
        }
    }
}
