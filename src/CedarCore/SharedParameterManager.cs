using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System;
using System.Diagnostics;
using System.IO;

namespace Jpp.Cedar.Core
{
    public class SharedParameterManager : ISharedParameterManager
    {
        private Application _application;

        public SharedParameterManager(Application application)
        {
            _application = application;
        }

        public void BindParameter(Document document, Definition definition, BuiltInCategory category)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            try
            {
                // create a category set and insert category of wall to it
                CategorySet myCategories = _application.Create.NewCategorySet();
                // use BuiltInCategory to get category of wall
                Category myCategory = Category.GetCategory(document, category);

                myCategories.Insert(myCategory);

                //Create an instance of InstanceBinding
                InstanceBinding instanceBinding = _application.Create.NewInstanceBinding(myCategories);

                // Get the BingdingMap of current document.
                BindingMap bindingMap = document.ParameterBindings;

                // Bind the definitions to the document
                //bool instanceBindOK = bindingMap.Insert(parameter, instanceBinding, group);
                //var test = RegisterParameter("Piling", "Easting", ParameterType.Length, false, "Easting");
                bool instanceBindOK = bindingMap.Insert(definition, instanceBinding);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public Definition RegisterParameter(ISharedParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            Definition result;

            //TODO: Move to config file
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\JPP Consulting\\Cedar\\SharedParameters.txt";
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\JPP Consulting\\Cedar";
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(directory);
                FileStream fs = File.Create(path);
                fs.Close();
            }

            string currentPath = _application.SharedParametersFilename;
            _application.SharedParametersFilename = path;
            //Why does disposing the file throw an invalid exception?!?!?!?!?!?
            /*using (DefinitionFile defFile = _application.OpenSharedParameterFile())
            {*/
            DefinitionFile defFile = _application.OpenSharedParameterFile();
                DefinitionGroup pilingGroup = defFile.Groups.get_Item(parameter.GroupName);
                if (pilingGroup == null)
                {
                    pilingGroup = defFile.Groups.Create(parameter.GroupName);
                }

                result = pilingGroup.Definitions.get_Item(parameter.Name);

                if (result == null)
                {
                    ExternalDefinitionCreationOptions newDefinition = new ExternalDefinitionCreationOptions(parameter.Name, parameter.Type);
                    newDefinition.UserModifiable = parameter.Editable;
                    newDefinition.Description = parameter.Description;
                    newDefinition.GUID = parameter.Id;
                    result = pilingGroup.Definitions.Create(newDefinition);
                }

                _application.SharedParametersFilename = currentPath;
                return result;
            //}
        }
    }
}
