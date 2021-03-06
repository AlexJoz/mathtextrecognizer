// FlowSegmenter.cs created with MonoDevelop
// User: luis at 15:35 07/04/2008

using System;
using System.Collections.Generic;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Projection;

namespace MathTextLibrary.BitmapSegmenters
{
	
	/// <summary>
	/// Implementa un segmentador de imagenes que usa un metodo que permite
	/// separar simbolos que no estan separados por huecos completamente
	/// verticales u horizontales, sino con formas arbitrarias.
	/// </summary>
	public class WaterfallSegmenter : BitmapSegmenter
	{
		private WaterfallSegmenterMode mode;
		private bool reflection;
		private bool search;
		
		/// <summary>
		/// Constructor de <c>WaterfallSegmenter</c>
		/// </summary>
		/// <param name="mode">
		/// A <see cref="WaterfallSegmenterMode"/>
		/// </param>
		/// <param name="search">
		/// true if the start point must be searched.
		/// </param>
		public WaterfallSegmenter(WaterfallSegmenterMode mode,
		                          bool reflection,
		                          bool search)
		{
			this.mode = mode;
			this.reflection = reflection;
			this.search = search;
		}

		/// <summary>
		/// Separa una imagen en los simbolos que la contienen usando el 
		/// metodo en cascada.
		/// </summary>
		/// <param name="mtb">
		/// A <see cref="MathTextBitmap"/>
		/// </param>
		/// <returns>
		/// A <see cref="List`1"/>
		/// </returns>
		public override List<MathTextBitmap> Segment (MathTextBitmap mtb)
		{
			return WaterfallSegment(mtb);
		}
		
		private List<MathTextBitmap> WaterfallSegment(MathTextBitmap bitmap)
		{
			FloatBitmap image = Rotate(bitmap.FloatImage);
			
			if(reflection)
			{
				image = Reflect(image);
			}
			
			
			// Buscamos un pixel blanco en el que empezar.
					
			List<Point> points = new List<Point>();
			
			// Aqui vamos a pintar la linea que divide las dos partes de la imagen.
			FloatBitmap cutImage = new FloatBitmap(image.Width,image.Height);
			
			// Buscamos por filas, desde abajo, para encontrar el primer pixel 
			// negro mas cercano a la esquina (0,0).
			int i,j,k;
			
			if(search)
			
			{
				bool startFound = false;
				for(j=0; j<image.Height && !startFound; j++)
				{
					for(i=0; i<image.Width && !startFound;i++)
					{
						
						if(image[i,j] != FloatBitmap.White)
						{
							for(k=0; k<j ;k++)
							{
								// Añadimos a la lista de puntos los puntos 
								// entre el borde inferior y el punto en la fila
								// inmediatamente inferior a la del punto negro.
								
								points.Add(new Point(i,k));
								cutImage[i,k] = FloatBitmap.Black;
							}						
							startFound = true;
						}					
					}
				}
			}
			else
			{
				points.Add(new Point(image.Width/2, 0));
				cutImage[image.Width/2,0] = FloatBitmap.Black;
			}
			
			
			if(points.Count ==0)
			{
				return new List<MathTextBitmap>();
			}
			
			int x = points[0].X;
			int y = points[0].Y;
				
			bool newPoints = true;
			bool borderFound = false;
			while(newPoints && !borderFound)
			{
				if(x == image.Width -1
				   || y == image.Height -1)
				{					
					borderFound = true;
				}
				else
				{
					// Aqui almacenamos los posibles vectores..
					int [] vectors = new int[]{0,1, 1,1, 1,0,  -1,0, -1,1, -1,-1, 0,-1};
						
					bool notNewFound = true;
					for ( k = 0; k < vectors.Length && notNewFound; k+=2)
					{
						
						int xd = vectors[k];
						int yd = vectors[k+1];
						// Vamos comprobando los casos
						// Tenemos que:
						// · No salirnos de los limites
						// · El pixel ha de ser blanco.
						// . No puede ser el anterior del actual, para no meternos
						//   en un bucle.	
						if(x + xd >= 0
						   && y + yd >= 0
						   && x + xd < image.Width
						   && y + yd < image.Height
						   && image[x+ xd, y+yd] == FloatBitmap.White
						   && !points.Contains(new Point(x +xd, y +yd)))
							
						{
							x = x +xd;
							y = y + yd;
							points.Add(new Point(x,y));
							cutImage[x,y]= FloatBitmap.Black;
							
							// Indicamos que hemos encontrado el valor.
							notNewFound = false;
						}
					}
					
					newPoints = !notNewFound;
					
				}
			}
			
			List<MathTextBitmap> children = new List<MathTextBitmap>();
			
			// Hemos encontrado el borde, cortamos la imagen.
			if(borderFound)
			{
				
				// Primero encontramos un punto blanco.
				bool whiteFound = false;
				int x0 = 0, y0 = 0;
				for(i = 0; i < cutImage.Width && !whiteFound; i++)
				{
					for(j = 0; j< cutImage.Height && !whiteFound; j++)
					{
						if(cutImage[i,j] != FloatBitmap.Black)
						{
							// Mas que el primer blaco, buscamos el primer
							// negro;
							whiteFound = true;
							x0 = i;
							y0 = j;							
						}
					}
				}
				
				if(reflection)
					cutImage = Reflect(cutImage);
				
				// Tenemos que rotar la imagen de corte para alinearla con la orginal.				
				cutImage = UndoRotate(cutImage);

				
				// Rellenamos la imagen de negro a partir del punto encontrado.
				cutImage.Fill(x0, y0, FloatBitmap.Black);
				
#if DEBUG			
				string path = System.IO.Path.GetTempFileName();
				cutImage.CreatePixbuf().Save(String.Format(path+"_{0}_{1}.png", mode, reflection), "png");
#endif				
				
				// Recorremos la imagen de corte, y sacamos dos imagenes;
				FloatBitmap res1 = new FloatBitmap(cutImage.Width, cutImage.Height);
				FloatBitmap res2 = new FloatBitmap(cutImage.Width, cutImage.Height);
				

				
				FloatBitmap origImage = bitmap.FloatImage;
				bool res1HasBlack = false;
				bool res2HasBlack = false;
				
				for(i=0; i < cutImage.Width; i++)
				{
					for(j = 0; j< cutImage.Height; j++)
					{
						// Si estamos en la zona negra de la imagen de corte,
						// copiamos en la primera imagen de resultado,
						// y sino, en la segunda.
						if(cutImage[i, j] == FloatBitmap.Black)
						{
							res1[i,j] = origImage[i,j];
							if (origImage[i,j]!=FloatBitmap.White)
							{
								res1HasBlack = true;
							}
						}
						else
						{
							res2[i,j] = origImage[i,j]; 
							if (origImage[i,j]!=FloatBitmap.White)
							{
								res2HasBlack = true;
							}
						}
					}
				}
				

				
				// Si las dos imágenes tienen pixeles negros, hemos separado la
				// imagen correctamente, sino, solo hemos rodeado algo
				// que no fuimos capaces de segmentar.
				// Si no, no segmenatamos nada, se devolverá la lista vacia.
				if(res1HasBlack && res2HasBlack)
				{
					Gdk.Point pd;
					Gdk.Size sd;					
					GetEdges(res1, out pd, out sd);		
					res1 = res1.SubImage(pd.X,pd.Y,sd.Width, sd.Height);
					children.Add(new MathTextBitmap(res1, 
					                                new Gdk.Point(bitmap.Position.X+pd.X,
					                                              bitmap.Position.Y+pd.Y)));
					
					GetEdges(res2, out pd, out sd);		
					res2 = res2.SubImage(pd.X,pd.Y,sd.Width, sd.Height);
					children.Add(new MathTextBitmap(res2, 
					                                new Gdk.Point(bitmap.Position.X+pd.X,
					                                              bitmap.Position.Y+pd.Y)));

				}	

			}
			
			return children;
			
		}
		
