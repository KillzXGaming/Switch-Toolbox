using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library.NodeWrappers;
using Bfres.Structs;
using Toolbox.Library.Animations;

namespace FirstPlugin.Forms
{
    public partial class SubFileEditor : STUserControl
    {
        public SubFileEditor()
        {
            InitializeComponent();

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;
            stTabControl1.Dock = DockStyle.Fill;
        }

        FMDL activeModel;
        FSKA activeFska;
        FMAA activeFmaa;
        FSHU activeFshu;
        FSHA activeFsha;
        FSCN activeFscn;
        FVIS activeFvis;
        FTXP activeFtxp;

        public void LoadSubFile<T>(object node) where T : STGenericWrapper
        {
            if (typeof(T) == typeof(FMDL)) LoadSubFile((FMDL)node);
            if (typeof(T) == typeof(FSKA)) LoadSubFile((FSKA)node);
            if (typeof(T) == typeof(FSHU)) LoadSubFile((FSHU)node);
            if (typeof(T) == typeof(FSHA)) LoadSubFile((FSHA)node);
            if (typeof(T) == typeof(FSCN)) LoadSubFile((FSCN)node);
            if (typeof(T) == typeof(FVIS)) LoadSubFile((FVIS)node);
            if (typeof(T) == typeof(FTXP)) LoadSubFile((FTXP)node);
            if (typeof(T) == typeof(FMAA)) LoadSubFile((FMAA)node);
        }

        public void LoadSubFile(FMDL fmdl)
        {
            activeModel = fmdl;

            if (fmdl.ModelU != null) {
                stPropertyGrid1.LoadProperty(fmdl.ModelU, OnPropertyChanged);
                userDataEditor1.LoadUserData(fmdl.ModelU.UserData);
            }
            else
            {
                userDataEditor1.LoadUserData(fmdl.Model.UserData);
                stPropertyGrid1.LoadProperty(fmdl.Model, OnPropertyChanged);
            }
        }
        public void LoadSubFile(FSKA anim)
        {
            activeFska = anim;


            if (anim.SkeletalAnimU != null)
            {
                stPropertyGrid1.LoadProperty(anim.SkeletalAnimU, OnPropertyChanged);
                userDataEditor1.LoadUserData(anim.SkeletalAnimU.UserData);
            }
            else
            {
                stPropertyGrid1.LoadProperty(anim.SkeletalAnim, OnPropertyChanged);
                userDataEditor1.LoadUserData(anim.SkeletalAnim.UserDatas);
            }
        }
        public void LoadSubFile(FMAA anim)
        {
            activeFmaa = anim;

            stPropertyGrid1.LoadProperty(anim.MaterialAnim, OnPropertyChanged);
            userDataEditor1.LoadUserData(anim.MaterialAnim.UserData);

            if (anim.AnimType == MaterialAnimation.AnimationType.TexturePattern)
            {

            }
            else 
            {
                AnimParamEditor editor = (AnimParamEditor)GetActiveControl(typeof(AnimParamEditor));
                if (editor == null)
                {
                    stPanel2.Controls.Clear();

                    editor = new AnimParamEditor();
                    editor.Dock = DockStyle.Fill;
                    stPanel2.Controls.Add(editor);
                }

                editor.LoadAnim(anim);
                editor.Refresh();
            }
        }

        private Control GetActiveControl(Type type = null)
        {
            foreach (Control control in stPanel2.Controls)
            {
                if (type == null)
                    return control;

                if (control.GetType() == type)
                    return control;
            }

            return null;
        }
        public void LoadSubFile(FSHU anim)
        {
            activeFshu = anim;

            stPropertyGrid1.LoadProperty(anim.ShaderParamAnim, OnPropertyChanged);
            userDataEditor1.LoadUserData(anim.ShaderParamAnim.UserData);

            AnimParamEditor editor = (AnimParamEditor)GetActiveControl(typeof(AnimParamEditor));
            if (editor == null)
            {
                stPanel2.Controls.Clear();

                editor = new AnimParamEditor();
                editor.Dock = DockStyle.Fill;
                stPanel2.Controls.Add(editor);
            }

            editor.LoadAnim(anim);
            editor.Refresh();
        }
        public void LoadSubFile(FSHA anim)
        {
            activeFsha = anim;

            if (anim.ShapeAnimU != null)
            {
                stPropertyGrid1.LoadProperty(anim.ShapeAnimU, OnPropertyChanged);
                userDataEditor1.LoadUserData(anim.ShapeAnimU.UserData);
            }
            else
            {
                stPropertyGrid1.LoadProperty(anim.ShapeAnim, OnPropertyChanged);
                userDataEditor1.LoadUserData(anim.ShapeAnim.UserData);
            }
        }
        public void LoadSubFile(FSCN anim)
        {
            activeFscn = anim;

            if (anim.SceneAnimU != null)
            {
                stPropertyGrid1.LoadProperty(anim.SceneAnimU, OnPropertyChanged);
                userDataEditor1.LoadUserData(anim.SceneAnimU.UserData);
            }
            else
            {
                stPropertyGrid1.LoadProperty(anim.SceneAnim, OnPropertyChanged);
                userDataEditor1.LoadUserData(anim.SceneAnim.UserData);
            }
        }
        public void LoadSubFile(FVIS anim)
        {
            activeFvis = anim;

            if (anim.VisibilityAnimU != null)
            {
                stPropertyGrid1.LoadProperty(anim.VisibilityAnimU, OnPropertyChanged);
                userDataEditor1.LoadUserData(anim.VisibilityAnimU.UserData);
            }
            else
            {
                stPropertyGrid1.LoadProperty(anim.VisibilityAnim, OnPropertyChanged);
                userDataEditor1.LoadUserData(anim.VisibilityAnim.UserData);
            }

            BoneVisualAnimEditor editor = (BoneVisualAnimEditor)GetActiveControl(typeof(BoneVisualAnimEditor));
            if (editor == null)
            {
                stPanel2.Controls.Clear();

                editor = new BoneVisualAnimEditor();
                editor.Dock = DockStyle.Fill;
                stPanel2.Controls.Add(editor);
            }

            editor.LoadVisualAnim(anim);
            editor.Refresh();
        }
        public void LoadSubFile(FTXP anim)
        {
            activeFtxp = anim;

            stPropertyGrid1.LoadProperty(anim.TexPatternAnim, OnPropertyChanged);
            userDataEditor1.LoadUserData(anim.TexPatternAnim.UserData);
        }

        public void OnPropertyChanged()
        {
            if (activeModel != null)
            {
                if (activeModel.Model != null)
                    activeModel.Text = activeModel.Model.Name;
                else
                    activeModel.Text = activeModel.ModelU.Name;
            }

            if (activeFska != null)
            {
                if (activeFska.SkeletalAnim != null)
                {
                    activeFska.Text = activeFska.SkeletalAnim.Name;
                    activeFska.FrameCount = activeFska.SkeletalAnim.FrameCount;
                }
                else
                {
                    activeFska.Text = activeFska.SkeletalAnimU.Name;
                    activeFska.FrameCount = activeFska.SkeletalAnimU.FrameCount;
                }
            }

            if (activeFvis != null)
            {
                BoneVisualAnimEditor editor = (BoneVisualAnimEditor)GetActiveControl(typeof(BoneVisualAnimEditor));
                if (editor != null)
                    editor.UpdateDataGrid();
            }
        }
    }
}
