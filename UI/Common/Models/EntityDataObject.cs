namespace Macabresoft.Macabre2D.UI.Common.Models {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Avalonia.Input;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// A <see cref="IDataObject" /> that wraps an <see cref="IEntity" />.
    /// </summary>
    public class EntityDataObject : IDataObject {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityDataObject" /> class.
        /// </summary>
        public EntityDataObject(IEntity entity) {
            this.Entity = entity ?? throw new ArgumentNullException(nameof(entity));
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        public IEntity Entity { get; }

        /// <inheritdoc />
        public bool Contains(string dataFormat) {
            return false;
        }

        /// <inheritdoc />
        public object Get(string dataFormat) {
            return this.Entity;
        }

        public IEnumerable<string> GetDataFormats() {
            return Enumerable.Empty<string>();
        }

        /// <inheritdoc />
        public IEnumerable<string>? GetFileNames() {
            return Enumerable.Empty<string>();
        }

        /// <inheritdoc />
        public string GetText() {
            return this.Entity.Name;
        }
    }
}