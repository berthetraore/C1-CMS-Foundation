﻿using System;
using System.Linq;
using System.Xml.Linq;
using Composite.Data.Types;


namespace Composite.Data.DynamicTypes.Foundation
{
    /// <summary>    
    /// </summary>
    /// <exclude />
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
    public static class DynamicTypesAlternateFormFacade
    {
        // returns null if no alternate form exists
        /// <exclude />
        public static string GetAlternateFormMarkup(DataTypeDescriptor dataTypeDescriptor)
        {
            string dynamicDataFormFolderPath = GetFolderPath(dataTypeDescriptor);
            string dynamicDataFormFileName = GetFilename(dataTypeDescriptor);

            IDynamicTypeFormDefinitionFile formOverride =
                DataFacade.GetData<IDynamicTypeFormDefinitionFile>()
                          .FirstOrDefault(f => f.FolderPath.Equals(dynamicDataFormFolderPath, StringComparison.OrdinalIgnoreCase)
                                            && f.FileName.Equals(dynamicDataFormFileName, StringComparison.OrdinalIgnoreCase));

            if (formOverride == null)
            {
                return null;
            }
            
            return formOverride.ReadAllText();
        }



        /// <exclude />
        public static void SetAlternateForm(DataTypeDescriptor dataTypeDescriptor, string newFormMarkup)
        {
            string dynamicDataFormFolderPath = GetFolderPath(dataTypeDescriptor);
            string dynamicDataFormFileName = GetFilename(dataTypeDescriptor);

            try
            {
                XDocument parsed = XDocument.Parse(newFormMarkup);
            }
            catch (Exception)
            {
                throw;
            }

            IDynamicTypeFormDefinitionFile formDefinitionFile =
                DataFacade.GetData<IDynamicTypeFormDefinitionFile>()
                  .FirstOrDefault(f => f.FolderPath.Equals(dynamicDataFormFolderPath, StringComparison.OrdinalIgnoreCase) 
                                    && f.FileName.Equals(dynamicDataFormFileName, StringComparison.OrdinalIgnoreCase));

            if (formDefinitionFile == null)
            {
                var newFile = DataFacade.BuildNew<IDynamicTypeFormDefinitionFile>();
                newFile.FolderPath = dynamicDataFormFolderPath;
                newFile.FileName = dynamicDataFormFileName;
                newFile.SetNewContent(newFormMarkup);
                formDefinitionFile = DataFacade.AddNew<IDynamicTypeFormDefinitionFile>(newFile);
            }
            else
            {
                formDefinitionFile.SetNewContent(newFormMarkup);
                DataFacade.Update(formDefinitionFile);
            }
        }



        private static string GetFilename(DataTypeDescriptor dataTypeDescriptor)
        {
            return string.Format("{0}.xml", dataTypeDescriptor.Name);
        }



        private static string GetFolderPath(DataTypeDescriptor dataTypeDescriptor)
        {
            return "\\" + dataTypeDescriptor.Namespace.Replace('.', '\\');
        }
    }
}
