using Newtonsoft.Json;

namespace MyCanvas.Computing
{
	public abstract class Figure
    {
        public List<Point> Points { get; set; } = new List<Point>();
        public int Size { get; protected set; } = 100;
        public int Angle { get; set; } = 0;
        public Point Center { get; set; }
        [JsonIgnore]
        public Image TrackingImage { get; set; }

        public virtual double CalculateArea() { return 0; }
        public virtual double CalcularePerimetr() { return 0.0; }
        public virtual void SizeUp() { }
        public virtual void SizeDown() { }

        protected void Rebase(int xShift, int yShift)
        {
            Clear();
            Build();
            Move(xShift, yShift);
            Rotate(Center, Angle);
        }

        double ConvertToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        double ConvertRadiansToDegrees(double radians)
        {
            var result = (180.0 / Math.PI) * radians;
            return result;
        }

        public virtual void Rotate(Point origin, int degrees = 5)
        {
            Clear();

            for (int i = 0; i < Points.Count; i++)
            {
                var currentPoint = Points[i];

                var radius = Math.Sqrt(Math.Pow(currentPoint.X - origin.X, 2) + Math.Pow(currentPoint.Y - origin.Y, 2));

                if (radius == 0)
                    continue;

                var xLength = currentPoint.X - origin.X;
                var yLength = currentPoint.Y - origin.Y;

                var radians = Math.Atan2(yLength, xLength);
                var currentAngle = ConvertRadiansToDegrees(radians);

                currentAngle += degrees;

                var x = (int)Math.Round(radius * Math.Cos(ConvertToRadians(currentAngle)) + origin.X);
                var y = (int)Math.Round(radius * Math.Sin(ConvertToRadians(currentAngle)) + origin.Y);

                currentPoint.X = x;
                currentPoint.Y = y;
            }

            //Draw();
        }

        public virtual void MoveAndDraw(int left, int top)
        {
            Clear();
            Move(left, top);
            Draw();
        }

        public List<Point> ConvertToUniversalCoord(List<Point> points)
        {
            var convertedPoints = new List<Point>();

            for (int i = 0; i < points.Count; i++)
            {
                var genX = points[i].X + TrackingImage.LeftTop.X;
                var genY = points[i].Y + TrackingImage.LeftTop.Y;
                convertedPoints.Add(new Point(genX, genY));
            }

            return convertedPoints;
        }

        protected (int uniX, int uniY) ConvertToUniversalCoord(Point point)
        {
            return (point.X + TrackingImage.LeftTop.X, point.Y + TrackingImage.LeftTop.Y);
        }

        public virtual void Draw()
        {
            if (TrackingImage == null && this is not Image)
                throw new InvalidOperationException("Image was not set!");

            foreach (var point in Points)
            {
                if (this is not Image)
                {
                    var uniCoords = ConvertToUniversalCoord(point);

                    if (uniCoords.uniX <= TrackingImage.LeftTop.X ||
                        uniCoords.uniX >= TrackingImage.LeftTop.X + TrackingImage.Width ||
                        uniCoords.uniY <= TrackingImage.LeftTop.Y ||
                        uniCoords.uniY >= TrackingImage.LeftTop.Y + TrackingImage.Heigth ||
                        uniCoords.uniX <= Console.WindowLeft ||
                        uniCoords.uniX >= Console.BufferWidth ||
                        uniCoords.uniY <= Console.WindowTop ||
                        uniCoords.uniY >= Console.WindowHeight)
                        continue;

                    Console.SetCursorPosition(uniCoords.uniX, uniCoords.uniY);
                    Console.Write("*");
                }
                else
                {
                    if (point.X <= Console.WindowLeft || point.Y <= Console.WindowTop || point.Y >= Console.WindowHeight || point.X >= Console.WindowWidth)
                        continue;

                    Console.SetCursorPosition(point.X, point.Y);
                    Console.Write("*");
                }
            }
        }

        public virtual void Clear()
        {
            foreach(var point in Points)
            {
                if (this is not Image)
                {
                    var uniCoords = ConvertToUniversalCoord(point);

                    if (uniCoords.uniX <= TrackingImage.LeftTop.X ||
                        uniCoords.uniX >= TrackingImage.LeftTop.X + TrackingImage.Width ||
                        uniCoords.uniY <= TrackingImage.LeftTop.Y ||
                        uniCoords.uniY >= TrackingImage.LeftTop.Y + TrackingImage.Heigth ||
                        uniCoords.uniX <= Console.WindowLeft ||
                        uniCoords.uniX >= Console.BufferWidth ||
                        uniCoords.uniY <= Console.WindowTop ||
                        uniCoords.uniY >= Console.WindowHeight)
                        continue;

                    Console.SetCursorPosition(uniCoords.uniX, uniCoords.uniY);
                    Console.Write(" ");
                }
                else
                {
                    if (point.X <= Console.WindowLeft || point.Y <= Console.WindowTop || point.Y >= Console.WindowHeight || point.X >= Console.WindowWidth)
                        continue;

                    Console.SetCursorPosition(point.X, point.Y);
                    Console.Write(" ");
                }
            }
        }

