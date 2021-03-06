using System;

using MathTextLibrary;
using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de agujeros grandes
	/// es igual a 2.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountNumberOfHolesHelper"/>
	public class NumberBigHolesEqualsTwoCharacteristic:BinaryCharacteristic
	{
		public NumberBigHolesEqualsTwoCharacteristic()
		{
			priority=80;
		}
		
		public override bool Apply(FloatBitmap image)
		{
			return (CountNumberOfHolesHelper.NumWhiteZones(image,true)-1) == 2;	
		}
	}
}
