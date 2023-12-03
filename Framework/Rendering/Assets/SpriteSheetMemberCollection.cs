namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Specialized;
using System.ComponentModel;

/// <summary>
/// Interface for a collection of <see cref="SpriteSheetMember" />.
/// </summary>
public interface ISpriteSheetMemberCollection : INameableCollection, INotifyCollectionChanged, INotifyPropertyChanged {
    /// <summary>
    /// Adds the member if it is the correct type.
    /// </summary>
    /// <param name="member">The member.</param>
    void AddMember(SpriteSheetMember member);

    /// <summary>
    /// Adds a new member.
    /// </summary>
    /// <returns>The newly added member.</returns>
    public SpriteSheetMember AddNewMember();

    /// <summary>
    /// Creates a new member without adding it to the collection.
    /// </summary>
    /// <returns>The member.</returns>
    public SpriteSheetMember CreateNewMember();

    /// <summary>
    /// Inserts the member if it is the correct type.
    /// </summary>
    /// <param name="index">The index to insert the member at.</param>
    /// <param name="member">The member.</param>
    void InsertMember(int index, SpriteSheetMember member);

    /// <summary>
    /// Removes a member if it is the correct type.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <returns>A value indicating whether or not the member was removed.</returns>
    public bool RemoveMember(SpriteSheetMember member);
}

/// <summary>
/// A generic base class for a collection of <see cref="SpriteSheetMember" />.
/// </summary>
/// <typeparam name="TMember"></typeparam>
public abstract class SpriteSheetMemberCollection<TMember> : NameableCollection<TMember>, ISpriteSheetMemberCollection where TMember : SpriteSheetMember, new() {
    /// <inheritdoc />
    public void AddMember(SpriteSheetMember member) {
        if (member is TMember actualMember) {
            this.Add(actualMember);
        }
    }

    /// <inheritdoc />
    public SpriteSheetMember AddNewMember() {
        var member = new TMember();
        this.Add(member);
        return member;
    }

    /// <inheritdoc />
    public SpriteSheetMember CreateNewMember() {
        return new TMember();
    }

    /// <inheritdoc />
    public void InsertMember(int index, SpriteSheetMember member) {
        if (member is TMember actualMember) {
            this.Insert(index, actualMember);
        }
    }

    /// <inheritdoc />
    public bool RemoveMember(SpriteSheetMember member) {
        return member is TMember actualMember && this.Remove(actualMember);
    }
}