using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UI_Test_Tool.Utility
{
    public static class AutomationUtility
    {
        /// <summary>
        /// Dictionary to hold the values from our file
        /// read as [fieldname, fieldvalue]
        /// </summary>
        public static Dictionary<string, string> dictionary = new Dictionary<string, string>();
        
        //base working directory, needs to be set before starting the test!!! 
        //--> MAKE SURE THIS ENDS WITH "\\" "C:\\Users\\erik\\Desktop\\PublishTemplate\\test\\"; //
        public const string baseDir = "\\\\scheduling-win-001\\Publishing_data\\Production\\3400GG\\1.4.0\\GooglePlay\\";
        //public const string xmlfile = "GooglePlayPublish.xml";
        
        /// <summary>
        /// Method to read the data from XML file
        /// </summary>
        /// <param name="path"></param>
        public static void ReadFromXml(string path)
        {
            try
            { 
                var database = XDocument.Load(path);
                
                //Generate an array from our XML file, from all entries marked tagged "def"
                IEnumerable<XElement> defs = from xml in database.Descendants("def") select xml;
                   
                //iterate through all our elements and add them as key-value pairs to the dictionary
                foreach (XElement def in defs)
                {
                    string fieldname = def.Attribute("fieldname").Value;
                    string fieldvalue = System.Net.WebUtility.HtmlDecode(def.Attribute("fieldvalue").Value);
                    
                    //Write to console for debugging
                    Console.WriteLine("fieldname: {0}, fieldvalue: {1}", fieldname, fieldvalue);
                    //Write to logfile
                    Program.log.WriteLine(Log.LogLevel.Info, String.Format("fieldname: {0}, fieldvalue: {1}", fieldname, fieldvalue));
                    
                    //Add the key and value to dictionary
                    dictionary.Add(fieldname, fieldvalue);
                }                
            }
            catch (Exception e) 
            {
                //Let the user know what went wrong.
                Console.WriteLine("The file(s) could not be read:");
                Console.WriteLine(e.Message);

                //Log the error
                Program.log.WriteLine(Log.LogLevel.Error, String.Format("The file(s) could not be read:"));
                Program.log.WriteLine(Log.LogLevel.Error, String.Format(e.Message));
            }            
        }
        
        /// <summary>
        /// Method to write specified data to the XML file
        /// </summary>
        /// <param name="path">xml file path</param>
        /// <param name="key">xml fieldname -> also the key in our dictionary</param>
        /// <param name="value">xml fieldvalue -> also the value of the given key</param>
        public static void WriteToXml(string path, string key, string value)
        {
            try
            {
                var database = XDocument.Load(path);
                IEnumerable<XElement> defs = from xml in database.Descendants("def") select xml;
                
                foreach (XElement def in defs)
                {
                    if (def.Attribute("fieldname").Value == key)
                    {
                        Console.WriteLine("Fieldname: " + key);
                        Console.WriteLine("Fieldvalue: " + value);

                        Program.log.WriteLine(Log.LogLevel.Info, "Fieldname: " + key);
                        Program.log.WriteLine(Log.LogLevel.Info, "Fieldvalue: " + value);

                        def.Attribute("fieldvalue").Value = value;
                    }
                }
                
                database.Save(path);
            }
            catch (Exception e)
            {
                //Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);

                //Log the error
                Program.log.WriteLine(Log.LogLevel.Error, String.Format("The file(s) could not be read:"));
                Program.log.WriteLine(Log.LogLevel.Error, String.Format(e.Message));
            }
        }
    }
}
