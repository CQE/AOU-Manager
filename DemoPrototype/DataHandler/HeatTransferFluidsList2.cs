using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using Windows.Storage;

using Windows.UI.Popups;

namespace DemoPrototype
{
    /************************************
    * heatTransferFluidsList XML Type
    * Example file. texatherm32.xml
    * 
    * http://xmltocsharp.azurewebsites.net/
    * http://codehandbook.org/c-object-xml/
    * http://stackoverflow.com/questions/364253/how-to-deserialize-xml-document
    ************************************/

    class HeatTransferFluidsList
    {
        private heatTransferFluidsList fluidList;

        public HeatTransferFluidsList()
        {
            // Init default from texatherm32.xml
            SetFluidListFromXMLString(GetDefaultXML());
        }

        public HeatTransferFluidsList(string xml)
        {
            // Init from xml file or xml string
            if (xml.StartsWith("<?xml"))
            {
                SetFluidListFromXMLString(xml); // xml text
            }
            else
            {
                SetFluidListFromXMLString(ReadXML(xml)); // File
            }
        }

        public HeatTransferFluidsList(heatTransferFluidsList list)
        {
            this.fluidList = list;
        }


        public heatTransferFluidsList GetHeatTransferFluidsList()
        {
            return fluidList;
        }


        public string GetXMLStringFromFluidList()
        {
            string xml = "";
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(heatTransferFluidsList));
                StringBuilder sb = new StringBuilder(xml);
                serializer.Serialize(new StringWriter(sb), this.fluidList);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public bool SetFluidListFromXMLString(string xml)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(heatTransferFluidsList));
                heatTransferFluidsList list = (heatTransferFluidsList)serializer.Deserialize(new StringReader(xml));
                this.fluidList = list;
                return true;
            }
            catch (Exception ex)
            {
                AppHelper.ShowMessageBox("Deserialize XML: " + ex.Message + " from: " + xml);

       //         var dialog = new MessageDialog("Deserialize XML: " + ex.Message + " from: " + xml);
        //        dialog.ShowAsync();

                return false;
            }
        }

        private string GetDefaultXML()
        {
            Uri appUri = new Uri("ms-appx:///Assets/texatherm32.xml");
      //      Uri appUri = new Uri("ms-appx:///Assets/Calflo AF.xml");

            try
            {
                // StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(appUri);
                StorageFile xmlFile = StorageFile.GetFileFromApplicationUriAsync(appUri).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

                string xmlText = FileIO.ReadTextAsync(xmlFile).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                return xmlText;
            }
            catch (Exception ex)
            {
                string err = ex.Message;

                return err;
            }
        }


        public string ReadXML(string xmlPath)
        {
            if (xmlPath[0] == '\\')
                xmlPath = xmlPath.Substring(1);
            StorageFolder dataFolder = KnownFolders.PicturesLibrary;

            try
            {
                StorageFile xmlFile = dataFolder.GetFileAsync(xmlPath).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                string xmlText = FileIO.ReadTextAsync(xmlFile).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                return xmlText;
            }
            catch (Exception ex)
            {
                AppHelper.ShowMessageBox("Read " + xmlPath + " : " + ex.Message);

           //     var dialog = new MessageDialog("Read " + xmlPath + " : " + ex.Message);
           //     dialog.ShowAsync();

                return ex.Message;
            }
        }

        public bool WriteXML(string xmlPath)
        {
            if (xmlPath[0] == '\\')
                xmlPath = xmlPath.Substring(1);
            if (!xmlPath.EndsWith(".xml", StringComparison.CurrentCultureIgnoreCase))
            {
                xmlPath += ".xml";
            }
            StorageFolder dataFolder = KnownFolders.PicturesLibrary;
            string xmlStr = GetXMLStringFromFluidList();

            try
            {
                StorageFile xmlFile = dataFolder.CreateFileAsync(xmlPath, CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                //  StorageFile xmlFile = dataFolder.GetFileAsync(xmlPath).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                FileIO.WriteTextAsync(xmlFile, xmlStr).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

                AppHelper.ShowMessageBox(xmlPath + " Saved");

              //  var dialog = new MessageDialog(xmlPath + " Saved");
              //  dialog.ShowAsync();

                return true;
            }
            catch (Exception ex)
            {
                AppHelper.ShowMessageBox("Save Settings: "+ ex.Message);

             //   var dialog = new MessageDialog("Save Settings: " + ex.Message);
             //   dialog.ShowAsync();

                return false;
            }
        }
    }

        /*
         * http://codehandbook.org/c-object-xml/
        Employee emp = new Employee();
        emp.FirstName = "Code";
        emp.LastName = "Handbook";
        string xml = GetXMLFromObject(emp);

        Object obj = ObjectToXML(xml,typeof(Employee));
        */

        /*
                public static string GetXMLFromObject(object o)
                {
                    StringWriter sw = new StringWriter();
                    XmlTextWriter tw = null;
                    try
                    {
                        XmlSerializer serializer = new XmlSerializer(o.GetType());
                        tw = new XmlTextWriter(sw);
                        serializer.Serialize(tw, o);
                    }
                    catch (Exception ex)
                    {
                        //Handle Exception Code
                    }
                    finally
                    {
                        sw.Close();
                        if (tw != null)
                        {
                            tw.Close();
                        }
                    }
                    return sw.ToString();
                }

                public static Object ObjectToXML(string xml, Type objectType)
                {
                    StringReader strReader = null;
                    XmlSerializer serializer = null;
                    XmlTextReader xmlReader = null;
                    Object obj = null;
                    try
                    {
                        strReader = new StringReader(xml);
                        serializer = new XmlSerializer(objectType);
                        xmlReader = new XmlTextReader(strReader);
                        obj = serializer.Deserialize(xmlReader);
                    }
                    catch (Exception exp)
                    {
                        //Handle Exception Code
                    }
                    finally
                    {
                        if (xmlReader != null)
                        {
                            xmlReader.Close();
                        }
                        if (strReader != null)
                        {
                            strReader.Close();
                        }
                    }
                    return obj;
                }
                */




        /****************************************************************
         * 
         * heatTransferFluidsList Xml Class
         * 
         * ***************************************************************/

        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public class heatTransferFluidsList
        {

            private heatTransferFluidsListHeatTransferFluid heatTransferFluidField;

            public heatTransferFluidsListHeatTransferFluid heatTransferFluid
            {
                get
                {
                    return this.heatTransferFluidField;
                }
                set
                {
                    this.heatTransferFluidField = value;
                }
            }
        }

        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class heatTransferFluidsListHeatTransferFluid
        {
            private string brandField;
            private string typeField;
            private heatTransferFluidsListHeatTransferFluidSigned signedField;
            private object descriptionField;
            private object noteField;

            private heatTransferFluidsListHeatTransferFluidSafety safetyField;  // OlFr

            private heatTransferFluidsListHeatTransferFluidTemperaturesList temperaturesListField;
            private heatTransferFluidsListHeatTransferFluidDensitiesList densitiesListField;
            private heatTransferFluidsListHeatTransferFluidViscositiesList viscositiesListField;
            private string idField;

            public string brand
            {
                get
                {
                    return this.brandField;
                }
                set
                {
                    this.brandField = value;
                }
            }

            public string type
            {
                get
                {
                    return this.typeField;
                }
                set
                {
                    this.typeField = value;
                }
            }

            public heatTransferFluidsListHeatTransferFluidSigned signed
            {
                get
                {
                    return this.signedField;
                }
                set
                {
                    this.signedField = value;
                }
            }

            public object description
            {
                get
                {
                    return this.descriptionField;
                }
                set
                {
                    this.descriptionField = value;
                }
            }

            public object note
            {
                get
                {
                    return this.noteField;
                }
                set
                {
                    this.noteField = value;
                }
            }

            public heatTransferFluidsListHeatTransferFluidSafety safety   // OlFr
            {
                get
                {
                    return this.safetyField;
                }
                set
                {
                    this.safetyField = value;
                }
            }

            public heatTransferFluidsListHeatTransferFluidTemperaturesList temperaturesList
            {
                get
                {
                    return this.temperaturesListField;
                }
                set
                {
                    this.temperaturesListField = value;
                }
            }

            public heatTransferFluidsListHeatTransferFluidDensitiesList densitiesList
            {
                get
                {
                    return this.densitiesListField;
                }
                set
                {
                    this.densitiesListField = value;
                }
            }

            public heatTransferFluidsListHeatTransferFluidViscositiesList viscositiesList
            {
                get
                {
                    return this.viscositiesListField;
                }
                set
                {
                    this.viscositiesListField = value;
                }
            }

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string id
            {
                get
                {
                    return this.idField;
                }
                set
                {
                    this.idField = value;
                }
            }
        }

        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class heatTransferFluidsListHeatTransferFluidSigned
        {
            private string nameField;
            private string organisationField;
            private System.DateTime dateField;

            public string name
            {
                get
                {
                    return this.nameField;
                }
                set
                {
                    this.nameField = value;
                }
            }

            public string organisation
            {
                get
                {
                    return this.organisationField;
                }
                set
                {
                    this.organisationField = value;
                }
            }

            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public System.DateTime date
            {
                get
                {
                    return this.dateField;
                }
                set
                {
                    this.dateField = value;
                }
            }
        }


        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class heatTransferFluidsListHeatTransferFluidSafety    // OlFr
        {
            private string dpdField;
            private string ohsaField;
            private string whmisField;

            private heatTransferFluidsListHeatTransferFluidSafetyOther otherField;  // OlFr

            public string dpd
            {
                get
                {
                    return this.dpdField;
                }
                set
                {
                    this.dpdField = value;
                }
            }

            public string ohsa
            {
                get
                {
                    return this.ohsaField;
                }
                set
                {
                    this.ohsaField = value;
                }
            }

            public string whmis
            {
                get
                {
                    return this.whmisField;
                }
                set
                {
                    this.whmisField = value;
                }
            }

            public heatTransferFluidsListHeatTransferFluidSafetyOther other   // OlFr
            {
                get
                {
                    return this.otherField;
                }
                set
                {
                    this.otherField = value;
                }
            }

        }


        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class heatTransferFluidsListHeatTransferFluidSafetyOther   // OlFr
        {
            private string orgField;
            private string healthField;

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string org
            {
                get
                {
                    return this.orgField;
                }
                set
                {
                    this.orgField = value;
                }
            }

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string health
            {
                get
                {
                    return this.healthField;
                }
                set
                {
                    this.healthField = value;
                }
            }
        }


        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class heatTransferFluidsListHeatTransferFluidTemperaturesList
        {
            private heatTransferFluidsListHeatTransferFluidTemperaturesListUnits unitsField;
            private int flashPointField;
            private int boilingPointField;
            private int autoIgnitionField;

            public heatTransferFluidsListHeatTransferFluidTemperaturesListUnits units
            {
                get
                {
                    return this.unitsField;
                }
                set
                {
                    this.unitsField = value;
                }
            }

            public int flashPoint
            {
                get
                {
                    return this.flashPointField;
                }
                set
                {
                    this.flashPointField = value;
                }
            }

            public int boilingPoint
            {
                get
                {
                    return this.boilingPointField;
                }
                set
                {
                    this.boilingPointField = value;
                }
            }

            public int autoIgnition
            {
                get
                {
                    return this.autoIgnitionField;
                }
                set
                {
                    this.autoIgnitionField = value;
                }
            }
        }

        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class heatTransferFluidsListHeatTransferFluidTemperaturesListUnits
        {
            private string temperatureField;

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string temperature
            {
                get
                {
                    return this.temperatureField;
                }
                set
                {
                    this.temperatureField = value;
                }
            }
        }

        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class heatTransferFluidsListHeatTransferFluidDensitiesList
        {
            private heatTransferFluidsListHeatTransferFluidDensitiesListUnits unitsField;
            private heatTransferFluidsListHeatTransferFluidDensitiesListDensity densityField;

            public heatTransferFluidsListHeatTransferFluidDensitiesListUnits units
            {
                get
                {
                    return this.unitsField;
                }
                set
                {
                    this.unitsField = value;
                }
            }

            public heatTransferFluidsListHeatTransferFluidDensitiesListDensity density
            {
                get
                {
                    return this.densityField;
                }
                set
                {
                    this.densityField = value;
                }
            }
        }

        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class heatTransferFluidsListHeatTransferFluidDensitiesListUnits
        {
            private string specificWeightField;
            private string temperatureField;

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string specificWeight
            {
                get
                {
                    return this.specificWeightField;
                }
                set
                {
                    this.specificWeightField = value;
                }
            }

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string temperature
            {
                get
                {
                    return this.temperatureField;
                }
                set
                {
                    this.temperatureField = value;
                }
            }
        }

        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class heatTransferFluidsListHeatTransferFluidDensitiesListDensity
        {
            private decimal specificWeightField;
            private byte temperatureField;

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public decimal specificWeight
            {
                get
                {
                    return this.specificWeightField;
                }
                set
                {
                    this.specificWeightField = value;
                }
            }

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public byte temperature
            {
                get
                {
                    return this.temperatureField;
                }
                set
                {
                    this.temperatureField = value;
                }
            }
        }

        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class heatTransferFluidsListHeatTransferFluidViscositiesList
        {
            private heatTransferFluidsListHeatTransferFluidViscositiesListUnits unitsField;
            private heatTransferFluidsListHeatTransferFluidViscositiesListViscosity[] viscosityField;

            public heatTransferFluidsListHeatTransferFluidViscositiesListUnits units
            {
                get
                {
                    return this.unitsField;
                }
                set
                {
                    this.unitsField = value;
                }
            }

            [System.Xml.Serialization.XmlElementAttribute("viscosity")]
            public heatTransferFluidsListHeatTransferFluidViscositiesListViscosity[] viscosity
            {
                get
                {
                    return this.viscosityField;
                }
                set
                {
                    this.viscosityField = value;
                }
            }
        }

        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class heatTransferFluidsListHeatTransferFluidViscositiesListUnits
        {
            private string kinematicField;
            private string temperatureField;

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string kinematic
            {
                get
                {
                    return this.kinematicField;
                }
                set
                {
                    this.kinematicField = value;
                }
            }

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string temperature
            {
                get
                {
                    return this.temperatureField;
                }
                set
                {
                    this.temperatureField = value;
                }
            }
        }

        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class heatTransferFluidsListHeatTransferFluidViscositiesListViscosity
        {
            private decimal kinematicField;
            private byte temperatureField;

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public decimal kinematic
            {
                get
                {
                    return this.kinematicField;
                }
                set
                {
                    this.kinematicField = value;
                }
            }

            [System.Xml.Serialization.XmlAttributeAttribute()]
            public byte temperature
            {
                get
                {
                    return this.temperatureField;
                }
                set
                {
                    this.temperatureField = value;
                }
            }
        }
}


    /***************************************************
    [XmlRoot("heatTransferFluidsList")]
    public class HeatTransferFluidsList
    {
        [XmlElement("heatTransferFluid")]
        public HeatTransferFluid[] HeatTransferFluids { get; set; }

        /*
        var transportAgreements = (TransportAgreementRoot)serializer.Deserialize(resultContentâ€Œâ€‹); 
        to(TransportAgreement[])serializer.Deserialize(resultContent)

        [XmlRoot("heatTransferFluid")]
        public class HeatTransferFluid
        {
            [XmlElement("id")]
            public int Id { get; set; }
            [XmlElement("brand")]
            public string Brand { get; set; }
            [XmlElement("type")]
            public string Type { get; set; }
            [XmlElement("description")]
            public string Description { get; set; } // Optional
            [XmlElement("note")]
            public string Note { get; set; } // Optional
        }

        [XmlRoot("signed")]
        public class Signed
        {
            [XmlElement("name")]
            public string Name { get; set; }
            [XmlElement("organisation")]
            public string Organisation { get; set; }
            [XmlElement("date")]
            public string Date { get; set; }
        }

        [XmlRoot("units")]
        public class TempUnits
        {
            [XmlElement("temperature")]
            public string Name { get; set; }
        }

        [XmlRoot("temperatures")]
        public class Temperatures
        {
            [XmlElement("flashPoint")]
            public string FlashPoint { get; set; }
            [XmlElement("boilingPoint")]
            public string BoilingPoint { get; set; }
            [XmlElement("autoIgnition")]
            public string AutoIgnition { get; set; }
        }

        [XmlRoot("densities")]
        public class Densities
        {
            [XmlElement("density")]
            public string Name { get; set; }
        }

        [XmlRoot("viscosityList")]
        public class ViscosityList
        {
            [XmlElement("viscosity")]
            public string Viscosity { get; set; }
        }


        */
