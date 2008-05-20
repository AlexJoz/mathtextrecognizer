// ExpressionItemOptionsDialog.cs created with MonoDevelop
// User: luis at 16:06 19/05/2008

using System;
using System.Collections.Generic;

using Gtk;
using Glade;

using MathTextLibrary.Analisys;

using MathTextCustomWidgets.Dialogs;

namespace MathTextRecognizer.SyntacticalRulesManager
{
	
	/// <summary>
	/// This class implements a dialog to show the options of a expression's
	/// items.
	/// </summary>
	public class ExpressionItemOptionsDialog : IExpressionItemContainer
	{
		
#region Glade widgets
		
		[Widget]
		private Dialog expressionItemOptionsDialog = null;
		
		[Widget]
		private ScrolledWindow itemOpRelatedItemsScroller = null;
		
		[Widget]
		private VBox itemOpRelatedItemsBox = null;
		
		[Widget]
		private CheckButton itemOpForceSearchCheck = null;
		
		[Widget]
		private ComboBox itemOpModifierCombo = null;
		
		[Widget]
		private Entry itemOpFormatEntry = null;
		
		[Widget]
		private Alignment itemOpFormatAlignment = null;
		
		[Widget]
		private Frame itemOpRelatedItemsFrame = null;
		
#endregion Glade widgets
		
#region Fields
		
		AddSubItemMenu addItemMenu;
		
#endregion Fields
		
		public ExpressionItemOptionsDialog(Window parent, Type expressionType)
		{
			XML gladeXml = new XML("mathtextrecognizer.glade",
			                       "expressionItemOptionsDialog");
			
			gladeXml.Autoconnect(this);
			
			addItemMenu = new AddSubItemMenu(this);
			
			this.expressionItemOptionsDialog.TransientFor = parent;
			
			InitializeWidgets(expressionType);
		}
		
#region Properties
		
		/// <value>
		/// Contains the number of related items.
		/// </value>
		public int ItemCount 
		{
			get 
			{
				return itemOpRelatedItemsBox.Children.Length;
			}
		}

		/// <summary>
		/// Allows the retrieve the info about a widget's position in 
		/// the related items box.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		public Gtk.Box.BoxChild this[Widget w] 
		{
			get 
			{
				return (Gtk.Box.BoxChild)(itemOpRelatedItemsBox[w]);
			}
		}
		
		/// <value>
		/// Contains the dialog's window.
		/// </value>
		public Window Window
		{
			get
			{
				return this.expressionItemOptionsDialog;
			}
		}
		
		/// <value>
		/// Contains the items options.
		/// </value>
		public ExpressionItemOptions Options
		{
			get
			{
				ExpressionItemOptions options = new ExpressionItemOptions();
				options.ForceTokenSearch = itemOpForceSearchCheck.Active;
				options.Modifier =
					(ExpressionItemModifier)(itemOpModifierCombo.Active);
				
				options.FormatString = itemOpFormatEntry.Text.Trim();
				
				foreach (RelatedItemWidget childWidget in itemOpRelatedItemsBox.Children) 
				{
					ExpressionItem childItem = childWidget.ExpressionItem;
					options.RelatedItems.Add(childItem);
				}
				
				return options;
			}
			
			set
			{
				itemOpForceSearchCheck.Active =  value.ForceTokenSearch;
				itemOpFormatEntry.Text =  value.FormatString;
				
				itemOpModifierCombo.Active = (int)(value.Modifier);	
				
				itemOpFormatEntry.Text = value.FormatString;
				
				foreach (ExpressionItem relatedItem in value.RelatedItems) 
				{
					ExpressionItemWidget relatedWidget = 
						ExpressionItemWidget.CreateWidget(relatedItem, this);
					
					RelatedItemWidget relatedWidgetHolder = 
						this.AddItemAux(relatedWidget);
					
					relatedWidgetHolder.ExpressionItem = relatedItem;
					
					Console.WriteLine(relatedItem.Position);
				}
				
			}
		}
		
#endregion Properties
		
#region Public methods


		
		/// <summary>
		/// Adds an item to the container.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		public void AddItem (ExpressionItemWidget widget)
		{
			RelatedItemWidget relatedItemWidget = 
				new RelatedItemWidget(widget, this);
			
			widget.SetRelatedMode();
						
			this.itemOpRelatedItemsBox.Add(relatedItemWidget);
			
			foreach (RelatedItemWidget relWidget in itemOpRelatedItemsBox.Children) 
			{
				relWidget.CheckPosition();
			}
			
			itemOpFormatAlignment.Sensitive = true;
			
			itemOpRelatedItemsScroller.Vadjustment.Value = 
				itemOpRelatedItemsScroller.Vadjustment.Upper;
		}

		/// <summary>
		/// Removes an item from the container.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		public void RemoveItem (ExpressionItemWidget widget)
		{
			itemOpRelatedItemsBox.Remove(widget);
			
			foreach (RelatedItemWidget relWidget in itemOpRelatedItemsBox.Children) 
			{
				relWidget.CheckPosition();
			}
			
			itemOpFormatAlignment.Sensitive = 
				itemOpRelatedItemsBox.Children.Length > 0;
		}

