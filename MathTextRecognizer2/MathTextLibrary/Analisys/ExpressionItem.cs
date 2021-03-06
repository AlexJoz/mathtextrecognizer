// SyntacticExpressionItem.cs created with MonoDevelop
// User: luis at 22:58 12/05/2008

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MathTextLibrary.Analisys
{
	
	/// <summary>
	/// This enumeration contains the possible kinds of appearance modifiers
	/// applicable to <see cref="ExpressionItem"/> instances.
	/// </summary>
	public enum ExpressionItemModifier
	{
		/// <summary>
		/// There is no modifier.
		/// </summary>
		None,
		/// <summary>
		/// The item is not required to be present.
		/// </summary>
		NonCompulsory,
		/// <summary>
		/// The item will appear one or more times.
		/// </summary>
		Repeating,
		
		/// <summary>
		/// The item may appear one or more times, or not appear even one time.
		/// </summary>
		RepeatingNonCompulsory
		
	}
	
		
	/// <summary>
	/// This enumerate details the positions in which an 
	/// <see cref="ExpresssionItem"/> can be placed with respect to
	/// another.
	/// </summary>
	public enum ExpressionItemPosition
	{
		/// <summary>
		/// The position is not applicable to the item.
		/// </summary>
		None,
		/// <summary>
		/// The item is placed above the related one.
		/// </summary>
		Above,
		
		/// <summary>
		/// The item will be placed below the related one.
		/// </summary>
		Below,
		
		/// <summary>
		/// The item is a super-index of the related one.
		/// </summary>
		SuperIndex,
		
		/// <summary>
		/// The item is a sub-index of the related one.
		/// </summary>
		SubIndex,
		
		/// <summary>
		/// The item is the index of a root-like symbol item.
		/// </summary>
		RootIndex,
		
		/// <summary>
		/// The expression is inside the related expression.
		/// </summary>
		Inside
	}
	
	/// <summary>
	/// This class is the base for all kinds of expression items.
	/// </summary>
	[XmlInclude(typeof(ExpressionItemModifier))]
	[XmlInclude(typeof(ExpressionItemPosition))]
	public abstract class ExpressionItem : SyntacticalMatcher
	{
		private ExpressionItemPosition position;
		
		protected string formatString;		
		
		private ExpressionItemModifier modifier;
		
		
		/// <summary>
		/// <see cref="ExpressionItem"/>'s constructor
		/// </summary>
		public ExpressionItem()
		{
			formatString = "{0}";
		}

		
		
#region Properties
		
	
		
		/// <value>
		/// Contains a value indicating if the item is compulsory.
		/// </value>
		[XmlIgnore]
		public bool IsCompulsory
		{
			get
			{
				return (modifier == ExpressionItemModifier.None 
				        || modifier == ExpressionItemModifier.Repeating);
			}
		}
		
		/// <value>
		/// The position of the item with respect the related item position.
		/// </value>
		public ExpressionItemPosition Position 
		{
			get 
			{
				return position;
			}
			set 
			{
				position = value;
			}
		}

		

		/// <value>
		/// The item's appearance modifier.
		/// </value>
		public ExpressionItemModifier Modifier 
		{
			get 
			{
				return modifier;
			}
			set 
			{
				modifier = value;
			}
		}
		
		/// <value>
		/// Contains the string used to format the group's matched contents.
		/// </value>
		public string FormatString 
		{
			get 
			{
				return formatString;
			}
			set 
			{
				formatString = value;
			}
		}
		
#endregion Properties
		
#region Public methods
		
	
		public override bool Match (ref TokenSequence sequence, out string res)
		{
			MatchingInvoker();
			
			LogSentInvoker("Se intentará concordar el elemento «"+this.Label+"»");
			
			bool result = true;
			res = "";
			int counter =0;		
			int initialCount =  sequence.Count;
			string auxOutput;
			switch(modifier)				
			{
				case ExpressionItemModifier.Repeating:
					while(sequence.Count > 0 
					      && this.MatchSequence(ref sequence, out auxOutput))
					{
						counter++;
						res +=auxOutput;
					}		
					if(counter == 0)
					{
						result =  false;
					}
										
					break;
				case ExpressionItemModifier.RepeatingNonCompulsory:
				
					while(sequence.Count > 0)
					{
						initialCount = sequence.Count;
						if(this.MatchSequence(ref sequence, out auxOutput))
						{
							counter++;
							res+= auxOutput;
						}
						else if (initialCount != sequence.Count)
						{
							result = false;
							break;
						}					
						else
						{
							break;
						}
					}
					
					break;
				case ExpressionItemModifier.NonCompulsory:
				
					if(sequence.Count>0)
					{
						result = this.MatchSequence(ref sequence, out auxOutput);
						res= auxOutput;
					}
					
					break;
				
				default:
					if(sequence.Count>0
					   && this.MatchSequence(ref sequence, out auxOutput))
					{
						res= auxOutput;
					}
					else
					{
						result = false;
					}
					break;
			}
			
			if(result)
			{
				LogSentInvoker("Se logró concordar exitosamente «"
				               +this.Label+
				               "», se sube al elemento padre.");
			}
			else
			{
				LogSentInvoker("No se pudo concordar «"+ 
				               this.Label+
				               "», se sube al elemento padre.");
			}
			
			MatchingFinishedInvoker(res);
			
			
			return result;
		}
		
		public override string ToString ()
		{
			String res ="";
			switch(this.Position)
			{
				case ExpressionItemPosition.Above:
					res+="↑";
					break;
				case ExpressionItemPosition.Below:
					res+="↓";
					break;
				case ExpressionItemPosition.Inside:
					res+="↶";
					break;
				case ExpressionItemPosition.RootIndex:
					res+="↖";
					break;						
				case ExpressionItemPosition.SubIndex:
					res+="↘";
					break;						
				case ExpressionItemPosition.SuperIndex:
					res+="↗";
					break;
					
			}	
			
			res += this.SpecificToString();
			
			switch(modifier)
			{
				case ExpressionItemModifier.NonCompulsory:
					res+="?";
					break;
				case ExpressionItemModifier.Repeating:
					res+="+";
					break;
				case ExpressionItemModifier.RepeatingNonCompulsory:
					res+="*";
					break;
			}
			
		
			
			return res;
		}

		
#endregion Public methods
		
#region Non-public methods
		
		protected abstract bool MatchSequence(ref TokenSequence sequence, out string output);
				      
		
		protected abstract string SpecificToString();
		
#endregion Non-public methods
		

	}
	
}

