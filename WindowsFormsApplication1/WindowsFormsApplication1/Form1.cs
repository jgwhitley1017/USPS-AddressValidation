using System;
using System.Net;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public const string APIUser = "979MULTI5286";
        public const string USPSURL = "http://production.shippingapis.com/ShippingAPI.dll";

        public Form1()
        {
            InitializeComponent();
        }

        private XDocument Request()
        {
            XDocument doc = new XDocument();
            XElement usps = new XElement("AddressValidateRequest");
            doc.Add(usps);
            usps.SetAttributeValue("USERID", APIUser);
            usps.Add(new XElement("IncludeOptionalElements", true));
            usps.Add(new XElement("ReturnCarrierRoute", true));
            XElement adr = new XElement("Address");
            adr.SetAttributeValue("ID", 0);
            adr.Add(new XElement("Address1", txtAddress1.Text));
            adr.Add(new XElement("Address2", txtAddress2.Text));
            adr.Add(new XElement("City", txtCity.Text));
            adr.Add(new XElement("State", txtState.Text));
            adr.Add(new XElement("Zip5", txtZip5.Text));
            adr.Add(new XElement("Zip4", txtZip4.Text));
            usps.Add(adr);
            return doc;
        }

        private void SendRequest()
        {
            WebClient url = new WebClient();
            url.QueryString.Add("API", "Verify");
            url.QueryString.Add("XML", Request().ToString());
            HandleResponse(url.DownloadString(USPSURL));
        }

        private void HandleResponse(string response)
        {
            XDocument doc = XDocument.Parse(response);
            XElement adr = doc.Element("AddressValidateResponse").Element("Address");
            if (adr.Element("Error") != null)
            {
                MessageBox.Show(adr.Element("Error").Element("Description").Value, "Error Code " + adr.Element("Error").Element("Number").Value);
            }
            else
            {
                XElement element;
                txtAddress1.Text = (element = adr.Element("Address2")) != null ? element.Value : "";
                txtAddress2.Text = (element = adr.Element("Address1")) != null ? element.Value : "";
                txtCity.Text = (element = adr.Element("City")) != null ? element.Value : "";
                txtState.Text = (element = adr.Element("State")) != null ? element.Value : "";
                txtState.Text = (element = adr.Element("Zip")) != null ? element.Value : "";
            }
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            SendRequest();
        }
    }
}
