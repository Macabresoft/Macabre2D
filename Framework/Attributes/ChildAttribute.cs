namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// An attribute which tags a field or property as a child.
    /// </summary>
    /// <seealso cref="System.Attribute"/>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ChildAttribute : Attribute {

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildAttribute"/> class.
        /// </summary>
        public ChildAttribute() : this(true, null) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ChildAttribute(string name) : this(true, name) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildAttribute"/> class.
        /// </summary>
        /// <param name="useExisting">if set to <c>true</c> [use existing].</param>
        public ChildAttribute(bool useExisting) : this(useExisting, null) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildAttribute"/> class.
        /// </summary>
        /// <param name="useExisting">if set to <c>true</c> [use existing].</param>
        /// <param name="name">The name.</param>
        public ChildAttribute(bool useExisting, string name) {
            this.Name = name;
            this.UseExisting = useExisting;
        }

        /// <summary>
        /// Gets the name of the child. If this is set to null, then name does not matter and the
        /// first child of the specified type will be found.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the current component should use an existing child;
        /// otherwise, it will create a new child.
        /// </summary>
        /// <value><c>true</c> if the component should use an existing child; otherwise, <c>false</c>.</value>
        public bool UseExisting { get; }
    }
}