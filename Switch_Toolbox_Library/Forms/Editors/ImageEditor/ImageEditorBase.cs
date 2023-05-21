using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Activities.Statements;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

namespace Toolbox.Library.Forms
{
    public partial class ImageEditorBase : UserControl
    {
        public int GetArrayDisplayLevel() => CurArrayDisplayLevel;
        public int GetMipmapDisplayLevel() => CurArrayDisplayLevel;

        public class ImageReplaceEventArgs : EventArgs
        {
            public Bitmap ReplacedTexture { get; private set; }

            public ImageReplaceEventArgs(Bitmap texture) {
                ReplacedTexture = texture;
            }
        }

        public delegate void StatusUpdateHandler(object sender, ImageReplaceEventArgs e);
        public event StatusUpdateHandler OnTextureReplaced;

        private void UpdateTextureReplace(Bitmap texture)
        {
            // Make sure someone is listening to event
            if (OnTextureReplaced == null) return;

            ImageReplaceEventArgs args = new ImageReplaceEventArgs(texture);
            OnTextureReplaced(this, args);
        }

        private void GammaFixPreviewAction(object sender, EventArgs args) {
            AdjustGammaImagePreview(previewGammaFixSmashUltimateToolStripMenuItem.Checked);
        }

        public void AdjustGammaImagePreview(bool preview)
        {
            Runtime.ImageEditor.PreviewGammaFix = preview;
            UpdateGamma();
        }

        private void UpdateGamma()
        {
            if (Runtime.ImageEditor.PreviewGammaFix)
                pictureBoxCustom1.Image = BitmapExtension.AdjustGamma(pictureBoxCustom1.Image, 1.0f / 2.2f);
            else
            {
                if (ActiveTexture != null)
                    UpdateImage(ActiveTexture, CurArrayDisplayLevel);
            }

            UpdateLabel();
        }

        private void UpdateLabel()
        {
            noteifcationLbl.Text = "";

            if (Runtime.ImageEditor.PreviewGammaFix)
                noteifcationLbl.Text = "Note: gamma correction enabled! ";
            if (!Runtime.ImageEditor.DisplayAlpha)
                noteifcationLbl.Text += "Note: alpha channel hidden in editor!";
        }

        private int _currentCacheIndex = -1;

        private int currentCacheIndex
        {
            get
            {
                return _currentCacheIndex;
            }
            set
            {

                SetRedoUndoMenuStrips(value);
                _currentCacheIndex = value;
            }
        }

        private void SetRedoUndoMenuStrips(int index)
        {
            if (index < ImageCache.Count && ImageCache.Count > 0 && index != -1)
                redoToolStripMenuItem.Enabled = true;
            else
                redoToolStripMenuItem.Enabled = false;

            if (index > 0 && ImageCache.Count > 0)
                undoToolStripMenuItem.Enabled = true;
            else
                undoToolStripMenuItem.Enabled = false;

        }

        private List<Image> _imageCache = new List<Image>();

        private List<Image> ImageCache
        {
            get
            {
                return _imageCache;
            }
            set
            {
                _imageCache = value;

                SetRedoUndoMenuStrips(currentCacheIndex);

                if (_imageCache.Count > 0)
                {
                    undoToolStripMenuItem.Enabled = true;
                }
                else
                {
                    undoToolStripMenuItem.Enabled = false;
                }
            }
        }

        public FileSystemWatcher FileWatcher;

        private Thread Thread;

        int CurMipDisplayLevel = 0;
        int CurArrayDisplayLevel = 0;
        uint TotalMipCount = 1;
        uint TotalArrayCount = 1;
        public STGenericTexture ActiveTexture;

        public bool PropertyShowTop = false;
        public bool CanModify = true;

        //Instead of disabling/enabling channels, this will only show the selected channel
        public bool UseChannelToggle = false;

        private bool hasBeenEdited = false;
        public bool HasBeenEdited
        {
            set
            {
                hasBeenEdited = value;

                if (ActiveTexture != null)
                    ActiveTexture.IsEdited = value;

                saveBtn.Enabled = hasBeenEdited;

                if (saveBtn.Enabled == false)
                    saveBtn.BackgroundImage = GetGrayscale(Properties.Resources.Save);
                else
                    saveBtn.BackgroundImage = Properties.Resources.Save;

                saveBtn.NotifyDefault(false);
            }
            get
            {
                return hasBeenEdited;
            }
        }

        public bool HasRedChannel = true;
        public bool HasBlueChannel = true;
        public bool HasGreenChannel = true;
        public bool HasAlphaChannel = true;

        ImagePropertiesEditor propertiesEditor;

        public bool ShowChannelEditor = true;

        public void ResetMenus()
        {
            fileToolStripMenuItem.DropDownItems.Clear();
            fileToolStripMenuItem.DropDownItems.Add(exportToolStripMenuItem);  
        }

        public void AddFileContextEvent(string Name, EventHandler eventHandler, bool Checked = false)
        {
            if (Name == "Save")
            {
                fileToolStripMenuItem.DropDownItems.Insert(0, new STToolStripItem(Name, eventHandler));
            }
            else
            {
                fileToolStripMenuItem.DropDownItems.Add(new STToolStripItem(Name, eventHandler) { Checked = Checked });
            }
        }

