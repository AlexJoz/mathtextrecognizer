// TokenNode.cs created with MonoDevelop
// User: luis at 23:17 06/05/2008

using System;
using System.Collections.Generic;

using Gtk;

using MathTextLibrary.Analisys;

namespace MathTextRecognizer.Controllers.Nodes
{
	
	
	
	
	/// <summary>
	/// This class implements a node for the sequence treeview.
	/// </summary>
	public class SequenceNode : TreeNode
	{
		private TokenSequence sequence;
		private Token token;
		
		private string sequenceLabel;
		private string tokensLabel;
		
		private string nodeName;
		
		private NodeView widget;
		
		public SequenceNode(TokenSequence sequence, NodeView widget)
		{
			this.sequence = sequence;			
			this.sequence.Changed += new EventHandler(OnSequenceChanged);	
			this.widget = widget;
		}
		
#region Properties
		
		/// <value>
		/// Contains the node's name;
		/// </value>
		[TreeNodeValue(Column=0)]
		public string NodeName
		{
			get
			{
				return nodeName;
			}
			set
			{
				nodeName = value;
			}
		}
		/// <value>
		/// Contains the node sequence column text.
		/// </value>
		[TreeNodeValue(Column =1)]
		public string SequenceText
		{
			get
			{
				return sequenceLabel;
			}
			set
			{
				sequenceLabel = value;
			}
		}
	
		/// <value>
		/// Contains the node tokens column text.
		/// </value>
		[TreeNodeValue(Column =2)]
		public string FoundTokenText
		{
			get
			{
				return tokensLabel;
			}
		}

		/// <value>
		/// Contains the tokens assigned to this node's token sequence.
		/// </value>
		public Token FoundToken
		{
			get 
			{
				return token;
			}
			set
			{
				SetToken(value);
			}
		}

		/// <value>
		/// Contains the node's token sequence.
		/// </value>
		public TokenSequence Sequence 
		{
			get 
			{
				return sequence;
			}
		}

			
		
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Adds a child to the node.
		/// </summary>
		/// <param name="childNode">
		/// A <see cref="SequenceNode"/>
		/// </param>
		public void AddChildSequence(SequenceNode childNode)
		{
			AddChildSequenceArgs _args = new AddChildSequenceArgs(childNode);
			Application.Invoke(this, 
			                   _args,
			                   delegate(object sender, EventArgs args)
			{
				AddChildSequenceArgs a = args as AddChildSequenceArgs;
			
				a.ChildNode.NodeName = 
					String.Format("{0}.{1}",this.NodeName,this.ChildCount+1);
				
				this.AddChild(a.ChildNode);
				
				if(widget!=null)
				{
					// We expand the node			
					widget.ExpandAll();
					widget.ColumnsAutosize();
				}
				
			});
		}
		
		
		
		
		
		/// <summary>
		/// Removes the node's children.
		/// </summary>
		public void RemoveSequenceChildren()
		{
			Application.Invoke(delegate(object sender, EventArgs args)
            {
				// We remove all children.
				while(this.ChildCount > 0)
					this.RemoveChild(this[0] as TreeNode);
			});
		}
		
		/// <summary>
		/// Selects the node.
		/// </summary>
		public void Select()
		{
			Application.Invoke(delegate(object sender, EventArgs args)
            {
				if(widget != null)
				{
					widget.NodeSelection.SelectNode(this);			
					TreePath path = widget.Selection.GetSelectedRows()[0];
					widget.ScrollToCell(path,widget.Columns[0],true,1,0);
				}
			});
		}
#endregion Public methods
		
#region Event handlers	
		/// <summary>
		/// Creates the label for the sequence column when the sequence 
		/// is modified.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="_args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnSequenceChanged(object sender, EventArgs _args)
		{
			Application.Invoke(delegate(object resender, EventArgs args)
			{
				SequenceText = sequence.ToString();
			
				if(SequenceText =="")
				{
					// If we have removed the label, we warn the user.
					SequenceText = "Error al reconocer";
				}
				
				if(widget != null)
				{
					// This shouldn't be required.
					widget.Columns[1].QueueResize();
					widget.QueueDraw();
					widget.ColumnsAutosize();
				}
					
			});
		}
	

		
		/// <summary>
		/// Appends a token to the node's token list.
		/// </summary>
		/// <param name="foundToken">
		/// A <see cref="Token"/>
		/// </param>
		private void SetToken(Token foundToken)
		{
			Application.Invoke(this, 
			                   new SetTokenArgs(foundToken),
			                   delegate(object sender, EventArgs args)
			{
				// We append the node to the list, then build the label.
				SetTokenArgs a = args as SetTokenArgs;
				
				token = a.FoundToken;
				
				if(token == null)
				{
					tokensLabel = "";
				}
				else 
				{
					tokensLabel = token.Type;
				}
				
				if(widget != null)
					widget.ColumnsAutosize();
			});
		}
		
	
		
#endregion Event handlers
	}
	
	/// <summary>
	/// Implements a helper class so we can pass the child node to the
	/// delegate adding childs in the application's thread.
	/// </summary>
	class AddChildSequenceArgs : EventArgs
	{
		private SequenceNode childNode;
		public AddChildSequenceArgs(SequenceNode childNode)
		{
			this.childNode= childNode;
		}
		
		/// <summary>
		/// Contains the node to be added.
		/// </summary>
		public SequenceNode ChildNode
		{
			get
			{
				return childNode;
			}
		}
	}
	
	/// <summary>
	/// Implements a helper class so we can pass the found token
	/// so its added in the app's main thread.
	/// </summary>
	class SetTokenArgs : EventArgs
	{
		private Token foundToken;
		public SetTokenArgs(Token foundToken)
		{
			this.foundToken= foundToken;
		}
		
		/// <summary>
		/// Contains the node to be added.
		/// </summary>
		public Token FoundToken
		{
			get
			{
				return foundToken;
			}
		}
	}
}