        public virtual void Build()
        {
            Console.WriteLine("default figure");
        }

        public string GetInfo()
        {
            return $"{this}, area {CalculateArea()}, size: {Size}, angle: {Angle}";
        }

        public virtual void Move(int left, int top)
        {
            Clear();

            foreach (var point in Points)
            {
                point.X += left;
                point.Y += top;
            }
        }
    }

    public class Circle : Figure
    {
        public int Radius { get; private set; }
        
        protected double r_in;
        protected double r_out;
        private bool _filled;


        public Circle(int radius, bool filled = false)
        {
            Radius = radius;
            _filled = filled;
        }

        public override double CalcularePerimetr()
        {
            return 2 * Math.PI * Radius;
        }

        public override double CalculateArea()
        {
            return Math.PI * Math.Pow(Radius, 2);
        }

        public override void Build()
        {
            Center = new Point(Radius, Radius);
            r_in = Radius - 0.4;
            r_out = Radius + 0.4;

            if (_filled)
                r_in = 0;

            Points.Clear();
            Points.Add(Center);

            for (int j = Radius * 2; j >= 0; j--)
            {
                for (int i = Radius * 2; i >= 0; i--)
                {
                    int x = i - Radius;
                    int y = j - Radius;

                    var value = x * x + y * y;

                    if (Math.Sqrt(value) >= r_in && Math.Sqrt(value) <= r_out)
                        Points.Add(new Point(i,j));
                }
            }
        }

        private (int xShift, int yShift) GetShifts()
        {
            return (Center.X - Radius, Center.Y - Radius);
        }

        public override void SizeUp()
        {
            var shifts = GetShifts();
            Radius++;
            Rebase(shifts.xShift, shifts.yShift);
        }

        public override void SizeDown()
        {
            if (Radius <= 0)
                return;

            var shifts = GetShifts();
            Radius--;
            Rebase(shifts.xShift, shifts.yShift);
        }
    }

    public sealed class Ellipse : Figure
    {
        public int XFocus { get => _xFocus; set => _xFocus = value; }
        public int YFocus { get => _yFocus; set => _yFocus = value; }

        private int _xFocus;
        private int _yFocus;
        private double r_in;
        private double r_out;
        private double _accuracy;
            
        public Ellipse(int xFocus, int yFocus, double accuracy = 0.2)
        {
            _xFocus = xFocus;
            _yFocus = yFocus;
            _accuracy = accuracy;
        }

        public override double CalcularePerimetr()
        {
            return (4 * Math.PI * XFocus * YFocus + Math.Pow(XFocus - YFocus, 2)) / (XFocus + YFocus);
        }

        public override double CalculateArea()
        {
            return Math.PI * XFocus * YFocus;
        }

        public override void Build()
        {
            Center = new Point(_xFocus, _yFocus);
            Points.Clear();
            Points.Add(Center);
            r_in = 1 - _accuracy;
            r_out = 1 + _accuracy;

            for (int j = _yFocus * 2; j >= 0; j--)
            {
                for (int i = _xFocus * 2; i >= 0; i--)
                {
                    var x = i - _xFocus;
                    var y = j - _yFocus;

                    var value = Math.Pow(x, 2) / Math.Pow(_xFocus, 2) + Math.Pow(y, 2) / Math.Pow(_yFocus, 2);

                    if (value >= r_in  && value <= r_out)
                        Points.Add(new Point(i, j));
                }
            }
        }

        private (int xShift, int yShift) ShiftsToCenter()
        {
            return (Center.X - XFocus, Center.Y - YFocus);
        }

        public override void SizeDown()
        {
            if (XFocus <= 0 || YFocus <= 0)
            {
                XFocus = 0;
                YFocus = 0;
                return;
            }

            var shifts = ShiftsToCenter();

            XFocus -= 3;
            YFocus -= 1;

            Size -= 10;

            Rebase(shifts.xShift, shifts.yShift);
        }

        public override void SizeUp()
        {
            var shifts = ShiftsToCenter();

            XFocus += 3;
            YFocus += 1;

            Size += 10;

            Rebase(shifts.xShift, shifts.yShift);
        }
    }

    public class FilledCircle : Circle
    {
        public FilledCircle(int radius) : base(radius, filled: true) { }
    }

    public class PartialConus : Figure
    {
        public Ellipse UpperFoundation { get; private set; }
        public Ellipse MainFoundation { get; private set; }

