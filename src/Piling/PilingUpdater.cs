// <copyright file="PilingUpdater.cs" company="JPP Consulting">
// Copyright (c) JPP Consulting. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Jpp.Cedar.Piling
{
    /// <summary>
    /// Summary goes here.
    /// </summary>
    public class PilingUpdater : IUpdater, IDisposable
    {
        private readonly UpdaterId updaterId; // TODO: Investigate analyzer warning as the field is disposed.
        private readonly PilingCoordinator pilingCoordinator;

        private bool disposed;

        private PilingUpdater(AddInId id, PilingCoordinator coordinator)
        {
            this.updaterId = new UpdaterId(id, new Guid("ddb23f37-892e-4b43-9e8a-0ad8ff381b2b"));
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

            List<ElementId> modifiedElementIds = new List<ElementId>();
            modifiedElementIds.AddRange(data.GetAddedElementIds());
            modifiedElementIds.AddRange(data.GetModifiedElementIds());

            foreach (ElementId id in modifiedElementIds)
            {
                this.pilingCoordinator.UpdateElement(document, id);
            }
        }

        /// <inheritdoc/>
        public string GetAdditionalInformation()
        {
            return "Updates all pile coordinates";
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
            return "Cedar Pile Updater";
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
        internal static PilingUpdater Register(AddInId addInId, PilingCoordinator coordinator)
        {
            PilingUpdater updater = new PilingUpdater(addInId, coordinator);
            UpdaterRegistry.RegisterUpdater(updater);

            using (ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation))
            {
                UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filter, Element.GetChangeTypeGeometry());
                UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filter, Element.GetChangeTypeElementAddition());

                return updater;
            }
        }

        /// <summary>
        /// Summary goes here.
        /// </summary>
        internal void Unregister()
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
