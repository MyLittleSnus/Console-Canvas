using MyCanvas;
using MyCanvas.Computing;

var canvas = new Canvas(needsDefault: false);

Image currentImg = null;
Figure currentFigure = null;

Console.CursorVisible = false;

Console.ReadKey(intercept: true);

while (true)
{
    try
    {
       switch (Console.ReadKey(intercept: true).Key)
       {
            case ConsoleKey.V:
                Console.SetCursorPosition(1, 1);
                foreach (var figure in currentImg.Figures)
                {
                    Console.WriteLine(figure.GetInfo());
                    Thread.Sleep(2000);
                }
                Console.SetCursorPosition(1, 1);
                foreach (var figure in currentImg.Figures)
                {
                    for (int i = 0; i < figure.GetInfo().Length; i++)
                        Console.Write(" ");
                }
                break;
            case ConsoleKey.F12:
                canvas.ScaleUpImage(currentImg);
                break;

            case ConsoleKey.F10:
                canvas.ScaleDownImage(currentImg);
                break;

            case ConsoleKey.B: // brush (delete)
                switch (Console.ReadKey(intercept: true).Key)
                {
                    case ConsoleKey.F:
                        if (currentImg == null || currentFigure == null)
                        {
                            Console.WriteLine("Error");
                            break;
                        }
                        currentImg.RemoveFigures(currentFigure);
                        currentFigure = null;
                        break;

                    case ConsoleKey.I:
                        canvas.DisposeImage(currentImg);
                        break;
                }

                break;

            case ConsoleKey.U: // size up figure
                currentFigure.SizeUp();
                canvas.Draw();
                break;

            case ConsoleKey.D: // size down figure
                currentFigure.SizeDown();
                canvas.Draw();
                break;

            case ConsoleKey.J: // saving to a file
                Console.Clear();
                Console.Write("Name of file to save: ");
                string name = Console.ReadLine();
                canvas.SaveImage(currentImg, name);
                Console.Clear();
                canvas.Draw();
                break;

            case ConsoleKey.P: // loading from a file
                Console.Clear();
                Console.Write("Name of file to load: ");
                string fileName = Console.ReadLine();
                currentImg = canvas.LoadImage(fileName);
                if (currentImg == null)
                    Thread.Sleep(1000);
                Console.Clear();
                canvas.Draw();
                break;

            case ConsoleKey.L: // layers
                switch (Console.ReadKey(intercept: true).Key)
                {
                    case ConsoleKey.UpArrow: // increase depth
                        var layerUp = currentImg.Depth + 1;
                        currentImg.SetDepth(layerUp);
                        canvas.Draw();
                        break;

                    case ConsoleKey.DownArrow: // decrease depth
                        var layerDown = currentImg.Depth - 1;
                        currentImg.SetDepth(layerDown);
                        canvas.Draw();
                        break;
                }
                break;

            case ConsoleKey.S: // select
                switch (Console.ReadKey(intercept: true).Key)
                {
                    case ConsoleKey.I: // image
                        switch (Console.ReadKey(intercept: true).Key)
                        {
                            case ConsoleKey.F1:
                                currentImg = canvas.SelectImage(0);
                                break;

                            case ConsoleKey.F2:
                                currentImg = canvas.SelectImage(1);
                                break;
                        }
                        break;

                    case ConsoleKey.F: // figure
                        switch (Console.ReadKey(intercept: true).Key)
                        {
                            case ConsoleKey.F1:
                                currentFigure = currentImg.Figures[0];
                                break;
                            case ConsoleKey.F2:
                                currentFigure = currentImg.Figures[1];
                                break;
                        }
                        break;
                }
                break;

            case ConsoleKey.R: // rotate
                switch (Console.ReadKey(intercept: true).Key)
                {
                    case ConsoleKey.F: // figure
                    FIGURE_ROTATE: switch (Console.ReadKey(intercept: true).Key)
                        {
                            case ConsoleKey.Q: // rotate left
                                currentFigure.Rotate(currentFigure.Center, -30);
                                currentFigure.Angle += -30;
                                canvas.Draw();
                                goto FIGURE_ROTATE;
                            case ConsoleKey.E: // rotate right
                                currentFigure.Rotate(currentFigure.Center, 30);
                                currentFigure.Angle += 30;
                                canvas.Draw();
                                goto FIGURE_ROTATE;
                        }
                        break;
                }
                break;
            case ConsoleKey.M: // move
                switch (Console.ReadKey(intercept: true).Key)
                {
                    case ConsoleKey.I: // image
                    IMG_CONTROL: switch (Console.ReadKey(intercept: true).Key)
                        {
                            case ConsoleKey.UpArrow:
                                currentImg.Move(0, -2);
                                canvas.Draw();
                                goto IMG_CONTROL;

                            case ConsoleKey.DownArrow:
                                currentImg.Move(0, 2);
                                canvas.Draw();
                                goto IMG_CONTROL;

                            case ConsoleKey.LeftArrow:
                                currentImg.Move(-2, 0);
                                canvas.Draw();
                                goto IMG_CONTROL;

                            case ConsoleKey.RightArrow:
                                currentImg.Move(2, 0);
                                canvas.Draw();
                                goto IMG_CONTROL;
                        }
                        break;

                    case ConsoleKey.F: // figure
                    FIGURE_CONTROL: switch (Console.ReadKey(intercept: true).Key)
                        {
                            case ConsoleKey.UpArrow:
                                currentFigure.Move(0, -2);
                                canvas.Draw();
                                goto FIGURE_CONTROL;

                            case ConsoleKey.DownArrow:
                                currentFigure.Move(0, 2);
                                canvas.Draw();
                                goto FIGURE_CONTROL;

                            case ConsoleKey.LeftArrow:
                                currentFigure.Move(-2, 0);
                                canvas.Draw();
                                goto FIGURE_CONTROL;

                            case ConsoleKey.RightArrow:
                                currentFigure.Move(2, 0);
                                canvas.Draw();
                                goto FIGURE_CONTROL;
                        }
                        break;
                }
                break;
            case ConsoleKey.C: // combine
                switch (Console.ReadKey(intercept: true).Key)
                {
                    case ConsoleKey.F1:
                        canvas.MergeImages(currentImg, canvas.GetImageById(0));
                        break;
                    case ConsoleKey.F2:
                        canvas.MergeImages(currentImg, canvas.GetImageById(1));
                        break;
                    case ConsoleKey.F3:
                        canvas.MergeImages(currentImg, canvas.GetImageById(2));
                        break;
                    case ConsoleKey.F4:
                        canvas.MergeImages(currentImg, canvas.GetImageById(3));
                        break;
                }
                break;
            case ConsoleKey.N: // new
                switch (Console.ReadKey(intercept: true).Key)
                {
                    case ConsoleKey.F: //figure
                        try
                        {
                            switch (Console.ReadKey(intercept: true).Key)
                            {
                                case ConsoleKey.C:
                                    var circle = canvas.GetFigure("circle", 5);
                                    canvas.AddOnImage(currentImg, circle);
                                    currentFigure = circle;
                                    canvas.Draw();
                                    break;

                                case ConsoleKey.E:
                                    var ellipse = canvas.GetFigure("ellipse", 10, 3);
                                    canvas.AddOnImage(currentImg, ellipse);
                                    currentFigure = ellipse;
                                    canvas.Draw();
                                    break;

                                case ConsoleKey.P:
                                    var partialConus = canvas.GetFigure("partial conus", 7, 4, 5);
                                    canvas.AddOnImage(currentImg, partialConus);
                                    currentFigure = partialConus;
                                    canvas.Draw();
                                    break;

                                case ConsoleKey.O:
                                    var conus = canvas.GetFigure("conus", 7, 10);
                                    canvas.AddOnImage(currentImg, conus);
                                    currentFigure = conus;
                                    canvas.Draw();
                                    break;

                                case ConsoleKey.L:
                                    var line = canvas.GetFigure("line", 1, 1, 5, 5);
                                    canvas.AddOnImage(currentImg, line);
                                    currentFigure = line;
                                    canvas.Draw();
                                    break;

                                case ConsoleKey.F:
                                    var filledCircle = canvas.GetFigure("filled circle", 5);
                                    canvas.AddOnImage(currentImg, filledCircle);
                                    currentFigure = filledCircle;
                                    canvas.Draw();
                                    break;

                            }
                        }
                        catch
                        {
                            Console.WriteLine("Image was not set");
                            break;
                        }
                        
                        break;


                    case ConsoleKey.I: // image
                        var img = canvas.CreateImage(new Point(90, 5), width: 4);
                        currentImg = img;
                        canvas.Draw();
                        break;
                }
                break;
       }
    }
    catch (Exception ex)
    {
        Console.WriteLine("error: " + ex.Message);
    }
}