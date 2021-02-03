namespace Macabresoft.Macabre2D.Framework {
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class ContentMetadata {
        public const string FileExtension = ".meta";

        [DataMember]
        private readonly List<IAsset> _assets = new();
    }
}