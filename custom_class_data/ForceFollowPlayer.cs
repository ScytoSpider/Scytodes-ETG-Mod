using Alexandria;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Dungeonator;
using Scytodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/* MASSIVE THANK YOU TO CAPTAIN PRETZEL IN THE "Mod The Gungeon" DISCORD SERVER!
 * HE WROTE THIS CODE! I DON'T UNDERSTAND IT ONE BIT BUT IT WORKS EXACTLY HOW I WANTED IT TO!
 * https://discord.gg/Ne9yh5Krsh */

public class ForceMoveTowardsTargetBehavior : OverrideBehaviorBase
{
    public float PathInterval = 0.25f;
    public float DesiredDistance = 500f;
    public bool CanAttackWhileMoving = false;

    public static readonly List<string> BannedEnemies = [
        EnemyGUIDs.Mountain_Cube_GUID,
        EnemyGUIDs.Lead_Cube_GUID,
        EnemyGUIDs.Flesh_Cube_GUID,
        EnemyGUIDs.Agonizer_GUID,
        EnemyGUIDs.Det_GUID,
        EnemyGUIDs.X_Det_GUID,
        EnemyGUIDs.Diagonal_X_Det_GUID,
        EnemyGUIDs.Vertical_Det_GUID,
        EnemyGUIDs.Diagonal_Det_GUID,
        EnemyGUIDs.Horizontal_Det_GUID,
        EnemyGUIDs.Vertical_X_Det_GUID,
        EnemyGUIDs.Horizontal_X_Det_GUID,
        EnemyGUIDs.Lead_Maiden_GUID,
        EnemyGUIDs.Fridge_Lead_Maiden_GUID,
        EnemyGUIDs.AIActor_Lord_Of_The_Jammed_GUID,
        EnemyGUIDs.Ser_Junkan_GUID,
        EnemyGUIDs.Baby_Good_Shelleton_Companion_GUID,
        EnemyGUIDs.Blank_Companion_GUID,
        EnemyGUIDs.Chicken_Companion_GUID,
        EnemyGUIDs.Phoenix_Companion_GUID,
        EnemyGUIDs.Blank_Companion_Synergy_Forme_GUID,
        EnemyGUIDs.Super_Space_Turtle_GUID,
        EnemyGUIDs.Dog_GUID,
        EnemyGUIDs.Cop_GUID,
        EnemyGUIDs.Wolf_GUID,
        EnemyGUIDs.Harmless_Snake_GUID,
        EnemyGUIDs.Pig_Cannon_Synergy_Forme_GUID,
        EnemyGUIDs.Dog_Puppy_Synergy_GUID,
        EnemyGUIDs.Wolf_Puppy_Synergy_GUID,
        EnemyGUIDs.Android_Cop_GUID,
        EnemyGUIDs.Raccoon_GUID,
        EnemyGUIDs.Turtle_GUID,
        EnemyGUIDs.Turkey_GUID,
        EnemyGUIDs.Paday_Shooter_1_GUID,
        EnemyGUIDs.Paday_Shooter_2_GUID,
        EnemyGUIDs.Paday_Shooter_3_GUID,
        EnemyGUIDs.Baby_Good_Mimic_GUID,

    ];

    private float m_repathTimer;
    private IntVector2? m_targetPos;
    private IntVector2 m_cachedPlayerCell;

    public override void Upkeep()
    {
        base.Upkeep();
        DecrementTimer(ref m_repathTimer);
    }

    public override bool OverrideOtherBehaviors()
    {
        return true;
    }

    public override BehaviorResult Update()
    {
        if (!m_targetPos.HasValue && m_repathTimer > 0f) { return BehaviorResult.Continue; }
        if (m_aiActor.TargetRigidbody) { m_targetPos = m_aiActor.TargetRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor); }
        else { m_targetPos = null; }
        if (!m_targetPos.HasValue) { return BehaviorResult.Continue; }
        if (m_repathTimer <= 0f)
        {
            if (m_targetPos.HasValue && m_aiActor.TargetRigidbody)
            {
                m_repathTimer = PathInterval;
                m_cachedPlayerCell = m_aiActor.TargetRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor);
                m_aiActor.PathfindToPosition(m_targetPos.Value.ToCenterVector2(), null, true, null, overridePathableTiles: (CellTypes.FLOOR | CellTypes.PIT));
            }
        }
        if (!m_targetPos.HasValue) { return BehaviorResult.Continue; }
        return CanAttackWhileMoving ? BehaviorResult.SkipRemainingClassBehaviors : BehaviorResult.SkipAllRemainingBehaviors;
    }

    private bool CellValidator(IntVector2 c)
    {
        for (int i = 0; i < m_aiActor.Clearance.x; i++)
        {
            for (int j = 0; j < m_aiActor.Clearance.y; j++)
            {
                if (GameManager.Instance.Dungeon.data.isTopWall(c.x + i, c.y + j)) { return false; }
            }
        }
        return true;
    }

    public static IEnumerator ForceFollowPlayer(AIActor actor, PlayerController playerTarget, float timeToFollowPlayer)
    {
        if (!actor || !playerTarget || timeToFollowPlayer <= 0) { yield break; } // nothing to do
        if (actor.behaviorSpeculator is not BehaviorSpeculator bs) { yield break; } // no behaviors to override
        if (actor.healthHaver is not HealthHaver hh1 || hh1.IsBoss || hh1.IsSubboss) { yield break; } // don't target bosses or minibosses
        if (actor is not AIActor enemy || BannedEnemies.Contains(enemy.EnemyGuid)) { yield break; } // don't target blacklisted enemies

        // remember who we were tracking before
        SpeculativeRigidbody oldOverrideTarget = actor.OverrideTarget;
        actor.OverrideTarget = playerTarget.specRigidbody;

        // set up the new override to forcibly track the player
        ForceMoveTowardsTargetBehavior forceMovement = new ForceMoveTowardsTargetBehavior();
        bs.OverrideBehaviors.Add(forceMovement);
        bs.RefreshBehaviors();

        // wait
        yield return new WaitForSeconds(timeToFollowPlayer);

        // restore our old target and remove the override behavior
        if (oldOverrideTarget) { actor.OverrideTarget = oldOverrideTarget; }
        else { actor.OverrideTarget = null; }
        bs.OverrideBehaviors.Remove(forceMovement);
        bs.RefreshBehaviors();

        // done
        yield break;
    }
}

