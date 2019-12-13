using Autodesk.Revit.DB;
using Jpp.Cedar.Core;
using System;

namespace Jpp.Cedar.Piling
{
    internal class PilingParameter : ISharedParameter
    {
        private const BuiltInCategory CATEGORY = BuiltInCategory.OST_StructuralFoundation;

        private ExternalDefinition _definition;
        private ISharedParameterManager _manager;

        public string GroupName => "Piling";
        public string Name { get; private set; }
        public ParameterType Type { get; private set; }
        public bool Editable { get; private set; }
        public string Description { get; private set; }
        public Guid Id { get; private set; }
        
        private PilingParameter(ISharedParameterManager manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public void Register()
        {
            _definition = (ExternalDefinition)_manager.RegisterParameter(this);
        }

        public void Bind(Document document)
        {
            if (_definition == null || !_definition.IsValidObject)
                Register();

            _manager.BindParameter(document, _definition, CATEGORY);
        }

        public static PilingParameter Easting(ISharedParameterManager manager)
        {
            return new PilingParameter(manager)
            {
                Name = "Easting",
                Type = ParameterType.Length,
                Editable = false,
                Description = "Easting",
                Id = new Guid("76af35ad-70d4-41ab-bdb9-e930aea81bf3")
            };
        }

        public static PilingParameter Northing(ISharedParameterManager manager)
        {
            return new PilingParameter(manager)
            {
                Name = "Northing",
                Type = ParameterType.Length,
                Editable = false,
                Description = "Northing",
                Id = new Guid("828e2c7f-416c-452b-91ae-69c9058634a8")
            };
        }

        public static PilingParameter CutOff(ISharedParameterManager manager)
        {
            return new PilingParameter(manager)
            {
                Name = "Cut-Off",
                Type = ParameterType.Length,
                Editable = false,
                Description = "Cut Off Level",
                Id = new Guid("95282567-0631-4ace-87bd-55b04ca2f222")
            };
        }

        public static PilingParameter PermanentLoad(ISharedParameterManager manager)
        {
            return new PilingParameter(manager)
            {
                Name = "Permanent Load",
                Type = ParameterType.Force,
                Editable = true,
                Description = "Permanent Vertical Load",
                Id = new Guid("f2b69461-d8cf-43e1-a4e1-8c58ffdb82c1")
            };
        }

        public static PilingParameter VariableLoad(ISharedParameterManager manager)
        {
            return new PilingParameter(manager)
            {
                Name = "Variable Load",
                Type = ParameterType.Force,
                Editable = true,
                Description = "Variable Vertical Load",
                Id = new Guid("02ba8899-560d-479a-bacd-81ec071da663")
            };
        }

        public static PilingParameter VerticalWindLoad(ISharedParameterManager manager)
        {
            return new PilingParameter(manager)
            {
                Name = "Vertical Wind Load",
                Type = ParameterType.Force,
                Editable = true,
                Description = "Vertical Wind Load",
                Id = new Guid("e3e3d7ca-5a04-45fa-8f22-a1a04c8f88ad")
            };
        }

        public static PilingParameter HorizontalWindLoad(ISharedParameterManager manager)
        {
            return new PilingParameter(manager)
            {
                Name = "Horizontal Wind Load",
                Type = ParameterType.Force,
                Editable = true,
                Description = "Horizontal Wind Load",
                Id = new Guid("e15c5f1f-5350-4168-8b17-72680be90c84")
            };
        }
    }
}