        public Image GetGrayscale(Image img)
        {
            // also fix the problem of indexed pixel format (if any)
            Bitmap imge = new Bitmap(img);

            //create graphics
            Graphics g = Graphics.FromImage(imge);
            //draw disabled image
            System.Windows.Forms.ControlPaint.DrawImageDisabled(g, img, 0, 0, Color.Transparent);

            //retrun result
            return imge;
        }

        public UserControl GetActiveTabEditor(Type type)
        {
            return propertiesEditor.GetActiveTabControl(type);
        }
        public void AddCustomControl(UserControl control, Type type)
        {
            propertiesEditor.AddTabPage(control, type);
        }
        public ImageEditorBase()
        {
            InitializeComponent();

            backgroundPB.BackColor = Runtime.CustomPicureBoxBGColor;
            backgroundPB.Visible = false;

            useComponentSelectorToolStripMenuItem.Checked = Runtime.ImageEditor.UseComponetSelector;
            enableZoomToolStripMenuItem.Checked = Runtime.ImageEditor.EnableImageZoom;

            displayAlphaToolStripMenuItem.Checked = Runtime.ImageEditor.DisplayAlpha;
            SetAlphaEnableUI(Runtime.ImageEditor.DisplayAlpha);

            propertiesEditor = new ImagePropertiesEditor();
            propertiesEditor.Dock = DockStyle.Fill;

            saveBtn.Enabled = HasBeenEdited;

            imageToolStripMenuItem.Enabled = false;
            adjustmentsToolStripMenuItem.Enabled = false;
            editBtn.BackgroundImage = BitmapExtension.GrayScale(Properties.Resources.Edit);

            foreach (var type in Enum.GetValues(typeof(Runtime.PictureBoxBG)).Cast<Runtime.PictureBoxBG>())
                imageBGComboBox.Items.Add(type);

            imageBGComboBox.SelectedItem = Runtime.pictureBoxStyle;
            UpdateBackgroundImage();
            SetZoomSetting();

            SetEditorOrientation(Runtime.ImageEditor.DisplayVertical, true);

            previewGammaFixSmashUltimateToolStripMenuItem.Checked = Runtime.ImageEditor.PreviewGammaFix;
            UpdateLabel();

            propertyGridToolStripMenuItem.Checked = Runtime.ImageEditor.ShowPropertiesPanel;

            if (!propertyGridToolStripMenuItem.Checked)
                HidePropertyGrid(true);
            else
                HidePropertyGrid(false);

            OnDataAcquiredEvent += new DataAcquired(ThreadReportsDataAquiredEvent);

            SetUpFileSystemWatcher();

            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = false;
        }

        private void SetUpFileSystemWatcher()
        {
            FileWatcher = new FileSystemWatcher();
            FileWatcher.Path = Path.GetTempPath();
            FileWatcher.NotifyFilter = NotifyFilters.Attributes |
                NotifyFilters.CreationTime |
                NotifyFilters.FileName |
                NotifyFilters.LastAccess |
                NotifyFilters.LastWrite |
                NotifyFilters.Size |
                NotifyFilters.Security;

            FileWatcher.EnableRaisingEvents = false;
            FileWatcher.Changed += new FileSystemEventHandler(OnFileWatcherChanged);
            FileWatcher.Filter = "";
        }

        public void SetEditorOrientation(bool ToVertical, bool IsStartup = false)
        {
            //Check if it's already set the setting and is not from creating a new instance. It's not necessary to reset if it is.
            if (displayVerticalToolStripMenuItem.Checked != ToVertical || IsStartup)
            {
                displayVerticalToolStripMenuItem.Checked = ToVertical;
                SetOrientation();
            }
        }

        private void UpdateBackgroundImage()
        {
            switch (Runtime.pictureBoxStyle)
            {
                case Runtime.PictureBoxBG.Black:
                    pictureBoxCustom1.GridDisplayMode = Cyotek.Windows.Forms.ImageBoxGridDisplayMode.None;
                    pictureBoxCustom1.BackColor = Color.Black;
                    break;
                case Runtime.PictureBoxBG.White:
                    pictureBoxCustom1.GridDisplayMode = Cyotek.Windows.Forms.ImageBoxGridDisplayMode.None;
                    pictureBoxCustom1.BackColor = Color.White;
                    break;
                case Runtime.PictureBoxBG.Checkerboard:
                    pictureBoxCustom1.GridDisplayMode = Cyotek.Windows.Forms.ImageBoxGridDisplayMode.Client;
                    break;
                case Runtime.PictureBoxBG.Custom:
                    pictureBoxCustom1.GridDisplayMode = Cyotek.Windows.Forms.ImageBoxGridDisplayMode.None;
                    pictureBoxCustom1.BackColor = Runtime.CustomPicureBoxBGColor;
                    break;
            }
        }

        public void LoadProperties(object prop, Action OnPropertyChanged = null) => propertiesEditor.LoadProperties(prop, OnPropertyChanged);

        public void LoadImage(STGenericTexture texture, int arrayLevel = 0)
        {
            editBtn.Enabled = false;
            editToolStripMenuItem.Enabled = false;

            //Disable the file watcher when an image is switched
            FileWatcher.EnableRaisingEvents = false;
            FileWatcher.Filter = "";

            UpdateImage(texture, arrayLevel);
        }

