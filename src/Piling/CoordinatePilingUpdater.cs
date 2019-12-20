// <copyright file="CoordinatePilingUpdater.cs" company="JPP Consulting">
// Copyright (c) JPP Consulting. All rights reserved.
// </copyright>

using System;
using Autodesk.Revit.DB;

namespace Jpp.Cedar.Piling
{
    /// <summary>
    /// Summary goes here.
    /// </summary>
    public class CoordinatePilingUpdater : IUpdater, IDisposable
    {
        private readonly UpdaterId updaterId; // TODO: Investigate analyzer warning as the field is disposed.
        private readonly PilingCoordinator pilingCoordinator;

        private bool disposed;

        private CoordinatePilingUpdater(AddInId id, PilingCoordinator coordinator)
        {
            this.updaterId = new UpdaterId(id, new Guid("a066aabd-7ccd-43c3-9a86-b2089ebabb99"));
            this.pilingCoordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
        }

        /// <inheritdoc/>
        public void Execute(UpdaterData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Document document = data.GetDocument();

            this.pilingCoordinator.RegisterDocument(document);

            using (FilteredElementCollector elementCollector = new FilteredElementCollector(document))
            {
                FilteredElementCollector foundationCollection = elementCollector.OfCategory(BuiltInCategory.OST_StructuralFoundation);

                foreach (Element element in foundationCollection)
                {
                    this.pilingCoordinator.UpdateElement(document, element.Id);
                }
            }
        }

        /// <inheritdoc/>
        public string GetAdditionalInformation()
        {
            return "Updates all pile coordinates in response to project coordinate change";
        }

        /// <inheritdoc/>
        public ChangePriority GetChangePriority()
        {
            return ChangePriority.Structure;
        }

        /// <inheritdoc/>
        public UpdaterId GetUpdaterId()
        {
            return this.updaterId;
        }

        /// <inheritdoc/>
        public string GetUpdaterName()
        {
            return "Cedar Coordinate Pile Updater";
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Summary goes here.
        /// </summary>
        /// <param name="addInId">AddInId param.</param>
        /// <param name="coordinator">PilingCoordinator param.</param>
        /// <returns>Registered piliing updater.</returns>
        internal static CoordinatePilingUpdater Register(AddInId addInId, PilingCoordinator coordinator)
        {
            CoordinatePilingUpdater updater = new CoordinatePilingUpdater(addInId, coordinator);
            UpdaterRegistry.RegisterUpdater(updater);

            using (ElementCategoryFilter basePointFilter = new ElementCategoryFilter(BuiltInCategory.OST_ProjectBasePoint))
            {
                UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), basePointFilter, Element.GetChangeTypeAny());

                return updater;
            }
        }

        /// <summary>
        /// Summary goes here.
        /// </summary>
        internal void UnRegister()
        {
            UpdaterRegistry.UnregisterUpdater(this.GetUpdaterId());
        }

        /// <summary>
        /// Add summary here.
        /// </summary>
        /// <param name="disposing">Disposing param.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.updaterId.Dispose();
                }

                this.disposed = true;
            }
        }
    }
}
