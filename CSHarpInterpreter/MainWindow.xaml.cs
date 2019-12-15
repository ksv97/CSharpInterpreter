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
using SyntaxAnalyzer;

namespace CSHarpInterpreter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Scaner scaner;
        private Parser parser;

        private string mainTextFile = "mainText.txt";
        private string testFile = "test.txt";

        public MainWindow()
        {
            InitializeComponent();
            LoadExampleText();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void LoadExampleText()
        {
            using (TextReader reader = File.OpenText(testFile))
            {
                TxtBoxInput.Text += reader.ReadToEnd();
            }
        }

        private void ScanText()
        {
            this.TxtBlockResult.Clear();
            scaner = new Scaner(this.TxtBoxInput.Text);

            try
            {
                scaner.ScanText();
            }
            catch (ParseErrorException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (IndexOutOfRangeException)
            {
                // Do nothing, it's just a signal that the text ended.
            }

            foreach (Token t in this.scaner.ResultTokens)
            {
                this.TxtBlockResult.Text += t.Value + " - " + t.TokenType.ToString("G");
                this.TxtBlockResult.Text += Environment.NewLine;
            }

            this.TxtBlockResult.Text += "===VARIABLES===" + Environment.NewLine;
            foreach (Variable variable in this.scaner.Variables)
            {
                this.TxtBlockResult.Text += variable.Name;
                this.TxtBlockResult.Text += Environment.NewLine;
            }
        }

        private void BtnScan_Click(object sender, RoutedEventArgs e)
        {
            this.ScanText();           
        }

        private void BtnParse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.parser = new Parser(this.scaner);
                parser.Parse();
                MessageBox.Show("Parse completed successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Parse error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
