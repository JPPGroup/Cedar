using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

namespace Jpp.Cedar.Core
{
    public interface ISharedParameterManager
    {
        /// <summary>
        /// Bind shared parameter to document
        /// </summary>
        /// <param name="document">Active document</param>
        /// <param name="definition">Definition to bind..</param>
        /// <param name="category">Category to bind to.</param>
        void BindParameter(Document document, Definition definition, BuiltInCategory category);

        /// <summary>
        /// Create new shared parameter
        /// </summary>
        /// <param name="parameter">Shared parameter to register.</param>
        /// <returns>Registered definition.</returns>
        Definition RegisterParameter(Application application, ISharedParameter parameter);
    }
}
