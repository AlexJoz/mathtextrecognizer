// ExpressionRuleCallWidget.cs created with MonoDevelop
// User: luis at 11:15 18/05/2008

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
	public class ExpressionRuleCallWidget : ExpressionItemWidget
	{
		
#region Glade widgets
		
		[Widget]
		private Entry expRuleNameEntry = null;
		
		[Widget]
		private HBox expressionRuleWidgetBase = null;
		
		[Widget]
		private Button expRuleNextBtn = null;
		
		[Widget]
		private Button expRulePreviousBtn = null;
		
		[Widget]
		private VSeparator expRuleSeparator = null;
		
		[Widget]
		private VBox expRuleButtonsBox = null;
		
#endregion Glade widgets
		
#region Fields
		
#endregion Fields
		
#region Constructors
		/// <summary>
		/// <see cref="ExpressionRuleCallWidget"/>'s constructor.
		/// </summary>
		/// <param name="container">
		/// A <see cref="IExpressionItemContainer"/>
		/// </param>
		public ExpressionRuleCallWidget(IExpressionItemContainer container) 
			: base(container)
		{
			Glade.XML gladeXml = new XML("mathtextrecognizer.glade",
			                             "expressionRuleWidgetBase");
			
			gladeXml.Autoconnect(this);
			
			this.Add(expressionRuleWidgetBase);
			
			this.HeightRequest = expressionRuleWidgetBase.HeightRequest;
			
			this.ShowAll();
		}
		
#endregion Constructors
		
#region Properties
		/// <value>
		/// Contains the widget's item.
		/// </value>
		public override ExpressionItem ExpressionItem 
		{
			get 
			{
				ExpressionRuleCallItem res = new ExpressionRuleCallItem();
				res.RuleName = expRuleNameEntry.Text.Trim();
				
				res.FormatString = options.FormatString;
				res.Modifier = options.Modifier;
				
				return res;
			}
			set 
			{
				if(value.GetType() != typeof(ExpressionRuleCallItem))
				{
					throw new ArgumentException("The type of the value wasn't ExpressionRuleCallItem");
				}
				
				options.Modifier = value.Modifier;
				options.FormatString = value.FormatString;
				
				expRuleNameEntry.Text = (value as ExpressionRuleCallItem).RuleName;
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
			expRuleNextBtn.Sensitive = position < container.ItemCount -1;
			expRuleSeparator.Visible = position < container.ItemCount -1;
			
			expRulePreviousBtn.Sensitive =  position > 0;
		}
		
		/// <summary>
		/// Sets the widget in a mode suitable to be shown inside 
		/// a <see cref="RelatedItemWidget"/> object.
		/// </summary>
		public override void SetRelatedMode ()
		{
			expRuleButtonsBox.Visible = false;
			expRuleSeparator.Visible = false;
		}
		
		/// <summary>
		/// Checks the rule call for errors.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> describing the found errors.
		/// </returns>
		public override List<string> CheckErrors ()
		{
			List<string> errors = new List<string>();
			
			int position = this.Position;
			
			if(String.IsNullOrEmpty(expRuleNameEntry.Text.Trim()))
			{
				errors.Add(String.Format("· El item en la posición {0} no contiene un nombre de regla. ",
				                         position));
					
			}
			
			return errors;
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
		protected void OnExpRuleRmBtnClicked(object sender, EventArgs args)
		{
			Remove();
		}
		
		/// <summary>
		/// Moves the widget fordwards.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		protected void OnExpRuleNextBtnClicked(object sender, EventArgs args)
		{
			this.MoveFordwards();
		}
		
		/// <summary>
		/// Moves the widget backwards.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		protected void OnExpRulePreviousBtnClicked(object sender, EventArgs args)
		{
			this.MoveBackwards();
		}
		
		/// <summary>
		/// Shows the option dialog for the associated item.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		protected void OnExpRuleOptionsBtnClicked(object sender, EventArgs args)
		{
			this.ShowOptions();
		}
		
#endregion Non-public methods
		
	}
}