		/// <summary>
		/// Moves an item towards the beggining of the container.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		public void MoveItemBackwards (ExpressionItemWidget widget)
		{
			int position = this[widget].Position;
			itemOpRelatedItemsBox.ReorderChild(widget, position-1);
			widget.CheckPosition();
			
			((RelatedItemWidget)itemOpRelatedItemsBox.Children[position]).CheckPosition();
		}

		/// <summary>
		/// Moves an item towards the end of the container.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		public void MoveItemFordwards (ExpressionItemWidget widget)
		{
			int position = this[widget].Position;
			itemOpRelatedItemsBox.ReorderChild(widget, position+1);
			widget.CheckPosition();
			
			((RelatedItemWidget)itemOpRelatedItemsBox.Children[position]).CheckPosition();
			
		}
		
		/// <summary>
		/// Shows the dialog and waits for its response.
		/// </summary>
		/// <returns>
		/// A <see cref="ResponseType"/>
		/// </returns>
		public ResponseType Show()
		{
			ResponseType res;
			// We wait for a valid response.
			do
			{
				res = (ResponseType)(this.expressionItemOptionsDialog.Run() );
				
			}
			while(res == ResponseType.None);
			
			return res;
		}
		
		/// <summary>
		/// Hides the dialog, and frees its resources.
		/// </summary>
		public void Destroy()
		{
			expressionItemOptionsDialog.Destroy();
		}

#endregion Public methods
		
#region Non-public methods
		
		/// <summary>
		/// Adds an item to the container.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		private RelatedItemWidget AddItemAux (ExpressionItemWidget widget)
		{
			RelatedItemWidget relatedItemWidget = 
				new RelatedItemWidget(widget, this);
			
			widget.SetRelatedMode();
						
			this.itemOpRelatedItemsBox.Add(relatedItemWidget);
			
			foreach (RelatedItemWidget relWidget in itemOpRelatedItemsBox.Children) 
			{
				relWidget.CheckPosition();
			}
			
			itemOpFormatAlignment.Sensitive = true;
			
			itemOpRelatedItemsScroller.Vadjustment.Value = 
				itemOpRelatedItemsScroller.Vadjustment.Upper;
			
			return relatedItemWidget;
		}
		
		
		private void InitializeWidgets(Type expressionType)
		{
			this.itemOpModifierCombo.Active = 0;
			
			bool isToken = expressionType == typeof(ExpressionTokenWidget);
			
			itemOpForceSearchCheck.Visible = isToken;
			itemOpFormatAlignment.Visible =  isToken;
			itemOpRelatedItemsFrame.Visible =  isToken;
			
			if(isToken)
			{
				this.expressionItemOptionsDialog.WidthRequest = 520;
			}
			
			itemOpRelatedItemsScroller.Vadjustment.ValueChanged+=
				delegate(object sender, EventArgs args)
			{
				itemOpRelatedItemsScroller.QueueDraw();
			};
		
		}
		
		private void OnItemOpAddRelatedBtnClicked(object sender,
		                                          EventArgs args)
		{
			addItemMenu.Popup();
		}
		
		/// <summary>
		/// Validates the dialog, and responds accordignly.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnItemOpOkBtnClicked(object sender, EventArgs args)
		{
			List<string> errors = new List<string>();
			
			if(itemOpRelatedItemsBox.Children.Length > 0)
			{
				if(String.IsNullOrEmpty(itemOpFormatEntry.Text.Trim()))
				{
					errors.Add("· No hay definida cadena de formato para el item y sus elementos relacionados.");
				}
				
				List<string> checkList =  new List<string>();
				// The checking string has to contemplate the token the 
				/// expresions are related to (hence the +1)
				for(int i = 0; i<itemOpRelatedItemsBox.Children.Length + 1; i++)
				{
					checkList.Add("test");
				}
				
				try
				{
					String.Format(itemOpFormatEntry.Text, checkList.ToArray());
				}
				catch(Exception)
				{
					errors.Add("· La cadena de formato para el item no es válida.");
				}
				
				foreach (RelatedItemWidget relatedWidget in 
				         itemOpRelatedItemsBox.Children) 
				{
					errors.AddRange(relatedWidget.CheckErrors());
				}
			}
			
			
			if(errors.Count> 0)
			{
				
				OkDialog.Show(this.expressionItemOptionsDialog,
				              MessageType.Info,
				              "Para continuar, debes solucionar los siguientes errores:\n\n{0}",
				              String.Join("\n", errors.ToArray()));
				
				this.itemOpRelatedItemsScroller.QueueDraw();
				
				this.expressionItemOptionsDialog.Respond(ResponseType.None);
			}
			else
			{
				this.expressionItemOptionsDialog.Respond(ResponseType.Ok);
			}
			
		}
		
#endregion Non-public methods
	}
	
	/// <summary>
	/// This class implements a way for the dialog to encapsulate the item's 
	/// options.
	/// </summary>
	public class ExpressionItemOptions
	{
		public ExpressionItemModifier Modifier;
		public bool ForceTokenSearch;
		public List<ExpressionItem> RelatedItems;
		public string FormatString;	
				
		public ExpressionItemOptions()
		{
			RelatedItems = new List<ExpressionItem>();
		}
		
	}
	

		
}
