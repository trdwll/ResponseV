using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Collections.Generic;
using ResponseV.GTAV;

using System;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("OfficerDown", CalloutProbability.VeryHigh)]
    internal sealed class OfficerDown : CalloutBase
    {
        private LHandle m_Pursuit;

        private List<Vehicle> m_Officers = new List<Vehicle>();

        private ECall m_CalloutType;
        
        private bool m_bMultiple;

        private Model[] m_PoliceVehicleModels;

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
            m_bMultiple = choice == 10;

            CalloutMessage = $"Reports of {LSPDFR.Radio.GetCallStringFromEnum(Enums.ECallType.CT_OFFICERDOWN, choice)}";
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetCalloutAudio(Enums.ECallType.CT_OFFICERDOWN, m_bMultiple ? 10 : choice)}", g_SpawnPoint);

            if (!m_bMultiple)
            {
                m_CalloutType = (ECall)choice;

                Main.MainLogger.Log($"OfficerDown: Casted m_CalloutType to {m_CalloutType}");
            }

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("OfficerDown: Callout accepted");

            Ped suspect = null;
            if (!m_bMultiple)
            {
                Vehicle veh = new Vehicle(Utils.GetRandValue(m_PoliceVehicleModels), g_SpawnPoint);
                veh.CreateRandomDriver();
                veh.Driver.Kill();
                m_Officers.Add(veh);

                suspect = new Ped(Utils.GetRandValue(g_PedModels), g_SpawnPoint, MathHelper.GetRandomInteger(1, 360));
                suspect.Tasks.FightAgainstClosestHatedTarget(30f);
                g_Suspects.Add(suspect);
            }

            switch (m_CalloutType)
            {
            default:
                // TODO: Spawn police car and kill officer
                break;

            case ECall.C_HIT:
                Vehicle v = new Vehicle(Utils.GetRandValue(g_Vehicles), g_SpawnPoint);
                v.CreateRandomDriver();

                g_Suspects.Add(v.Driver);
                break;

            case ECall.C_FIRE:
                break;

            case ECall.C_INJ:
                break;

            case ECall.C_STAB:
                suspect.Inventory.GiveNewWeapon(WeaponHash.Knife, 1, true);
                break;

            case ECall.C_DOWN:
            case ECall.C_SHOT:
                suspect.Inventory.GiveNewWeapon(Utils.GetRandValue(g_WeaponList), (short)MathHelper.GetRandomInteger(10, 60), true);
                break;

            case ECall.C_UFIRE:
            case ECall.C_MDOWN:
                // Spawn suspects
                for (int i = 0; i < MathHelper.GetRandomInteger(1, 5); i++)
                {
                    Ped sus = new Ped(Utils.GetRandValue(g_PedModels), g_SpawnPoint, MathHelper.GetRandomInteger(1, 360));
                    sus.Inventory.GiveNewWeapon(Utils.GetRandValue(g_WeaponList), (short)MathHelper.GetRandomInteger(10, 60), true);
                    sus.Tasks.FightAgainstClosestHatedTarget(30f);
                    g_Suspects.Add(sus);
                }

                // Spawn police
                for (int j = 0; j < MathHelper.GetRandomInteger(2, 5); j++)
                {
                    Vehicle veh = new Vehicle(Utils.GetRandValue(Utils.GetRandValue(m_PoliceVehicleModels)), g_SpawnPoint.Around(10f));
                    veh.CreateRandomDriver();
                    if (Utils.GetRandBool())
                    {
                        veh.Driver.Kill();
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

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
  
            if (g_bOnScene && !g_bIsPursuit)
            {
                OfcDown();
            }

            if (g_bIsPursuit && !Functions.IsPursuitStillRunning(m_Pursuit))
            {
                End();
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

                if (m_bMultiple)
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