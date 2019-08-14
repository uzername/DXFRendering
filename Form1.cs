using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DXFRendering
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //we set how the lines are displayed only once. 
            userControlForPaint1.setupCollectionOfLayerDefinitions();
        }

        private void FolderBtn_Click(object sender, EventArgs e) {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() != DialogResult.Cancel) {
                String usedPathToShowFiles = folderBrowserDialog.SelectedPath;
                textBoxFolderPath.Text = usedPathToShowFiles;
                listBoxDxfFiles.DataSource = DXFRendering.LOGICAL.DxfReadWrapper.listAllDxfFilesInFolder(usedPathToShowFiles);
            }
        }

        private void TextBoxFolderPath_KeyUp(object sender, KeyEventArgs e)
        {
            if( e.KeyCode == Keys.Enter)  {
                string usedPathToShowFiles = textBoxFolderPath.Text;
                listBoxDxfFiles.DataSource = DXFRendering.LOGICAL.DxfReadWrapper.listAllDxfFilesInFolder(usedPathToShowFiles);
            }
        }

        private void ListBoxDxfFiles_SelectedValueChanged(object sender, EventArgs e) {
            DXFRendering.LOGICAL.singleDXFListBoxItem value = (DXFRendering.LOGICAL.singleDXFListBoxItem)this.listBoxDxfFiles.SelectedItem;
            //retrieve the logical structure of dxf file
            DXFRendering.LOGICAL.completeDxfStruct obtainedStruct = DXFRendering.LOGICAL.DxfReadWrapper.processDxfFile(value.fullPath);
            //transform it to initial graphical structure used during rendering
            this.userControlForPaint1.VerticalScroll.Value = 0;
            this.userControlForPaint1.HorizontalScroll.Value = 0;
            this.userControlForPaint1.setupLogicalAndGraphicalDXFstructures(obtainedStruct);
            this.userControlForPaint1.prepareActualGraphicalDXFStructure();
            //invoke transformations on graphical structure (later, when needed)
            // MEOW!
            //redraw? 
            this.userControlForPaint1.Refresh();
            
            //crutch to position drawing thing properly
            //this.userControlForPaint1.UserControlForPaint_Resize(null, null);
            //crutch to remove stray scrollbars
            this.userControlForPaint1.PerformLayout();
        }

        private void UserControlForPaint1_internalScaleFactorChangedInternally(double in_valueOfScaleFactor)
        {
            this.textBoxScaleFactor.Text = in_valueOfScaleFactor.ToString();
        }

        private void TextBoxScaleFactor_Validating(object sender, CancelEventArgs e)
        {
            double numericValue;
            if (Double.TryParse(textBoxScaleFactor.Text,out numericValue)==false) {
                textBoxScaleFactor.Undo();
                e.Cancel = true;
            } else {
                this.userControlForPaint1.performRescaling(numericValue);
            }
        }
    }
}
