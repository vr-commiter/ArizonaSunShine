using MelonLoader;
using HarmonyLib;
using Valve.VR.InteractionSystem;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyTrueGear;

namespace ArizonaSunShine_TrueGear
{
    public static class BuildInfo
    {
        public const string Name = "ArizonaSunShine_TrueGear"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "TrueGear Mod for ArizonaSunShine"; // Description for the Mod.  (Set as null if none)
        public const string Author = "HuangLY"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class ArizonaSunShine_TrueGear : MelonMod
    {
        private static TrueGearMod _TrueGear = null;

        private static bool isHeartBeat = false;
        private static bool isDeath = true;
        private static float healVlaue = 0;
        public override void OnInitializeMelon() {
            MelonLogger.Msg("OnApplicationStart");
            HarmonyLib.Harmony.CreateAndPatchAll(typeof(ArizonaSunShine_TrueGear));
            _TrueGear = new TrueGearMod();
        }

        public static KeyValuePair<float, float> GetAngle(Vector3 playerPosition, Vector3 hitPosition, Quaternion playerRotation)
        {
            //MelonLogger.Msg("-----------------------------------------------");
            //MelonLogger.Msg($"playerPosition | X : {playerPosition.x},Y : {playerPosition.y},Z : {playerPosition.z}");
            //MelonLogger.Msg($"hitPosition | X : {hitPosition.x},Y : {hitPosition.y},Z : {hitPosition.z}");
            Vector3 directionToHit = hitPosition - playerPosition;
            Vector3 relativeDirection = Quaternion.Inverse(playerRotation) * directionToHit;
            float angle = Mathf.Atan2(relativeDirection.x, relativeDirection.z) * Mathf.Rad2Deg;
            angle = (360f - angle) % 360f;
            //MelonLogger.Msg($"angle :{angle}");
            // 返回角度，其中正值表示右侧，负值表示左侧
            float verticalDifference = hitPosition.y - playerPosition.y;
            return new KeyValuePair<float, float>(angle, verticalDifference);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Player), "ZombieHit")]
        private static void Player_ZombieHit_Postfix(Player __instance, Zombie zombie)
        {
            if (!__instance.IsLocalPlayer)
            {
                return;
            }

            if (isDeath)
            {
                return;
            }

            Vector3 playerPosition = __instance.Transform.position;
            Vector3 hitPosition = zombie.Position;
            Quaternion playerRotation = __instance.HeadRotation;

            var angle = GetAngle(playerPosition, hitPosition, playerRotation);
            MelonLogger.Msg("-------------------------------------------");
            MelonLogger.Msg("DefaultDamage," + angle.Key + "," + angle.Value);
            //if (zombie.Locomotion.IsCrawling)
            //{
            //    MelonLogger.Msg("PlayerHitIsCrawling");
            //    return;
            //}
            //MelonLogger.Msg("PlayerHit");
            _TrueGear.PlayAngle("DefaultDamage",angle.Key,angle.Key);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Player), "Heal")]
        private static void Player_Heal_Postfix(Player __instance, float amount)
        {
            if (!__instance.IsLocalPlayer)
            {
                return;
            }
            healVlaue += amount;
            MelonLogger.Msg("amount :" + amount);
            if (healVlaue < 5f)
            {
                return;
            }
            healVlaue = 0;
            MelonLogger.Msg("-------------------------------------------");
            MelonLogger.Msg("Healing");
            _TrueGear.Play("Healing");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Player), "Kill")]
        private static void Player_Kill_Postfix(Player __instance)
        {
            if (!__instance.IsLocalPlayer)
            {
                return;
            }
            MelonLogger.Msg("-------------------------------------------");
            MelonLogger.Msg("PlayerDeath");
            _TrueGear.Play("PlayerDeath");
            isDeath = true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Player), "Update")]
        private static void Player_Update_Postfix(Player __instance)
        {
            if (!__instance.IsLocalPlayer)
            {
                return;
            }
            if (__instance.Health > 0 && isDeath)
            {
                isDeath = false;
                MelonLogger.Msg("-------------------------------------------");
                MelonLogger.Msg("Relife");
            }
            if (__instance.Health <= 33f && !isHeartBeat)
            {
                MelonLogger.Msg("-------------------------------------------");
                MelonLogger.Msg("StartHeartBeat");
                _TrueGear.StartHeartBeat();
                isHeartBeat = true;
                return;
            }
            else if (__instance.Health > 33f && isHeartBeat)
            {
                MelonLogger.Msg("-------------------------------------------");
                MelonLogger.Msg("StopHeartBeat");
                _TrueGear.StopHeartBeat();
                isHeartBeat = false;
            }
        }




        [HarmonyPostfix, HarmonyPatch(typeof(ExplosiveItem), "Explode")]
        private static void ExplosiveItem_Explode_Postfix(ExplosiveItem __instance)
        {
            MelonLogger.Msg("-------------------------------------------");
            MelonLogger.Msg("Explode1");
            _TrueGear.Play("Explosion");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(ExplosiveBehaviour), "Explode")]
        private static void ExplosiveBehaviour_Explode_Postfix(ExplosiveBehaviour __instance)
        {
            MelonLogger.Msg("-------------------------------------------");
            MelonLogger.Msg("Explode2");
            _TrueGear.Play("Explosion");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(ExplosiveHittableBehaviour), "Explode")]
        private static void ExplosiveHittableBehaviour_Explode_Postfix(ExplosiveBehaviour __instance)
        {
            MelonLogger.Msg("-------------------------------------------");
            MelonLogger.Msg("Explode3");
            _TrueGear.Play("Explosion");
        }



        [HarmonyPostfix, HarmonyPatch(typeof(Gun), "ShootBullet")]
        private static void Gun_ShootBullet_Postfix(Gun __instance, bool __result)
        {
            if (!__result)
            {
                return;
            }
            if (!__instance.IsControlledLocally)
            {
                return;
            }

            MelonLogger.Msg("-------------------------------------------");
            if (__instance.AmmoType == E_AMMO_TYPE.AMMO_BULLET)
            {
                if (__instance.IsTwoHanded && __instance.IsTwoHandedOffHandAttached)
                {
                    MelonLogger.Msg("RightHandPistolShoot");
                    MelonLogger.Msg("LeftHandPistolShoot");
                    _TrueGear.Play("RightHandPistolShoot");
                    _TrueGear.Play("LeftHandPistolShoot");
                    return;
                }
                if (__instance.EquipmentSlot.SlotID == E_EQUIPMENT_SLOT_ID.RIGHT_HAND)
                {
                    MelonLogger.Msg("RightHandPistolShoot");
                    _TrueGear.Play("RightHandPistolShoot");
                }
                else
                {
                    MelonLogger.Msg("LeftHandPistolShoot");
                    _TrueGear.Play("LeftHandPistolShoot");
                }
            }
            else if (__instance.AmmoType == E_AMMO_TYPE.AMMO_MACHINE)
            {
                if (__instance.IsTwoHanded && __instance.IsTwoHandedOffHandAttached)
                {
                    MelonLogger.Msg("RightHandRifleShoot");
                    MelonLogger.Msg("LeftHandRifleShoot");
                    _TrueGear.Play("RightHandRifleShoot");
                    _TrueGear.Play("LeftHandRifleShoot");
                    return;
                }
                if (__instance.EquipmentSlot.SlotID == E_EQUIPMENT_SLOT_ID.RIGHT_HAND)
                {
                    MelonLogger.Msg("RightHandRifleShoot");
                    _TrueGear.Play("RightHandRifleShoot");
                }
                else
                {
                    MelonLogger.Msg("LeftHandRifleShoot");
                    _TrueGear.Play("LeftHandRifleShoot");
                }
            }
            else
            {
                MelonLogger.Msg(__instance.AmmoType);
                if (__instance.IsTwoHanded && __instance.IsTwoHandedOffHandAttached)
                {
                    MelonLogger.Msg("RightHandShotgunShoot");
                    MelonLogger.Msg("LeftHandShotgunShoot");
                    _TrueGear.Play("RightHandShotgunShoot");
                    _TrueGear.Play("LeftHandShotgunShoot");
                    return;
                }
                if (__instance.EquipmentSlot.SlotID == E_EQUIPMENT_SLOT_ID.RIGHT_HAND)
                {
                    MelonLogger.Msg("RightHandShotgunShoot");
                    _TrueGear.Play("RightHandShotgunShoot");
                }
                else
                {
                    MelonLogger.Msg("LeftHandShotgunShoot");
                    _TrueGear.Play("LeftHandShotgunShoot");
                }
            }            
        }


        [HarmonyPostfix, HarmonyPatch(typeof(HandView), "OnInteractionStartedEvent")]
        private static void HandView_OnInteractionStartedEvent_Postfix(HandView __instance, EventArgs<E_EQUIPMENT_SLOT_ID, EventArgs<InteractableHandle, bool>> e)
        {
            if (!e.ValueTwo.ValueTwo)
            {
                return;
            }
            if (!__instance.IsMyHand(e.ValueOne))
            {
                return;
            }
            if(__instance.isLeftHand)
            {
                MelonLogger.Msg("-------------------------------------------");
                MelonLogger.Msg("LeftHandPickupItem");
                _TrueGear.Play("LeftHandPickupItem");
            }
            else
            {
                MelonLogger.Msg("-------------------------------------------");
                MelonLogger.Msg("RightHandPickupItem");
                _TrueGear.Play("RightHandPickupItem");
            }
        }



        [HarmonyPostfix, HarmonyPatch(typeof(EquipmentSlotBehaviour), "OnItemEquiped")]
        private static void EquipmentSlotBehaviour_OnItemEquiped_Postfix(EquipmentSlotBehaviour __instance, EquipmentSlot sender)
        {
            if (!__instance.IsLocal)
            {
                return;
            }
            if (isDeath)
            {
                return;
            }
            MelonLogger.Msg("-------------------------------------------");
            MelonLogger.Msg("OnItemEquiped");
            if (sender.SlotID == E_EQUIPMENT_SLOT_ID.RIGHT_HAND)
            {
                MelonLogger.Msg("RightHandPickupItem");
                _TrueGear.Play("RightHandPickupItem");
            }
            else if (sender.SlotID == E_EQUIPMENT_SLOT_ID.LEFT_HAND)
            {
                MelonLogger.Msg("LeftHandPickupItem");
                _TrueGear.Play("LeftHandPickupItem");
            }
            else if (sender.SlotID == E_EQUIPMENT_SLOT_ID.HOLSTER_LEFT)
            {
                MelonLogger.Msg("LeftHipSlotInputItem");
                _TrueGear.Play("LeftHipSlotInputItem");
            }
            else if (sender.SlotID == E_EQUIPMENT_SLOT_ID.HOLSTER_RIGHT)
            {
                MelonLogger.Msg("RightHipSlotInputItem");
                _TrueGear.Play("RightHipSlotInputItem");
            }
            else if (sender.SlotID == E_EQUIPMENT_SLOT_ID.CHEST_UPPER || sender.SlotID == E_EQUIPMENT_SLOT_ID.CHEST_LOWER)
            {
                MelonLogger.Msg("UpperChestSlotInputItem");
                _TrueGear.Play("UpperChestSlotInputItem");
            }
            else if (sender.SlotID == E_EQUIPMENT_SLOT_ID.HOLSTER_BACK)
            {
                MelonLogger.Msg("BackSlotInputItem");
                _TrueGear.Play("BackSlotInputItem");
            }
            MelonLogger.Msg(sender.SlotID);
        }



        [HarmonyPostfix, HarmonyPatch(typeof(EquipmentSlotBehaviour), "OnItemUnequiped")]
        private static void EquipmentSlotBehaviour_OnItemUnequiped_Postfix(EquipmentSlotBehaviour __instance, EquipmentSlot sender)
        {
            if (!__instance.IsLocal)
            {
                return;
            }
            if (sender.SlotID == E_EQUIPMENT_SLOT_ID.LEFT_HAND || sender.SlotID == E_EQUIPMENT_SLOT_ID.RIGHT_HAND || sender.SlotID == E_EQUIPMENT_SLOT_ID.OTHER)
            {
                return;
            }
            if (isDeath)
            {
                return;
            }

            MelonLogger.Msg("-------------------------------------------");
            MelonLogger.Msg("OnItemUnequiped");
            if (sender.SlotID == E_EQUIPMENT_SLOT_ID.HOLSTER_LEFT)
            {
                MelonLogger.Msg("LeftHipSlotOutputItem");
                _TrueGear.Play("LeftHipSlotOutputItem");
            }
            else if (sender.SlotID == E_EQUIPMENT_SLOT_ID.HOLSTER_RIGHT)
            {
                MelonLogger.Msg("RightHipSlotOutputItem");
                _TrueGear.Play("RightHipSlotOutputItem");
            }
            else if (sender.SlotID == E_EQUIPMENT_SLOT_ID.CHEST_UPPER || sender.SlotID == E_EQUIPMENT_SLOT_ID.CHEST_LOWER)
            {
                MelonLogger.Msg("UpperChestSlotOutputItem");
                _TrueGear.Play("UpperChestSlotOutputItem");
            }
            else if (sender.SlotID == E_EQUIPMENT_SLOT_ID.HOLSTER_BACK)
            {
                MelonLogger.Msg("BackSlotOutputItem");
                _TrueGear.Play("BackSlotOutputItem");
            }
            MelonLogger.Msg(sender.SlotID);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(AmmoDispenserItem), "AddBullets")]
        private static void AmmoDispenserItem_AddBullets_Postfix(AmmoDispenserItem __instance,int __result)
        {
            if (!__instance.CurrentEquippedPlayer.IsLocalPlayer)
            {
                return;
            }
            MelonLogger.Msg("-------------------------------------------");
            MelonLogger.Msg("AddBullets");
            _TrueGear.Play("ChestSlotInputItem");
            MelonLogger.Msg(__result);
        }


        [HarmonyPostfix, HarmonyPatch(typeof(AmmoDispenserItem), "RemoveBullets")]
        private static void AmmoDispenserItem_RemoveBullets_Postfix(AmmoDispenserItem __instance,int __result)
        {
            if (!__instance.CurrentEquippedPlayer.IsLocalPlayer)
            {
                return;
            }
            MelonLogger.Msg("-------------------------------------------");
            MelonLogger.Msg("RemoveBullets");
            _TrueGear.Play("ChestSlotOutputItem");
            MelonLogger.Msg(__result);
        }












    }
}