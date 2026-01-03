using System;
using System.Collections.Generic;
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

        private int _mutations = 0;
        private List<int> _mutationIDs = new List<int>();
        private System.Random _rand = new System.Random();
        private const float NormalGravity = -19.6f;
        private const float MutationGravity = -6f;
        private const float HealInterval = 1f;
        private const float SpeedMultiplier = 1.3f;
        private const float SpawnHintDuration = 3f;
        private const float SpeedMutationDuration = 60f;
        private const int HealthToHeal = 10;
        private const int HealthAfterHeal = 50;

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

        private void OnPlayerSpawned(SpawnedEventArgs eventArgs)
        {
            Role role = eventArgs.Player.Role;
            FpcRole fpc = role as FpcRole;
            fpc.Gravity = new Vector3(0f, NormalGravity, 0f);
            if (HasMutation(eventArgs.Player)) return;
            if (_rand.Next(0, 100) < Config.MutationChance)
            {
                if (_mutations < Config.MaxMutations)
                {
                    _mutations++;
                    eventArgs.Player.ShowHint(new Hint("Вы стали мутантом, ваша скорость и сила прыжка увеличены.", SpawnHintDuration));
                    _mutationIDs.Add(eventArgs.Player.Id);
                    fpc.Gravity = new Vector3(0f, MutationGravity, 0f);
                    fpc.SprintingSpeed *= SpeedMultiplier;
                    Timing.CallDelayed(SpeedMutationDuration, () =>
                    {
                        fpc.SprintingSpeed /= SpeedMultiplier;
                    });
                }
            }
        }

        private bool HasMutation(Player player)
        {
            return _mutationIDs.Contains(player.Id);
        }

        private void OnPlayerHurt(HurtEventArgs eventArgs)
        {
            if (HasMutation(eventArgs.Player) && eventArgs.Player.Health <= HealthToHeal)
            {
                Timing.RunCoroutine(Heal(eventArgs.Player));
            }
        }

        private IEnumerator<float> Heal(Player player)
        {
            while (player.Health < HealthAfterHeal)
            {
                player.Heal(1);
                yield return Timing.WaitForSeconds(HealInterval);
            }
        }
    }
}
