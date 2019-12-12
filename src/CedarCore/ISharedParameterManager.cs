using Autodesk.Revit.DB;

namespace Jpp.Cedar.Core
{
    public interface ISharedParameterManager
    {
        /// <summary>
        /// Bind shared parameter to document
        /// </summary>
        /// <param name="document">Active document</param>
        /// <param name="parameter">Shared parameter to bind</param>
        void BindParameter(Document document, ISharedParameter parameter);

        /// <summary>
        /// Create new shared parameter
        /// </summary>
        /// <param name="parameter">Shared parameter to register.</param>
        /// <returns></returns>
        ExternalDefinition RegisterParameter(ISharedParameter parameter);
    }
}