        private void UpdateImage(STGenericTexture texture, int arrayLevel = 0)
        {
            ResetChannelEditor();

            HasBeenEdited = false;

            if (texture.CanEdit)
            {
                imageToolStripMenuItem.Enabled = true;
                adjustmentsToolStripMenuItem.Enabled = true;
            }

            ActiveTexture = texture;

            if (ActiveTexture.CanEdit)
            {
                editBtn.Enabled = true;
                editToolStripMenuItem.Enabled = true;
                editBtn.BackgroundImage = Properties.Resources.Edit;
            }
            else
            {
                editBtn.BackgroundImage = BitmapExtension.GrayScale(Properties.Resources.Edit);
            }

            CurMipDisplayLevel = 0;
            CurArrayDisplayLevel = arrayLevel;
            hasBeenEdited = false;

            UpdateMipDisplay();
        }
        private Bitmap SetChannel(Bitmap image, int ChannelIndex)
        {
            if (ChannelIndex == 0)
                return image;
            else if (ChannelIndex == 1)
                return BitmapExtension.ShowChannel(image, STChannelType.Red);
            else if (ChannelIndex == 2)
                return BitmapExtension.ShowChannel(image, STChannelType.Green);
            else if (ChannelIndex == 3)
                return BitmapExtension.ShowChannel(image, STChannelType.Blue);
            else
                return BitmapExtension.ShowChannel(image, STChannelType.Alpha);
        }
        private bool isFinished;
        private bool IsFinished
        {
            get
            {
                return isFinished;
            }
            set
            {
                isFinished = value;

                if (value)
                    SetBottomBar();
            }
        }

        private void SetPicture(Image img)
        {
            if (pictureBoxCustom1.InvokeRequired)
            {
                pictureBoxCustom1.Invoke(new MethodInvoker(
                delegate ()
                {
                    pictureBoxCustom1.Image = img;
                }));
            }
            else
            {
                pictureBoxCustom1.Image = img;
            }
        }

        public delegate void DataAcquired(object sender);

        public event DataAcquired OnDataAcquiredEvent;

        private bool DecodeProcessFinished = false; //Used to determine when the decode process is done

        private void ImagePreviewZoom()
        {

        }

        public Image BaseImage;

        private void UpdateTreeIcon(TreeNode node, Image image)
        {
            if (node is ISingleTextureIconLoader)
            {
                ObjectEditor editor = LibraryGUI.GetObjectEditor();
                if (editor != null) //The editor isn't always in object editor so check
                {
                    editor.UpdateTextureIcon((ISingleTextureIconLoader)node, image);
                }
            }
        }

        private void UpdatePictureBox(int ChannelIndex = 0)
        {
            if (ActiveTexture == null)
                return;

            DecodeProcessFinished = false;

            PushImage(Properties.Resources.LoadingImage);

            var image = ActiveTexture.GetBitmap(CurArrayDisplayLevel, CurMipDisplayLevel);

            //Keep base image for channel viewer updating/editing
            if (image != null)
                BaseImage = new Bitmap(image);
            else
                BaseImage = null;

            if (propertiesEditor.InvokeRequired)
            {
                propertiesEditor.Invoke(new MethodInvoker(
                 delegate ()
                 {
                     LoadChannelEditor(image);
                 }));
            }
            else
            {
                LoadChannelEditor(image);
            }

            if (image != null)
            {
                if (ActiveTexture.Parameters.FlipY)
                    image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                if (ChannelIndex == 1)
                    BitmapExtension.SetChannel(image, STChannelType.Red, STChannelType.Red, STChannelType.Red, STChannelType.One);
                else if (ChannelIndex == 2)
                    BitmapExtension.SetChannel(image, STChannelType.Green, STChannelType.Green, STChannelType.Green, STChannelType.One);
                else if (ChannelIndex == 3)
                    BitmapExtension.SetChannel(image, STChannelType.Blue, STChannelType.Blue, STChannelType.Blue, STChannelType.One);
                else if (ChannelIndex == 4)
                    BitmapExtension.SetChannel(image, STChannelType.Alpha, STChannelType.Alpha, STChannelType.Alpha, STChannelType.One);
                else
                {
                    STChannelType AlphaDisplay = ActiveTexture.AlphaChannel;
                    if (!Runtime.ImageEditor.DisplayAlpha || HasZeroAlpha())
                        AlphaDisplay = STChannelType.One;

                    //For RGBA types try to only load the alpha toggle to load quicker
                    //Loading components would not be necessary as it is default to RGBA
                    if (UseRGBA())
                    {
                        if (!Runtime.ImageEditor.DisplayAlpha)
                            BitmapExtension.SetChannel(image, STChannelType.Red, STChannelType.Green, STChannelType.Blue, AlphaDisplay);
                    }
                    else
                    {
                        //Check components for the channels
                        if (Runtime.ImageEditor.UseComponetSelector)
                        {
                            BitmapExtension.SetChannel(image, ActiveTexture.RedChannel, ActiveTexture.GreenChannel, ActiveTexture.BlueChannel, AlphaDisplay);
                        }
                        else
                        {
                            if (!Runtime.ImageEditor.DisplayAlpha)
                                BitmapExtension.SetChannel(image, STChannelType.Red, STChannelType.Green, STChannelType.Blue, AlphaDisplay);
                        }
                    }
                }

                DecodeProcessFinished = true;

                if (Runtime.ImageEditor.PreviewGammaFix && image != null)
                    image = BitmapExtension.AdjustGamma(image, 1.0f / 2.2f);

                // BitmapExtension.SetChannels(image, HasRedChannel, HasBlueChannel, HasGreenChannel, HasAlphaChannel);
                PushImage(image);


                if (image != null)
                    UpdateTreeIcon(ActiveTexture, image);
            }
        }