		/// <summary>
		/// Gira la imagen para adecuarla al algoritmo.
		/// </summary>
		/// <param name="bitmap">
		/// La imagen a rotar.
		/// </param>
		/// <returns>
		/// La imagen girada.
		/// </returns>
		private FloatBitmap Rotate(FloatBitmap bitmap)
		{
			FloatBitmap image = null;
			switch(mode)
			{
				
				case(WaterfallSegmenterMode.RightToLeft):
					image = bitmap.Rotate90();
					break;
				case(WaterfallSegmenterMode.TopToBottom):
					image = new FloatBitmap(bitmap);
					break;
				case(WaterfallSegmenterMode.BottomToTop):					
					image = bitmap.Rotate90().Rotate90();
					break;
				default:
					image = bitmap.Rotate90().Rotate90().Rotate90();
					break;
			}
			
			return image;
		
		}
		
		/// <summary>
		/// Reflects an image horizontally.
		/// </summary>
		/// <param name="image">
		/// A <see cref="FloatBitmap"/>
		/// </param>
		/// <returns>
		/// A <see cref="FloatBitmap"/>
		/// </returns>
		private FloatBitmap Reflect(FloatBitmap image)
		{
			FloatBitmap reflected = new FloatBitmap(image.Width, image.Height);
			
			int offset = image.Width -1;
			
			for(int i =0; i < image.Width; i++)
			{
				for(int j=0; j < image.Height; j++)
				{
					reflected[offset - i, j] = image[i,j];
				}
			}
			
			return reflected;
		}
		
		/// <summary>
		/// Deshace la rotacion hecha con el metodo <c>Rotate</c>.
		/// </summary>
		/// <param name="bitmap">
		/// La imagen a rotar.
		/// </param>
		/// <returns>
		/// La imagen girada.
		/// </returns>
		private FloatBitmap UndoRotate(FloatBitmap bitmap)
		{
			FloatBitmap image = null;
			switch(mode)
			{
				
				case(WaterfallSegmenterMode.RightToLeft):
					image = bitmap.Rotate90().Rotate90().Rotate90();
					break;
				case(WaterfallSegmenterMode.TopToBottom):
					image = new FloatBitmap(bitmap);
					break;
				case(WaterfallSegmenterMode.BottomToTop):					
					image = bitmap.Rotate90().Rotate90();
					break;
				default:
					image = bitmap.Rotate90();
					break;
			}
			
			return image;
		}
		
		/// <summary>
		/// THis class implements the point used in the waterfall segmenter.
		/// </summary>
		private class Point
		{
			private int x;
			
			private int y;
			
			public Point(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
			
			public int X
			{
				get
				{
					return x;
				}
			}
			
			public int Y
			{
				get
				{
					return y;
				}
			}
				
			public override bool Equals (object o)
			{
				Point p = (Point) o;
				
				return p.x == this.x && p.y == this.y;
			}

		}
	}
}
