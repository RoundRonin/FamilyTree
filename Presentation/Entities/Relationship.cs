﻿using FamilyTreeBlazor.presentation.Infrastructure;

namespace FamilyTreeBlazor.presentation.Entities;

public enum RelationshipType
{
    Parent,
    Spouse
}

public class Relationship(int PersonId1, int PersonId2, RelationshipType RelationshipType): IEntity
{
    public int Id { get; set; }
    public int PersonId1 { get; set; } = PersonId1;
    public int PersonId2 { get; set; } = PersonId2;
    public RelationshipType RelationshipType { get; set; } = RelationshipType;
}