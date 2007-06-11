// created on 29/12/2005 at 13:26

using System;
using System.Collections;

using MathTextLibrary;
using MathTextLibrary.Projection;

namespace MathTextLibrary.BitmapSegmenters
{

	/// <summary>
	/// Especializa la clase abstracta <c>ProjectionBitmapSegmenter</c> eliminando los
	/// huecos menores a aquel que se da mas veces, y por tanto agrupando algunas zonas
	/// de la imagen.
	/// </summary>
	public class MostFrequentHoleProjectionBitmapSegmenter: ProjectionBitmapSegmenter
	{
		
		public MostFrequentHoleProjectionBitmapSegmenter(ProjectionMode mode)
			:base(mode){}
			
			
		protected override int GetImageCutThreshold(IList holes)
		{
			int [] histoHoles = CreateHolesHistogram(holes);
			int	i;
			int numMax=0;
			int threshold=0;
			for(i=0;i<histoHoles.Length;i++){
				if(histoHoles[i]>numMax){
					numMax=histoHoles[i];
					threshold=((Hole)holes[i]).Size;
				}			
			}	
			return threshold;	
		
		}
	
	}
}
