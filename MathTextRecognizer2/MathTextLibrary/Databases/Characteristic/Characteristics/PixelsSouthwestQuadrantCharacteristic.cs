using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de pixeles negros en el
	/// cuadrante suroeste es mayor al numero de pixeles negros en cada uno
	/// de los demas cuadrantes.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountPixelsHelper"/>
	public class PixelsSouthwestQuadrantCharacteristic:BinaryCharacteristic
	{
		private const float epsilon = 0.01f;
		
		public PixelsSouthwestQuadrantCharacteristic()
		{
			priority=150;
		}

		public override bool Apply(FloatBitmap image)
		{
			int npixelsNW=CountPixelsHelper.NumBlackPixelsInQuadrant(image, Quadrant.NW);
			int npixelsNE=CountPixelsHelper.NumBlackPixelsInQuadrant(image, Quadrant.NE);
			int npixelsSW=CountPixelsHelper.NumBlackPixelsInQuadrant(image, Quadrant.SW);
			int npixelsSE=CountPixelsHelper.NumBlackPixelsInQuadrant(image, Quadrant.SE);
			
			int tolerance = (int)((image.Width * image.Height)*epsilon);
			
			return (npixelsSW>npixelsNE + tolerance) 
				&& (npixelsSW>npixelsNW + tolerance)
				&& (npixelsSW>npixelsSE + tolerance);
		}
	}
}