        public int MainRadius { get; set; }
        public int UpperRadius { get; set; }
        public int Height { get; set; }

        public PartialConus(int mainRadius, int upperRadius, int height)
        {
            MainRadius = mainRadius;
            UpperRadius = upperRadius;
            Height = height;
        }

        public override double CalcularePerimetr()
        {
            return MainFoundation.CalcularePerimetr() + UpperFoundation.CalcularePerimetr();
        }

        public override double CalculateArea()
        {
            var L = Math.Sqrt(Math.Pow(Height, 2) + (Math.Pow(MainRadius, 2) - Math.Pow(UpperRadius, 2)));
            return Math.PI * (Math.Pow(MainRadius, 2) + Math.Pow(UpperRadius, 2) + (MainRadius + UpperRadius) * L);
        }

        public override void Build()
        {
            MainFoundation = new Ellipse(xFocus: MainRadius, yFocus: (int)Math.Round((decimal)MainRadius / 4));
            UpperFoundation = new Ellipse(xFocus: UpperRadius, yFocus: (int)Math.Round((decimal)UpperRadius / 4));
        
            MainFoundation.Build();
            UpperFoundation.Build();

            Center = MainFoundation.Center;

            UpperFoundation.TrackingImage = this.TrackingImage;
            MainFoundation.TrackingImage = this.TrackingImage;

            var xMainCopy = MainFoundation.Center.X;
            var xUpperCopy = UpperFoundation.Center.X;

            var shift = xMainCopy - xUpperCopy;

            UpperFoundation.Move(shift, 0);
            MainFoundation.Move(0, Height);

            var leftLine = new Line(
                new Point(UpperFoundation.Center.X - UpperFoundation.XFocus, UpperFoundation.Center.Y),
                new Point(MainFoundation.Center.X - MainFoundation.Center.Y, MainFoundation.Center.Y));
            var rightLine = new Line(
                new Point(UpperFoundation.Center.X + UpperFoundation.XFocus, UpperFoundation.Center.Y),
                new Point(MainFoundation.Center.X + MainFoundation.XFocus, MainFoundation.Center.Y));
            var behindLine = new Line(
                new Point(UpperFoundation.Center.X, UpperFoundation.Center.Y + UpperFoundation.YFocus),
                new Point(MainFoundation.Center.X, MainFoundation.Center.Y - MainFoundation.YFocus));

            leftLine.Build();
            rightLine.Build();
            behindLine.Build();

            Points.Clear();
            Points.Add(Center);
            Points = Points
                .Union(MainFoundation.Points)
                .Union(UpperFoundation.Points)
                .Union(leftLine.Points)
                .Union(rightLine.Points)
                .Union(behindLine.Points)
                .ToList();

            leftLine.TrackingImage = this.TrackingImage;
            rightLine.TrackingImage = this.TrackingImage;
            behindLine.TrackingImage = this.TrackingImage;
        }

        public override void SizeUp()
        {
            Height++;
            UpperRadius++;
            MainRadius++;
            Size += 10;

            var tempCenterX = MainRadius;
            var tempCenterY = Height + (int)Math.Round((decimal)MainRadius/ 4);

            var xShift = Center.X - tempCenterX;
            var yShift = Center.Y - tempCenterY;

            Rebase(xShift, yShift);
        }

        public override void SizeDown()
        {
            if (Height < 0 || UpperRadius < 0 || MainRadius < 0)
                return;

            Height--;
            UpperRadius--;
            MainRadius--;
            Size -= 10;
            

            var tempCenterX = MainRadius;
            var tempCenterY = Height + (int)Math.Round((decimal)MainRadius / 4);

            var xShift = Center.X - tempCenterX;
            var yShift = Center.Y - tempCenterY;

            Rebase(xShift, yShift);
        }
    }

    public class Conus : Figure
    {
        private int _radius;
        private int _height;

        private Ellipse _foundation;

        public Conus(int radius, int height)
        {
            _radius = radius;
            _height = height;
        }

        public override double CalcularePerimetr()
        {
            return _foundation.CalcularePerimetr();
        }

        public override double CalculateArea()
        {
            var L = Math.Sqrt(Math.Pow(_height, 2) + Math.Pow(_radius, 2));
            return Math.PI * _radius * (_radius + L);
        }

        public override void SizeUp()
        {
            _height++;
            _radius++;
            Size += 10;

            var tempCenterX = _radius;
            var tempCenterY = _height + (int)Math.Round((decimal)_radius / 4);

            var xShift = Center.X - tempCenterX;
            var yShift = Center.Y - tempCenterY;

            Rebase(xShift, yShift);
        }

