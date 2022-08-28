using MyCanvas.Configuration;
using Newtonsoft.Json;

namespace MyCanvas.Computing
{
	public sealed class Image : Figure
	{
		public List<Figure> Figures { get; } = new();
		public int Depth { get => _depth;}
		public int Width { get => _width; set => _width = value; }
		public int Heigth { get; set; } = 6;
		public Point? LeftTop { get; set; }
		[JsonIgnore]
		public ImageSet? ImageSet { get; set; }
		new public Point Center { get; }

		private int _width;
		private int _widthPercentage;
		private int _depth = 0;

		public Image(Point leftTop)
		{
			LeftTop = leftTop;
			_depth = 0;
			Center = CalculateCenter();
		}

		private Point CalculateCenter()
        {
			var xBottom = LeftTop.X + Width;
			var yBottom = LeftTop.Y + Heigth;
			var x = (int)Math.Round((LeftTop.X + xBottom) / 2.0);
			var y = (int)Math.Round((LeftTop.Y + yBottom) / 2.0);

			return new Point(x, y);
        }

		public void SetWidthByPercentage(int num)
        {
			_width = (int)Math.Round((double)num / 12 * Console.WindowWidth);
			_widthPercentage = num;
		}

		public void SetDepth(int depth)
        {
			_depth = depth;
			ImageSet.ChangeLayerOfImage(this);
		}

        public void AddFigures(params Figure[] figures)
        {
			foreach (var figure in figures)
            {
				figure.TrackingImage = this;
				figure.Build();
				Figures.Add(figure);
			}
        }

		public void RemoveFigures(params Figure[] figures)
        {
			foreach (var figure in figures)
            {
				Figures.Remove(figure);
				figure.Clear();
				figure.TrackingImage = null;
			}
        }

		public override void Build()
        {
			var topLine = new Line(LeftTop, new Point(LeftTop.X + Width, LeftTop.Y));
			var bottomLine = new Line(new Point(topLine.StartPoint.X, topLine.StartPoint.Y + Heigth), new Point(topLine.EndPoint.X, topLine.EndPoint.Y + Heigth));
			var leftLine = new Line(topLine.StartPoint, bottomLine.StartPoint);
			var rightLine = new Line(topLine.EndPoint, bottomLine.EndPoint);

			topLine.Build();
			bottomLine.Build();
			leftLine.Build();
			rightLine.Build();

			Points.Clear();

			Points.Add(LeftTop);

			Points = topLine.Points
                .Union(bottomLine.Points)
                .Union(leftLine.Points)
                .Union(rightLine.Points)
                .ToList();
        }

        public override double CalcularePerimetr()
        {
			var sum = 0.0;
			foreach (var figure in Figures)
				sum += figure.CalcularePerimetr();

			return sum;
        }

        public void MergeImages(Image secondImage)
        {
			if (secondImage == null)
				return;

			foreach (var externalFigure in secondImage.Figures)
            {
				Figures.Add(externalFigure);
				externalFigure.TrackingImage = this;
				//AddFigures(externalFigure);
			}
        }

		public double CalculateAllArea()
        {
			var sum = 0.0;
			foreach (var figure in Figures)
				sum += figure.CalculateArea();

			return sum;
        }

		public void DrawByRect(List<Point> restrRect = null)
        {
			Draw();

			foreach (var point in restrRect)
            {
				Console.SetCursorPosition(point.X, point.Y);
				Console.Write(" ");
            }
        }

        public override void Draw()
        {
			base.Draw();

			foreach (var figure in Figures)
				figure.Draw();
        }

		public override void Clear()
        {
			base.Clear();

			Figures.ForEach(x => x.Clear());
        }

        public override void SizeUp()
        {
			Clear();

			//_widthPercentage += 2;
			Width+=5;
			SetWidthByPercentage(_widthPercentage);
			Heigth += 5;
            LeftTop.X--;

			Build();

			Figures.ForEach(x => x.SizeUp());

			Draw();
        }

        public override void SizeDown()
        {
			Clear();
			//_widthPercentage -= 2;
			Width-=5;
			SetWidthByPercentage(_widthPercentage);
			Heigth -= 5;
			LeftTop.X++;

			Build();

			Figures.ForEach(x => x.SizeDown());

			Draw();
		}

        public override double CalculateArea()
        {
			var sum = 0.0;

			Figures.ForEach(x => sum += x.CalculateArea());

			return sum;
        }
    }
}