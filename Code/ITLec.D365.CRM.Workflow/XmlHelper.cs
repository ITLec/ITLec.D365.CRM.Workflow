using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ITLec.D365.CRM.Workflow
{
    public class XmlHelper
    {

        private static void SerializeObj(string filePath, object obj)
        {
            var writer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            var file = new System.IO.StreamWriter(System.IO.File.Open(filePath, FileMode.OpenOrCreate));
            writer.Serialize(file, obj);
        }

        private static object DeserializeObj(string filePath, object obj)
        {
            object retObj = new object();
            if (System.IO.File.Exists(filePath))
            {
                var writer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
                var file = new System.IO.StreamReader(filePath);
                retObj = writer.Deserialize(file);
                file.Close();
            }
            return retObj;
        }

        public static string Serialize<T>(T value)
        {

            if (value == null)
            {
                return null;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Encoding = new UnicodeEncoding(false, false), // no BOM in a .NET string
                Indent = false,
                OmitXmlDeclaration = false
            };

            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, value);
                }
                return textWriter.ToString();
            }
        }

        public static T Deserialize<T>(string xml)
        {

            if (string.IsNullOrEmpty(xml))
            {
                return default(T);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlReaderSettings settings = new XmlReaderSettings();
            // No settings need modifying here

            using (StringReader textReader = new StringReader(xml))
            {
                using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
                {
                    return (T)serializer.Deserialize(xmlReader);
                }
            }
        }
    }
}