        private bool HasZeroAlpha()
        {
            if (ActiveTexture.AlphaChannel == STChannelType.Zero)
                return true;
            else
                return false;
        }

        private bool UseRGBA()
        {
            if (ActiveTexture.RedChannel == STChannelType.Red &&
                ActiveTexture.GreenChannel == STChannelType.Green &&
                ActiveTexture.BlueChannel == STChannelType.Blue &&
                ActiveTexture.AlphaChannel == STChannelType.Alpha)
                return true;
            else
                return false;
        }

        private void PushImage(Image image)
        {
            if (pictureBoxCustom1.InvokeRequired)
            {
                pictureBoxCustom1.Invoke(this.OnDataAcquiredEvent,
                new object[] { image });
            }
            else
            {
                pictureBoxCustom1.Image = image;
                pictureBoxCustom1.Refresh();

                if (DecodeProcessFinished)
                    IsFinished = true;
            }
        }

        private void ThreadReportsDataAquiredEvent(object sender)
        {
            pictureBoxCustom1.Image = (Image)sender;
            pictureBoxCustom1.Refresh();

            if (DecodeProcessFinished)
                IsFinished = true;
        }

        private void UpdateImageCounter()
        {
            if (ActiveTexture == null)
                return;

            if (ActiveTexture.ArrayCount == 0)
                ActiveTexture.ArrayCount = 1;
            if (ActiveTexture.MipCount == 0)
                ActiveTexture.MipCount = 1;

            TotalMipCount = ActiveTexture.MipCount - 1;
            TotalArrayCount = ActiveTexture.ArrayCount - 1;

            arrayLevelCounterLabel.Text = $"Array Level: {CurArrayDisplayLevel} / {TotalArrayCount}";
            mipLevelCounterLabel.Text = $"Mip Level: {CurMipDisplayLevel} / {TotalMipCount}";
        }

        public int CurrentChannelIndex;

        bool IsCancelled = false;
        public void UpdateMipDisplay()
        {
            if (HasBeenEdited)
            {
                var message = MessageBox.Show("This texture has been edited! Would you like to apply the changes made?", "Texture Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (message == DialogResult.Yes)
                    ApplyEdit(pictureBoxCustom1.Image);

                HasBeenEdited = false;
            }

            IsFinished = false;

            CurrentChannelIndex = 0;

            if (propertiesEditor != null && propertiesEditor.channelListView.SelectedIndices.Count > 0)
                CurrentChannelIndex = propertiesEditor.channelListView.SelectedIndices[0];

            if (this.Thread != null && this.Thread.IsAlive && Thread.ThreadState.ToString() != "AbortRequested")
            {
                IsCancelled = true;
            }

            Thread = new Thread((ThreadStart)(() =>
            {
                UpdatePictureBox(CurrentChannelIndex);
            }));
            Thread.Start();

            UpdateImageCounter();

            if (CurMipDisplayLevel != TotalMipCount)
                BtnMipsRight.Enabled = true;
            else
                BtnMipsRight.Enabled = false;

            if (CurMipDisplayLevel != 0)
                BtmMipsLeft.Enabled = true;
            else
                BtmMipsLeft.Enabled = false;

            if (CurArrayDisplayLevel != TotalArrayCount)
                btnRightArray.Enabled = true;
            else
                btnRightArray.Enabled = false;

            if (CurArrayDisplayLevel != 0)
                btnLeftArray.Enabled = true;
            else
                btnLeftArray.Enabled = false;
        }
        private void imageBGComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Runtime.pictureBoxStyle = (Runtime.PictureBoxBG)imageBGComboBox.SelectedItem;
            if (Runtime.pictureBoxStyle == Runtime.PictureBoxBG.Custom)
                backgroundPB.Visible = true;
            else if (backgroundPB.Visible)
                backgroundPB.Visible = false;

            UpdateBackgroundImage();
        }

        private void BtmMipsLeft_Click(object sender, EventArgs e)
        {
            if (CurMipDisplayLevel != 0)
                CurMipDisplayLevel -= 1;

            UpdateMipDisplay();
        }

        private void BtnMipsRight_Click(object sender, EventArgs e)
        {
            if (CurMipDisplayLevel != TotalMipCount)
                CurMipDisplayLevel += 1;

            UpdateMipDisplay();
        }

        private void btnLeftArray_Click(object sender, EventArgs e)
        {
            if (CurArrayDisplayLevel != 0)
                CurArrayDisplayLevel -= 1;

            UpdateMipDisplay();
        }

