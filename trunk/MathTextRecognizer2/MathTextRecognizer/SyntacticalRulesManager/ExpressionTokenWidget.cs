// ExpressionItemWIdget.cs created with MonoDevelop
// User: luis at 17:52 17/05/2008

using System;
using System.Collections.Generic;

using Gtk;
using Glade;

using MathTextLibrary.Analisys;

namespace MathTextRecognizer.SyntacticalRulesManager
{
	
	/// <summary>
	/// This class implements a widget to set a token expression item.
	/// </summary>
	public class ExpressionTokenWidget : ExpressionItemWidget
	{
		
#region Glade widgets
		
		[Widget]
		private Entry expTokenTypeEntry = null;
		
		[Widget]
		private HBox expressionTokenWidgetBase = null;
		
		[Widget]
		private Button expTokenNextBtn = null;
		
		[Widget]
		private Button expTokenPreviousBtn = null;
		
		[Widget]
		private VSeparator expTokenSeparator = null;
		
		
#endregion Glade widgets
		
#region Fields
		
#endregion Fields
		
		/// <summary>
		/// <see cref="ExpressionTokenWidget"/>'s constructor.
		/// </summary>
		/// <param name="container">
		/// A <see cref="IExpressionItemContainer"/>
		/// </param>
		public ExpressionTokenWidget(IExpressionItemContainer container) 
			: base(container)
		{
			Glade.XML gladeXml = new XML("mathtextrecognizer.glade",
			                             "expressionTokenWidgetBase");
			
			gladeXml.Autoconnect(this);
			
			this.Add(expressionTokenWidgetBase);
			
			this.HeightRequest = expressionTokenWidgetBase.HeightRequest;
			
			this.ShowAll();
		}
		
#region Properties
		/// <value>
		/// Contains the widget's item.
		/// </value>
		public override ExpressionItem ExpressionItem 
		{
			get 
			{
				ExpressionTokenItem res = new ExpressionTokenItem();
				res.TokenType = expTokenTypeEntry.Text.Trim();
				
				return res;
			}
			set 
			{
				if(value.GetType() != typeof(ExpressionTokenItem))
				{
					throw new ArgumentException("The type of the value wasn't ExpressionTokenItem");
				}
				
				expTokenTypeEntry.Text = (value as ExpressionTokenItem).TokenType;
			}
		}
		
		
#endregion Properties	

#region Public methods
		
		/// <summary>
		/// Checks the item's position, and sets some controls consecuently.
		/// </summary>
		/// <param name="position">
		/// A <see cref="System.Int32"/>
		/// </param>
		public override void CheckPosition()
		{
			int position = container[this].Position;
			expTokenNextBtn.Sensitive = position < container.ItemCount -1;
			expTokenSeparator.Visible = position < container.ItemCount -1;
			
			expTokenPreviousBtn.Sensitive =  position > 0;
		}
		
#endregion Public methods
		
#region Non-public methods
		
		/// <summary>
		/// Removes the expression token widget from its container.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		protected void OnExpTokenRmBtnClicked(object sender, EventArgs args)
		{
			container.RemoveItem(this);
		}
		
		private void OnExpTokenNextBtnClicked(object sender, EventArgs args)
		{
			this.MoveFordwards();
		}
		
		private void OnExpTokenPreviousBtnClicked(object sender, EventArgs args)
		{
			this.MoveBackwards();
		}
		
#endregion Non-public methods
		
	}
}
