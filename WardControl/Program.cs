using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ensage;
using Ensage.Common;
using SharpDX;

namespace WardControl
{
    class Program
    {
        static bool adaward = false;
        static bool wcd = false;
        static Vector3 Position = new Vector3();
        static Vector2 Size = new Vector2(75, 50);
        static String namaward;
        static int[] dispen = new int[5];
        static int[] dispen2 = new int[5];
        static bool[] inven = new bool[5];
        static bool[] bdispen = new bool[5];

        static List<Hero> ldispen = new List<Hero>();
        static List<Hero> linven = new List<Hero>();

        public static void Main(string[] args)
        {
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += Game_OnUpdate;
        }

        public static void Game_OnUpdate(EventArgs args)
        {
            if (!Game.IsInGame)
                return;
            Hero me = ObjectMgr.LocalHero;
            int index = 0;
            List<Hero> enemies = ObjectMgr.GetEntities<Hero>().Where(x => x.Team != me.Team).ToList();
            foreach (Hero enemy in enemies)
            {
                if (!enemy.IsVisible && (ldispen.Contains(enemy) || linven.Contains(enemy)))
                {
                    ldispen.Remove(enemy);
                    linven.Remove(enemy);
                    dispen[index] = 0;
                    dispen2[index] = 0;
                    continue;
                }
                else if (!enemy.IsVisible)
                    continue;
                List<Item> wards = enemy.Inventory.Items.Where(x => x.Name == "item_ward_sentry" || x.Name == "item_ward_observer" || x.Name == "item_ward_dispenser").ToList();
                if (wards.Any())
                {
                    if (!linven.Contains(enemy))
                        linven.Add(enemy);
                    foreach (Item ward in wards)
                    {
                        if (ward.Name == "item_ward_dispenser")
                        {
                            if (!ldispen.Contains(enemy))
                                ldispen.Add(enemy);
                            int idispen = Convert.ToInt16(ward.CurrentCharges);
                            int idispen2 = Convert.ToInt16(ward.SecondaryCharges);
                            if (idispen < dispen[index])
                                eksekusi(enemy, "observer");
                            if (idispen2 < dispen2[index])
                                eksekusi(enemy, "sentry");
                            dispen[index] = idispen;
                            dispen2[index] = idispen2;
                        }
                        else if (ward.AbilityState.Equals(AbilityState.OnCooldown))
                        {
                            if (!wcd)
                                eksekusi(enemy, ward.Name.Replace("item_ward_", ""));
                            wcd = true;
                        }
                        else if (ldispen.Contains(enemy))
                        {
                            eksekusi(enemy, "dispenser");
                            ldispen.Remove(enemy);
                        }
                        else
                            wcd = false; 
                        
                    }
                }
                else if (linven.Contains(enemy) && !wards.Any())
                {
                    eksekusi(enemy, "dispenser");
                    linven.Remove(enemy);
                }
                else
                    linven.Remove(enemy);
                index++;
            }
        }

        public static void OnDraw(EventArgs args)
        {
            if (!Game.IsInGame)
                return;
            Vector2 ScreenPos;
            if (adaward)
            {
                Drawing.WorldToScreen(Position, out ScreenPos);
                if (namaward == "sentry")
                    Drawing.DrawRect(ScreenPos, Size, Drawing.GetTexture("materials/ensage_ui/items/ward_sentry.vmat"));
                if (namaward == "observer")
                    Drawing.DrawRect(ScreenPos, Size, Drawing.GetTexture("materials/ensage_ui/items/ward_observer.vmat"));
                if (namaward == "dispenser")
                    Drawing.DrawRect(ScreenPos, Size, Drawing.GetTexture("materials/ensage_ui/items/ward_dispenser.vmat"));
            }
        }
        public static void eksekusi(Hero enemy, String wardname)
        {
            namaward = wardname;
            Game.ExecuteCommand("say_team " + enemy.Name.Replace("npc_dota_hero_", "").Replace("_", " "));
            Game.ExecuteCommand("chatwheel_say 57");
            Position = enemy.Position;
            adaward = true;
        }
    }
}
