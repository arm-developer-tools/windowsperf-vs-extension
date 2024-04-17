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

using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace WindowsPerfGUI.Components.TreeListView
{
    public class TreeListItem : ListViewItem, INotifyPropertyChanged
    {
        #region Properties

        private TreeNode _node;

        public TreeNode Node
        {
            get { return _node; }
            internal set
            {
                _node = value;
                OnPropertyChanged("Node");
            }
        }

        #endregion

        public TreeListItem() { }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (Node != null && Node.IsExpandable)
            {
                Node.IsExpanded = !Node.IsExpanded;
            }

            base.OnMouseDoubleClick(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Node == null)
            {
                base.OnKeyDown(e);
                return;
            }

            switch (e.Key)
            {
                case Key.Right:
                    e.Handled = true;
                    if (!Node.IsExpanded)
                    {
                        Node.IsExpanded = true;
                        ChangeFocus(Node);
                    }
                    else if (Node.Children.Count > 0)
                        ChangeFocus(Node.Children[0]);

                    break;

                case Key.Left:

                    e.Handled = true;
                    if (Node.IsExpanded && Node.IsExpandable)
                    {
                        Node.IsExpanded = false;
                        ChangeFocus(Node);
                    }
                    else
                        ChangeFocus(Node.Parent);

                    break;

                case Key.Subtract:
                    e.Handled = true;
                    Node.IsExpanded = false;
                    ChangeFocus(Node);
                    break;

                case Key.Add:
                    e.Handled = true;
                    Node.IsExpanded = true;
                    ChangeFocus(Node);
                    break;
                default:
                    base.OnKeyDown(e);
                    return;
            }
        }

        private void ChangeFocus(TreeNode node)
        {
            var tree = node.Tree;
            if (tree == null)
            {
                return;
            }

            TreeListItem item = (TreeListItem)tree.ItemContainerGenerator.ContainerFromItem(node);
            if (item != null)
                item.Focus();
            else
                tree.PendingFocusNode = node;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
