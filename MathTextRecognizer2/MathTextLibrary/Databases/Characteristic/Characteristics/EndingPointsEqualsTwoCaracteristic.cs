using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de puntos finales (aquellos
	/// con solo un vecino) es igual a 2.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountPixelsWithXNeighboursHelper"/>
	public class EndingPointsEqualsTwoCharacteristic:IBinaryCharacteristic
	{
		public EndingPointsEqualsTwoCharacteristic()
		{
			priority=260;
		}
		
		public override bool Apply(MathTextBitmap image){
			return CountPixelsWithXNeighboursHelper.numPixelsXNeighbours(image, 1)==2;
		}	
	}
}
