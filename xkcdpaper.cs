//http://xkcd.com/json.html
//compile command  PS C:\Windows\Microsoft.NET\Framework64\v4.0.30319> ./csc.exe /r:C:\Users\ob123\Desktop\xkcd\Newtonsoft.Json.dll /out:C:\Users\ob123\Desktop\xkcd\xkcdpaper.exe C:\Users\ob123\Desktop\xkcd\xkcdpaper.cs

using System.Text;
//https://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.aspx
//
// Huge thanks to Neil N's answer on SO for setting the wallpaper
// http://stackoverflow.com/a/1061682
//
using System.Net;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Newtonsoft.Json;

public class xkcdpaper {
	public const string latestURL = "http://xkcd.com/info.0.json";
	public const int bufferSize = 4096;
	public static void Main(String[] args) {
		xkcdpaper paper = new xkcdpaper();
		Console.WriteLine("Grabbing the latest comic data");
		comic com;
		try {
			com = paper.grabComic();
		}
		catch (Exception e) {
			Console.WriteLine(e.Message);
			Console.WriteLine("Fatal error. Quitting");
			com = null;
			Environment.Exit(0);
		}
		Console.WriteLine("Grabbing picture");
		com.getComicImg();
		com.setComicWallpaper();
	}
	public comic grabComic() {
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(latestURL);
		HttpWebResponse response = (HttpWebResponse)request.GetResponse();
		//Get Response stream
		Stream streamResponse = response.GetResponseStream();
		StreamReader streamRead = new StreamReader(streamResponse);
		char[] streamBuffer = new char[bufferSize];
		int bufferLen = streamRead.Read(streamBuffer,0,bufferSize);
		//Console.WriteLine("Response...");
		string apiResponse = new String(streamBuffer,0,bufferLen);
		//Console.Write(apiResponse);
		//Movie m = JsonConvert.DeserializeObject<Movie>(json);
		comic c = JsonConvert.DeserializeObject<comic>(apiResponse);
		return c;
	}
}
public class comic {
	[DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

	public int num;
	public string link;
	public string safe_Title;
	public string alt;
	public System.Drawing.Image comicImg;
	public string img;
	public string title;
	public System.Drawing.Image comicPic;
	public string wallpaperPath = null;
	public bool gotWallpaper = false;
	
	enum FileType {
		JPG,	//0
		PNG,	//1
		BMP,	//2
		UNKNOWN	//3
	}
	
	private FileType getType() {

		string[] dots = img.Split('.');
		if (dots[dots.Length - 1].Equals("JPG",StringComparison.	CurrentCultureIgnoreCase)) {
			return FileType.JPG;
		}
		else if (dots[dots.Length - 1].Equals("JPEG",StringComparison.	CurrentCultureIgnoreCase)) {
			return FileType.JPG;
		}
		else if (dots[dots.Length - 1].Equals("PNG",StringComparison.	CurrentCultureIgnoreCase)) {
			return FileType.PNG;
		}
		else if (dots[dots.Length - 1].Equals("BMP",StringComparison.	CurrentCultureIgnoreCase)) {
			return FileType.BMP;
		}
		else {
			return FileType.UNKNOWN;
		}
	}
	private string fixPath(string path) {
		//path: file:\C:\Users\ob123\Desktop\xkcd\wallpaper.PNG
		//should become C:/Users/ob123/Desktop/xkcd/wallpaper.PNG
		if (path.Length <= 6) {
			nullAndQuit();
		}
		string newString = path.Substring(6);
		newString.Replace((char)92,'/');
		return newString;
	}
	private void nullAndQuit() {
		//prints out that the path is null and then quits
		Console.WriteLine("ERROR. Path is null. Quitting");
		Environment.Exit(-1);
	}
	private void printType(FileType type) {
		if (type == FileType.JPG) Console.WriteLine("Comic is a JPG");
		else if (type == FileType.PNG) Console.WriteLine("Comic is a PNG");
		else if (type == FileType.BMP) Console.WriteLine("Comic is a BMP");
		else Console.WriteLine("Comic is unsupported");
	}
	public void getComicImg() {
		Console.WriteLine("Getting comic Image");
		System.IO.Stream s = new System.Net.WebClient().OpenRead(img);
		comicPic = System.Drawing.Image.FromStream(s);
		FileType type = getType();
		printType(type);
		string tempPath, origPath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
		switch (type) {
			case FileType.JPG:
				tempPath = Path.Combine(Path.GetDirectoryName(origPath),"wallpaper.JPG");
				tempPath = fixPath(tempPath);
				if (tempPath == null) nullAndQuit();
				try {
					comicPic.Save(tempPath, System.Drawing.Imaging.ImageFormat.Jpeg);
					wallpaperPath = tempPath;
					gotWallpaper = true;
				}
				catch {
					Console.WriteLine("Failed to save wallpaper. Quitting.");
					Environment.Exit(-1);
				}
				break;
			case FileType.PNG:
				tempPath = Path.Combine(Path.GetDirectoryName(origPath),"wallpaper.PNG");
				tempPath = fixPath(tempPath);
				if (tempPath == null) nullAndQuit();
				try {
					comicPic.Save(tempPath, System.Drawing.Imaging.ImageFormat.Png);
					wallpaperPath = tempPath;
					gotWallpaper = true;
				}
				catch {
					Console.WriteLine("Failed to save wallpaper. Quitting.");
					Environment.Exit(-1);
				}
				break;
			case FileType.BMP:
				tempPath = Path.Combine(Path.GetDirectoryName(origPath),"wallpaper.BMP");
				tempPath = fixPath(tempPath);
				if (tempPath == null) nullAndQuit();
				try {
					comicPic.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);
					wallpaperPath = tempPath;
					gotWallpaper = true;
				}
				catch {
					Console.WriteLine("Failed to save wallpaper. Quitting.");
					Environment.Exit(-1);
				}
				break;
			case FileType.UNKNOWN:
				Console.WriteLine("Unsupported file type. Quitting.");
				Environment.Exit(0);
				break;
		}
	}
	public void setComicWallpaper() {
		const int SPI_SETDESKWALLPAPER = 20;
		const int SPIF_UPDATEINIFILE = 0x01;
		const int SPIF_SENDWININICHANGE = 0x02;
		RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
		//set Desktop to centered style
		key.SetValue(@"WallpaperStyle", 0.ToString());
        key.SetValue(@"TileWallpaper", 0.ToString());
		SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaperPath,
		SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
		Console.WriteLine("Set wallpaper to '" + title + "'.");
	}
}