        private void btnRightArray_Click(object sender, EventArgs e)
        {
            if (CurArrayDisplayLevel != TotalArrayCount)
                CurArrayDisplayLevel += 1;

            UpdateMipDisplay();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActiveTexture.ExportImage();
        }

        private void propertyGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HidePropertyGrid(!propertyGridToolStripMenuItem.Checked);
            Runtime.ImageEditor.ShowPropertiesPanel = propertyGridToolStripMenuItem.Checked;
        }

        private void stPropertyGrid1_Load(object sender, EventArgs e)
        {
        }

        private void btnRedCompSel_Click(object sender, EventArgs e)
        {
            if (HasRedChannel)
                HasRedChannel = false;
            else
                HasRedChannel = true;

            UpdateMipDisplay();
        }

        private void btnGreenCompSel_Click(object sender, EventArgs e)
        {
            if (HasGreenChannel)
                HasGreenChannel = false;
            else
                HasGreenChannel = true;

            UpdateMipDisplay();
        }

        private void btnBlueCompSel_Click(object sender, EventArgs e)
        {
            if (HasBlueChannel)
                HasBlueChannel = false;
            else
                HasBlueChannel = true;

            UpdateMipDisplay();
        }

        private void btnAlphaCompSel_Click(object sender, EventArgs e)
        {
            if (HasAlphaChannel)
                HasAlphaChannel = false;
            else
                HasAlphaChannel = true;

            UpdateMipDisplay();
        }

