using MyCanvas.Computing;
using Newtonsoft.Json;

namespace Savers
{
	public class PictureSaver
	{
		private JsonSerializer _jsonSerializer;

		public Dictionary<dynamic, dynamic> Settings { get; set; } = new();

		public PictureSaver()
		{
			_jsonSerializer = new JsonSerializer();
			JsonSerializerSettings settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
			Settings["JsonSettings"] = settings;
			Settings["SavingDirectory"] = "/Users/mihailtkacenko/Desktop/Coding/C#/SomeProjects/FigureDrawer/FigureDrawer";
			Settings["SavingFolderName"] = "Cancas_Saves";
		}

		public void SaveImage(Image image, string fileName)
        {
			string imageSerialized = JsonConvert.SerializeObject(image, Settings["JsonSettings"]);
			var folderPath = Path.Combine(Settings["SavingDirectory"], Settings["SavingFolderName"]);

			if (!Directory.Exists(folderPath))
				Directory.CreateDirectory(folderPath);

			var filePath = Path.Combine(folderPath, fileName + ".json");
			File.WriteAllText(filePath,imageSerialized);
        }

		public Image LoadImage(string fileName)
        {
			var path = Path.Combine(Path.Combine(Settings["SavingDirectory"], Settings["SavingFolderName"]), fileName);
			var fileContent = File.ReadAllText(path);
			var image = JsonConvert.DeserializeObject<Image>(fileContent, Settings["JsonSettings"]);

			return image;
        }

		public void SaveImageAsync(Image image, string fileName)
        {
			Task.Run(() =>
			{
				SaveImage(image, fileName);
			});
        }
	}
}