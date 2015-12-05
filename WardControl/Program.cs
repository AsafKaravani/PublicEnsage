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
        static Vector3 Position = new Vector3();
        static Vector2 Size = new Vector2(75, 50);
        static String namaward;
        static int[] dispen = new int[5];
        static bool[] inven = new bool[5];
        static bool[] bdispen = new bool[5];
        public static void Main(string[] args)
        {
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!Game.IsInGame)
                return;
            Hero me = ObjectMgr.LocalHero;
            int index = 0;
            List<Hero> enemies = ObjectMgr.GetEntities<Hero>().Where(x => x.Team != me.Team).ToList();
            foreach (Hero enemy in enemies)
            {
                if (!enemy.IsVisible)
                {
                    inven[index] = false;
                    bdispen[index] = false;
                }
                List<Item> wards = enemy.Inventory.Items.Where(x => x.Name == "item_ward_sentry" || x.Name == "item_ward_observer" || x.Name=="item_ward_dispenser").ToList();
                if (wards.Any())
                {
                    inven[index] = true;
                    foreach (Item ward in wards)
                    {
                        if (ward.Name=="item_ward_dispenser")
                        {
                            bdispen[index] = true;
                            String sdispen = ward.CurrentCharges.ToString();
                            if (Convert.ToInt16(sdispen) < dispen[index])
                                eksekusi(enemy, "observer");
                            dispen[index] = Convert.ToInt16(sdispen);
                        }
                        else if (ward.AbilityState.Equals(AbilityState.OnCooldown))
                            eksekusi(enemy,ward.Name.Replace("item_ward_", ""));
                        else if (bdispen[index])
                        {
                            eksekusi(enemy, "dispenser");
                            bdispen[index] = false;
                        }
                    }
                }
                else if (inven[index])
                {
                    eksekusi(enemy, "dispenser");
                    inven[index] = false;
                }
                else
                    inven[index] = false;
                index++;
            }
        }

        public static void OnDraw(EventArgs args)
        {
            if (!Game.IsInGame)
                return;
            Hero me = ObjectMgr.LocalHero;
            Vector2 ScreenPos;
            if (adaward)
            {
                Drawing.WorldToScreen(Position, out ScreenPos);  
                if (namaward=="sentry")
                    Drawing.DrawRect(ScreenPos, Size, Drawing.GetTexture("materials/ensage_ui/items/ward_sentry.vmat"));
                if (namaward == "observer")
                    Drawing.DrawRect(ScreenPos, Size, Drawing.GetTexture("materials/ensage_ui/items/ward_observer.vmat"));
                if (namaward == "dispenser")
                    Drawing.DrawRect(ScreenPos, Size, Drawing.GetTexture("materials/ensage_ui/items/ward_dispenser.vmat"));
            }
        }
        public static void eksekusi(Hero enemy,String wardname)
        {
            namaward = wardname;
            Game.ExecuteCommand("say_team " + enemy.Name.Replace("npc_dota_hero_", "").Replace("_"," "));
            Game.ExecuteCommand("chatwheel_say 57");
            Position = enemy.Position;
            adaward = true;
        }
    }
}