        public override void SizeDown()
        {
            if (_height <= 0 || _radius <= 0)
                return;

            _height--;
            _radius--;
            Size -= 10;

            var tempCenterX = _radius;
            var tempCenterY = _height + (int)Math.Round((decimal)_radius / 4);

            var xShift = Center.X - tempCenterX;
            var yShift = Center.Y - tempCenterY;

            Rebase(xShift, yShift);
        }

        public override void Build()
        {
            _foundation = new Ellipse(xFocus: _radius, yFocus: (int)Math.Round((decimal)_radius / 4));
            _foundation.Build();
            _foundation.TrackingImage = this.TrackingImage;
            _foundation.Move(0, _height);

            Center = _foundation.Center;

            Points.Clear();
            Points.Add(Center);

            var line1 = new Line(new Point(_foundation.Center.X, _foundation.Center.Y - _height), new Point(_foundation.Center.X - _foundation.XFocus, _height));
            var line2 = new Line(new Point(_foundation.Center.X, _foundation.Center.Y - _height), new Point(_foundation.Center.X + _foundation.XFocus, _height));
            var line3 = new Line(new Point(_foundation.Center.X, _foundation.Center.Y - _height), new Point(_foundation.Center.X, _foundation.Center.Y - _foundation.YFocus));

            line1.Build();
            line2.Build();
            line3.Build();

            line1.TrackingImage = this.TrackingImage;
            line2.TrackingImage = this.TrackingImage;
            line3.TrackingImage = this.TrackingImage;

            Points = Points
                .Union(_foundation.Points)
                .Union(line1.Points)
                .Union(line2.Points)
                .Union(line3.Points)
                .ToList();
        }
    }

    public class Point : Figure
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point() { }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{X} {Y}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Point || obj == null)
                return false;

            var anotherPoint = obj as Point;

            return anotherPoint.X == X && anotherPoint.Y == Y ? true : false;
        }
    }

    public class Line : Figure
    {
        public Point StartPoint { get => _startPoint; }
        public Point EndPoint { get => _endPoint; }

        private Point _startPoint;
        private Point _endPoint;
        private double multiplier;

        public Line(Point startPoint, Point endPoint)
        {
            _startPoint = startPoint;
            _endPoint = endPoint;
        }

        public double CalculateLength()
        {
            return Math.Sqrt(Math.Pow(_endPoint.X - _startPoint.X, 2) + Math.Pow(_endPoint.Y - StartPoint.Y, 2));
        }

        public override void SizeUp()
        {
            multiplier = 1.4;

            var xShift = Center.X;
            var yShift = Center.Y;

            Size += 10;

            ReCalculateEdges();
            Clear();
            Build();
            Move(xShift, yShift);
        }

        public override void SizeDown()
        {
            multiplier = 0.6;

            var xShift = Center.X;
            var yShift = Center.Y;

            Size -= 10;

            ReCalculateEdges();
            Clear();
            Build();
            Move(xShift, yShift);
        }

        private void ReCalculateEdges()
        {
            (int x, int y) vector1 = (x: EndPoint.X - Center.X, y: EndPoint.Y - Center.Y);
            (int x, int y) vector2 = (x: StartPoint.X - Center.X, y: StartPoint.Y - Center.Y);

            EndPoint.X = (int)Math.Round(vector1.x * multiplier);
            EndPoint.Y = (int)Math.Round(vector1.y * multiplier);

            StartPoint.X = (int)Math.Round(vector2.x * multiplier);
            StartPoint.Y = (int)Math.Round(vector2.y * multiplier);
        }

        public override void Build()
        {
            Points.Clear();
            Center = new Point(x: (StartPoint.X + EndPoint.X) / 2, y: (StartPoint.Y + EndPoint.Y) / 2);
            Points.Add(Center);
            Points.Add(StartPoint);
            Points.Add(EndPoint);

            (int X, int Y) main_vector = (_endPoint.X - _startPoint.X, _endPoint.Y - _startPoint.Y);

            var action = (int x, int y) =>
            {
                (int X, int Y) vector = (x - _endPoint.X, y - _endPoint.Y);

                var value = vector.X * main_vector.Y - vector.Y * main_vector.X;

                if (value >= 0 - 2 && value <= 0 + 2)
                    Points.Add(new Point(x, y));
            };

            var action1 = (int y) =>
            {
                if (_startPoint.X > _endPoint.X)
                {
                    for (int x = _startPoint.X; x >= _endPoint.X; x--)
                        action(x, y);

                    return;
                }
                for (int x = _startPoint.X; x <= _endPoint.X; x++)
                    action(x, y);
            };

            if (_startPoint.Y > _endPoint.Y)
            {
                for (int y = _startPoint.Y; y >= _endPoint.Y; y--)
                    action1(y);

                return;
            }
            for (int y = _startPoint.Y; y <= _endPoint.Y; y++)
                action1(y);
        }
    }
}