using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace PyinstallerHelper
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            
            if (!Directory.Exists(Constants.AppDataPath))
            {
                Directory.CreateDirectory(Constants.AppDataPath);
            } if (!Directory.Exists(Constants.DistPath))
            {
                Directory.CreateDirectory(Constants.DistPath);
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Routines.main = new Form1();
            if (args.Length > 0)
            {
                Routines.main.LoadFile(args[0]);
            }
            Application.Run(Routines.main);
        }
    }
    public static class Routines
    {
        public static Form1 main;
        public static string GenerateNewTempdir()
        {
            string bases = Environment.ExpandEnvironmentVariables("%TEMP%\\PYIH-");
            Random r = new Random();
            bases += r.Next();
            Directory.CreateDirectory(bases);
            return bases;
        }

        public static string PadVersionRight(string inv)
        {
            int ln = Regex.Matches(inv,@"\.").Count+1;
            if (ln == 1)
            {
                return inv + ".0.0.0";
            } else if (ln == 2)
            {
                return inv + ".0.0";
            } else if (ln == 3)
            {
                return inv + ".0";
            } else
            {
                return inv;
            }
        }
        public static bool ExistsOnPath(string fileName)
        {
            return GetFullPath(fileName) != null;
        }

        public static string GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
                return Path.GetFullPath(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(';'))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }
        public static DialogResult RunGUICommand(string fullcommand,int expectedlines,string title,Form parent)
        {
            VerboseCommandExecutorForm vcf = new VerboseCommandExecutorForm();
            string[] splcommand = CLISplit.SplitCommandLine(fullcommand).ToArray();
            string cmdhead = splcommand[0];
            string[] finall = new string[splcommand.Length - 1];
            splcommand.ToList().CopyTo(1,finall,0, splcommand.Length-2);
            string cmdargs = string.Join(" ", finall);
            vcf.EFile = cmdhead;
            vcf.EArgs = cmdargs;
            vcf.Title = title;
            vcf.ExpectedLines = expectedlines;
            return vcf.ShowDialog(parent);

        }

    }
    public class PyinstallerHelperProject
    {
        public bool HideConsole;
        public string SourceFile;
        public bool UseCustomIcon;
        public bool UseDefaultIcon;
        public bool Onefile;
        public string OutputDirectory;
        public bool UseVersionFile;
        public string CustomIconPath;
        public string CompanyName;
        public string Description;
        public string AppVersion;
        public string InternalName;
        public string Copyright;
        public string OriginalFilename;
        public string AppName;
        public string ProductVersion;
        public PyinstallerHelperProject(Form1 inputformsettings)
        {
            HideConsole = inputformsettings.radioButton2.Checked;
            SourceFile = inputformsettings.textBox1.Text;
            UseCustomIcon = inputformsettings.radioButton5.Checked;
            if (UseCustomIcon)
            {
                CustomIconPath = inputformsettings.textBox2.Text;
            }
            UseDefaultIcon = inputformsettings.radioButton4.Checked;//NOTE! UNfinished
            Onefile = inputformsettings.checkBox1.Checked;
            OutputDirectory = inputformsettings.textBox3.Text;
            UseVersionFile = inputformsettings.checkBox2.Checked;
            if (UseVersionFile)
            {
                CompanyName = inputformsettings.textBox4.Text;
                Description = inputformsettings.textBox5.Text;
                AppVersion = inputformsettings.textBox6.Text;
                InternalName = inputformsettings.textBox7.Text;
                Copyright = inputformsettings.textBox8.Text;
                OriginalFilename = inputformsettings.textBox9.Text;
                AppName  = inputformsettings.textBox10.Text;
                ProductVersion = inputformsettings.textBox11.Text;
            }
        }
        public PyinstallerHelperProject() { }//Will this fix XML problem?
        public string OutToXML ()
        {
            XmlSerializer xml = new XmlSerializer(typeof(PyinstallerHelperProject));
            Utf8StringWriter sw = new Utf8StringWriter();
            xml.Serialize(sw, this);
            return sw.ToString();
        }
        public void WriteControls(Form1 f)
        {
            f.radioButton2.Checked = HideConsole;
            f.textBox1.Text = SourceFile;
            f.radioButton5.Checked = UseCustomIcon;
            if (UseCustomIcon)
            {
                f.textBox2.Text = CustomIconPath;
            }
            f.radioButton4.Checked = UseDefaultIcon;
            if (!UseCustomIcon && !UseDefaultIcon)
            {
                f.radioButton3.Checked = true;
            }
            f.checkBox1.Checked = Onefile;
            f.textBox3.Text = OutputDirectory;
            f.checkBox2.Checked = UseVersionFile;
            if (UseVersionFile)
            {
                f.textBox4.Text = CompanyName;
                f.textBox5.Text = Description;
                f.textBox6.Text = AppVersion;
                f.textBox7.Text = InternalName;
                f.textBox8.Text = Copyright;
                f.textBox9.Text = OriginalFilename;
                f.textBox10.Text = AppName;
                f.textBox11.Text = ProductVersion;
            }
        }
    }
    public class PYIHPWrapper
    {
        public string TitleName;
        public string FullPath;
        public PyinstallerHelperProject LastSave;

        public PYIHPWrapper(string filename, PyinstallerHelperProject lastSave)
        {
            FullPath = filename;
            TitleName = filename.Split('\\').Last();
            LastSave = lastSave;
        }
        public PYIHPWrapper() { }
        public static PYIHPWrapper Blank()
        {
            PYIHPWrapper p = new PYIHPWrapper();
            p.TitleName = "New Project";
            p.FullPath = "";
            p.LastSave = null;
            return p;
        }
        public bool NeedsSaving(Form1 f)
        {
            var CurrentSave = new PyinstallerHelperProject(f);
            return ! CurrentSave.OutToXML().Equals(LastSave.OutToXML());
        }
        public void UpdateTitle(Form1 f)
        {
            f.Text = $"Pyinstaller Helper: [{TitleName}]";
            
            if (NeedsSaving(f))
            {
                f.Text = $"Pyinstaller Helper: [{TitleName}] *";
                
            }
        }
    
    }
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
    public static class Constants
    {
        public static string AppDataPath = Environment.ExpandEnvironmentVariables("%APPDATA%\\PyinstallerHelper");
        public static string DistPath = Environment.ExpandEnvironmentVariables(AppDataPath+"\\dist");
        public static string VersionFileTemplate = @"# UTF-8
#
# For more details about fixed file info 'ffi' see:
# http://msdn.microsoft.com/en-us/library/ms646997.aspx

VSVersionInfo(
  ffi=FixedFileInfo(
    # filevers and prodvers should be always a tuple with four items: (1, 2, 3, 4)
    # Set not needed items to zero 0. Must always contain 4 elements.
    filevers=(#VERSSION),
    prodvers=(#VERSPSION),
    # Contains a bitmask that specifies the valid bits 'flags'r
    mask=0x3f,
    # Contains a bitmask that specifies the Boolean attributes of the file.
    flags=0x0,
    # The operating system for which this file was designed.
    # 0x4 - NT and there is no need to change it.
    OS=0x40004,
    # The general type of file.
    # 0x1 - the file is an application.
    fileType=0x1,
    # The function of the file.
    # 0x0 - the function is not defined for this fileType
    subtype=0x0,
    # Creation date and time stamp.
    date=(0, 0)
    ),
  kids=[
    StringFileInfo(
      [
      StringTable(
        u'040904B0',
        [StringStruct(u'CompanyName', u'#COMPANYNAME'),
        StringStruct(u'FileDescription', u'#DESCR'),
        StringStruct(u'FileVersion', u'#VERSION'),
        StringStruct(u'InternalName', u'#INAME'),
        StringStruct(u'LegalCopyright', u'#CO'),
        StringStruct(u'OriginalFilename', u'#OGF'),
        StringStruct(u'ProductName', u'#PNAME'),
        StringStruct(u'ProductVersion', u'#VERPSION')])
      ]), 
    VarFileInfo([VarStruct(u'Translation', [1033, 1200])])
  ]
)";
    }
    public static class CLISplit
    {
        //100% from SO!

        public static IEnumerable<string> SplitCommandLine(string commandLine)
        {
            bool inQuotes = false;

            return commandLine.Split(c =>
            {
                if (c == '\"')
                    inQuotes = !inQuotes;

                return !inQuotes && c == ' ';
            })
                              .Select(arg => arg.Trim().TrimMatchingQuotes('\"'))
                              .Where(arg => !string.IsNullOrEmpty(arg));
        }
        public static IEnumerable<string> Split(this string str,
                                         Func<char, bool> controller)
        {
            int nextPiece = 0;

            for (int c = 0; c < str.Length; c++)
            {
                if (controller(str[c]))
                {
                    yield return str.Substring(nextPiece, c - nextPiece);
                    nextPiece = c + 1;
                }
            }

            yield return str.Substring(nextPiece);
        }
        public static string TrimMatchingQuotes(this string input, char quote)
        {
            if ((input.Length >= 2) &&
                (input[0] == quote) && (input[input.Length - 1] == quote))
                return input.Substring(1, input.Length - 2);

            return input;
        }
    }
}
