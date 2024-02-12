// BSD 3-Clause License
//
// Copyright (c) 2022, Arm Limited
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice, this
//    list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//
// 3. Neither the name of the copyright holder nor the names of its
//    contributors may be used to endorse or promote products derived from
//    this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WindowsPerfGUI.Components.TreeListView
{
    public class TreeList : ListView
    {
        #region Properties

        /// <summary>
        /// Internal collection of rows representing visible nodes, actually displayed in the ListView
        /// </summary>
        internal ObservableCollectionTreeList<TreeNode> Rows { get; private set; }


        private ITreeModel _model;

        public ITreeModel Model
        {
            get { return _model; }
            set
            {
                if (_model != value)
                {
                    _model = value;
                    _root.Children.Clear();
                    Rows.Clear();
                    CreateChildrenNodes(_root);
                }
            }
        }

        public void UpdateTreeList()
        {
            _root.Children?.Clear();
            Rows.Clear();
            CreateChildrenNodes(_root);
        }

        public void ClearTreeList()
        {
            _root?.Children?.Clear();
            Rows?.Clear();
            Model = null;
        }

        private TreeNode _root;

        internal TreeNode Root
        {
            get { return _root; }
        }

        public ReadOnlyCollection<TreeNode> Nodes
        {
            get { return Root.Nodes; }
        }

        internal TreeNode PendingFocusNode { get; set; }

        public ICollection<TreeNode> SelectedNodes
        {
            get { return SelectedItems.Cast<TreeNode>().ToArray(); }
        }

        public TreeNode SelectedNode
        {
            get
            {
                if (SelectedItems.Count > 0)
                    return SelectedItems[0] as TreeNode;
                else
                    return null;
            }
        }

        #endregion

        public TreeList()
        {
            Rows = new ObservableCollectionTreeList<TreeNode>();
            _root = new TreeNode(this, null);
            _root.IsExpanded = true;
            ItemsSource = Rows;
            ItemContainerGenerator.StatusChanged += ItemContainerGeneratorStatusChanged;
        }

        void ItemContainerGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated && PendingFocusNode != null)
            {
                var item = ItemContainerGenerator.ContainerFromItem(PendingFocusNode) as TreeListItem;
                if (item != null)
                    item.Focus();
                PendingFocusNode = null;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListItem;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var ti = element as TreeListItem;
            var node = item as TreeNode;
            if (ti != null && node != null)
            {
                ti.Node = item as TreeNode;
                base.PrepareContainerForItemOverride(element, node.Tag);
            }
        }

        internal void SetIsExpanded(TreeNode node, bool value)
        {
            if (value)
            {
                if (!node.IsExpandedOnce)
                {
                    node.IsExpandedOnce = true;
                    node.AssignIsExpanded(value);
                    CreateChildrenNodes(node);
                }
                else
                {
                    node.AssignIsExpanded(value);
                    CreateChildrenRows(node);
                }
            }
            else
            {
                DropChildrenRows(node, false);
                node.AssignIsExpanded(value);
            }
        }

        internal void CreateChildrenNodes(TreeNode node)
        {
            var children = GetChildren(node);
            if (children != null)
            {
                int rowIndex = Rows.IndexOf(node);
                node.ChildrenSource = children as INotifyCollectionChanged;
                foreach (object obj in children)
                {
                    TreeNode child = new TreeNode(this, obj);
                    child.HasChildren = HasChildren(child);
                    node.Children.Add(child);
                }

                Rows.InsertRange(rowIndex + 1, node.Children.ToArray());
            }
        }

        private void CreateChildrenRows(TreeNode node)
        {
            int index = Rows.IndexOf(node);
            if (index >= 0 || node == _root) // ignore invisible nodes
            {
                var nodes = node.AllVisibleChildren.ToArray();
                Rows.InsertRange(index + 1, nodes);
            }
        }

        internal void DropChildrenRows(TreeNode node, bool removeParent)
        {
            int start = Rows.IndexOf(node);
            if (start >= 0 || node == _root) // ignore invisible nodes
            {
                int count = node.VisibleChildrenCount;
                if (removeParent)
                    count++;
                else
                    start++;
                Rows.RemoveRange(start, count);
            }
        }

        private IEnumerable GetChildren(TreeNode parent)
        {
            if (Model != null)
                return Model.GetChildren(parent.Tag);
            else
                return null;
        }

        private bool HasChildren(TreeNode parent)
        {
            if (parent == Root)
                return true;
            else if (Model != null)
                return Model.HasChildren(parent.Tag);
            else
                return false;
        }

        internal void InsertNewNode(TreeNode parent, object tag, int rowIndex, int index)
        {
            TreeNode node = new TreeNode(this, tag);
            if (index >= 0 && index < parent.Children.Count)
                parent.Children.Insert(index, node);
            else
            {
                index = parent.Children.Count;
                parent.Children.Add(node);
            }

            Rows.Insert(rowIndex + index + 1, node);
        }
    }
}