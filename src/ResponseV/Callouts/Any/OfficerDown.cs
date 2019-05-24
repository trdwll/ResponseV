using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Collections.Generic;
using ResponseV.GTAV;

using System;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("OfficerDown", CalloutProbability.Always)]
    internal sealed class OfficerDown : CalloutBase
    {
        private LHandle m_Pursuit;

        private List<Vehicle> m_Officers = new List<Vehicle>();

        private ECall m_CalloutType;
        
        private Model[] m_PoliceVehicleModels;

        private Vector3 m_OfficerFireLocation;

        private bool m_bAgitated = Utils.GetRandBool();

        enum ECall : uint
        {
            C_DOWN = 1,
            C_HIT = 2,
            C_INJ = 3,
            C_SHOT = 4,
            C_STAB = 5,
            C_UFIRE = 6,
            C_FIRE = 7,
            C_MDOWN = 10
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            m_PoliceVehicleModels = Game.LocalPlayer.Character.Position.GetArea() == WorldZone.EWorldArea.Blaine_County ? g_BlaineCountyPoliceVehicles : g_LosSantosPoliceVehicles;

            uint choice = (uint)Enums.RandomEnumValue<ECall>();
            Main.MainLogger.Log($"OfficerDown: choose a random enum value {choice}");
            //m_bMultiple = choice == 10;

            m_CalloutType = (ECall)choice;

            CalloutMessage = $"Reports of {LSPDFR.Radio.GetCallStringFromEnum(Enums.ECallType.CT_OFFICERDOWN, choice)}";
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetCalloutAudio(Enums.ECallType.CT_OFFICERDOWN, choice)}", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("OfficerDown: Callout accepted");

            Ped suspect = null;
            Ped officer = null;
            Vector3 tmpVec3 = g_SpawnPoint.Around(5f);
            if (m_CalloutType != ECall.C_MDOWN)
            {
                Vehicle veh = new Vehicle(Utils.GetRandValue(m_PoliceVehicleModels), tmpVec3)
                {
                    IsPersistent = true
                };
                float heading = Extensions.GetClosestVehicleNodeHeading(veh.Position);
                veh.Heading = heading;

                officer = veh.CreateRandomDriver();
                officer.IsPersistent = true;
                officer.Kill();
                m_Officers.Add(veh);

                if (m_CalloutType != ECall.C_HIT)
                {
                    suspect = new Ped(Utils.GetRandValue(g_PedModels), g_SpawnPoint.Around(5f, 8f), MathHelper.GetRandomInteger(1, 360));
                    suspect.Tasks.FightAgainstClosestHatedTarget(30f);
                    suspect.IsPersistent = true;
                    g_Suspects.Add(suspect);
                }
            }

            switch (m_CalloutType)
            {
            case ECall.C_HIT:
                Vehicle v = new Vehicle(Utils.GetRandValue(g_Vehicles), g_SpawnPoint);
                v.IsPersistent = true;
                v.CreateRandomDriver();
                v.Driver.IsPersistent = true;
                g_Suspects.Add(v.Driver);

                float heading = Extensions.GetClosestVehicleNodeHeading(v.Position);
                v.Heading = -heading;

                if (!m_bAgitated)
                {
                    suspect.Tasks.Clear();
                }

                if (Utils.GetRandBool())
                {
                    officer.Resurrect();
                }

                officer.ApplyDamagePack(Utils.GetRandValue(DamagePack.BigHitByVehicle, DamagePack.HitByVehicle, DamagePack.BigRunOverByVehicle, DamagePack.RunOverByVehicle), 100f, 0f);
                break;

            case ECall.C_FIRE:
                officer.Position = tmpVec3.Around(2f);
                m_OfficerFireLocation = officer.Position;

                if (officer != null && Main.g_bBetterEMS)
                {
                    BetterEMS.API.EMSFunctions.OverridePedDeathDetails(officer, "", "Fire", Game.GameTime, (float)MathHelper.GetRandomDouble(0.0, 0.7));
                }

                suspect.Inventory.GiveNewWeapon(WeaponHash.Pistol, 200, false);
                suspect.Inventory.GiveNewWeapon(WeaponHash.Molotov, 5, true);

                if (Utils.GetRandBool())
                {
                    suspect.Inventory.GiveNewWeapon(WeaponHash.PetrolCan, 1, true);
                }

                officer.ApplyDamagePack(Utils.GetRandValue(
                    DamagePack.Burnt_Ped_0, DamagePack.Burnt_Ped_Head_Torso,
                    DamagePack.Burnt_Ped_Left_Arm, DamagePack.Burnt_Ped_Limbs,
                    DamagePack.Burnt_Ped_Right_Arm), 100f, 1f);
                break;

            case ECall.C_STAB:
                suspect.Inventory.GiveNewWeapon(WeaponHash.Knife, 1, true);
                break;

            case ECall.C_INJ:
            case ECall.C_DOWN:
            case ECall.C_SHOT:
                suspect.Inventory.GiveNewWeapon(Utils.GetRandValue(g_WeaponList), (short)MathHelper.GetRandomInteger(10, 60), true);

                if (officer != null && Main.g_bBetterEMS)
                {
                    BetterEMS.API.EMSFunctions.OverridePedDeathDetails(officer, "", "Gun shot wound", Game.GameTime, (float)MathHelper.GetRandomDouble(0.0, 0.7));
                }
                break;

            case ECall.C_UFIRE:
            case ECall.C_MDOWN:
                // Spawn suspects
                for (int i = 0; i < MathHelper.GetRandomInteger(1, 5); i++)
                {
                    Ped sus = new Ped(Utils.GetRandValue(g_PedModels), g_SpawnPoint.Around(5f, 10f), MathHelper.GetRandomInteger(1, 360));
                    sus.Inventory.GiveNewWeapon(Utils.GetRandValue(g_WeaponList), (short)MathHelper.GetRandomInteger(10, 60), true);
                    sus.Tasks.FightAgainstClosestHatedTarget(30f);
                    sus.IsPersistent = true;
                    g_Suspects.Add(sus);

                    if (Utils.GetRandBool())
                    {
                        sus.Kill();
                        if (sus != null && Main.g_bBetterEMS)
                        {
                            BetterEMS.API.EMSFunctions.OverridePedDeathDetails(sus, "", "Gun shot wound", Game.GameTime, (float)MathHelper.GetRandomDouble(0.0, 0.5));
                        }

                        sus.ApplyDamagePack(DamagePack.TD_PISTOL_FRONT_KILL, 100f, 0f);
                    }
                }

                // Spawn police
                for (int j = 0; j < MathHelper.GetRandomInteger(2, 5); j++)
                {
                    Vehicle veh = new Vehicle(Utils.GetRandValue(Utils.GetRandValue(m_PoliceVehicleModels)), g_SpawnPoint.Around(25f));
                    veh.IsPersistent = true;

                    Ped ofc = veh.CreateRandomDriver();
                    ofc.IsPersistent = true;

                    if (Utils.GetRandBool())
                    {
                        ofc.Kill();

                        if (ofc != null && Main.g_bBetterEMS)
                        {
                            BetterEMS.API.EMSFunctions.OverridePedDeathDetails(ofc, "", "Gun shot wound", Game.GameTime, (float)MathHelper.GetRandomDouble(0.0, 0.7));
                        }

                        ofc.ApplyDamagePack(DamagePack.TD_PISTOL_FRONT_KILL, 100f, 0f);
                    }

                    m_Officers.Add(veh);
                }
                break;
            }

            // TODO: Fix the lighting, it doesn't work atm.
            m_Officers.ForEach(o =>
            {
                if (o.VehicleModelIsELS())
                {
                    o.IsSirenOn = false;
                    o.IsSirenSilent = false;
                }
                else
                {
                    o.IsSirenOn = true;
                    o.IsSirenSilent = true;
                }
            });

            // If cops are under fire then don't request EMS
            if (m_CalloutType != ECall.C_UFIRE)
            {
                GameFiber fiber = GameFiber.StartNew(delegate
                {
                    GameFiber.Sleep(2000);
                    LSPDFR.RequestEMS(g_SpawnPoint);

                    if (m_CalloutType == ECall.C_FIRE)
                    {
                        GameFiber.Sleep(1000);
                        LSPDFR.RequestFire(g_SpawnPoint);
                    }
                }, "OfficerDownRequestEMSFireFiber");

                Main.g_GameFibers.Add(fiber);
            }

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
  
            if (g_bOnScene)
            {
                if (m_CalloutType == ECall.C_FIRE)
                {
                    World.SpawnExplosion(m_OfficerFireLocation, 3, 10.0f, false, false, 0.0f);
                    World.SpawnExplosion(m_OfficerFireLocation, 3, 10.0f, false, false, 0.0f);
                    World.SpawnExplosion(m_OfficerFireLocation, 3, 10.0f, false, false, 0.0f);
                }

                if (!g_bIsPursuit)
                {
                    if ((m_bAgitated && m_CalloutType == ECall.C_HIT && Utils.GetRandBool()) || m_CalloutType != ECall.C_HIT)// && Utils.GetRandBool())
                    {
                        OfcDown();
                    }
                }

                if (g_bIsPursuit && !Functions.IsPursuitStillRunning(m_Pursuit))
                {
                    End();
                }
                else if (m_CalloutType != ECall.C_UFIRE)
                {
                    if (!Utils.m_bCheckingEMS)
                    {
                        g_Logger.Log("OfficerDown: Checking for EMS");
                        Utils.CheckEMSOnScene(g_SpawnPoint, "Overdose");
                    }

                    if (Utils.m_bEMSOnScene)
                    {
                        g_Logger.Log("OfficerDown: EMS on Scene, end call.");

                        End();
                    }
                }
            }
        }

        void OfcDown()
        {
            g_Logger.Log("OfficerDown: Starting pursuit");

            g_bIsPursuit = true;
            GameFiber fiber = GameFiber.StartNew(delegate
            {
                m_Pursuit = Functions.CreatePursuit();
                g_Suspects.ForEach(suspect =>
                {
                    Functions.AddPedToPursuit(m_Pursuit, suspect);
                });

                Functions.SetPursuitIsActiveForPlayer(m_Pursuit, true);

                if (m_CalloutType == ECall.C_MDOWN)
                {
                    LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 3, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                    LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 1, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.SwatTeam);
                    LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 1, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.AirUnit);
                }
                else
                {
                    LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 2, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                }

            }, "OfficerDownPursuitFiber");

            Main.g_GameFibers.Add(fiber);
        }

        public override void End()
        {
            m_Officers?.ForEach(veh => veh.Dismiss());
            g_Suspects?.ForEach(suspect => suspect.Dismiss());

            g_Logger.Log("OfficerDown: End call");

            base.End();
        }
    }
}