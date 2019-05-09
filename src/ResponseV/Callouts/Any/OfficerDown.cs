using System.Linq;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Collections.Generic;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("OfficerDown", CalloutProbability.Low)]
    public class OfficerDown : Callout
    {
        private Vector3 m_SpawnPoint;
        private LHandle m_Pursuit;
        private Blip m_Blip;

        private List<Vehicle> m_Officers = new List<Vehicle>();
        private List<Ped> m_Suspects = new List<Ped>();

        private Enums.Callout m_State;
        private Enums.CalloutType m_CalloutType;
        
        private bool m_bMultiple;

        private Model[] m_VehicleModels = { "police", "police2", "police3", "police4", "sheriff", "sheriff2" };
        private WeaponHash[] m_WeaponList = { WeaponHash.AssaultRifle, WeaponHash.Pistol, WeaponHash.SawnOffShotgun };

        public override bool OnBeforeCalloutDisplayed()
        {
            m_SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(400, 550)));

            ShowCalloutAreaBlipBeforeAccepting(m_SpawnPoint, 25f);

            m_bMultiple = Utils.GetRandBool();

            CalloutMessage = "Reports of " + (m_bMultiple ? "Multiple Officers": "an Officer") + " Down";
            CalloutPosition = m_SpawnPoint;

            string aud;
            if (!m_bMultiple)
            {
                m_CalloutType = Utils.GetRandValue(Enums.CalloutType.Shooting, Enums.CalloutType.Stabbing, Enums.CalloutType.Unknown);

                switch (m_CalloutType)
                {
                    default:
                    case Enums.CalloutType.Unknown: aud = LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.OFFICER_DOWN); break;
                    case Enums.CalloutType.Shooting: aud = LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.OFFICER_SHOT); break;
                    case Enums.CalloutType.Stabbing: aud = "CRIME_OFFICER_STABBED";  break;
                }
            }
            else
            {
                m_CalloutType = Enums.CalloutType.Shooting;
                aud = LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.OFFICERS_DOWN);
            }

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} {aud} IN_OR_ON_POSITION", m_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_State = Enums.Callout.EnRoute;

            // TODO: Move this into Process?
            Ped suspect;
            Vehicle veh;
            switch (m_CalloutType)
            {
                default:
                case Enums.CalloutType.Unknown:
                case Enums.CalloutType.Stabbing:
                    veh = new Vehicle(Utils.GetRandValue(m_VehicleModels), m_SpawnPoint);
                    veh.CreateRandomDriver();
                    veh.Driver.Kill();

                    m_Officers.Add(veh);

                    suspect = new Ped(Utils.GetRandValue(RageMethods.GetPedModels()), m_SpawnPoint, Utils.GetRandInt(1, 360));

                    if (m_CalloutType == Enums.CalloutType.Stabbing)
                    {
                        suspect.Inventory.GiveNewWeapon(WeaponHash.Knife, 1, true);
                        suspect.Tasks.FightAgainstClosestHatedTarget(30f);
                    }
                    m_Suspects.Add(suspect);
                    break;
                case Enums.CalloutType.Shooting:
                    if (m_bMultiple)
                    {
                        // Spawn suspects
                        for (int i = 0; i < Utils.GetRandInt(1, 5); i++)
                        {
                            Ped sus = new Ped(Utils.GetRandValue(RageMethods.GetPedModels()), m_SpawnPoint, Utils.GetRandInt(1, 360));
                            sus.Inventory.GiveNewWeapon(Utils.GetRandValue(m_WeaponList), (short)Utils.GetRandInt(10, 60), true);
                            sus.Tasks.FightAgainstClosestHatedTarget(30f);
                            m_Suspects.Add(sus);
                        }

                        // Spawn police
                        for (int j = 0; j < Utils.GetRandInt(2, 5); j++)
                        {
                            Vehicle veh2 = new Vehicle(Utils.GetRandValue(m_VehicleModels), m_SpawnPoint.Around(10f));
                            veh2.CreateRandomDriver();
                            if (Utils.GetRandBool())
                            {
                                veh2.Driver.Kill();
                            }

                            m_Officers.Add(veh2);
                        }
                    }
                    else
                    {
                        veh = new Vehicle(Utils.GetRandValue(m_VehicleModels), m_SpawnPoint);
                        veh.CreateRandomDriver();
                        veh.Driver.Kill();

                        m_Officers.Add(veh);

                        suspect = new Ped(Utils.GetRandValue(RageMethods.GetPedModels()), m_SpawnPoint, Utils.GetRandInt(1, 360));
                        suspect.Inventory.GiveNewWeapon(Utils.GetRandValue(m_WeaponList), (short)Utils.GetRandInt(10, 60), true);
                        m_Suspects.Add(suspect);
                    }
                    break;
            }

            // Turn on police car lighting
            m_Officers.ForEach(v =>
            {
                v.IsSirenOn = true;
                v.IsSirenSilent = true;
                v.IsEngineOn = true;
            });

            m_Blip = new Blip(m_SpawnPoint)
            {
                IsRouteEnabled = true
            };
            m_Blip.EnableRoute(System.Drawing.Color.Blue);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            base.Process();
  
            if (m_State == Enums.Callout.EnRoute && Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) < 70)
            {
                m_State = Enums.Callout.OnScene;

                m_Blip.IsRouteEnabled = false;

                OfcDown();
            }

            if (m_State == Enums.Callout.InPursuit)
            {
                if (!Functions.IsPursuitStillRunning(m_Pursuit))
                {
                    m_State = Enums.Callout.Done;
                    End();
                }
            }
        }

        void OfcDown()
        {
            GameFiber.StartNew(delegate
            {
                m_Pursuit = Functions.CreatePursuit();
                m_Suspects.ForEach(suspect =>
                {
                    Functions.AddPedToPursuit(m_Pursuit, suspect);
                });
                m_Blip.Delete();

                m_State = Enums.Callout.InPursuit;

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
            });
        }

        public override void End()
        {
            m_Blip.Delete();
            m_Officers.ForEach(veh => veh.Dismiss());
            m_Suspects.ForEach(suspect => suspect.Dismiss());

            base.End();
        }
    }
}