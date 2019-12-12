using Autodesk.Revit.DB;
using System;

namespace Jpp.Cedar.Core
{
    public interface ISharedParameter
    {
        /// <summary>
        /// Gets parameter group name.
        /// </summary>
        string GroupName { get; }
        /// <summary>
        /// Gets parameter name.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets parameter type.
        /// </summary>
        ParameterType Type { get; }
        /// <summary>
        /// Gets parameter editable.
        /// </summary>
        bool Editable { get; }
        /// <summary>
        /// Gets parameter description.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Gets parameter id.
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// Gets parameter category.
        /// </summary>
        BuiltInCategory Category { get; }
        /// <summary>
        /// Gets parameter definition.
        /// </summary>
        ExternalDefinition Definition { get; }

        /// <summary>
        /// Registers parameter definition.
        /// </summary>
        /// <param name="manager">Shared parameter manager.</param>
        void Register(ISharedParameterManager manager);

        /// <summary>
        /// Binds parameter to document.
        /// </summary>
        /// <param name="manager">Shared parameter manager.</param>
        /// <param name="document">Active document</param>
        void Bind(ISharedParameterManager manager, Document document);
    }
}
