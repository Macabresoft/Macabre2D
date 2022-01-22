namespace Macabresoft.Macabre2D.Gameplay;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;

public class MyFirstEntity : Entity {
    [DataMember(Name = "Message", Order = 100)]
    public string Text { get; set; } = "Hello, World!";
}