﻿using System;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Drawing;
using System.Media;

namespace UTXL
{
    public partial class UTXL : Form
    {
        string allowedFileTypes = "Text Files (.txt)|*.txt|CSS Files (.css)|*.css";
        OpenFileDialog ofd = new OpenFileDialog();
        FolderBrowserDialog fbd = new FolderBrowserDialog();
        SaveFileDialog sfd = new SaveFileDialog();


        string currentPath;
        bool saved = true;


        // this function will run when the form get Initialized
        public UTXL()
        {
            InitializeComponent();
            enableSave(false);
            initRichTextBox();


            if (splitContainer.SplitterDistance > 0)
            {
                showFilesNavigatorToolStripMenuItem.Text = "Hide Files Navigator";
            }
            else
            {
                showFilesNavigatorToolStripMenuItem.Text = "Show Files Navigator";
            }

            richTextBox.RightMargin = richTextBox.Size.Width - 10;
            richTextBox.SelectionIndent += 10;

            // set allowed file types for open and save dialogs
            ofd.Filter = allowedFileTypes;
            sfd.Filter = allowedFileTypes;
        }


        /*************************** File ***************************/

        // this method will be called when new menu item is clicked
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.Text = "";
            saveToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
        }
        
        // this method will be called when open file menu item is clicked
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // check if file is selected successfully
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamReader sr = new StreamReader(File.OpenRead(ofd.FileName));
                // save selected file path
                this.currentPath = ofd.FileName;

                // get parent directory of the selected file
                string parentDirectory = Path.GetDirectoryName(this.currentPath);

                // add parent directory of the file to the tree view
                listDirectory(treeView, parentDirectory);

                // read text inside the selected file
                richTextBox.Text = sr.ReadToEnd();

                // enable save buttons
                enableSave(true);

                // dispose the stream reader, so we can use it in another place later
                sr.Dispose();
            }
        }
        
        // this method will be called when open directory menu item is clicked
        private void openDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // add selected directory to the tree view
                listDirectory(treeView, fbd.SelectedPath);
            }
        }

        // this method will be called when save menu item is clicked
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StreamWriter sr = new StreamWriter(currentPath);
            UTF8Encoding utf8 = new UTF8Encoding();
            // Save file without byte order mark (BOM)
            // ref: http://msdn.microsoft.com/en-us/library/s064f8w2.aspx
            sr.Write(richTextBox.Text, false, utf8);
            sr.Dispose();
        }

        // this method will be called when save as menu item is clicked
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = sfd.FileName;
                BinaryWriter bw = new BinaryWriter(File.Create(path));
                bw.Write(richTextBox.Text);
                bw.Dispose();
            }
        }

        // this method will be called when exit button is clicked
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        /*************************** Edit ***************************/
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.Cut();
        }
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox.SelectedText != null && richTextBox.SelectedText.Length != 0)
            {
                Clipboard.SetText(richTextBox.SelectedText);
            }

        }
        private void copyPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentPath != null)
            {
                Clipboard.SetText(currentPath);
            }
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.SelectedText = Clipboard.GetText();
        }
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.SelectAll();
        }
        private void uPPERCASEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.SelectedText = richTextBox.SelectedText.ToUpper();
        }
        private void lowerCaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.SelectedText = richTextBox.SelectedText.ToLower();
        }


        /*************************** View ***************************/

        // this method will be called when show files navigator button is clicked
        private void showFilesNavigatorToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (splitContainer.SplitterDistance > 0)
            {
                splitContainer.SplitterDistance = 0;
                showFilesNavigatorToolStripMenuItem.Text = "Show Files Navigator";
            }
            else
            {
                splitContainer.SplitterDistance = 180;
                showFilesNavigatorToolStripMenuItem.Text = "Hide Files Navigator";
            }

        }


        /*************************** Tools ***************************/
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm stg = new SettingsForm();
            stg.Show();
        }


        /*************************** Tree View ***************************/
        private void treeView_DoubleClick(object sender, EventArgs e)
        {
            string path = ((TreeView)sender).SelectedNode.ToolTipText;
            MessageBox.Show(path);
        }

        private void listDirectory(TreeView treeView, string path)
        {
            treeView.Nodes.Clear();
            var rootDirectoryInfo = new DirectoryInfo(path);
            treeView.Nodes.Add(createDirectoryNode(rootDirectoryInfo));
        }
        private static TreeNode createDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeNode(directoryInfo.Name);
            foreach (var directory in directoryInfo.GetDirectories())
                directoryNode.Nodes.Add(createDirectoryNode(directory));

            foreach (var file in directoryInfo.GetFiles())
                if (file.Extension.ToString() == ".txt")
                {
                    TreeNode node = new TreeNode(file.Name);
                    node.ToolTipText = file.FullName;
                    directoryNode.Nodes.Add(node);
                }

            return directoryNode;
        }


        /*************************** Rich Text Box ***************************/
        private void richTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            //MessageBox.Show(e.KeyValue.ToString());

            if (e.KeyValue == 219)
            {
                richTextBox.SelectedText += " }";
            }
        }



        /*************************** Helpers ***************************/
        public void enableSave(Boolean enable)
        {
            saveToolStripMenuItem.Enabled = enable;
            saveAsToolStripMenuItem.Enabled = enable;
        }
        

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.Redo();
        }

        private void richTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // regester keys to undo/redo when space or enter keys are pressed
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                this.SuspendLayout();
                richTextBox.Undo();
                richTextBox.Redo();
                this.ResumeLayout();
            }

        }

        private void increaseFontSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox.Font.Size < 100)
            {
                Font fnt = new Font(richTextBox.Font.Name, richTextBox.Font.Size + 2);
                richTextBox.Font = fnt;
            }
        }

        private void decreaseFontSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox.Font.Size > 2)
            {
                Font fnt = new Font(richTextBox.Font.Name, richTextBox.Font.Size - 2);
                richTextBox.Font = fnt;
            }

        }

        private void resetFontSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            initRichTextBox();
        }
        private void initRichTextBox()
        {

            if (Properties.Settings.Default.dark_mode)
            {
                richTextBox.BackColor = System.Drawing.ColorTranslator.FromHtml("#253238");
                richTextBox.ForeColor = Color.White;

                treeView.BackColor = System.Drawing.ColorTranslator.FromHtml("#253238");
                treeView.ForeColor = Color.White;

                splitContainer.BackColor = System.Drawing.ColorTranslator.FromHtml("#1F292E");
                splitContainer.ForeColor = Color.White;


                statusStrip.BackColor = System.Drawing.ColorTranslator.FromHtml("#1F292E");
                statusStrip.ForeColor = Color.White;

                this.BackColor = System.Drawing.ColorTranslator.FromHtml("#1F292E");
                this.ForeColor = Color.White;

                menuStrip.BackColor = System.Drawing.ColorTranslator.FromHtml("#1F292E");
                menuStrip.ForeColor = Color.White;

                toolStrip.Renderer = new ToolStripOverride();
                toolStrip.BackColor = System.Drawing.ColorTranslator.FromHtml("#1F292E");
                toolStrip.ForeColor = Color.White;
            }


            // set font and font size
            Font fnt = new Font(Properties.Settings.Default.font_name, Properties.Settings.Default.font_size);
            richTextBox.Font = fnt;

        }

    }
}

// a workarround to remove the white border of toolStrip in dark mode
public class ToolStripOverride: ToolStripProfessionalRenderer
{
    public ToolStripOverride() { }
    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e){ }
}