namespace Macabresoft.Macabre2D.Framework {
    using System.Runtime.Serialization;

    [DataContract]
    public class GameProject {
        
        [DataMember]
        public GameSettings GameSettings { get; } = new();
    }
}