using MyCanvas.Computing;

namespace MyCanvas.Configuration
{
	public class ImageSet
	{
		public List<Image> ImageStore { get; set; } = new();

		private Dictionary<int, List<Image>> _layers { get; set; } = new();

		public ImageSet() { }

		public Image this[int index]
        {
			get => ImageStore[index];
        }

		public bool HasImage(Image image)
        {
			return ImageStore.Contains(image);
        }

		public void UntrackImage(Image image)
		{
			ImageStore.Remove(image);

			foreach (var imgs in _layers.Values)
            {
				if (imgs.Contains(image))
                {
					imgs.Remove(image);
					return;
                }
            }
		}

		public void TrackImages(params Image[] images)
        {
			for (int i = 0; i < images.Length; i++)
            {
				images[i].ImageSet = this;

				if (!ImageStore.Contains(images[i]))
					ImageStore.Add(images[i]);

				var layer = images[i].Depth;

				if (_layers.ContainsKey(layer))
                {
					_layers[layer].Add(images[i]);
					continue;
                }

				_layers.Add(layer, new List<Image>() { images[i] });
            }
        }

		public void UntrackImages(params Image[] images)
        {
			for (int i = 0; i < images.Length; i++)
            {
				images[i].ImageSet = null;
				ImageStore.Remove(images[i]);
				_layers.Values
					.Select(x => x
						.Remove(x.Find(y => y == images[i])));
            }
        }

		public void ChangeLayerOfImage(Image image)
        {
			foreach (var imgList in _layers.Values)
            {
				var trackingImage = imgList.Find(x => x == image);

				if (trackingImage != null)
                {
					imgList.Remove(image);
					TrackImages(image);
					break;
                }
            }
        }

		public void HandleLayers()
        {
			var sordedDict = _layers
				.OrderBy(x => x.Key)
				.ToDictionary(x => x.Key, x => x.Value);
			var keys = sordedDict.Keys.ToList();

			for (int i = 0; i < keys.Count; i++)
            {
				for (int j = 0; j < sordedDict[keys[i]].Count; j++)
				{
					var currentImage = sordedDict[keys[i]][j];
					List<Point> intersectionRect = new();

					for (int k = i + 1; k < keys.Count; k++)
                    {
						var requiredKey = keys[k];
						var layerImages = sordedDict[requiredKey];

                        foreach (var layerImg in layerImages)
                        {
                            var currImgFigurePointsConverted = GetAllFiguresUni(currentImage);

							var leftTop = layerImg.LeftTop;
							var xCopy = leftTop.X;
							var yCopy = leftTop.Y;
							var bottomRight = new Point(xCopy + layerImg.Width, yCopy = leftTop.Y + layerImg.Heigth);

							var frame = currentImage.Points;
							var allPoints = frame.Concat(currImgFigurePointsConverted).ToList();

							foreach (var point in allPoints)
                            {
								if (point.X >= leftTop.X && point.X <= bottomRight.X && point.Y >= leftTop.Y && point.Y <= bottomRight.Y)
									intersectionRect.Add(point);
                            }
						}
                    }

					currentImage.Clear();
					currentImage.DrawByRect(intersectionRect);
				}
            }
        }

		private List<Point> GetAllFiguresUni(Image image)
        {
			var currentImgFigureConverted = new List<Point>();

			foreach (var figure in image.Figures)
			{
				var figurePointsConverted = figure.ConvertToUniversalCoord(figure.Points);
				currentImgFigureConverted = currentImgFigureConverted
					.Concat(figurePointsConverted)
					.ToList();
			}

			return currentImgFigureConverted;
		}
	}
}