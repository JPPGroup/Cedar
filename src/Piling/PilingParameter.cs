// <copyright file="PilingParameter.cs" company="JPP Consulting">
// Copyright (c) JPP Consulting. All rights reserved.
// </copyright>

using System;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Jpp.Cedar.Core;

namespace Jpp.Cedar.Piling
{
    /// <summary>
    /// Summary goes here.
    /// </summary>
    internal class PilingParameter : ISharedParameter
    {
        private const BuiltInCategory CATEGORY = BuiltInCategory.OST_StructuralFoundation;
        private readonly ISharedParameterManager manager;
        private ExternalDefinition definition;

        private PilingParameter(ISharedParameterManager manager)
        {
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        /// <inheritdoc/>
        public string GroupName => "Piling";

        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public ParameterType Type { get; private set; }

        /// <inheritdoc/>
        public bool Editable { get; private set; }

        /// <inheritdoc/>
        public string Description { get; private set; }

        /// <inheritdoc/>
        public Guid Id { get; private set; }

        /// <summary>
        /// Summary goes here.
        /// </summary>
        /// <param name="manager">Shared Parameter Manager.</param>
        /// <returns>Easting Piling Parameter.</returns>
        public static PilingParameter Easting(ISharedParameterManager manager)
        {
            return new PilingParameter(manager)
            {
                Name = "Easting",
                Type = ParameterType.Length,
                Editable = false,
                Description = "Easting",
                Id = new Guid("76af35ad-70d4-41ab-bdb9-e930aea81bf3"),
            };
        }

        /// <summary>
        /// Summary goes here.
        /// </summary>
        /// <param name="manager">Shared Parameter Manager.</param>
        /// <returns>Northing Piling Parameter.</returns>
        public static PilingParameter Northing(ISharedParameterManager manager)
        {
            return new PilingParameter(manager)
            {
                Name = "Northing",
                Type = ParameterType.Length,
                Editable = false,
                Description = "Northing",
                Id = new Guid("828e2c7f-416c-452b-91ae-69c9058634a8"),
            };
        }

        /// <summary>
        /// Summary goes here.
        /// </summary>
        /// <param name="manager">Shared Parameter Manager.</param>
        /// <returns>Cut-Off Piling Parameter.</returns>
        public static PilingParameter CutOff(ISharedParameterManager manager)
        {
            return new PilingParameter(manager)
            {
                Name = "Cut-Off",
                Type = ParameterType.Length,
                Editable = false,
                Description = "Cut Off Level",
                Id = new Guid("95282567-0631-4ace-87bd-55b04ca2f222"),
            };
        }

        /// <summary>
        /// Summary goes here.
        /// </summary>
        /// <param name="manager">Shared Parameter Manager.</param>
        /// <returns>Permanent Load Piling Parameter.</returns>
        public static PilingParameter PermanentLoad(ISharedParameterManager manager)
        {
            return new PilingParameter(manager)
            {
                Name = "Permanent Load",
                Type = ParameterType.Force,
                Editable = true,
                Description = "Permanent Vertical Load",
                Id = new Guid("f2b69461-d8cf-43e1-a4e1-8c58ffdb82c1"),
            };
        }

        /// <summary>
        /// Summary goes here.
        /// </summary>
        /// <param name="manager">Shared Parameter Manager.</param>
        /// <returns>Variable Load Piling Parameter.</returns>
        public static PilingParameter VariableLoad(ISharedParameterManager manager)
        {
            return new PilingParameter(manager)
            {
                Name = "Variable Load",
                Type = ParameterType.Force,
                Editable = true,
                Description = "Variable Vertical Load",
                Id = new Guid("02ba8899-560d-479a-bacd-81ec071da663"),
            };
        }

        /// <summary>
        /// Summary goes here.
        /// </summary>
        /// <param name="manager">Shared Parameter Manager.</param>
        /// <returns>Vertical Wind Load Piling Parameter.</returns>
        public static PilingParameter VerticalWindLoad(ISharedParameterManager manager)
        {
            return new PilingParameter(manager)
            {
                Name = "Vertical Wind Load",
                Type = ParameterType.Force,
                Editable = true,
                Description = "Vertical Wind Load",
                Id = new Guid("e3e3d7ca-5a04-45fa-8f22-a1a04c8f88ad"),
            };
        }

        /// <summary>
        /// Summary goes here.
        /// </summary>
        /// <param name="manager">Shared Parameter Manager.</param>
        /// <returns>Horizontal Wind Load Piling Parameter.</returns>
        public static PilingParameter HorizontalWindLoad(ISharedParameterManager manager)
        {
            return new PilingParameter(manager)
            {
                Name = "Horizontal Wind Load",
                Type = ParameterType.Force,
                Editable = true,
                Description = "Horizontal Wind Load",
                Id = new Guid("e15c5f1f-5350-4168-8b17-72680be90c84"),
            };
        }

        /// <inheritdoc/>
        public bool TrySetParameterValue(Parameter parameter, double value)
        {
            if (parameter.Definition.Name.Equals(this.Name, StringComparison.CurrentCultureIgnoreCase))
            {
                return parameter.Set(value);
            }

            return false;
        }

        /// <inheritdoc/>
        public void Register(Application application)
        {
            this.definition = (ExternalDefinition)this.manager.RegisterParameter(application, this);
        }

        /// <inheritdoc/>
        public void Bind(Document document)
        {
            if (this.definition == null || !this.definition.IsValidObject)
            {
                this.Register(document.Application);
            }

            if (!document.ParameterBindings.Contains(this.definition))
            {
                this.manager.BindParameter(document, this.definition, CATEGORY);
            }
        }
    }
}
