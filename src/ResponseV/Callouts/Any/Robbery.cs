using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("Robbery", CalloutProbability.VeryHigh)]
    internal sealed class Robbery : CalloutBase
    {
        private Vehicle m_Vehicle;
        private LHandle m_Pursuit;

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Reports of an Armed Robbery";
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} CRIME_ARMED_ROBBERY IN_OR_ON_POSITION", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_Vehicle = new Vehicle(Utils.GetRandValue(g_Vehicles), g_SpawnPoint);
            g_Suspects.Add(m_Vehicle.CreateRandomDriver());

            if (Utils.GetRandBool())
            {
                for (int i = 0; i < Utils.GetRandInt(1, 2); i++)
                {
                    g_Suspects.Add(new Ped(Utils.GetRandValue(g_PedModels), g_SpawnPoint, Utils.GetRandInt(1, 360)));
                }

                g_Suspects.ForEach(s => s.Tasks.EnterVehicle(m_Vehicle, -2)); // Doesn't work for some reason
            }

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(g_SpawnPoint) < 50 && !g_bIsPursuit)
            {
                g_bOnScene = true;

                m_Vehicle.Driver.Tasks.CruiseWithVehicle(Utils.GetRandInt(40, 80));

                Pursuit();
            }

            if (g_bIsPursuit && !Functions.IsPursuitStillRunning(m_Pursuit))
            {
                End();
            }
        }

        void Pursuit()
        {
            g_bIsPursuit = true;
            GameFiber fiber = GameFiber.StartNew(delegate
            {
                m_Pursuit = Functions.CreatePursuit();

                g_Suspects.ForEach(s =>
                {
                    Functions.AddPedToPursuit(m_Pursuit, s);
                    s.Inventory.GiveNewWeapon(Utils.GetRandValue(WeaponHash.AssaultRifle, WeaponHash.CombatMG, WeaponHash.MicroSMG, WeaponHash.Pistol), 200, true);

                    if (Utils.GetRandBool())
                    {
                        // TODO: fix this to have the peds shoot at nearest police vehicle (or we could just make them shoot at us)
                        s.Tasks.FireWeaponAt(Vector3.RandomUnit, 10, FiringPattern.BurstFireDriveby);
                    }
                });

                Functions.SetPursuitIsActiveForPlayer(m_Pursuit, true);

                GameFiber.Sleep(5000);
                LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 3, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
            }, "RobberyPursuitFiber");

            Main.g_GameFibers.Add(fiber);
        }
    }
}