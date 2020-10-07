namespace Macabresoft.MonoGame.Core2D {

    using System;

    /// <summary>
    /// A component command that can be recognized and called from the editor. Must be placed on a
    /// parameterless method.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ComponentCommandAttribute : Attribute {

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentCommandAttribute" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ComponentCommandAttribute(string name) {
            this.Name = name;
        }

        /// <summary>
        /// Gets the name to appear in the editor.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }
    }
}