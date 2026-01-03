using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace Mutation
{
    public class Main : Plugin<Config>
    {
        public override string Name => "Mutation";

        public override string Prefix => "MT";

        public override string Author => "I'm not playing in real life";

        public override Version Version => new Version(1, 0, 0);

        public override Version RequiredExiledVersion => new Version(9, 12, 2);

        private int Mutations = 0;
        private List<int> MutationsIDs = new List<int>();
        private System.Random rand = new System.Random();


        public override void OnDisabled()
        {
            base.OnDisabled();
            Exiled.Events.Handlers.Player.Spawned -= OnPlayerSpawned;
            Exiled.Events.Handlers.Player.Hurt -= OnPlayerHurt;
            Log.Info("Plugin succesfully disabled");
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            Exiled.Events.Handlers.Player.Spawned += OnPlayerSpawned;
            Exiled.Events.Handlers.Player.Hurt += OnPlayerHurt;
            Log.Info("Plugin succesfully enabled");
        }

        private void OnPlayerSpawned(SpawnedEventArgs ev)
        {
            Role role = ev.Player.Role;
            FpcRole fpc = role as FpcRole;
            fpc.Gravity = new Vector3(0f, -19.6f, 0f);
            if (hasMutation(ev.Player)) return;
            if (rand.Next(0, 100) < Config.MutationChance)
            {
                if (Mutations < Config.MaxMutations)
                {
                    Mutations++;
                    ev.Player.ShowHint(new Hint("Вы стали мутантом, ваша скорость и сила прыжка увеличены.", 5f));
                    MutationsIDs.Add(ev.Player.Id);
                    fpc.Gravity = new Vector3(0f, -6f, 0f);
                    fpc.SprintingSpeed *= 1.3f;
                    Timing.CallDelayed(60f, () =>
                    {
                        fpc.SprintingSpeed /= 1.3f;
                    });
                }
            }
        }

        private bool hasMutation(Player p)
        {
            return MutationsIDs.Contains(p.Id);
        }

        private void OnPlayerHurt(HurtEventArgs ev)
        {
            if (hasMutation(ev.Player) && ev.Player.Health <= 10)
            {
                Timing.RunCoroutine(Heal(ev.Player));
            }
        }

        private IEnumerator<float> Heal(Player p)
        {
            while (p.Health < 50)
            {
                p.Heal(1);
                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
