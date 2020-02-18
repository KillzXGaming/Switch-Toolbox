using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Toolbox.Library;

namespace Toolbox.Library.Forms
{
	public class MultiselectTreeView : TreeView
	{
		#region Public Properties

		private List<TreeNode> m_SelectedNodes = null;
		public List<TreeNode> SelectedNodes
		{
			get
			{
				return new List<TreeNode>() { SelectedNode };
			}
			set
			{
				//ClearSelectedNodes();
				if (value != null)
				{
					foreach (TreeNode node in value)
					{
						//ToggleNode(node, true);
					}
				}
			}
		}

		#endregion

		public MultiselectTreeView()
		{
			m_SelectedNodes = new List<TreeNode>();
			base.SelectedNode = null;
		}
	}
}
