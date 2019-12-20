// <copyright file="PilingCoordinator.cs" company="JPP Consulting">
// Copyright (c) JPP Consulting. All rights reserved.
// </copyright>

using System;
using Autodesk.Revit.DB;
using Jpp.Cedar.Core;
using Jpp.Cedar.Piling.Properties;

namespace Jpp.Cedar.Piling
{
    /// <summary>
    /// Summary goes here.
    /// </summary>
    public class PilingCoordinator
    {
        private readonly FailureDefinitionId warnIsFamilyDocumentId;
        private readonly FailureDefinition warnIsFamilyDocumentDef;
        private readonly PilingUpdater pilingUpdater;
        private readonly CoordinatePilingUpdater coordinatePilingUpdater;
        private readonly ISharedParameter easting;
        private readonly ISharedParameter northing;
        private readonly ISharedParameter cutOff;
        private readonly ISharedParameter permanentLoad;
        private readonly ISharedParameter variableLoad;
        private readonly ISharedParameter verticalWindLoad;
        private readonly ISharedParameter horizontalWindLoad;

        /// <summary>
        /// Initializes a new instance of the <see cref="PilingCoordinator"/> class.
        /// </summary>
        /// <param name="addInId">Add-In Id param.</param>
        /// <param name="spManager">Shared Parameter Manager param.</param>
        public PilingCoordinator(AddInId addInId, ISharedParameterManager spManager)
        {
            this.easting = PilingParameter.Easting(spManager);
            this.northing = PilingParameter.Northing(spManager);
            this.cutOff = PilingParameter.CutOff(spManager);
            this.permanentLoad = PilingParameter.PermanentLoad(spManager);
            this.variableLoad = PilingParameter.VariableLoad(spManager);
            this.verticalWindLoad = PilingParameter.VerticalWindLoad(spManager);
            this.horizontalWindLoad = PilingParameter.HorizontalWindLoad(spManager);

            this.warnIsFamilyDocumentId = new FailureDefinitionId(new Guid("2c644284-59fe-4f5c-b8b3-e89977af7d15"));
            this.warnIsFamilyDocumentDef = FailureDefinition.CreateFailureDefinition(this.warnIsFamilyDocumentId, FailureSeverity.Warning, Resources.FailureDefinition_WarnIsFamilyMessage);

            this.pilingUpdater = PilingUpdater.Register(addInId, this);
            this.coordinatePilingUpdater = CoordinatePilingUpdater.Register(addInId, this);
        }

        /// <summary>
        /// Summary goes here.
        /// </summary>
        public void UnRegister()
        {
            this.pilingUpdater.UnRegister();
            this.coordinatePilingUpdater.UnRegister();
        }

        /// <summary>
        /// Summary goes here.
        /// </summary>
        /// <param name="document">Document param.</param>
        internal void RegisterDocument(Document document)
        {
            this.easting.Bind(document);
            this.northing.Bind(document);
            this.cutOff.Bind(document);
            this.permanentLoad.Bind(document);
            this.variableLoad.Bind(document);
            this.verticalWindLoad.Bind(document);
            this.horizontalWindLoad.Bind(document);
        }

        /// <summary>
        /// Summary goes here.
        /// </summary>
        /// <param name="document">Document param.</param>
        /// <param name="id">Element Id param.</param>
        internal void UpdateElement(Document document, ElementId id)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (document.IsFamilyDocument)
            {
                this.PostWarningIsFamilyDocument(document, id);
            }
            else
            {
                Element foundation = document.GetElement(id);

                if (foundation.Location != null)
                {
                    if (foundation.Location is LocationPoint locationPoint)
                    {
                        XYZ location = CoordinateHelper.GetWorldCoordinates(document, locationPoint.Point);

                        this.UpdateParameters(foundation, location);
                    }
                }
            }
        }

        private void UpdateParameters(Element foundation, XYZ location)
        {
            foreach (Parameter para in foundation.Parameters)
            {
                this.easting.TrySetParameterValue(para, location.X);
                this.northing.TrySetParameterValue(para, location.Y);
                this.cutOff.TrySetParameterValue(para, location.Z);
            }
        }

        private void PostWarningIsFamilyDocument(Document document, ElementId id)
        {
            using (FailureMessage message = new FailureMessage(this.warnIsFamilyDocumentId))
            {
                message.SetFailingElement(id);
                document.PostFailure(message);
            }
        }
    }
}
