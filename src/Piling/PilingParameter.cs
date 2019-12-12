using Autodesk.Revit.DB;
using Jpp.Cedar.Core;
using System;

namespace Jpp.Cedar.Piling
{
    internal class PilingParameter : ISharedParameter
    {
        public string GroupName => "Piling";
        public string Name { get; private set; }
        public ParameterType Type { get; private set; }
        public bool Editable { get; private set; }
        public string Description { get; private set; }
        public Guid Id { get; private set; }
        public BuiltInCategory Category => BuiltInCategory.OST_StructuralFoundation;
        public ExternalDefinition Definition { get; private set; }

        private PilingParameter() { }

        public void Register(ISharedParameterManager manager)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            Definition = manager.RegisterParameter(this);
        }

        public void Bind(ISharedParameterManager manager, Document document)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            if (Definition == null || !Definition.IsValidObject)
                Register(manager);

            manager.BindParameter(document, this);
        }

        public static PilingParameter Easting()
        {
            return new PilingParameter
            {
                Name = "Easting",
                Type = ParameterType.Length,
                Editable = false,
                Description = "Easting",
                Id = new Guid("76af35ad-70d4-41ab-bdb9-e930aea81bf3")
            };
        }

        public static PilingParameter Northing()
        {
            return new PilingParameter
            {
                Name = "Northing",
                Type = ParameterType.Length,
                Editable = false,
                Description = "Northing",
                Id = new Guid("828e2c7f-416c-452b-91ae-69c9058634a8")
            };
        }

        public static PilingParameter CutOff()
        {
            return new PilingParameter
            {
                Name = "Cut-Off",
                Type = ParameterType.Length,
                Editable = false,
                Description = "Cut Off Level",
                Id = new Guid("95282567-0631-4ace-87bd-55b04ca2f222")
            };
        }

        public static PilingParameter PermanentLoad()
        {
            return new PilingParameter
            {
                Name = "Permanent Load",
                Type = ParameterType.Force,
                Editable = true,
                Description = "Permanent Vertical Load",
                Id = new Guid("f2b69461-d8cf-43e1-a4e1-8c58ffdb82c1")
            };
        }

        public static PilingParameter VariableLoad()
        {
            return new PilingParameter
            {
                Name = "Variable Load",
                Type = ParameterType.Force,
                Editable = true,
                Description = "Variable Vertical Load",
                Id = new Guid("02ba8899-560d-479a-bacd-81ec071da663")
            };
        }

        public static PilingParameter VerticalWindLoad()
        {
            return new PilingParameter
            {
                Name = "Vertical Wind Load",
                Type = ParameterType.Force,
                Editable = true,
                Description = "Vertical Wind Load",
                Id = new Guid("e3e3d7ca-5a04-45fa-8f22-a1a04c8f88ad")
            };
        }

        public static PilingParameter HorizontalWindLoad()
        {
            return new PilingParameter
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
