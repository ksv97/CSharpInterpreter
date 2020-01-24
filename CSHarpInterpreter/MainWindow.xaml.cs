using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Scanner;

namespace CSHarpInterpreter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Scaner scaner;

        public MainWindow()
        {
            InitializeComponent();
            LoadExampleText();
        }

        private void LoadExampleText()
        {
            using (TextReader reader = File.OpenText("test.txt"))
            {
                TxtBoxInput.Text += reader.ReadToEnd();
            }
        }

        private void BtnScan_Click(object sender, RoutedEventArgs e)
        {
            this.TxtBlockResult.Clear();
            scaner = new Scaner(this.TxtBoxInput.Text);

            string exc = scaner.ScanText();

            if (string.IsNullOrWhiteSpace(exc))
            {
                int tokenIndex = 0;
                foreach (Token t in this.scaner.ResultTokens)
                {
                    this.TxtBlockResult.Text += "(" + tokenIndex + "): " + t.Value + " - " + t.TokenType.ToString("G");
                    this.TxtBlockResult.Text += Environment.NewLine;
                    tokenIndex++;
                }

                SyntaxAnalyzer analyzer = new SyntaxAnalyzer(scaner.ResultTokens, scaner.constsAndVariables);
                string syntaxException = analyzer.StartSyntaxAnalysis();
                this.Errors.Text = "";
                this.Errors.Text += syntaxException;
            }
            else
            {
                this.Errors.Text += exc + "\n";
            }
        }
    }
}
