using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jpp.Cedar.Core
{
    public interface ISharedParameterManager
    {
        /// <summary>
        /// Bind shared parameter to document
        /// </summary>
        /// <param name="document">Active document</param>
        /// <param name="parameter">Definition to bind</param>
        /// <param name="group">Property group to place under</param>
        void BindParameters(Document document, Definition parameter, BuiltInParameterGroup group = BuiltInParameterGroup.INVALID);

        /// <summary>
        /// Create new shared parameter
        /// </summary>
        /// <param name="groupName">Group</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterType">Type</param>
        /// <param name="editable">User editable</param>
        /// <param name="description">Description</param>
        /// <returns></returns>
        Definition RegisterParameter(string groupName, string parameterName, ParameterType parameterType, bool editable, string description);
    }
}
