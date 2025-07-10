using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalReactionData 
{
    public float damage = 0;
    public ReactionType reactionType = ReactionType.None;

    public ElementalReactionData(float damage, ReactionType reactionType)
    {
        this.damage = damage;
        this.reactionType = reactionType;
    }
}
