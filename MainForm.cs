using CSSimpleFunctions;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace FTKStartup
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(() =>
            {
                if (!File.Exists("Tkinter-Designer-1.0.7.zip"))
                {
                    SimpleFileHandler.ProjectToLocation(Assembly.GetExecutingAssembly(), "Tkinter-Designer-1.0.7.zip");
                    ZipFile.ExtractToDirectory("Tkinter-Designer-1.0.7.zip", Directory.GetCurrentDirectory());
                }
                PyCS pyCS = new PyCS();
                if (!File.Exists("pipdone"))
                {
                    pyCS.InstallPip();
                    pyCS.PipUpgrade(new string[] { "setuptools", "wheel" });
                    pyCS.Pip(new string[] { "poetry.core" });
                    pyCS.Pip(new string[] { "tkinter-embed" });
                    pyCS.Pip(new string[] { ".[tool.poetry.dependencies]" });
                    SimpleFileHandler.Write("pipdone", string.Empty);
                }
                if (!File.Exists("envdone"))
                {
                    string[] gui = SimpleFileHandler.Read("build/gui.py").Split('\n');
                    SimpleFileHandler.Write("build/gui.py", "import os\nos.environ['TCL_LIBRARY'] = r'python3_13\\Lib\\site-packages\\tcl\\tcl8.6'\n");
                    foreach (string line in gui)
                    {
                        if (!line.Contains("ASSETS_PATH = OUTPUT_PATH / Path") && !line.Contains("window.geometry"))
                        {
                            SimpleFileHandler.Append("build/gui.py", line);
                        }
                        else
                        {
                            if (line.Contains("ASSETS_PATH = OUTPUT_PATH / Path"))
                            {
                                SimpleFileHandler.Append("build/gui.py", "ASSETS_PATH = OUTPUT_PATH / Path(r\"assets\\frame0\")\n");
                            }
                            if (line.Contains("window.geometry"))
                            {
                                SimpleFileHandler.Append("build/gui.py", "width = 640\nheight = 480\nscreen_width = window.winfo_screenwidth()\nscreen_height = window.winfo_screenheight()\nx = (screen_width // 2) - (width // 2)\ny = (screen_height // 2) - (height // 2)\nwindow.geometry(f\"{width}x{height}+{x}+{y}\")\n");
                            }
                        }
                    }
                    SimpleFileHandler.Write("envdone", string.Empty);
                }
                Invoke(new MethodInvoker(() =>
                {
                    Hide();
                }));
                pyCS.RunFile("build/gui.py");
                Invoke(new MethodInvoker(() =>
                {
                    Close();
                }));
            })).Start();
        }
    }
}
