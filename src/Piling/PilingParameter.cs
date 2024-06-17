using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using JPP.Cedar.Core;
using System;

namespace JPP.Cedar.Piling
{
    internal class PilingParameter : ISharedParameter
    {
        private const BuiltInCategory CATEGORY = BuiltInCategory.OST_StructuralFoundation;

        private ExternalDefinition _definition;
        private ISharedParameterManager _manager;

        /// <inheritdoc/> 
        public string GroupName => "Piling";
        /// <inheritdoc/> 
        public string Name { get; private set; }
#if REVIT2022 || REVIT2021 || REVIT2020 || REVIT2019 || REVIT2017
        /// <inheritdoc/> 
        public ParameterType Type { get; private set; }
#else 
        /// <inheritdoc/> 
        public ForgeTypeId Type { get; private set; }
#endif
        /// <inheritdoc/> 
        public bool Editable { get; private set; }
        /// <inheritdoc/> 
        public string Description { get; private set; }
        /// <inheritdoc/> 
        public Guid Id { get; private set; }

        private PilingParameter(ISharedParameterManager manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        /// <inheritdoc/> 
        public bool TrySetParameterValue(Parameter parameter, double value)
        {
            if (parameter.Definition.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase))
            {
                return parameter.Set(value);
            }

            return false;
        }

        /// <inheritdoc/> 
        public void Register(Application application)
        {
            _definition = (ExternalDefinition)_manager.RegisterParameter(application, this);
        }

        /// <inheritdoc/> 
        public void Bind(Document document)
        {
            if (_definition == null || !_definition.IsValidObject)
                Register(document.Application);

            _manager.BindParameter(document, _definition, CATEGORY);
        }

        public static PilingParameter Easting(ISharedParameterManager manager)
        {
            return new PilingParameter(manager)
            {
                Name = "Easting",
#if REVIT2022 || REVIT2021 || REVIT2020 || REVIT2019 || REVIT2017
                Type = ParameterType.Length,  
#else
                Type = SpecTypeId.Length,
#endif
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
#if REVIT2022 || REVIT2021 || REVIT2020 || REVIT2019 || REVIT2017
                Type = ParameterType.Length,  
#else
                Type = SpecTypeId.Length,
#endif
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
#if REVIT2022 || REVIT2021 || REVIT2020 || REVIT2019 || REVIT2017
                Type = ParameterType.Length,  
#else
                Type = SpecTypeId.Length,
#endif
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
#if REVIT2022 || REVIT2021 || REVIT2020 || REVIT2019 || REVIT2017
                Type = ParameterType.Force,  
#else
                Type = SpecTypeId.Force,
#endif
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
#if REVIT2022 || REVIT2021 || REVIT2020 || REVIT2019 || REVIT2017
                Type = ParameterType.Force,  
#else
                Type = SpecTypeId.Force,
#endif
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
#if REVIT2022 || REVIT2021 || REVIT2020 || REVIT2019 || REVIT2017
                Type = ParameterType.Force,  
#else
                Type = SpecTypeId.Force,
#endif
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
#if REVIT2022 || REVIT2021 || REVIT2020 || REVIT2019 || REVIT2017
                Type = ParameterType.Force,  
#else
                Type = SpecTypeId.Force,
#endif
                Editable = true,
                Description = "Horizontal Wind Load",
                Id = new Guid("e15c5f1f-5350-4168-8b17-72680be90c84")
            };
        }
    }
}
