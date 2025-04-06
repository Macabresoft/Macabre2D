namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Interface for a collection of <see cref="SpriteSheetMember" />.
/// </summary>
public interface ISpriteSheetMemberCollection : INameableCollection, INotifyCollectionChanged, INotifyPropertyChanged {

    /// <summary>
    /// Gets the type of members in this collection.
    /// </summary>
    Type MemberType { get; }

    /// <summary>
    /// Adds the member if it is the correct type.
    /// </summary>
    /// <param name="member">The member.</param>
    void AddMember(SpriteSheetMember member);

    /// <summary>
    /// Inserts the member if it is the correct type.
    /// </summary>
    /// <param name="index">The index to insert the member at.</param>
    /// <param name="member">The member.</param>
    void InsertMember(int index, SpriteSheetMember member);

    /// <summary>
    /// Moves the specified item to the new index.
    /// </summary>
    /// <param name="originalIndex">The original index.</param>
    /// <param name="newIndex">The new index.</param>
    void Move(int originalIndex, int newIndex);

    /// <summary>
    /// Removes a member if it is the correct type.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <returns>A value indicating whether the member was removed.</returns>
    public bool RemoveMember(SpriteSheetMember member);

    /// <summary>
    /// Creates a new member without adding it to the collection.
    /// </summary>
    /// <param name="member">The created member.</param>
    /// <returns>A value indicating whether a new member could be created.</returns>
    bool TryCreateNewMember([NotNullWhen(true)] out SpriteSheetMember? member);
}

/// <summary>
/// A generic base class for a collection of <see cref="SpriteSheetMember" />.
/// </summary>
/// <typeparam name="TMember"></typeparam>
public abstract class SpriteSheetMemberCollection<TMember> : NameableCollection<TMember>, ISpriteSheetMemberCollection where TMember : SpriteSheetMember {

    /// <inheritdoc />
    public Type MemberType => typeof(TMember);

    /// <inheritdoc />
    public void AddMember(SpriteSheetMember member) {
        if (member is TMember actualMember) {
            this.Add(actualMember);
        }
    }

    /// <inheritdoc />
    public void InsertMember(int index, SpriteSheetMember member) {
        if (member is TMember actualMember) {
            this.Insert(index, actualMember);
        }
    }

    /// <inheritdoc />
    public bool RemoveMember(SpriteSheetMember member) => member is TMember actualMember && this.Remove(actualMember);

    /// <inheritdoc />
    public abstract bool TryCreateNewMember([NotNullWhen(true)] out SpriteSheetMember? member);
}