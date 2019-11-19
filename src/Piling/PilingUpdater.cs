using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;
using Jpp.Cedar.Core;

namespace Jpp.Cedar.Piling
{
    public class PilingUpdater : IUpdater
    {
        AddInId _appId;
        UpdaterId _updaterId;
        bool registered = false;
        public Definition eastingDefintiion { get; set; }

        public PilingUpdater(AddInId id)
        {
            _appId = id;
            _updaterId = new UpdaterId(_appId, new Guid("ddb23f37-892e-4b43-9e8a-0ad8ff381b2b"));
        }

        public static void Register(UIControlledApplication application)
        {
            PilingUpdater updater = new PilingUpdater(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater);
            
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filter, Element.GetChangeTypeGeometry());

            //TODO: Move to config file
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\JPP Consulting\\Cedar\\SharedParameters.txt";
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\JPP Consulting\\Cedar";
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(directory);
                FileStream fs = File.Create(path);
                fs.Close();
            }

            application.ControlledApplication.SharedParametersFilename = path;
            DefinitionFile defFile = application.ControlledApplication.OpenSharedParameterFile();

            
            DefinitionGroup pilingGroup = defFile.Groups.get_Item("Piling");
            if (pilingGroup == null)
            {
                pilingGroup = defFile.Groups.Create("Piling");
            }

            updater.eastingDefintiion = pilingGroup.Definitions.get_Item("Instance_Easting");

            if (updater.eastingDefintiion == null)
            {
                ExternalDefinitionCreationOptions easting = new ExternalDefinitionCreationOptions("Instance_Easting", ParameterType.Length);
                easting.UserModifiable = false;
                easting.Description = "Wall product date";
                updater.eastingDefintiion = pilingGroup.Definitions.Create(easting);
            }
        }

        public void RegisterDocument(Document document)
        {       
            // create a category set and insert category of wall to it
            CategorySet myCategories = document.Application.Create.NewCategorySet();
            // use BuiltInCategory to get category of wall
            Category myCategory = Category.GetCategory(document, BuiltInCategory.OST_StructuralFoundation);

            myCategories.Insert(myCategory);

            //Create an instance of InstanceBinding
            InstanceBinding instanceBinding = document.Application.Create.NewInstanceBinding(myCategories);

            // Get the BingdingMap of current document.
            BindingMap bindingMap = document.ParameterBindings;

            // Bind the definitions to the document
            bool instanceBindOK = bindingMap.Insert(eastingDefintiion,
                instanceBinding, BuiltInParameterGroup.INVALID);
        }

        public void Execute(UpdaterData data)
        {

            Document document = data.GetDocument();

            if (!registered)
                RegisterDocument(document);

            List<ElementId> modifiedElementIds = new List<ElementId>();
            modifiedElementIds.AddRange(data.GetAddedElementIds());
            modifiedElementIds.AddRange(data.GetModifiedElementIds());

            foreach (ElementId id in modifiedElementIds)
            {
                Element foundation = document.GetElement(id);
                if (foundation.Location != null)
                {
                    XYZ location = CoordinateHelper.GetWorldCoordinates(document, (foundation.Location as LocationPoint).Point);
                   
                    foreach (Parameter para in foundation.Parameters)
                    {
                        if (para.Definition.Name.Equals("Instance_Easting", StringComparison.OrdinalIgnoreCase))
                        {
                            //para.SetValueString(location.X.ToString());
                            para.Set(location.X);
                        }
                    }
                }
            }


            // Cache the wall type
            /*if (m_wallType == null)
            {
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                collector.OfClass(typeof(WallType));
                var wallTypes = from element in collector
                    where
                        element.Name == "Exterior - Brick on CMU"
                    select element;
                if (wallTypes.Count() > 0)
                {
                    m_wallType = wallTypes.Cast().ElementAt(0);
                }
            }

            if (m_wallType != null)
            {
                // Change the wall to the cached wall type.
                foreach (ElementId addedElemId in data.GetAddedElementIds())
                {
                    Wall wall = doc.GetElement(addedElemId) as Wall;
                    if (wall != null)
                    {
                        wall.WallType = m_wallType;
                    }
                }
            }*/
        }

        public string GetAdditionalInformation()
        {
            return "Updates all pile coordinates";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.Structure;
        }

        public UpdaterId GetUpdaterId()
        {
            return _updaterId;
        }

        public string GetUpdaterName()
        {
            return "Cedar Pile Updater";
        }
    }
}
