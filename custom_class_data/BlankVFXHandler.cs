using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dungeonator;
using UnityEngine;
using Alexandria;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Scytodes;
using System.Linq;
using System.Text;

namespace Scytodes
{
    public class BlankVFXHandler : MonoBehaviour
    {
        public static float? s_MaxRadiusLimiter;

        private Camera m_camera;

        private Material m_distortionMaterial;

        private float dIntensity;

        private float dRadius;

        public bool ForceNoDamage = true;

        public bool UsesCustomProjectileCallback;

        public Action<Projectile> OnCustomBlankedProjectile;

        public void TriggerSilencer(Vector2 centerPoint, float expandSpeed, float maxRadius, GameObject silencerVFX, float distIntensity, float distRadius, float pushForce, float pushRadius, float knockbackForce, float knockbackRadius, float additionalTimeAtMaxRadius, PlayerController user, bool breaksWalls = true, bool skipBreakables = false)
        {
            bool flag = true;
            float num = 10f;
            float num2 = 7f;
            float num3 = 1f;
            if (maxRadius < 5f)
            {
                flag = true;
                num = 10f;
                num2 = maxRadius;
            }
            float? num4 = s_MaxRadiusLimiter;
            if (num4.HasValue)
            {
                maxRadius = s_MaxRadiusLimiter.Value;
            }
            dIntensity = distIntensity;
            dRadius = distRadius;
            m_camera = GameManager.Instance.MainCameraController.GetComponent<Camera>();
            if (silencerVFX != null)
            {
                GameObject obj = UnityEngine.Object.Instantiate(silencerVFX, centerPoint.ToVector3ZUp(centerPoint.y), Quaternion.identity);
                UnityEngine.Object.Destroy(obj, 1f);
            }
            Exploder.DoRadialPush(centerPoint.ToVector3ZUp(), pushForce, pushRadius);
            Exploder.DoRadialKnockback(centerPoint.ToVector3ZUp(), knockbackForce * num3, knockbackRadius);
            if (!skipBreakables)
            {
                Exploder.DoRadialMinorBreakableBreak(centerPoint.ToVector3ZUp(), knockbackRadius);
            }
            if (breaksWalls)
            {
                RoomHandler absoluteRoomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(centerPoint.ToIntVector2(VectorConversions.Floor));
                for (int j = 0; j < StaticReferenceManager.AllMajorBreakables.Count; j++)
                {
                    if (StaticReferenceManager.AllMajorBreakables[j].IsSecretDoor)
                    {
                        RoomHandler absoluteRoomFromPosition2 = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(StaticReferenceManager.AllMajorBreakables[j].transform.position.IntXY(VectorConversions.Floor));
                        if (absoluteRoomFromPosition2 == absoluteRoomFromPosition)
                        {
                            StaticReferenceManager.AllMajorBreakables[j].ApplyDamage(1E+10f, Vector2.zero, isSourceEnemy: false, isExplosion: true, ForceDamageOverride: true);
                            StaticReferenceManager.AllMajorBreakables[j].ApplyDamage(1E+10f, Vector2.zero, isSourceEnemy: false, isExplosion: true, ForceDamageOverride: true);
                            StaticReferenceManager.AllMajorBreakables[j].ApplyDamage(1E+10f, Vector2.zero, isSourceEnemy: false, isExplosion: true, ForceDamageOverride: true);
                        }
                    }
                }
            }
            if (flag && !ForceNoDamage)
            {
                Exploder.DoRadialDamage(num, centerPoint.ToVector3ZUp(), num2, damagePlayers: false, damageEnemies: true);
            }
            if (distIntensity > 0f)
            {
                m_distortionMaterial = new Material(ShaderCache.Acquire("Brave/Internal/DistortionWave"));
                Vector4 centerPointInScreenUV = GetCenterPointInScreenUV(centerPoint);
                m_distortionMaterial.SetVector("_WaveCenter", centerPointInScreenUV);
                Pixelator.Instance.RegisterAdditionalRenderPass(m_distortionMaterial);
            }
            if (maxRadius > 10f)
            {
                List<BulletScriptSource> allBulletScriptSources = StaticReferenceManager.AllBulletScriptSources;
                for (int k = 0; k < allBulletScriptSources.Count; k++)
                {
                    BulletScriptSource bulletScriptSource = allBulletScriptSources[k];
                    if (!bulletScriptSource.IsEnded && bulletScriptSource.RootBullet != null && bulletScriptSource.RootBullet.EndOnBlank)
                    {
                        bulletScriptSource.ForceStop();
                    }
                }
                ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
                for (int num5 = allProjectiles.Count - 1; num5 >= 0; num5--)
                {
                    Projectile projectile = allProjectiles[num5];
                    if ((bool)projectile.braveBulletScript && projectile.braveBulletScript.bullet != null && projectile.braveBulletScript.bullet.EndOnBlank)
                    {
                        if (UsesCustomProjectileCallback && OnCustomBlankedProjectile != null)
                        {
                            OnCustomBlankedProjectile(projectile);
                        }
                        projectile.DieInAir(suppressInAirEffects: false, allowActorSpawns: true, allowProjectileSpawns: true, killedEarly: true);
                    }
                }
            }
            Pixelator.Instance.StartCoroutine(BackupDistortionCleanup());
            StartCoroutine(HandleSilence(centerPoint, expandSpeed, maxRadius, additionalTimeAtMaxRadius, user));
        }

        private Vector4 GetCenterPointInScreenUV(Vector2 centerPoint)
        {
            Vector3 vector = m_camera.WorldToViewportPoint(centerPoint.ToVector3ZUp());
            return new Vector4(vector.x, vector.y, dRadius, dIntensity);
        }

        private IEnumerator HandleSilence(Vector2 centerPoint, float expandSpeed, float maxRadius, float additionalTimeAtMaxRadius, PlayerController user)
        {
            float currentRadius = 0f;
            //float previousRadius = 0f;
            while (currentRadius < maxRadius)
            {
                currentRadius += expandSpeed * BraveTime.DeltaTime;
                if (m_distortionMaterial != null)
                {
                    Vector4 centerPointInScreenUV = GetCenterPointInScreenUV(centerPoint);
                    m_distortionMaterial.SetVector("_WaveCenter", centerPointInScreenUV);
                    m_distortionMaterial.SetFloat("_DistortProgress", currentRadius / maxRadius);
                }
                yield return null;
            }
            CleanupDistortion();
            float elapsed = 0f;
            while (elapsed < additionalTimeAtMaxRadius)
            {
                elapsed += BraveTime.DeltaTime;
                bool pVP_ENABLED = GameManager.PVP_ENABLED;
                float? previousRadius2 = maxRadius;
                yield return null;
            }
            UnityEngine.Object.Destroy(gameObject);
        }

        private void CleanupDistortion()
        {
            if (Pixelator.Instance != null && m_distortionMaterial != null)
            {
                Pixelator.Instance.DeregisterAdditionalRenderPass(m_distortionMaterial);
                UnityEngine.Object.Destroy(m_distortionMaterial);
                m_distortionMaterial = null;
            }
        }

        private void OnDestroy()
        {
            CleanupDistortion();
        }

        private IEnumerator BackupDistortionCleanup()
        {
            yield return new WaitForSeconds(3f);
            CleanupDistortion();
        }
    }
}
