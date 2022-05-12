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
using System.Xml;
using System.Xml.Linq;

namespace LINQtoXML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }



        #region Convert
        private void Convert_8()
        {
            XElement root = new XElement ("root",
                from line in Initial.Text.Split('\n') select (new XElement("line", 
                from word in line.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) select (new XElement("word",
                from ch in word.Trim() select new XElement("char", ch))))));

            XDocument xmlDoc = new XDocument(new XDeclaration("1.0", "windows-1251", null), root);

            Converted.Text = xmlDoc.ToString();
        }

        private void Convert_18()
        {
            XDocument xmlDoc = XDocument.Parse(Initial.Text);
            Converted.Text = "";

            var res = xmlDoc.Root.DescendantsAndSelf()
                .SelectMany(
                    el => el.Attributes()
                    .Select(attr => new { Name=attr.Name, Value=attr.Value}))
                .GroupBy(x => x.Name, x => x.Value)
                .OrderBy(x => x.Key.ToString());

            foreach(var group in res)
            {
                Converted.Text += group.Key + ":\n";
                foreach (var item in group)
                    Converted.Text += "   " + item + "\n";
                Converted.Text += "\n";
            }
        }

        private void Convert_28()
        {
            XDocument xmlDoc = XDocument.Parse(Initial.Text);
            Converted.Text = "";

            var res = xmlDoc.Root.Descendants().Where(el => el.Ancestors().Count() == 2).DescendantNodes().OfType<XText>().ToList();

            foreach (var item in res)
                item.Remove();
            Converted.Text += xmlDoc;
        }

        private void Convert_38()
        {
            XDocument xmlDoc = XDocument.Parse(Initial.Text);
            Converted.Text = "";

            foreach(var el in xmlDoc.Root.Descendants())
                el.Name = el.Parent.Name + "-" + el.Name;

            Converted.Text += xmlDoc;
        }

        private void Convert_48()
        {
            XDocument xmlDoc = XDocument.Parse(Initial.Text);
            Converted.Text = "";

            var res = xmlDoc.Root.DescendantsAndSelf().Where(el => el.Elements().Count() >= 2);

            foreach(var el in res)
            {
                if (el.Nodes().Any(c => c.NodeType == XmlNodeType.ProcessingInstruction))
                    el.LastNode.AddBeforeSelf(new XElement("has-instructions", true));
                else el.LastNode.AddBeforeSelf(new XElement("has-instructions", false));
            }

            Converted.Text += xmlDoc;
        }

        private void Convert_58()
        {
            XDocument xmlDoc = XDocument.Parse(Initial.Text);
            Converted.Text = "";

            XNamespace ns = xmlDoc.Root.Element("namespace").Value;
            xmlDoc.Root.Element("namespace").Remove();

            xmlDoc.Root.Add(new XAttribute(XNamespace.Xmlns + "node", ns));

            foreach(var el in xmlDoc.Root.DescendantsAndSelf())
            {
                el.Add(new XAttribute(ns + "count", el.DescendantNodes().Count()));
                el.Add(new XAttribute(XNamespace.Xml + "count", el.Descendants().Count()));
            }

            Converted.Text += xmlDoc;
        }

        private void Convert_68()
        {
            XDocument xmlDoc = XDocument.Parse(Initial.Text);
            Converted.Text = "";

            foreach(var el in xmlDoc.Root.Elements().ToList())
            {
                el.ReplaceWith(
                    new XElement("station",
                        new XAttribute("company", el.Element("company").Value),
                        new XAttribute("street", el.Element("street").Value),
                        new XElement("info",
                            new XElement("brand", el.Element("brand").Value),
                            new XElement("price", el.Element("price").Value))));
            }

            Converted.Text += xmlDoc;
        }


        /*
<root>
<debt house="12" flat="23">
 <name>Иванов А.В.</name>
 <value>1245.64</value>
</debt> 
</root>
        */
        private void Convert_78()
        {
            XDocument xmlDoc = XDocument.Parse(Initial.Text);
            Converted.Text = "";

            var res = xmlDoc.Root.Elements().Select(
                el => new
                {
                    House = el.Attribute("house").Value,
                    Flat = el.Attribute("flat").Value,
                    Entrance = int.Parse(el.Attribute("flat").Value) / 10,
                    Name = el.Element("name").Value,
                    Value = el.Element("value").Value
                }).OrderBy(el => el.House).ThenBy(el => el.Entrance).ThenBy(el => el.Value)
                .GroupBy(el => new { House = el.House, Entrance = el.Entrance})
                .GroupBy(g => g.Key.House).ToList();

            xmlDoc.Root.RemoveAll();
            foreach(var houseGroup in res)
            {
                XElement houseEl = new XElement("house", new XAttribute("number", houseGroup.Key));
                xmlDoc.Root.Add(houseEl);

                foreach(var entrGroup in houseGroup)
                {
                    XElement entrEl = new XElement("entrance", new XAttribute("number", entrGroup.Key.Entrance.ToString()));
                    houseEl.Add(entrEl);

                    foreach(var debt in entrGroup)
                    {
                        entrEl.Add(new XElement("debt",
                            new XAttribute("name", debt.Name),
                            new XAttribute("flat", debt.Flat),
                            debt.Value));
                    }
                }
            }

            Converted.Text += xmlDoc;
        }
        #endregion



        #region UIevents
        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            switch (((ListBoxItem)ConversionTypeListbox.SelectedItem).Content.ToString())
            {
                case "8":
                    Convert_8();
                    break;

                case "18":
                    Convert_18();
                    break;

                case "28":
                    Convert_28();
                    break;

                case "38":
                    Convert_38();
                    break;

                case "48":
                    Convert_48();
                    break;

                case "58":
                    Convert_58();
                    break;

                case "68":
                    Convert_68();
                    break;

                case "78":
                    Convert_78();
                    break;

                default:
                    Converted.Text = "Неизвестный способ преобразования";
                    break;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }



        private void ConversionTypeListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConverstionTypeExpander.Header = "Способ преобразования: " +
                ((ListBoxItem)ConversionTypeListbox.SelectedItem).Content.ToString();
        }
        #endregion
    }
}

/*
<library>
  <book>
    <page>
      <library>
        <book>
          <page>asdf</page>
          <page>asdf</page>
          <page>asdf</page>
        </book>
      </library>
    </page>
    <page></page>
    <page></page>
  </book>
</library>
*/