        private void channelViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadChannelEditor(pictureBoxCustom1.Image);
        }

        public delegate void UpdateChannelViewer();
        private void LoadChannelEditor(Image bitmap)
        {
            if (bitmap == null)
                return;

            propertiesEditor.LoadImage(new Bitmap(bitmap), this);
        }

        private void ResetChannelEditor()
        {
            propertiesEditor.ResetChannels();
        }

        private void InvokeMethod()
        {
        }

        private void fliVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RotateImage(RotateFlipType.RotateNoneFlipY);
        }

        private void flipHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RotateImage(RotateFlipType.RotateNoneFlipX);
        }

        private void rotate90ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RotateImage(RotateFlipType.Rotate90FlipNone);
        }

        private void rotate90CounterClockwiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RotateImage(RotateFlipType.Rotate90FlipX);
        }

        private void rToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RotateImage(RotateFlipType.Rotate180FlipNone);
        }

        private void RotateImage(RotateFlipType rotation)
        {
            Image Image = pictureBoxCustom1.Image;
            if (Image != null)
            {
                Image.RotateFlip(rotation);
                UpdateEditCached(Image);
            }
        }

        private void UpdateEditCached(Image image)
        {
            if (IsFinished)
                UpdateEdit(image);
        }

        private void UpdateEdit(Image image)
        {
            if (ActiveTexture.ArrayCount == 0) ActiveTexture.ArrayCount = 1;

            ActiveTexture.EditedImages = new EditedBitmap[ActiveTexture.ArrayCount];
            ActiveTexture.EditedImages[CurArrayDisplayLevel] = new EditedBitmap()
            {
                bitmap = new Bitmap(image),
                ArrayLevel = CurArrayDisplayLevel
            };

            pictureBoxCustom1.Image = image;
            pictureBoxCustom1.Refresh();

            UpdateTreeIcon(ActiveTexture, image);

            TotalMipCount = ActiveTexture.MipCount - 1;
            TotalArrayCount = ActiveTexture.ArrayCount - 1;

            arrayLevelCounterLabel.Text = $"Array Level: {CurArrayDisplayLevel} / {TotalArrayCount}";
            mipLevelCounterLabel.Text = $"Mip Level: {CurMipDisplayLevel} / {TotalMipCount}";

            propertiesEditor.UpdateProperties();
            propertiesEditor.LoadImage(new Bitmap(image), this);

            HasBeenEdited = true;
        }

        private void ApplyEdit(Image Image)
        {
            STProgressBar progressBar = new STProgressBar();
            progressBar.Task = "Applying Edits";
            progressBar.Value = 0;
            progressBar.IsConstant = true;
            progressBar.StartPosition = FormStartPosition.CenterScreen;
            progressBar.Show();
            progressBar.Refresh();

            ActiveTexture.Width = (uint)Image.Width;
            ActiveTexture.Height = (uint)Image.Height;

            ActiveTexture.SetImageData(new Bitmap(Image), CurArrayDisplayLevel);

            CurMipDisplayLevel = 0;
            HasBeenEdited = false;

            if (ActiveTexture.EditedImages != null && ActiveTexture.EditedImages[CurArrayDisplayLevel] != null)
            {
                if (ActiveTexture.EditedImages[CurArrayDisplayLevel].bitmap != null)
                    ActiveTexture.EditedImages[CurArrayDisplayLevel].bitmap.Dispose();
                ActiveTexture.EditedImages[CurArrayDisplayLevel] = null;
            }

            progressBar.Value = 100;

            UpdateTextureReplace(new Bitmap(Image));
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (pictureBoxCustom1.Image != null)
                ApplyEdit(pictureBoxCustom1.Image);
        }

        private void resizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageResizeDialog resizeEditor = new ImageResizeDialog();
            resizeEditor.LoadImage(pictureBoxCustom1.Image);
            if (resizeEditor.ShowDialog() == DialogResult.OK)
            {
                UpdateEditCached(resizeEditor.newImage);
                ApplyEdit(resizeEditor.newImage);

                UpdateImage(ActiveTexture);
            }
        }

        private void reEncodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsFinished)
                return;

            ImageReEncodeDialog encodingEditor = new ImageReEncodeDialog();
            encodingEditor.LoadImage(pictureBoxCustom1.Image, ActiveTexture);
            if (encodingEditor.ShowDialog() == DialogResult.OK)
            {
                ActiveTexture.Format = encodingEditor.Format;
                ActiveTexture.MipCount = (uint)encodingEditor.MipCount;

                UpdateEditCached(encodingEditor.newImage);
                ApplyEdit(encodingEditor.newImage);

                UpdateImage(ActiveTexture);
            }
        }

        public Image GetActiveImage() => pictureBoxCustom1.Image;

        private void hueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsFinished)
                return;

        /*  //  HueSaturationAdjuster hsvEditor = new HueSaturationAdjuster();
           // hsvEditor.LoadBitmap(pictureBoxCustom1);

            if (hsvEditor.ShowDialog() == DialogResult.OK)
            {
            }*/
        }

        private void toggleAlphaChk_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsFinished)
                return;

            UpdatePictureBox();
        }


        private void propertyGridToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void SetOrientation()
        {
            if (displayVerticalToolStripMenuItem.Checked)
            {
                DisplayVertical();
            }
            else
            {
                DisplayHorizontal();
            }
        }

        private void displayVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (displayVerticalToolStripMenuItem.Checked)
            {
                displayVerticalToolStripMenuItem.Checked = false;
            }
            else
            {
                displayVerticalToolStripMenuItem.Checked = true;
            }

            Runtime.ImageEditor.DisplayVertical = displayVerticalToolStripMenuItem.Checked;

            SetOrientation();
            SaveSettings();
        }

        private void HidePropertyGrid(bool Hide)
        {
            if (Hide)
            {
                if (splitContainer1.Panel1.Controls.Contains(propertiesEditor))
                {
                    splitContainer1.Panel1Collapsed = true;
                    splitContainer1.Panel1.Hide();
                }
                if (splitContainer1.Panel2.Controls.Contains(propertiesEditor))
                {
                    splitContainer1.Panel2Collapsed = true;
                    splitContainer1.Panel2.Hide();
                }
            }
            else
            {
                if (splitContainer1.Panel1.Controls.Contains(propertiesEditor))
                {
                    splitContainer1.Panel1Collapsed = false;
                    splitContainer1.Panel1.Show();
                }
                if (splitContainer1.Panel2.Controls.Contains(propertiesEditor))
                {
                    splitContainer1.Panel2Collapsed = false;
                    splitContainer1.Panel2.Show();
                }
            }
        }

        private void DisplayHorizontal()
        {
            if (splitContainer1.Panel1Collapsed || propertiesEditor == null)
                return;

            var ImagePanel = stPanel1;
            var PropertiesEditor = propertiesEditor;

            //Swap panels
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel2.Controls.Clear();

            splitContainer1.Orientation = Orientation.Vertical;
            splitContainer1.Panel1.Controls.Add(ImagePanel);
            splitContainer1.Panel2.Controls.Add(PropertiesEditor);
            PropertiesEditor.HideHintPanel(false);

            PropertiesEditor.Width = this.Width / 2;

            splitContainer1.SplitterDistance = this.Width / 2;
        }

        private void DisplayVertical()
        {
            if (splitContainer1.Panel2Collapsed || propertiesEditor == null)
                return;

            var ImagePanel = stPanel1;
            var PropertiesEditor = propertiesEditor;

            //Swap panels
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel2.Controls.Clear();

            splitContainer1.Orientation = Orientation.Horizontal;
            splitContainer1.Panel2.Controls.Add(ImagePanel);
            splitContainer1.Panel1.Controls.Add(PropertiesEditor);
            PropertiesEditor.HideHintPanel(true);

            splitContainer1.SplitterDistance = this.Height / 2;

        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            if (!IsFinished)
                return;

            EditInExternalProgram();
        }

        private TEX_FORMAT FormatToChange = TEX_FORMAT.UNKNOWN;
        private void EditInExternalProgram(bool UseDefaultEditor = true)
        {
            if (!ActiveTexture.CanEdit || !IsFinished)
                return;

            ImageProgramSettings settings = new ImageProgramSettings();
            settings.LoadImage(ActiveTexture);
            if (settings.ShowDialog() == DialogResult.OK)
            {
                UseDefaultEditor = !settings.OpenDefaultProgramSelection;

                string UseExtension = settings.GetSelectedExtension();
                FormatToChange = settings.GetSelectedImageFormat();

                string TemporaryName = Path.GetTempFileName();
                Utils.DeleteIfExists(Path.ChangeExtension(TemporaryName, UseExtension));
                File.Move(TemporaryName, Path.ChangeExtension(TemporaryName, UseExtension));
                TemporaryName = Path.ChangeExtension(TemporaryName, UseExtension);

                switch (UseExtension)
                {
                    case ".dds":
                        ActiveTexture.SaveDDS(TemporaryName, true, false, CurArrayDisplayLevel, CurMipDisplayLevel);
                        break;
                    case ".astc":
                        ActiveTexture.SaveASTC(TemporaryName, true, false, CurArrayDisplayLevel, CurMipDisplayLevel);
                        break;
                    case ".tga":
                        ActiveTexture.SaveTGA(TemporaryName, true, false, CurArrayDisplayLevel, CurMipDisplayLevel);
                        break;
                    default:
                        ActiveTexture.SaveBitMap(TemporaryName, true, false, CurArrayDisplayLevel, CurMipDisplayLevel);
                        break;
                }

                //Start watching for changes
                FileWatcher.EnableRaisingEvents = true;
                FileWatcher.Filter = Path.GetFileName(TemporaryName);

                if (UseDefaultEditor)
                    Process.Start(TemporaryName);
                else
                    ShowOpenWithDialog(TemporaryName);
            }
        }

        public static Process ShowOpenWithDialog(string path)
        {
            var args = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shell32.dll");
            args += ",OpenAs_RunDLL " + path;
            return Process.Start("rundll32.exe", args);
        }

        private void OnFileWatcherChanged(object sender, FileSystemEventArgs e)
        {
            string FileName = e.FullPath;

            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
            var Result = MessageBox.Show("Texture has been modifed in external program! Would you like to apply the edits?", "Texture Editor",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

            if (Result == DialogResult.Yes)
            {
                bool DecodeTextureBack = false;
                if (FormatToChange != TEX_FORMAT.UNKNOWN && ActiveTexture.Format != FormatToChange)
                {
                    ActiveTexture.Format = FormatToChange;
                    DecodeTextureBack = true;
                }

                if (FileName.EndsWith(".dds"))
                {
                    DDS dds = new DDS(FileName);
                    if (dds.Format != ActiveTexture.Format)
                        DecodeTextureBack = true;

                    SaveAndApplyImage(dds.GetBitmap(), DecodeTextureBack, true);
                }
                else
                {
                    SaveAndApplyImage(new Bitmap(FileName), DecodeTextureBack, false);
                }
            }

            FileWatcher.Filter = "";
            FileWatcher.EnableRaisingEvents = false;
        }

        public void SaveAndApplyImage(Bitmap image, bool DecodeBack, bool isDDS)
        {
            if (image == null)
                return;

            if (Runtime.ImageEditor.PreviewGammaFix && !isDDS) {
                image = BitmapExtension.AdjustGamma(image, 2.2f);
                DecodeBack = true;
            }

            if (saveBtn.InvokeRequired)
            {
                saveBtn.Invoke(new MethodInvoker(
                delegate ()
                {
                    UpdateEditCached(image);
                    ApplyEdit(image);

                    //Update the image with decoding as format could change
                    if (DecodeBack)
                        UpdateImage(ActiveTexture);
                }));
            }
            else
            {
                UpdateEditCached(image);
                ApplyEdit(image);

                //Update the image with decoding as format could change
                if (DecodeBack)
                    UpdateImage(ActiveTexture);
            }

            propertiesEditor.UpdateProperties();
        }

        private void generateMipmapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image Image = pictureBoxCustom1.Image;
            if (Image != null)
            {
                HasBeenEdited = true;

                //Apply edits first to update mip map data
                ApplyEdit(Image);
                UpdateEditCached(Image);
            }
        }

        private void copyImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsFinished)
                return;

            Clipboard.SetImage(pictureBoxCustom1.Image);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsFinished)
                return;

            Clipboard.SetImage(pictureBoxCustom1.Image);
        }

        private void editInExternalProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditInExternalProgram();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentCacheIndex == 0)
                return;

            if (currentCacheIndex == -1)
            {
                currentCacheIndex = ImageCache.Count - 1;
            }
            else
            {
                currentCacheIndex -= 1;
            }

            UpdateEdit(ImageCache[currentCacheIndex]);
            ApplyEdit(ImageCache[currentCacheIndex]);
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentCacheIndex == ImageCache.Count)
                return;

            if (currentCacheIndex == -1)
            {
                currentCacheIndex = ImageCache.Count;
            }
            else
            {
                currentCacheIndex += 1;
            }

            UpdateEdit(ImageCache[currentCacheIndex]);
            ApplyEdit(ImageCache[currentCacheIndex]);
        }

        private void useComponentSelectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (useComponentSelectorToolStripMenuItem.Checked)
                useComponentSelectorToolStripMenuItem.Checked = false;
            else
                useComponentSelectorToolStripMenuItem.Checked = true;

            Runtime.ImageEditor.UseComponetSelector = useComponentSelectorToolStripMenuItem.Checked;
            UpdateMipDisplay();
            SaveSettings();
        }

        private void UpdateAlphaEnable()
        {
            if (Runtime.ImageEditor.DisplayAlpha)
                displayAlphaToolStripMenuItem.Checked = false;
            else
                displayAlphaToolStripMenuItem.Checked = true;

            SetAlphaEnableUI(displayAlphaToolStripMenuItem.Checked);

            Runtime.ImageEditor.DisplayAlpha = displayAlphaToolStripMenuItem.Checked;
            UpdateMipDisplay();
            UpdateLabel();

            SaveSettings();
        }

        private void SetAlphaEnableUI(bool UseAlpha)
        {
            if (UseAlpha)
                alphaBtn.BackgroundImage = Properties.Resources.AlphaIcon;
            else
                alphaBtn.BackgroundImage = Properties.Resources.AlphaIconDisabled;
        }

        private void displayAlphaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateAlphaEnable();
        }

        private void alphaBtn_Click(object sender, EventArgs e)
        {
            UpdateAlphaEnable();
        }

        private void enableZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (enableZoomToolStripMenuItem.Checked)
                enableZoomToolStripMenuItem.Checked = false;
            else
                enableZoomToolStripMenuItem.Checked = true;

            Runtime.ImageEditor.EnableImageZoom = enableZoomToolStripMenuItem.Checked;
            SetZoomSetting();
            SaveSettings();
        }
            
        private void SetZoomSetting()
        {
            if (pictureBoxCustom1.InvokeRequired)
            {
                pictureBoxCustom1.Invoke(new MethodInvoker(
                delegate ()
                {
                    ApplyZoom();
                }));
            }
            else
                ApplyZoom();
        }
        private void ApplyZoom()
        {
            if (Runtime.ImageEditor.EnableImageZoom)
            {
                pictureBoxCustom1.AllowZoom = true;
                pictureBoxCustom1.AllowClickZoom = false;
                pictureBoxCustom1.SizeMode = Cyotek.Windows.Forms.ImageBoxSizeMode.Normal;
            }
            else
            {
                pictureBoxCustom1.AllowZoom = false;
                pictureBoxCustom1.AllowClickZoom = false;
                pictureBoxCustom1.SizeMode = Cyotek.Windows.Forms.ImageBoxSizeMode.Fit;
            }
            SetBottomBar();
        }


        private void SaveSettings()
        {
            Config.Save();
        }

        private void pictureBoxCustom1_ZoomChanged(object sender, EventArgs e) {
            SetBottomBar();
        }

        private void SetBottomBar()
        {
            if (!IsFinished)
                return;

            if (pictureBoxCustom1.Image != null)
            {
                if (Runtime.ImageEditor.EnableImageZoom)
                    bottomLabel.Text = $"Zoom: {pictureBoxCustom1.Zoom}% Image {pictureBoxCustom1.Image.Width} x {pictureBoxCustom1.Image.Height} Data Size: {ActiveTexture.DataSize}";
                else
                    bottomLabel.Text = $"Zoom: 100% Image {pictureBoxCustom1.Image.Width} x {pictureBoxCustom1.Image.Height} Data Size: {ActiveTexture.DataSize}";
            }
        }

        private void fillColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image Image = pictureBoxCustom1.Image;
            if (Image == null)
                return;

            ImageFillColor dialog = new ImageFillColor();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (dialog.ResizeSmall)
                {
                    ActiveTexture.Width = 1;
                    ActiveTexture.Height = 1;
                }
                Bitmap newImage = BitmapExtension.FillColor((int)ActiveTexture.Width, (int)ActiveTexture.Height, dialog.FillColor);

                HasBeenEdited = true;
                UpdateEditCached(newImage);
                ApplyEdit(newImage);
            }
        }

        private void replacRedToolStripMenuItem_Click(object sender, EventArgs e) {
            propertiesEditor.EditChannel(STChannelType.Red);
        }

        private void replaceGreenToolStripMenuItem_Click(object sender, EventArgs e) {
            propertiesEditor.EditChannel(STChannelType.Green);
        }

        private void replaceBlueToolStripMenuItem_Click(object sender, EventArgs e) {
            propertiesEditor.EditChannel(STChannelType.Blue);
        }

        private void replaceAlphaToolStripMenuItem_Click(object sender, EventArgs e) {
            propertiesEditor.EditChannel(STChannelType.Alpha);
        }

        private void previewCubemapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ActiveTexture.IsCubemap)
                return;

            CubeMapFaceViewer viewer = new CubeMapFaceViewer();
            viewer.LoadTexture(ActiveTexture);
            viewer.Show();
        }

        private void previewCubemap3DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CubeMapFaceViewer3D viewer = new CubeMapFaceViewer3D();
            viewer.LoadTexture(ActiveTexture);
            viewer.Show();
        }


        private STColorDialog colorDlg;
        private bool dialogActive = false;
        private void imageBGComboBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (dialogActive)
            {
                colorDlg.Focus();
                return;
            }

            dialogActive = true;
            colorDlg = new STColorDialog(((PictureBox)sender).BackColor);
            colorDlg.AllowTransparency = false;
            colorDlg.FormClosed += delegate
            {
                dialogActive = false;
            };
            colorDlg.ColorChanged += delegate
            {
                ((PictureBox)sender).BackColor = colorDlg.ColorRGB;
                Runtime.CustomPicureBoxBGColor = colorDlg.ColorRGB;
                UpdateBackgroundImage();
            };
            colorDlg.Show();
        }

        private void stPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void gammaFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image Image = pictureBoxCustom1.Image;
            if (Image != null)
                UpdateEditCached(BitmapExtension.AdjustGamma(Image, 2.2f));
        }
    }
}
