﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Composite.IO;
using System;


namespace Composite.WebClient
{
    /// <exclude />
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
    public enum CompositeScriptMode
    {
        OPERATE,
        DEVELOP,
        COMPILE,
    };


    /// <exclude />
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
    public static class ScriptHandler
    {
        private static string _compileScriptsFilename = "CompileScripts.xml";



        public static string MergeScripts(string type, IEnumerable<string> scriptFilenames, string folderPath, string targetPath)
        {
            string sourcesFilename = targetPath + "\\" + type + "-uncompressed.js";

            StringBuilder newLine = new StringBuilder();
            newLine.AppendLine();
            newLine.AppendLine();

            FileEx.RemoveReadOnly(sourcesFilename);

            File.WriteAllText(sourcesFilename, GetTimestampString());

            foreach (string scriptFilename in scriptFilenames)
            {
                string scriptPath = scriptFilename.Replace("${root}", folderPath).Replace("/", "\\");

                string lines = File.ReadAllText(scriptPath);

                
                File.AppendAllText(sourcesFilename, lines);
                File.AppendAllText(sourcesFilename, newLine.ToString());
            }

            return sourcesFilename;
        }



        public static string BuildTopLevelClassNames(IEnumerable<string> scriptFilenames, string folderPath, string targetPath)
        {
            StringBuilder classes = new StringBuilder();

            classes.AppendLine("var topLevelClassNames = [ // Don't edit! This file is automatically generated.");

            bool first = true;
            foreach (string scriptFilename in scriptFilenames)
            {
                string scriptPath = scriptFilename.Replace("${root}", folderPath);
                if (scriptPath.IndexOf("/scripts/source/page/") == -1)
                {
                    if (first == true)
                    {
                        first = false;
                    }
                    else
                    {
                        classes.AppendLine(",");
                    }

                    int _start = scriptPath.LastIndexOf("/") + 1;
                    int _length = scriptPath.LastIndexOf(".js") - _start;

                    string className = scriptPath.Substring(_start, _length);

                    classes.Append("\t\"" + className + "\"");
                }
            }

            classes.AppendLine("];");

            string classesFilename = targetPath + "\\" + "toplevelclassnames.js";

            FileEx.RemoveReadOnly(classesFilename);

            File.WriteAllText(classesFilename, GetTimestampString());
            File.AppendAllText(classesFilename, classes.ToString());

            return classesFilename;
        }



        public static IEnumerable<string> GetTopScripts(CompositeScriptMode scriptMode, string folderPath)
        {
            IEnumerable<string> result = GetStrings("top", scriptMode.ToString().ToLower(), folderPath);

            return result;
        }



        public static IEnumerable<string> GetSubScripts(CompositeScriptMode scriptMode, string folderPath)
        {
            IEnumerable<string> result = GetStrings("sub", scriptMode.ToString().ToLower(), folderPath);

            return result;
        }



        private static IEnumerable<string> GetStrings(string type, string mode, string folderPath)
        {
            string filename = Path.Combine(folderPath, _compileScriptsFilename);

            XDocument doc = XDocument.Load(filename);

            if (mode == "compile") mode = "develop";

            XElement topElement = doc.Root.Elements().Where(f => f.Attribute("name").Value == type).Single();
            XElement modeElement = topElement.Elements().Where(f => f.Attribute("name").Value == mode).Single();

            return
                from e in modeElement.Elements()
                select e.Attribute("filename").Value;
        }



        private static string GetTimestampString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/*");
            sb.AppendLine(" * Created: " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
            sb.AppendLine(" */");
            sb.AppendLine();
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
