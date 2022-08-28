using MyCanvas.Computing;
using MyCanvas.Configuration;
using Savers;

namespace MyCanvas
{
	public class Canvas
	{
		private ImageSet _imageSet;
		private Image _defaultImage;
		private PictureSaver _pictureSaver;

		public Figure ControlFigure { get; private set; }

		public Canvas(bool needsDefault = true, Image image = null)
        {
			_imageSet = new ImageSet();
			_pictureSaver = new PictureSaver();

			if (needsDefault == true)
				_defaultImage = CreateImage(new Point(1, 1));
			else if (needsDefault == false && image != null)
				_defaultImage = image;
        }

		public void SetControlFor(Figure figure)
        {
			ControlFigure = figure;
        }

		public void AddOnImage(Image image = null, params Figure[] figures)
        {
			Image onImage = image;

			if (image == null)
				onImage = _defaultImage;

			onImage.AddFigures(figures);
        }

		public void RemoveFromImage(Image image = null, params Figure[] figures)
        {
			Image offImage = image;

			if (image == null)
				offImage = _defaultImage;

			offImage.RemoveFigures(figures);
        }

		public void RemoveImages(params Image[] images)
        {
			foreach(var image in images)
            {
				_imageSet.UntrackImages(image);
				image.Clear();
            }
        }

		public Figure GetFigure(string name, params int[] figureParams)
        {
			switch (name)
            {
				case "circle":
					return new Circle(figureParams[0]);
				case "filled circle":
					return new FilledCircle(figureParams[0]);
				case "ellipse":
					return new Ellipse(figureParams[0], figureParams[1]);
				case "conus":
					return new Conus(figureParams[0], figureParams[1]);
				case "point":
					return new Point(figureParams[0], figureParams[1]);
				case "line":
					return new Line(new Point(figureParams[0], figureParams[1]), new Point(figureParams[2], figureParams[3]));
				case "partial conus":
					return new PartialConus(figureParams[0], figureParams[1], figureParams[2]);
				case "dot":
					return new Point(figureParams[0], figureParams[1]);
				default:
					return null;
            }
        }

		public void DisposeImage(Image image)
        {
			_imageSet.UntrackImage(image);
			image.Clear();
        }

		public Image SelectImage(int index)
        {
            try
            {
				return _defaultImage = _imageSet[index];
            }
			catch (IndexOutOfRangeException)
            {
				throw new InvalidOperationException($"Image with index {index} does not exist!");
            }
        }

		public void MergeImages(Image image1, Image image2)
        {
			DisposeImage(image2);
			image1.MergeImages(image2);
			Draw();
        }

		public Image CreateImage(Point LeftTop, int width = 6, int height = 15)
        {
			var img = new Image(LeftTop);
			img.SetWidthByPercentage(width);
			img.Heigth = height;
			img.Build();
			_imageSet.TrackImages(img);

			return img;
        }

		public Image GetImageById(int id)
        {
            try
            {
				return _imageSet[id];
			}
			catch
            {
				return null;
            }
        }

		public void Draw()
        {
			_imageSet.HandleLayers();
        }

		public void ScaleUpImage(Image image)
        {
			image.SizeUp();
			Draw();
        }

		public void ScaleDownImage(Image image)
        {
			image.SizeDown();
			Draw();
        }

		public void SaveImage(Image image, string fileName)
        {
			_pictureSaver.SaveImageAsync(image, fileName);
        }

		public Image LoadImage(string fileName)
        {
			Image image = null;
			try
            {
				image = _pictureSaver.LoadImage(fileName);
				image.Build();
			}
			catch(FileNotFoundException)
            {
				Console.Clear();
				Console.WriteLine("file was not found");
				return null;
            }
			catch (Exception ex)
            {
				Console.Clear();
				Console.WriteLine(ex.Message);
				return null;
            }

			foreach (var figure in image.Figures)
				figure.TrackingImage = image;

			_imageSet.TrackImages(image);

			return image;
        }
	}
}