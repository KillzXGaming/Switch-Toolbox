using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel.Design;

namespace Toolbox.Library.Forms
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public partial class STDropDownPanel : STPanel
    {
        private Size _previousParentSize = Size.Empty;

        public Image SetIcon
        {
            get
            {
                return stCollapsePanelButton1.SetIcon;
            }
            set
            {
                stCollapsePanelButton1.SetIcon = value;
            }
        }

        public Color SetIconColor   
        {
            get
            {
                return stCollapsePanelButton1.SetIconColor;
            }
            set
            {
                stCollapsePanelButton1.SetIconColor = value;
            }
        }

        public Color SetIconAlphaColor
        {
            get
            {
                return stCollapsePanelButton1.SetIconAlphaColor;
            }
            set
            {
                stCollapsePanelButton1.SetIconAlphaColor = value;
            }
        }

        /// <summary>
        /// Height of panel in expanded state
        /// </summary>
        private int _expandedHeight;


        /// <summary>
        /// Height of panel in collapsed state
        /// </summary>
        private readonly int _collapsedHeight;

        public string PanelName
        {
            get
            {
                return stCollapsePanelButton1.PanelName;
            }
            set
            {
                stCollapsePanelButton1.PanelName = value;
            }
        }

        public string PanelValueName
        {
            get
            {
                return stCollapsePanelButton1.PanelValueName;
            }
            set
            {
                stCollapsePanelButton1.PanelValueName = value;
            }
        }

        private InternalPanelState _internalPanelState;

        private enum InternalPanelState
        {
            Normal,
            Expanding,
            Collapsing
        }

        public void AddColorPreview(Color color)
        {
          
        }
        public STDropDownPanel()
        {
            InitializeComponent();

            _collapsedHeight = stCollapsePanelButton1.Location.Y + stCollapsePanelButton1.Size.Height;
           // _collapsedHeight = stCollapsePanelButton1.Location.Y + stCollapsePanelButton1.Size.Height + stCollapsePanelButton1.Margin.Bottom;

            stCollapsePanelButton1.Size = new Size(ClientSize.Width - stCollapsePanelButton1.Margin.Left - stCollapsePanelButton1.Margin.Right,
                         stCollapsePanelButton1.Height);

            stCollapsePanelButton1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            stCollapsePanelButton1.AutoSize = true;

            stCollapsePanelButton1.IsExpanded = true;
            stCollapsePanelButton1.ExpandCollapse += BtnExpandCollapseExpandCollapse;

            stCollapsePanelButton1.SetIconColor = FormThemes.BaseTheme.DropdownButtonBackColor;
        }

        public void ResetColors()
        {
            stCollapsePanelButton1.SetIconColor = FormThemes.BaseTheme.DropdownButtonBackColor;
            stCollapsePanelButton1.SetIconAlphaColor = FormThemes.BaseTheme.DropdownButtonBackColor;
        }

        public bool IsExpanded
        {
            get { return stCollapsePanelButton1.IsExpanded; }
            set
            {
                if (stCollapsePanelButton1.IsExpanded != value)
                    stCollapsePanelButton1.IsExpanded = value;
            }
        }

        public int ExpandedHeight
        {
            get { return _expandedHeight; }
            set
            {
                _expandedHeight = value;
                if (IsExpanded)
                {
                    Height = _expandedHeight;
                }
            }
        }

        public event EventHandler<ExpandCollapseEventArgs> ExpandCollapse;

        private void BtnExpandCollapseExpandCollapse(object sender, ExpandCollapseEventArgs e)
        {
            if (e.IsExpanded) // if button is expanded now
            {
                Expand(); // expand the panel
            }
            else
            {
                Collapse(); // collapse the panel
            }

            // Retrieve expand-collapse state changed event for panel
            EventHandler<ExpandCollapseEventArgs> handler = ExpandCollapse;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void Expand()
        {
            _internalPanelState = InternalPanelState.Normal;
            Size = new Size(Size.Width, _expandedHeight);
        }

        protected virtual void Collapse()
        {
            if (_internalPanelState == InternalPanelState.Normal)
            {
                // store current panel height in expanded state
                _expandedHeight = Size.Height;
            }

            Size = new Size(Size.Width, _collapsedHeight);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            // we always manually scale expand-collapse button for filling the horizontal space of panel:
            stCollapsePanelButton1.Size = new Size(ClientSize.Width - stCollapsePanelButton1.Margin.Left - stCollapsePanelButton1.Margin.Right,
                stCollapsePanelButton1.Height);

            #region Handling panel's Anchor property sets to Bottom when panel collapsed

            if (!IsExpanded // if panel collapsed
                && ((Anchor & AnchorStyles.Bottom) != 0) //and panel's Anchor property sets to Bottom
                && Size.Height != _collapsedHeight // and panel height is changed (it could happens only if parent control just has resized)
                && Parent != null) // and panel has the parent control
            {
                // main, calculate the parent control resize diff and add it to expandedHeight value:
                _expandedHeight += Parent.Height - _previousParentSize.Height;

                // reset resized height (by base.OnSizeChanged anchor.Bottom handling) to collapsedHeight value:
                Size = new Size(Size.Width, _collapsedHeight);
            }

            // store previous size of parent control (however we need only height)
            if (Parent != null)
                _previousParentSize = Parent.Size;
            #endregion
        }
    }
}
