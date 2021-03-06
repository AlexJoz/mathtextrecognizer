using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de puntos de arbol (aquellos
	/// con tres o mas vecinos) es igual a 2.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountPixelsWithXNeighboursHelper"/>
	public class TreePointsEqualsTwoCharacteristic:BinaryCharacteristic
	{
		public TreePointsEqualsTwoCharacteristic()
		{
			priority=300;
		}
				
		public override bool Apply(FloatBitmap image)
		{
			return CountPixelsWithXNeighboursHelper.CountPixelsXOrMoreNeighbours(image, 3)==2;
		}
	}
}
