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
                if (!File.Exists("python3_13/gotopy.py"))
                {
                    SimpleFileHandler.ProjectToLocation(Assembly.GetExecutingAssembly(), "gotopy.py");
                    File.Move("gotopy.py", "python3_13/gotopy.py");
                }
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
                    string gui = SimpleFileHandler.Read("build/gui1.py");
                    SimpleFileHandler.Write("build/gui1.py", "import gotopy\nimport os\nos.environ['TCL_LIBRARY'] = r'python3_13\\Lib\\site-packages\\tcl\\tcl8.6'\n");
                    SimpleFileHandler.Append("build/gui1.py", gui);
                    SimpleFileHandler.Write("envdone", string.Empty);
                }
                Invoke(new MethodInvoker(() =>
                {
                    Hide();
                }));
                pyCS.RunFile("build/gui1.py");
                Invoke(new MethodInvoker(() =>
                {
                    Close();
                }));
            })).Start();
        }
    }
}
